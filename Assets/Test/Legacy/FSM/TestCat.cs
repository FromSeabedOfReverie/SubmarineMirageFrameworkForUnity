//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using UniRx;
	using FSM;
	using Process;
	using Extension;
	using Owner = TestCat;
	using FSM = TestAIManager<TestCat>;
	using State = FSM.State< TestCat, TestAIManager<TestCat> >;



	// 有限状態機械のテスト



	public class TestCat : MonoBehaviourProcess, IFiniteStateMachineOwner<FSM> {


		public FSM _fsm { get; private set; }


		protected override void Constructor() {
			base.Constructor();

			var states = new State[] {
				new WaitState<Owner>( this ),
				new WalkState<Owner>( this ),
				new RunState<Owner>( this ),
//				new UIFade<Owner>( this ),
			};
			_fsm = new FSM( this, states );
			_fsm.Initialize();


			InputManager.s_instance.GetPressedEvent( InputManager.Event.Decide ).Subscribe( _ => {
				_fsm.ChangeState< WalkState<Owner> >();
			} )
			.AddTo( gameObject );

			InputManager.s_instance.GetPressedEvent( InputManager.Event.Quit ).Subscribe( _ => {
				_fsm.ChangeState< RunState<Owner> >();
			} )
			.AddTo( gameObject );
		}
	}
}