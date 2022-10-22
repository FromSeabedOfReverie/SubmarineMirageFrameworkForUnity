//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	///====================================================================================================
	/// <summary>
	/// ■ 情報読み書きの基盤クラス
	///		各種書類読み書きクラスは、このクラスを継承する。
	/// </summary>
	///====================================================================================================
	public abstract class BaseSMDataLoader : SMStandardBase {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>読込中の数</summary>
		public int _loadingCount		{ get; protected set; }
		/// <summary>保存中の数</summary>
		public int _savingCount			{ get; protected set; }
		/// <summary>配信中の数</summary>
		public int _downloadingCount	{ get; protected set; }
		/// <summary>配信後の数</summary>
		public int _downloadedCount		{ get; protected set; }
		/// <summary>失敗の数</summary>
		public int _errorCount			{ get; protected set; }
		
		/// <summary>読込中か？</summary>
		public bool _isLoading		=> _loadingCount > 0;
		/// <summary>保存中か？</summary>
		public bool _isSaving		=> _savingCount > 0;
		/// <summary>配信中か？</summary>
		public bool _isDownloading	=> _downloadingCount > 0;
		/// <summary>配信後か？</summary>
		public bool _isDownloaded	=> _downloadedCount > 0;
		/// <summary>失敗か？</summary>
		public bool _isError		=> _errorCount > 0;
		/// <summary>成功か？</summary>
		public bool _isSuccess		=> !_isLoading && !_isSaving && !_isDownloading && !_isError;

		protected SMFileManager _fileManager	{ get; private set; }
		protected readonly SMAsyncCanceler _asyncCanceler = new SMAsyncCanceler();
		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public BaseSMDataLoader( SMFileManager fileManager ) {
			_fileManager = fileManager;

			_disposables.AddFirst( () => {
				_asyncCanceler.Dispose();
				ResetCount();
				_fileManager = null;
			} );
		}

		/// <summary>
		/// ● 設定
		/// </summary>
		public virtual void Setup() {
		}

		/// <summary>
		/// ● 再初期化（回数）
		/// </summary>
		public void ResetCount() {
			_loadingCount = 0;
			_savingCount = 0;
			_downloadingCount = 0;
			_downloadedCount = 0;
			_errorCount = 0;
		}
	}
}