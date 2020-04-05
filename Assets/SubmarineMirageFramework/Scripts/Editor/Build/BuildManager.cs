//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Editor.Build {
	using System;
	using System.IO;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.Build;
	using UnityEditor.Build.Reporting;
	using Data.Save;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ ビルドの管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class BuildManager : IPreprocessBuildWithReport, IPostprocessBuildWithReport {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>版</summary>
		const SettingData.Edition EDITION =
#if UNITY_STANDALONE
			SettingData.Edition.Trial;
#else
			SettingData.Edition.Product;	// スマホ版は必ずProductとする（ストアの都合上、同一アプリの為）
#endif
		/// <summary>番号</summary>
		const float VERSION = 0.1f;
		/// <summary>評価版の場面名</summary>
		static readonly List<string> TRIAL_SCENE_NAMES = new List<string> {
			"TrialSceneName",
		};

		/// <summary>移動先フォルダ</summary>
		static readonly string MOVE_TO_FOLDER =
			Path.Combine( Application.dataPath, "Plugins/ExternalAssets/Systems/LunarConsole" );
		/// <summary>移動元フォルダ</summary>
		static readonly string MOVE_FROM_FOLDER = Path.Combine( MOVE_TO_FOLDER, "Resources" );

		/// <summary>実行順</summary>
		public int callbackOrder => 0;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● ビルド前のコールバック関数
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void OnPreprocessBuild( BuildReport report ) {
			SetName();
			SetVersion();
			SetScene();
			MoveResources();
			Log.Debug( "ビルド前処理完了", Log.Tag.Build );
			SetDebug();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 名前設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetName() {
			// 名前、バンドル識別子を設定
			var name = "SubmarineMirageFrameworkForUnity";
			var bundleID = "";
#if UNITY_ANDROID || UNITY_IOS
			bundleID += "com";
#elif UNITY_STANDALONE
			bundleID += "unity";
#endif
			bundleID += "." + PlayerSettings.companyName + ".";

			switch ( EDITION ) {
				case SettingData.Edition.Trial:
// スマホ版は、統一する為、表記しない
#if UNITY_STANDALONE
					name += "（評価版）";
#endif
					bundleID += "SubmarineMirageFrameworkForUnity_Trial";
					break;
				case SettingData.Edition.Product:
					bundleID += "SubmarineMirageFrameworkForUnity_Product";
					break;
				case SettingData.Edition.Exhibition:
					name += "（展示版）";
					bundleID += "SubmarineMirageFrameworkForUnity_Exhibition";
					break;
			}
			// 名前、バンドル識別子を適用
			PlayerSettings.productName = name;
#if UNITY_ANDROID
			PlayerSettings.SetApplicationIdentifier( BuildTargetGroup.Android,		bundleID );
#elif UNITY_IOS
			PlayerSettings.SetApplicationIdentifier( BuildTargetGroup.iOS,			bundleID );
#elif UNITY_STANDALONE
			PlayerSettings.SetApplicationIdentifier( BuildTargetGroup.Standalone,	bundleID );
#endif
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 版設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetVersion() {
			// 番号適用
			PlayerSettings.bundleVersion = VERSION.ToString();
			PlayerSettings.Android.bundleVersionCode = ( int )Mathf.Round( VERSION * 100 );
			PlayerSettings.macOS.buildNumber = VERSION.ToString();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 場面設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetScene() {
			// 挿入場面を設定
			EditorBuildSettings.scenes = EditorBuildSettings.scenes
				.Select( scene => {
					switch ( EDITION ) {
// スマホ版は統一する為、全部入れる
#if UNITY_STANDALONE
						// 評価版の場合
						case SettingData.Edition.Trial:
							scene.enabled = false;

							var paths = scene.path.Split( '/' );
							var scene_name = paths[paths.Length - 1];
							// イベント場面の場合、指定場面のみ活動化
							if ( scene_name.Contains( "Event" ) ) {
								foreach ( var n in TRIAL_SCENE_NAMES ) {
									if ( scene_name.Contains( n ) ) {
										scene.enabled = true;
										break;
									}
								}
							// 通常場面の場合、活動化
							} else {
								scene.enabled = true;
							}
							break;
#endif
						// それ以外の版の場合
						default:
							scene.enabled = true;
							break;
					}
					return scene;
				} )
				.ToArray();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● リソースファイル移動
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void MoveResources() {
#if DEVELOP
			return;
#endif
			try {
				Directory.Move( MOVE_FROM_FOLDER, MOVE_TO_FOLDER );
			} catch ( Exception e ) {
				Log.Error( e.Message, Log.Tag.Build );
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● デバッグ設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetDebug() {
			var isDebug =
#if DEVELOP
				true;
#else
				false;
#endif

			// デバッグを非設定
			EditorUserBuildSettings.development = isDebug;

			// ログ表示を無効化
			var logType = isDebug ? StackTraceLogType.ScriptOnly : StackTraceLogType.None;
			PlayerSettings.SetStackTraceLogType( LogType.Error,		logType );
			PlayerSettings.SetStackTraceLogType( LogType.Assert,	logType );
			PlayerSettings.SetStackTraceLogType( LogType.Warning,	logType );
			PlayerSettings.SetStackTraceLogType( LogType.Log,		logType );
			PlayerSettings.SetStackTraceLogType( LogType.Exception,	logType );
			Debug.unityLogger.logEnabled = isDebug;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● ビルド後のコールバック関数
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void OnPostprocessBuild( BuildReport report ) {
			ResetScene();
			ResetDebug();
			RemoveResources();

			Log.Debug( "ビルド後処理完了", Log.Tag.Build );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 場面リセット
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void ResetScene() {
			// 挿入場面を全設定
			EditorBuildSettings.scenes = EditorBuildSettings.scenes
				.Select( scene => {
					scene.enabled = true;
					return scene;
				} )
				.ToArray();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● デバッグリセット
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void ResetDebug() {
			// デバッグを設定
			EditorUserBuildSettings.development = true;

			// ログ表示を有効化
			PlayerSettings.SetStackTraceLogType( LogType.Error,		StackTraceLogType.ScriptOnly );
			PlayerSettings.SetStackTraceLogType( LogType.Assert,	StackTraceLogType.ScriptOnly );
			PlayerSettings.SetStackTraceLogType( LogType.Warning,	StackTraceLogType.ScriptOnly );
			PlayerSettings.SetStackTraceLogType( LogType.Log,		StackTraceLogType.ScriptOnly );
			PlayerSettings.SetStackTraceLogType( LogType.Exception,	StackTraceLogType.ScriptOnly );
			Debug.unityLogger.logEnabled = true;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● リソースファイル再移動
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void RemoveResources() {
#if DEVELOP
			return;
#endif
			try {
				Directory.Move( MOVE_TO_FOLDER, MOVE_FROM_FOLDER );
			} catch ( Exception e ) {
				Log.Error( e, Log.Tag.Build );
			}
		}
	}
}