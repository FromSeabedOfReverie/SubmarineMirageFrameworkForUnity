//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Data.Save {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx.Async;
	using Debug;
	using Test.Audio;
	///====================================================================================================
	/// <summary>
	/// ■ 設定情報のクラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class SettingData : SaveData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>アプリケーションの版</summary>
		public enum Edition {
			/// <summary>評価版</summary>
			Trial,
			/// <summary>製品版</summary>
			Product,
			/// <summary>展示版</summary>
			Exhibition,
		}
		/// <summary>画面状態</summary>
		public enum ScreenMode {
			/// <summary>Window画面</summary>
			Window,
			/// <summary>全画面</summary>
			Full,
		};
		/// <summary>画面の大きさ</summary>
		public enum ScreenSize {
			/// <summary>横960、縦540</summary>
			_960X540,
			/// <summary>横1440、縦810</summary>
			_1440X810,
			/// <summary>横1920、縦1080</summary>
			_1920X1080
		};
		/// <summary>描画品質</summary>
		public enum Quality {
			/// <summary>最低</summary>
			VeryLow,
			/// <summary>低</summary>
			Low,
			/// <summary>並</summary>
			Middle,
			/// <summary>高</summary>
			High,
			/// <summary>最高</summary>
			VeryHigh
		}
		/// <summary>1秒間の画面描画回数（FPS）</summary>
		public enum FrameRate {
			/// <summary>1秒間に、30回描画（FPS）</summary>
			_30,
			/// <summary>1秒間に、60回描画（FPS）</summary>
			_60
		}
		/// <summary>アプリケーション版　→　版名へ変換の為の、辞書</summary>
		static readonly Dictionary<Edition, string> EDITION_TO_NAME = new Dictionary<Edition, string> {
			{ Edition.Trial,		"評価版" },
			{ Edition.Product,		"製品版" },
			{ Edition.Exhibition,	"展示版" },
		};
		/// <summary>画面の大きさ　→　ベクトルへ変換の為の、辞書</summary>
		static readonly Dictionary<ScreenSize, Vector2Int> SCREEN_SIZE_TO_VECTORS
			= new Dictionary<ScreenSize, Vector2Int> {
				{ ScreenSize._960X540,		new Vector2Int( 960, 540 )		},
				{ ScreenSize._1440X810,		new Vector2Int( 1440, 810 )		},
				{ ScreenSize._1920X1080,	new Vector2Int( 1920, 1080 )	},
			};
		/// <summary>描画品質名　→　Unityの品質番号へ変換の為の、辞書</summary>
		static readonly Dictionary<string, int> QUALITY_NAME_TO_IDS
			= QualitySettings.names
				.Select( ( s, i ) => new {s, i} )
				.ToDictionary( pair => pair.s, pair => pair.i );
		/// <summary>描画品質名　→　Unityの品質名へ変換の為の、辞書</summary>
		static readonly Dictionary<Quality, string> QUALITY_TO_NAMES = new Dictionary<Quality, string> {
			{ Quality.VeryLow,	"Fastest"	},
			{ Quality.Low,		"Fast"		},
			{ Quality.Middle,	"Simple"	},
			{ Quality.High,		"Good"		},
			{ Quality.VeryHigh,	"Fantastic"	},
		};
		/// <summary>FPS型　→　整数へ変換の為の、辞書</summary>
		static readonly Dictionary<FrameRate, int> FRAME_RATE_TO_INT = new Dictionary<FrameRate, int> {
			{ FrameRate._30,	30 },
			{ FrameRate._60,	60 },
		};

		/// <summary>現在のアプリケーションの版</summary>
		public Edition _edition;
		/// <summary>現在のアプリケーションの更新版</summary>
		public string _version;
		/// <summary>画面状態</summary>
		public ScreenMode _screenMode;
		/// <summary>画面の大きさ</summary>
		public ScreenSize _screenSize;
		/// <summary>描画品質</summary>
		public Quality _quality;
		/// <summary>目標の、1秒間の画面描画回数（FPS）</summary>
		public FrameRate _frameRate;
		/// <summary>音楽の音量</summary>
		public float _bgmVolume;
		/// <summary>声音の音量</summary>
		public float _voiceVolume;
		/// <summary>効果音の音量</summary>
		public float _seVolume;
		/// <summary>デバッグ表示するか？</summary>
		public bool  _isViewDebug;
		/// <summary>広告削除機能を購入したか？</summary>
		public bool _isPurchasedDeleteAdvertisement;
		/// <summary>遊戯情報一覧の内、読込中の遊戯情報の添字</summary>
		public int _playDataIndex;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public SettingData() {
			// 名前から版を設定
			var name = Application.productName;
			_edition = (
				name.Contains( "評価版" )	? Edition.Trial :
				name.Contains( "展示版" )	? Edition.Exhibition
											: Edition.Product
			);

			_version = Application.version;
			_screenMode = ScreenMode.Full;

#if UNITY_ANDROID || UNITY_IOS
			// スマートフォン版、低クオリティに設定（動かない可能性がある為）
			_screenSize = ScreenSize._960X540;
			_quality = Quality.VeryLow;
			_frameRate = FrameRate._30;

#elif UNITY_STANDALONE
			// PC版、中クオリティに設定
			_screenSize = ScreenSize._1920X1080;
			_quality = Quality.Middle;
			_frameRate = FrameRate._60;
#endif

			_bgmVolume = 1;
			_voiceVolume = 1;
			_seVolume = 1;

#if DEVELOP
			_isViewDebug = true;
#endif

			_isPurchasedDeleteAdvertisement = false;
			_playDataIndex = 0;


			// TODO : デバッグ用、削除する
			_screenMode = ScreenMode.Window;
			_screenSize = ScreenSize._960X540;
			_quality = Quality.Middle;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 適用
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void Apply() {
			// 解像度、画面表示を適用
			var size = SCREEN_SIZE_TO_VECTORS[_screenSize];
			Screen.SetResolution( size.x, size.y, _screenMode == ScreenMode.Full );
/*
			var screenRate = 480.0f / Screen.width;
			if (screenRate > 1)	screenRate = 1;
			var width = ( int )( Screen.width * screenRate );
			var height = ( int ) (Screen.height * screenRate );
			Screen.SetResolution( width, height, true, 15 );
*/

			// 画質を適用
			var name = QUALITY_TO_NAMES[_quality];
			QualitySettings.SetQualityLevel( QUALITY_NAME_TO_IDS[name] );

			// FPSを設定
			Application.targetFrameRate = FRAME_RATE_TO_INT[_frameRate];
			Time.fixedDeltaTime = 1f / 30;//rate;	// 物理重いので固定

			// 音量を設定
			AudioListener.volume = 1;
			TestAudioManager.s_instance._bgmVolume = _bgmVolume;
			TestAudioManager.s_instance._voiceVolume = _voiceVolume;
			TestAudioManager.s_instance._seVolume = _seVolume;

			// デバッグ表示を適用
			DebugDisplay.s_instance._isDraw = _isViewDebug;
/*
			var resolutionRate = 0.25f + ( ( int )_screenSize + 1 ) * 0.25f;
			DebugDisplay.s_instance._fontSize = ( int )( 40 * resolutionRate );
			DebugDisplay.s_instance._y = ( int )( 40 * resolutionRate );
*/

			// マウスカーソルを設定
//			Cursor.visible = true;
//			Cursor.lockState = CursorLockMode.Confined;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Load() {
			Apply();
			await base.Load();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 保存
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Save() {
			_version = Application.version;
			Apply();
			await base.Save();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文字列に変換（版）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public string ToEditionString() {
			return EDITION_TO_NAME[_edition];
		}
	}
}