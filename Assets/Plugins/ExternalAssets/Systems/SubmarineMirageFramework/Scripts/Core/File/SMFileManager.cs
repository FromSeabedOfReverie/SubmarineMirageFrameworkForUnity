//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using KoganeUnityLib;
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
		[SMShowLine] public override SMTaskRunType _type => SMTaskRunType.Sequential;

		/// <summary>読み書きクラスの一覧</summary>
		readonly Dictionary<Type, BaseSMDataLoader> _loaders = new Dictionary<Type, BaseSMDataLoader>();
		/// <summary>キャッシュ情報</summary>
		public SMTemporaryCacheDataManager _tempCaches { get; private set; }

		/// <summary>全読み書きクラス総合の、読込中の数</summary>
		[SMShow] public int _loadingCount		=> _loaders.Sum( pair => pair.Value._loadingCount );
		/// <summary>全読み書きクラス総合の、保存中の数</summary>
		[SMShow] public int _savingCount		=> _loaders.Sum( pair => pair.Value._savingCount );
		/// <summary>全読み書きクラス総合の、配信中の数</summary>
		[SMShow] public int _downloadingCount	=> _loaders.Sum( pair => pair.Value._downloadingCount );
		/// <summary>全読み書きクラス総合の、読込後の数</summary>
		[SMShow] public int _downloadedCount	=> _loaders.Sum( pair => pair.Value._downloadedCount );
		/// <summary>全読み書きクラス総合の、失敗の数</summary>
		[SMShow] public int _errorCount			=> _loaders.Sum( pair => pair.Value._errorCount );

		/// <summary>全読み書きクラスが、総合的に読込中か？</summary>
		[SMShow] public bool _isLoading		=> _loadingCount > 0;
		/// <summary>全読み書きクラスが、総合的に保存中か？</summary>
		[SMShow] public bool _isSaving		=> _savingCount > 0;
		/// <summary>全読み書きクラスが、総合的に配信中か？</summary>
		[SMShow] public bool _isDownloading	=> _downloadingCount > 0;
		/// <summary>全読み書きクラスが、総合的に配信後か？</summary>
		[SMShow] public bool _isDownloaded	=> _downloadedCount > 0;
		/// <summary>全読み書きクラスが、総合的に失敗か？</summary>
		[SMShow] public bool _isError		=> _errorCount > 0;
		/// <summary>全読み書きクラスが、総合的に成功か？</summary>
		[SMShow] public bool _isSuccess		=> !_isLoading && !_isSaving && !_isDownloading && !_isError;

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMFileManager() {
			Register( new SMFileLoader( this ) );
			Register( new SMCryptoLoader( this ) );
			Register( new SMCSVLoader( this ) );


			_disposables.AddFirst( () => {
				ResetAllCount();
				_tempCaches = null;

				_loaders.ForEach( pair => pair.Value.Dispose() );
				_loaders.Clear();
			} );
		}

		/// <summary>
		/// ● 作成
		/// </summary>
		public override void Create() {
			// 一時キャッシュを登録
			var allDataManager = SMServiceLocator.Resolve<SMAllDataManager>();
			_tempCaches = allDataManager.Register( new SMTemporaryCacheDataManager() );

			_loaders.ForEach( pair => pair.Value.Setup() );
		}

		/// <summary>
		/// ● 全回数をリセット
		/// </summary>
		public void ResetAllCount()
			=> _loaders.ForEach( pair => pair.Value.ResetCount() );

		///------------------------------------------------------------------------------------------------
		/// ● 登録、解除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録
		/// </summary>
		public void Register( BaseSMDataLoader loader ) {
			_loaders[loader.GetType()] = loader;
		}

		///------------------------------------------------------------------------------------------------
		/// ● 取得
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得
		/// </summary>
		public T Get<T>() where T : BaseSMDataLoader
			=> _loaders.GetOrDefault( typeof( T ) ) as T;
	}
}