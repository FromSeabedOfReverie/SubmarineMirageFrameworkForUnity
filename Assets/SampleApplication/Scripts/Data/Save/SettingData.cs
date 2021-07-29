//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestSetting
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using SubmarineMirage.Service;
using SubmarineMirage.Data.Save;
using SubmarineMirage.Setting;
using SubmarineMirage.Debug;
///========================================================================================================
/// <summary>
/// ■ 設定情報のクラス
/// </summary>
///========================================================================================================
public class SettingData : BaseSMSaveData {
	///----------------------------------------------------------------------------------------------------
	/// ● 要素
	///----------------------------------------------------------------------------------------------------
	/// <summary>アプリケーション版 → 版名へ変換の為の、辞書</summary>
	static readonly Dictionary<SMEdition, string> EDITION_TO_NAME = new Dictionary<SMEdition, string> {
		{ SMEdition.Trial,		"評価版" },
		{ SMEdition.Product,	"製品版" },
		{ SMEdition.Exhibition,	"展示版" },
	};
	/// <summary>画面の大きさ → ベクトルへ変換の為の、辞書</summary>
	static readonly Dictionary<SMScreenSize, Vector2Int> SCREEN_SIZE_TO_VECTORS
		= new Dictionary<SMScreenSize, Vector2Int> {
			{ SMScreenSize._960X540,	new Vector2Int( 960, 540 )		},
			{ SMScreenSize._1440X810,	new Vector2Int( 1440, 810 )		},
			{ SMScreenSize._1920X1080,	new Vector2Int( 1920, 1080 )	},
		};
	/// <summary>描画品質名 → Unityの品質番号へ変換の為の、辞書</summary>
	static readonly Dictionary<string, int> QUALITY_NAME_TO_IDS
		= QualitySettings.names
			.Select( ( s, i ) => new {s, i} )
			.ToDictionary( pair => pair.s, pair => pair.i );
	/// <summary>描画品質名 → Unityの品質名へ変換の為の、辞書</summary>
	static readonly Dictionary<SMQuality, string> QUALITY_TO_NAMES = new Dictionary<SMQuality, string> {
		{ SMQuality.VeryLow,	"Fastest"	},
		{ SMQuality.Low,		"Fast"		},
		{ SMQuality.Middle,		"Simple"	},
		{ SMQuality.High,		"Good"		},
		{ SMQuality.VeryHigh,	"Fantastic"	},
	};
	/// <summary>FPS型 → 整数へ変換の為の、辞書</summary>
	static readonly Dictionary<SMFrameRate, int> FRAME_RATE_TO_INT = new Dictionary<SMFrameRate, int> {
		{ SMFrameRate._30,	30 },
		{ SMFrameRate._60,	60 },
	};

	/// <summary>現在のアプリケーションの版</summary>
	public SMEdition _edition;
	/// <summary>現在のアプリケーションの更新版</summary>
	public string _version;
	/// <summary>画面状態</summary>
	public SMScreenMode _screenMode;
	/// <summary>画面の大きさ</summary>
	public SMScreenSize _screenSize;
	/// <summary>描画品質</summary>
	public SMQuality _quality;
	/// <summary>目標の、1秒間の画面描画回数（FPS）</summary>
	public SMFrameRate _frameRate;
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

	///----------------------------------------------------------------------------------------------------
	/// ● 作成、削除
	///----------------------------------------------------------------------------------------------------
	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public SettingData() {
		_edition = GetEdition();
		_version = SMMainSetting.APPLICATION_VERSION;
		_screenMode = SMScreenMode.Full;

		switch ( SMMainSetting.PLATFORM ) {
			// PC版、中クオリティに設定
			case SMPlatformType.Windows:
			case SMPlatformType.MacOSX:
			case SMPlatformType.Linux:
				_screenSize = SMScreenSize._1920X1080;
				_quality = SMQuality.Middle;
				_frameRate = SMFrameRate._60;
				break;

			// スマートフォン版、低クオリティに設定（動かない可能性がある為）
			case SMPlatformType.Android:
			case SMPlatformType.IOS:
				_screenSize = SMScreenSize._960X540;
				_quality = SMQuality.VeryLow;
				_frameRate = SMFrameRate._30;
				break;
		}
		if ( SMDebugManager.IS_UNITY_EDITOR ) {
			_screenMode = SMScreenMode.Window;
			_screenSize = SMScreenSize._960X540;
			_quality = SMQuality.Middle;
		}

		_bgmVolume = 1;
		_voiceVolume = 1;
		_seVolume = 1;

		_isViewDebug = SMDebugManager.IS_DEVELOP;

		_isPurchasedDeleteAdvertisement = false;
		_playDataIndex = 0;
	}
	///----------------------------------------------------------------------------------------------------
	/// <summary>
	/// ● 適用
	/// </summary>
	///----------------------------------------------------------------------------------------------------
	void Apply() {
		// 解像度、画面表示を適用
		var size = SCREEN_SIZE_TO_VECTORS[_screenSize];
		Screen.SetResolution( size.x, size.y, _screenMode == SMScreenMode.Full );
/*
		var screenRate = 480.0f / Screen.width;
		if ( screenRate > 1 )	screenRate = 1;
		var width = ( int )( Screen.width * screenRate );
		var height = ( int ) ( Screen.height * screenRate );
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
//		GameAudioManager.s_instance._bgmVolume = _bgmVolume;
//		GameAudioManager.s_instance._voiceVolume = _voiceVolume;
//		GameAudioManager.s_instance._seVolume = _seVolume;

		// デバッグ表示を適用
		var displayLog = SMServiceLocator.Resolve<SMDisplayLog>();
		displayLog._isDraw = _isViewDebug;
/*
		var resolutionRate = 0.25f + ( ( int )_screenSize + 1 ) * 0.25f;
		displayLog._fontSize = ( int )( 40 * resolutionRate );
		displayLog._y = ( int )( 40 * resolutionRate );
*/

		// マウスカーソルを設定
//		Cursor.visible = true;
//		Cursor.lockState = CursorLockMode.Confined;

		// 中央設定の版を設定
		var setting = SMServiceLocator.Resolve<SMMainSetting>();
		setting._editionBySave = _edition;
		setting._versionBySave = _version;
	}

	///----------------------------------------------------------------------------------------------------
	/// ● 読み書き
	///----------------------------------------------------------------------------------------------------
	/// <summary>
	/// ● 読込
	/// </summary>
	public override async UniTask Load() {
		Apply();
		await base.Load();
	}

	/// <summary>
	/// ● 保存
	/// </summary>
	public override async UniTask Save() {
		_version = SMMainSetting.APPLICATION_VERSION;
		Apply();
		await base.Save();
	}

	///----------------------------------------------------------------------------------------------------
	/// ● 取得
	///----------------------------------------------------------------------------------------------------
	/// <summary>
	/// ● 版を取得
	///		アプリ名から版を取得。
	/// </summary>
	public static SMEdition GetEdition()
		=> EDITION_TO_NAME
			.First( pair => Application.productName.Contains( pair.Value ) )
			.Key;

	///----------------------------------------------------------------------------------------------------
	/// <summary>
	/// ● 文字列に変換（版）
	/// </summary>
	///----------------------------------------------------------------------------------------------------
	public string ToEditionString()
		=> EDITION_TO_NAME[_edition];
}