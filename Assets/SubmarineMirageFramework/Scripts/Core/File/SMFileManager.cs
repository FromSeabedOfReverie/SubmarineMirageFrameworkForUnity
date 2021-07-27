//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.File {
	using System.IO;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Service;
	using Task;
	using Data.Save;
	using Setting;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 書類の管理クラス
	///		書類の読み書きを行う。
	/// </summary>
	///====================================================================================================
	public class SMFileManager : SMTask, ISMService {
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


		[SMShowLine] public override SMTaskRunType _type => SMTaskRunType.Sequential;

		/// <summary>キャッシュ情報</summary>
		public readonly CacheDataManager _cacheData = new CacheDataManager();

		/// <summary>書類読み書き</summary>
		public readonly SMFileLoader _fileLoader = new SMFileLoader();
		/// <summary>暗号化書類読み書き</summary>
		public readonly SMCryptoLoader _cryptoLoader = new SMCryptoLoader();
		/// <summary>CSV書類読み書き</summary>
		public readonly SMCSVLoader _csvLoader = new SMCSVLoader();
		/// <summary>読み書きクラスの一覧</summary>
		readonly List<BaseSMDataLoader> _loaders;

		/// <summary>全読み書きクラス総合の、読込中の数</summary>
		public int _loadingCount		=> _loaders.Sum( l => l._loadingCount );
		/// <summary>全読み書きクラス総合の、保存中の数</summary>
		public int _savingCount			=> _loaders.Sum( l => l._savingCount );
		/// <summary>全読み書きクラス総合の、配信中の数</summary>
		public int _downloadingCount	=> _loaders.Sum( l => l._downloadingCount );
		/// <summary>全読み書きクラス総合の、読込後の数</summary>
		public int _downloadedCount		=> _loaders.Sum( l => l._downloadedCount );
		/// <summary>全読み書きクラス総合の、失敗の数</summary>
		public int _errorCount			=> _loaders.Sum( l => l._errorCount );

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
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMFileManager() {
			_loaders = new List<BaseSMDataLoader>() {
				_fileLoader, _cryptoLoader, _csvLoader,
			};

			_disposables.AddFirst( () => {
				ResetAllCount();

				_loaders.ForEach( l => l.Dispose() );
				_loaders.Clear();

				_cacheData.Dispose();
			} );
		}

		/// <summary>
		/// ● 作成
		/// </summary>
		public override void Create() {
			_loaders.ForEach( l => l.Setup( this ) );
		}

		/// <summary>
		/// ● 全回数をリセット
		/// </summary>
		public void ResetAllCount()
			=> _loaders.ForEach( l => l.ResetCount() );
		///------------------------------------------------------------------------------------------------
		/// ● 階層
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 階層を作成
		/// </summary>
		public void CreatePath( string path ) {
			// 階層が存在する場合、未処理
			if ( Directory.Exists( path ) )	{ return; }

			// 階層作成
			Directory.CreateDirectory( path );
			SMLog.Debug( $"階層作成 : {path}", SMLogTag.File );
		}

		/// <summary>
		/// ● 階層を削除
		/// </summary>
		public void DeletePath( string path ) {
			// 階層が存在しない場合、未処理
			if ( !Directory.Exists ( path ) )	{ return; }

			// 階層削除
			Directory.Delete( path, true );
			SMLog.Debug( $"階層削除 : {path}", SMLogTag.File );
		}
	}
}