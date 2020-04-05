//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using System.Collections;
	///====================================================================================================
	/// <summary>
	/// ■ 状態のクラス
	///----------------------------------------------------------------------------------------------------
	///		有限状態機械への参照を所持する為、ジェネリッククラスとなっている。
	/// </summary>
	///====================================================================================================
	public class State<TOwner, TFSM>
		where TOwner : IFiniteStateMachineOwner<TFSM>
		where TFSM : IFiniteStateMachine
	{
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>運用者</summary>
		protected TOwner _owner	{ get; private set; }
		/// <summary>有限状態機械</summary>
		public TFSM _fsm	{ get; set; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public State( TOwner owner ) {
			_owner = owner;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 初期化
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Initialize() {
			// コンストラクタ中は、有限状態機械の作成中、代入中なので、初期化関数に分離する必要がある
			// キモい
			_fsm = _owner._fsm;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 入口（呼戻）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual IEnumerator OnEnter() {
			yield break;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 更新（呼戻）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual IEnumerator OnUpdate() {
			while ( true ) {
				yield return null;
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 更新（微分）（呼戻）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual void OnUpdateDelta() {
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 出口（呼戻）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual IEnumerator OnExit() {
			yield break;
		}
	}
}