//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Utility {
	using Base;
	using Service;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ 装飾管理クラス
	///		NGUI、UGUIの文字描画の装飾を行う。
	/// </summary>
	///====================================================================================================
	public class SMDecorationManager : SMStandardBase, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>NGUI装飾</summary>
		public SMNGUIDecoration _nGUI { get; private set; }
		/// <summary>UGUI装飾</summary>
		public SMUGUIDecoration _uGUI { get; private set; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public SMDecorationManager() {
			_nGUI = new SMNGUIDecoration();
			_uGUI = new SMUGUIDecoration();

			_disposables.AddLast( () => {
				_nGUI.Dispose();
				_uGUI.Dispose();
			} );
		}
	}
}