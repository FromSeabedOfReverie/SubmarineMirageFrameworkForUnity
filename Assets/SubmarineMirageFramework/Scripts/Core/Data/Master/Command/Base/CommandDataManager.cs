//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using System.Collections.Generic;
	using UniRx.Async;
	using KoganeUnityLib;
	using Debug;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ 命令情報の管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public abstract class CommandDataManager<TKey, TData> : CSVDataManager<int, TData>, ICommandDataManager
		where TData : CSVData<int>, ICommandData, new()
	{
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>全情報の辞書</summary>
		protected readonly new Dictionary< TKey, List<TData> > _allData = new Dictionary< TKey, List<TData> >();
		/// <summary>辞書への登録鍵</summary>
		protected TKey _registerKey;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public CommandDataManager() : base( "", "", default, 0 ) {
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Register( TKey key, TData data ) {
			// 要素が無い場合、動的配列を作成
			if ( !_allData.ContainsKey( key ) ) {
				_allData[key] = new List<TData>();
			}
			_allData[key].Add( data );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public List<TData> Get( TKey key ) {
			return _allData.GetOrDefault( key, new List<TData>() );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 全取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public new Dictionary< TKey, List<TData> > GetAll() {
			return _allData;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Load() {
			await base.Load();

//			Log.Debug( _allData.ToDeepString() );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected override void Set( string fileName, int index, List<string> texts ) {
			// データを設定
			var data = new TData();
			data.Set( fileName, index, texts, _registerKey );

			// 登録鍵設定の場合、設定
			if ( data._isSetGroupKey ) {
				_registerKey = (TKey)data.GetGroupKey();

			// 通常命令の場合、登録
			} else {
				Register( _registerKey, data );
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 解放
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Dispose() {
			_allData.Clear();
			base.Dispose();
		}
	}
}