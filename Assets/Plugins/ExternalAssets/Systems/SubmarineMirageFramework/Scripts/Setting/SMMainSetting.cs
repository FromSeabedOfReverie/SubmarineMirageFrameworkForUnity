//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System.IO;
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ 中心設定のクラス
	/// </summary>
	///====================================================================================================
	public class SMMainSetting : SMStandardBase, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		public const SMPlatformType PLATFORM =
#if UNITY_STANDALONE_WIN || UNITY_WSA
			SMPlatformType.Windows;
#elif UNITY_STANDALONE_OSX
			SMPlatformType.MacOSX;
#elif UNITY_STANDALONE_LINUX
			SMPlatformType.Linux;
#elif UNITY_ANDROID
			SMPlatformType.Android;
#elif UNITY_IOS
			SMPlatformType.IOS;
#elif UNITY_WEBGL
			SMPlatformType.WebGL;
#endif


		/// <summary>外部の階層</summary>
		static readonly string EXTERNAL_PATH =
#if UNITY_ANDROID || UNITY_IOS
			// スマートフォンビルドの場合
			Application.persistentDataPath;
#else
			// PC、WEBビルドの場合
			Application.dataPath.Substring( 0, Application.dataPath.LastIndexOf( "/" ) + 1 );
#endif
		/// <summary>リソース書類の読込階層</summary>
		public const string LOAD_RESOURCE_PATH = "Data";
		/// <summary>外部書類の読込階層</summary>
		public static readonly string LOAD_EXTERNAL_PATH = EXTERNAL_PATH;
		/// <summary>情報を保存する階層</summary>
		public static readonly string SAVE_EXTERNAL_PATH = EXTERNAL_PATH;


		/// <summary>暗号書類の階層</summary>
		public const string CRYPTO_PATH = "Data";
		/// <summary>暗号化の鍵</summary>
		public const string ENCRYPT_KEY = "ENCRYPT_KEY";
		/// <summary>暗号パスワードの数</summary>
		public const int ENCRYPT_PASSWORD_COUNT = 16;
		/// <summary>パスワード</summary>
		public const string CRYPTO_PASSWORD = "PASSWORD";


		/// <summary>キャッシュの保存名</summary>
		public const string CACHE_FILE_NAME = "CacheData.data";
		/// <summary>無指定の場合、キャッシュを使うか？</summary>
		public const bool IS_DEFAULT_USE_CACHE = false;


		/// <summary>現在のアプリ版</summary>
		public static readonly string APPLICATION_VERSION = Application.version;
		/// <summary>版の初期値</summary>
		public const string INITIAL_VERSION = "0.01";


		/// <summary>設定情報の書類名</summary>
		public const string SETTING_FILE_NAME = "SettingData.data";


		/// <summary>遊戯情報の書式</summary>
		public const string PLAY_FILE_NAME_FORMAT = "PlayData{0}.data";
		/// <summary>遊戯情報のセーブ可能数</summary>
		public const int MAX_PLAY_DATA_COUNT = 1;


		/// <summary>写真の保存階層</summary>
		public static readonly string PICTURE_FILE_PATH = Path.Combine( CRYPTO_PATH, "Pictures" );
		/// <summary>写真の書式</summary>
		public const string PICTURE_FILE_NAME_FORMAT = "Picture{0}.jpg";


		/// <summary>CSV書類の拡張子</summary>
		public const string CSV_EXTENSION = ".csv";


		/// <summary>アプリ情報の階層</summary>
		public const string APPLICATION_DATA_PATH =
"https://docs.google.com/spreadsheets/d/e/2PACX-1vQZjl0KQ3qdx1ghjDLczrLpmWQ11Ao75IdaSobLMoFHjuzhG4pTCX0bXvZgGl_P4-2fjLCdCbBKHaRE/pub?gid=1615869423&single=true&output=csv"
			;
		/// <summary>宣伝情報の階層</summary>
		public const string CM_DATA_PATH =
"https://docs.google.com/spreadsheets/d/e/2PACX-1vQZjl0KQ3qdx1ghjDLczrLpmWQ11Ao75IdaSobLMoFHjuzhG4pTCX0bXvZgGl_P4-2fjLCdCbBKHaRE/pub?gid=1475705626&single=true&output=csv"
			;
		/// <summary>アプリの宣伝情報の階層</summary>
		public const string APPLICATION_CM_DATA_PATH =
"https://docs.google.com/spreadsheets/d/e/2PACX-1vQZjl0KQ3qdx1ghjDLczrLpmWQ11Ao75IdaSobLMoFHjuzhG4pTCX0bXvZgGl_P4-2fjLCdCbBKHaRE/pub?gid=1057403382&single=true&output=csv"
			;


		/// <summary>音のリソース読込の階層</summary>
		public const string Audio_RESOURCE_PATH = "Audio";


		/// <summary>最大プレイヤー数</summary>
		public const int MAX_PLAYERS = 2;


		/// <summary>商品版（保存）</summary>
		public SMEdition _editionBySave		{ get; set; }
		/// <summary>アプリ版（保存）</summary>
		public string _versionBySave		{ get; set; } = INITIAL_VERSION;
		/// <summary>サーバー版（保存）</summary>
		public string _serverVersionBySave	{ get; set; } = INITIAL_VERSION;

		/// <summary>商品版（サーバー）</summary>
		public SMEdition _editionByServer		{ get; set; }
		/// <summary>アプリ版（サーバー）</summary>
		public string _versionByServer			{ get; set; } = INITIAL_VERSION;
		/// <summary>サーバー版（サーバー）</summary>
		public string _serverVersionByServer	{ get; set; } = INITIAL_VERSION;

		/// <summary>アプリケーション更新を要求か？</summary>
		public bool _isRequestUpdateApplication	=> _versionBySave != _versionByServer;
		/// <summary>サーバーキャッシュ情報更新を要求か？</summary>
		public bool _isRequestUpdateServer		=> _serverVersionBySave != _serverVersionByServer;

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMMainSetting() {
		}
	}
}