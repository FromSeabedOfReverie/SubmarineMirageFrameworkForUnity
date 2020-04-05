//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.File {
	using System.IO;
	using System.Linq;
	using UnityEngine;
	using Singleton;
	using Save;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 書類の管理クラス
	///----------------------------------------------------------------------------------------------------
	///		書類の読み書きを行う。
	/// </summary>
	///====================================================================================================
	public class FileManager : Singleton<FileManager> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>外部の階層</summary>
		static readonly string EXTERNAL_PATH =
// スマートフォンビルドの場合
#if UNITY_ANDROID || UNITY_IOS
			Application.persistentDataPath;
// PC、WEBビルドの場合
#else
			Application.dataPath.Substring( 0, Application.dataPath.LastIndexOf( "/" ) + 1 );
#endif
		/// <summary>リソース書類の読込階層</summary>
		public const string LOAD_RESOURCE_PATH = "Data";
		/// <summary>外部書類の読込階層</summary>
		public static readonly string LOAD_EXTERNAL_PATH = EXTERNAL_PATH;
		/// <summary>情報を保存する階層</summary>
		public static readonly string SAVE_EXTERNAL_PATH = EXTERNAL_PATH;

		/// <summary>全読み込みクラスの一覧</summary>
		readonly ClassDataManager<DataLoader> _allData = new ClassDataManager<DataLoader>();
		/// <summary>キャッシュ情報</summary>
		public readonly CacheDataManager _cacheData = new CacheDataManager();
		/// <summary>書類読み書き</summary>
		public readonly FileLoader _fileLoader = new FileLoader();
		/// <summary>暗号化書類読み書き</summary>
		public readonly CryptoLoader _cryptoLoader = new CryptoLoader();
		/// <summary>CSV書類読み書き</summary>
		public readonly CSVLoader _csvLoader = new CSVLoader();

		/// <summary>全読み書きクラス総合の、読込中の数</summary>
		public int _loadingCount		=> _allData.GetAll().Sum( pair => pair.Value._loadingCount );
		/// <summary>全読み書きクラス総合の、保存中の数</summary>
		public int _savingCount			=> _allData.GetAll().Sum( pair => pair.Value._savingCount );
		/// <summary>全読み書きクラス総合の、配信中の数</summary>
		public int _downloadingCount	=> _allData.GetAll().Sum( pair => pair.Value._downloadingCount );
		/// <summary>全読み書きクラス総合の、読込後の数</summary>
		public int _downloadedCount		=> _allData.GetAll().Sum( pair => pair.Value._downloadedCount );
		/// <summary>全読み書きクラス総合の、失敗の数</summary>
		public int _errorCount			=> _allData.GetAll().Sum( pair => pair.Value._errorCount );
		/// <summary>全読み書きクラスが、総合的に読込中か？</summary>
		public bool _isLoading		=> _loadingCount > 0;
		/// <summary>全読み書きクラスが、総合的に保存中か？</summary>
		public bool _isSaving		=> _savingCount > 0;
		/// <summary>全読み書きクラスが、総合的に配信中か？</summary>
		public bool _isDownloading	=> _downloadingCount > 0;
		/// <summary>全読み書きクラスが、総合的に配信後か？</summary>
		public bool _isDownloaded	=> _downloadedCount > 0;
		/// <summary>全読み書きクラスが、総合的に失敗か？</summary>
		public bool _isError		=> _errorCount > 0;
		/// <summary>全読み書きクラスが、総合的に成功か？</summary>
		public bool _isSuccess		=> !_isLoading && !_isSaving && !_isDownloading && !_isError;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public FileManager() {
			// ● 読込
			_loadEvent += async () => {
				_allData.Register( _fileLoader );
				_allData.Register( _cryptoLoader );
				_allData.Register( _csvLoader );
				await _allData.Load();
			};
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public T Get<T>() where T : DataLoader {
			return _allData.Get<T>();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 階層を作成
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void CreatePath( string path ) {
			// 階層が存在する場合、未処理
			if ( Directory.Exists( path ) )	{ return; }

			// 階層作成
			Directory.CreateDirectory( path );
			Log.Debug( $"階層作成 : {path}", Log.Tag.File );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 階層を削除
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void DeletePath( string path ) {
			// 階層が存在しない場合、未処理
			if ( !Directory.Exists ( path ) )	{ return; }

			// 階層削除
			Directory.Delete( path, true );
			Log.Debug( $"階層削除 : {path}", Log.Tag.File );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 再初期化（全回数）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void ResetAllCount() {
			_fileLoader.ResetCount();
			_cryptoLoader.ResetCount();
			_csvLoader.ResetCount();
		}
	}
}