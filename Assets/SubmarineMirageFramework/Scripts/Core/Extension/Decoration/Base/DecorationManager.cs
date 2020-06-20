//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using SMTask;
	using Singleton.New;
	///====================================================================================================
	/// <summary>
	/// ■ 装飾管理クラス
	///----------------------------------------------------------------------------------------------------
	///		NGUI、UGUIの文字描画の装飾を行う。
	/// </summary>
	///====================================================================================================
	public class DecorationManager : Singleton<DecorationManager> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>処理の型</summary>
		public override SMTaskType _type => SMTaskType.DontWork;
		/// <summary>NGUI装飾</summary>
		public NGUIDecoration _nGUI { get; private set; }
		/// <summary>UGUI装飾</summary>
		public UGUIDecoration _uGUI { get; private set; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public DecorationManager() {
			_nGUI = new NGUIDecoration();
			_uGUI = new UGUIDecoration();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 作成
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Create() {}
	}
}