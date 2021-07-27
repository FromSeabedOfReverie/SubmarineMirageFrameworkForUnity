//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using Cysharp.Threading.Tasks;
	using Base;
	using Utility;
	///====================================================================================================
	/// <summary>
	/// ■ 情報の基盤クラス
	///		各種情報クラスは、このクラスを継承する。
	/// </summary>
	///====================================================================================================
	public abstract class BaseSMData : SMLightBase, ISMSerializeData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		public bool _isDispose	{ get; private set; }

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public BaseSMData() {
		}

		/// <summary>
		/// ● 解放
		/// </summary>
		public override void Dispose() {
			if ( _isDispose )	{ return; }
			_isDispose = true;

			DisposeSub();
		}

		/// <summary>
		/// ● 解放（補助）
		/// </summary>
		protected virtual void DisposeSub() {

		}

		///------------------------------------------------------------------------------------------------
		/// ● 読み書き
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		public virtual UniTask Load()
			=> UTask.DontWait();

		/// <summary>
		/// ● 保存
		/// </summary>
		public virtual UniTask Save()
			=> UTask.DontWait();
	}
}