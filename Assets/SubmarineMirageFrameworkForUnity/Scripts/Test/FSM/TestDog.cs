//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Test.FSM {
	using SubmarineMirageFramework.FSM;
	using SubmarineMirageFramework.Process;
	using Owner = TestDog;
	using FSM = TestAIManager<TestDog>;
	using State = SubmarineMirageFramework.FSM.State< TestDog, TestAIManager<TestDog> >;



	// 有限状態機械のテスト



	public class TestDog : MonoBehaviourProcess, IFiniteStateMachineOwner<FSM> {


		public FSM _fsm { get; private set; }


		protected override void Constructor() {
			base.Constructor();

			var states = new State[] {
				new WaitState<Owner>( this ),
				new WalkState<Owner>( this ),
				new RunState<Owner>( this ),
			};
			_fsm = new FSM( this, states );
		}
	}
}