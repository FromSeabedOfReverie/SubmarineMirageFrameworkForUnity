//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Utility {
	using Process;
	///====================================================================================================
	/// <summary>
	/// ■ デバッグ時のみの表示物のクラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class DevelopOnlyObject : MonoBehaviourProcess {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
#if !DEVELOP
		/// <summary>登録するか？</summary>
		public override bool _isRegister => false;
		/// <summary>中心処理初期化後まで待機するか？</summary>
		public override bool _isWaitInitializedCoreProcesses => false;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ（疑似的）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected override void Constructor() {
			gameObject.SetActive( false );
			base.Constructor();
		}
#endif
	}
}