//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 情報管理の基盤クラス
	///		各種情報管理クラスは、このクラスを継承する。
	/// </summary>
	///====================================================================================================
	public class BaseSMDataManager<TKey, TValue> : SMStandardBase, IBaseSMDataManager
		where TValue : BaseSMData
	{
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>名前</summary>
		[SMShow] public string _name	{ get; protected set; }
		/// <summary>イベント登録鍵</summary>
		[SMShow] protected readonly string _registerEventKey;
		/// <summary>全情報の辞書</summary>
		[SMShow] protected readonly Dictionary<TKey, TValue> _datas = new Dictionary<TKey, TValue>();

		public SMAsyncEvent _loadEvent	{ get; private set; } = new SMAsyncEvent();
		public SMAsyncEvent _saveEvent	{ get; private set; } = new SMAsyncEvent();

#region ToString
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文章変換を設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _datas ), i => _toStringer.DefaultValue( _datas, i, true ) );
		}
#endregion

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public BaseSMDataManager() {
			_name = typeof( TValue ).GetName();
			_registerEventKey = _name;

			_loadEvent.AddLast( _registerEventKey, async canceler => {
				foreach ( var pair in _datas ) {
					await pair.Value.Load();
				}
			} );
			_saveEvent.AddLast( _registerEventKey, async canceler => {
				foreach ( var pair in _datas ) {
					await pair.Value.Save();
				}
			} );

			_disposables.AddFirst( () => {
				_datas.ForEach( pair => pair.Value.Dispose() );
				_datas.Clear();

				_loadEvent.Dispose();
				_saveEvent.Dispose();
			} );
		}

		///------------------------------------------------------------------------------------------------
		/// ● 登録、解除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録
		/// </summary>
		public virtual void Register( TKey key, TValue data ) {
			if ( IsLoaded( key ) ) {
				throw new InvalidOperationException( string.Join( "\n",
					$"登録済みの値を再登録 : ",
					$"{nameof( key )} : {key}",
					$"{nameof( data )} last : {Get( key )}",
					$"{nameof( data )} new : {data}",
					$"{this}"
				) );
			}

			_datas[key] = data;
		}

		/// <summary>
		/// ● 登録解除
		/// </summary>
		public void Unregister( TKey key ) {
			_datas.Remove( key );
		}

		///------------------------------------------------------------------------------------------------
		/// ● 取得
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得
		/// </summary>
		public TValue Get( TKey key )
			=> _datas.GetOrDefault( key );

		/// <summary>
		/// ● 全取得
		/// </summary>
		public IEnumerable<TValue> GetAlls()
			=> _datas.Select( pair => pair.Value );

		/// <summary>
		/// ● 読込済か？
		/// </summary>
		public bool IsLoaded( TKey key )
			=> _datas.ContainsKey( key );
	}
}