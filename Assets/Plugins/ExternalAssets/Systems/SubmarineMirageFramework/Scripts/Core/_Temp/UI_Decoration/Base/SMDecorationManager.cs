//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	///====================================================================================================
	/// <summary>
	/// ■ 装飾管理クラス
	///		NGUI、UGUIの文字描画の装飾を行う。
	/// </summary>
	///====================================================================================================
	public class SMDecorationManager : SMTask, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>実行型</summary>
		public override SMTaskRunType _type => SMTaskRunType.Dont;
		/// <summary>NGUI装飾</summary>
		public readonly SMNGUIDecoration _nGUI = new SMNGUIDecoration();
		/// <summary>UGUI装飾</summary>
		public readonly SMUGUIDecoration _uGUI = new SMUGUIDecoration();
		///------------------------------------------------------------------------------------------------
		/// ● 生成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMDecorationManager() {
			_disposables.AddFirst( () => {
				_nGUI.Dispose();
				_uGUI.Dispose();
			} );
		}
		/// <summary>
		/// ● 作成
		/// </summary>
		public override void Create() {
		}
	}
}