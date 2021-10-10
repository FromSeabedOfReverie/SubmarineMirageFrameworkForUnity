//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace Game {
	using UnityEngine;
	using DG.Tweening;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using SubmarineMirage;
	using SystemTask = System.Threading.Tasks.Task;



	/// <summary>
	/// ■ ゲーム起動開始のクラス
	///		ゲーム起動時に、このクラスから実行される。
	/// </summary>
	public static class GameMain {

		/// <summary>
		/// ● 外部ライブラリを初期化
		/// </summary>
		static async UniTask InitializePlugin() {
			// DOTWeen初期化
			DOTween.Init(
				false,
				!SMDebugManager.IS_DEVELOP,
				LogBehaviour.Verbose
			);
			DOTween.defaultAutoPlay = AutoPlay.None;

			await SystemTask.Delay( 1 );

			// UniTask初期化
			UniTaskScheduler.UnobservedExceptionWriteLogType = LogType.Error;

			if ( SMDebugManager.IS_DEVELOP ) {
				var prefab = Resources.Load<GameObject>( "LunarConsole" );
				var go = prefab.Instantiate();
				go.DontDestroyOnLoad();
			}
		}

		/// <summary>
		/// ● 設定データを登録
		/// </summary>
		static async UniTask RegisterSettings() {
			// 入力設定を登録
			SMServiceLocator.Register<BaseSMInputSetting>( new SMInputSetting() );
			// データ設定を登録
			SMServiceLocator.Register<BaseSMDataSetting>( new SMDataSetting() );
			// シーン設定を登録
			SMServiceLocator.Register<BaseSMSceneSetting>( new SMSceneSetting() );

			await UTask.DontWait();
		}

		/// <summary>
		/// ● 実行開始
		///		ゲームは、ここから開始される。
		/// </summary>
		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
		static async void Main() {
//			return;

			var framework = SMServiceLocator.Register( new SubmarineMirageFramework() );
			await framework.TakeOff( InitializePlugin, RegisterSettings );
		}
	}
}