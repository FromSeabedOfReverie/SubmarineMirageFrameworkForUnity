//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Setting {
	using UnityEngine;
	using Base;
	using Service;
	///====================================================================================================
	/// <summary>
	/// ■ 中心設定のクラス
	/// </summary>
	///====================================================================================================
	public class SMMainSetting : SMStandardBase, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
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
		/// <summary>キャッシュ作成直後の初期値</summary>
		public const string INITIAL_CACHE_VERSION = "0.00";
		/// <summary>無指定の場合、キャッシュを使うか？</summary>
		public const bool IS_DEFAULT_USE_CACHE = false;


		/// <summary>書類の拡張子</summary>
		public const string CSV_EXTENSION = ".csv";

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