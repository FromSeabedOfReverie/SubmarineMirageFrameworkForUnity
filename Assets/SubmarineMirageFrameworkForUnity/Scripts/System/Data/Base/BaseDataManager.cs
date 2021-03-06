//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Data {
	using System.Collections.Generic;
	using UniRx.Async;
	using KoganeUnityLib;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ 情報管理の基盤クラス
	///----------------------------------------------------------------------------------------------------
	///		各種情報管理クラスは、このクラスを継承する。
	/// </summary>
	///====================================================================================================
	public abstract class BaseDataManager<TKey, TValue> : IBaseDataManager where TValue : IBaseData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>全情報の辞書</summary>
		protected readonly Dictionary<TKey, TValue> _allData = new Dictionary<TKey, TValue>();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual void Register( TKey key, TValue data ) {
			if ( !_allData.ContainsKey( key ) ) {
				_allData[key] = data;
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録解除
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual void UnRegister( TKey key ) {
			_allData.Remove( key );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual TValue Get( TKey key ) {
			return _allData.GetOrDefault( key );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 全取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public Dictionary<TKey, TValue> GetAll() {
			return _allData;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual async UniTask Load() {
			foreach ( var pair in _allData ) {
				await pair.Value.Load();
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 保存
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual async UniTask Save() {
			foreach ( var pair in _allData ) {
				await pair.Value.Save();
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文字列に変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override string ToString() {
			return this.ToDeepString();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 解放
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual void Dispose() {
			_allData.Clear();
		}
	}
}