//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using System;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Base;
	///====================================================================================================
	/// <summary>
	/// ■ 情報管理の基盤クラス
	///		各種情報管理クラスは、このクラスを継承する。
	/// </summary>
	///====================================================================================================
	public abstract class BaseSMDataManager<TKey, TValue> : SMStandardBase, ISMSerializeData
		where TValue : BaseSMData
	{
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>全情報の辞書</summary>
		protected readonly Dictionary<TKey, TValue> _datas = new Dictionary<TKey, TValue>();

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public BaseSMDataManager() {
			_disposables.AddFirst( () => {
				_datas.ForEach( pair => pair.Value.Dispose() );
				_datas.Clear();
			} );
		}

		///------------------------------------------------------------------------------------------------
		/// ● 登録、解除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録
		/// </summary>
		public virtual void Register( TKey key, TValue data ) {
			if ( _datas.ContainsKey( key ) ) {
				throw new InvalidOperationException( string.Join( "\n",
					$"登録済みの値を再登録 : ",
					$"{nameof( key )} : {key}",
					$"{nameof( data )} last : {data}",
					$"{nameof( data )} new : {data}",
					$"{this}"
				) );
			}

			_datas[key] = data;
		}

		/// <summary>
		/// ● 登録解除
		/// </summary>
		public virtual void Unregister( TKey key ) {
			_datas.Remove( key );
		}

		///------------------------------------------------------------------------------------------------
		/// ● 取得
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得
		/// </summary>
		public virtual TValue Get( TKey key )
			=> _datas.GetOrDefault( key );

		///------------------------------------------------------------------------------------------------
		/// ● 読み書き
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		public virtual async UniTask Load() {
			foreach ( var pair in _datas ) {
				await pair.Value.Load();
			}
		}

		/// <summary>
		/// ● 保存
		/// </summary>
		public virtual async UniTask Save() {
			foreach ( var pair in _datas ) {
				await pair.Value.Save();
			}
		}
	}
}