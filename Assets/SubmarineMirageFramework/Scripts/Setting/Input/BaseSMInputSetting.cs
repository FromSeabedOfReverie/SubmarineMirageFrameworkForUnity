//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Setting {
	using Base;
	using Service;
	///====================================================================================================
	/// <summary>
	/// ■ 入力設定の基盤クラス
	/// </summary>
	///====================================================================================================
	public abstract class BaseSMInputSetting : SMStandardBase, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		protected SMInputManager _inputManager	{ get; private set; }

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public BaseSMInputSetting() {
		}

		/// <summary>
		/// ● 設定
		/// </summary>
		public virtual void Setup( SMInputManager inputManager ) {
			_inputManager = inputManager;
		}
	}
}