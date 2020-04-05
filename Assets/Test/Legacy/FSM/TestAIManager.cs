//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Test {
	using System.Collections;
	using UnityEngine;
	using FSM;



	// 有限状態機械のテスト



	public class TestAIManager<TOwner> : FiniteStateMachine< TOwner, TestAIManager<TOwner> >
		where TOwner : IFiniteStateMachineOwner< TestAIManager<TOwner> >
	{

		public bool _isDeath;

		public TestAIManager( TOwner owner, State< TOwner, TestAIManager<TOwner> >[] states )
				: base( owner, states )
		{
		}
	}


	public class WaitState<TOwner> : State< TOwner, TestAIManager<TOwner> >
		where TOwner : IFiniteStateMachineOwner< TestAIManager<TOwner> >
	{

		public WaitState( TOwner owner ) : base( owner ) {
		}

		public override IEnumerator OnEnter() {
			var s = _owner._fsm._state;
			_owner._fsm._isDeath = false;
			yield break;
		}
	}


	public class WalkState<TOwner> : State< TOwner, TestAIManager<TOwner> >
		where TOwner : IFiniteStateMachineOwner< TestAIManager<TOwner> >
	{

		public WalkState( TOwner owner ) : base( owner ) {
		}

		public override IEnumerator OnEnter() {
			yield return base.OnEnter();
			yield return new WaitForSeconds( 1 );
		}

		public override IEnumerator OnExit() {
			yield return base.OnExit();
			yield return new WaitForSeconds( 1 );
		}
	}


	public class RunState<TOwner> : State< TOwner, TestAIManager<TOwner> >
		where TOwner : IFiniteStateMachineOwner< TestAIManager<TOwner> >
	{

		public RunState( TOwner owner ) : base( owner ) {
		}

		public override IEnumerator OnEnter() {
			yield return base.OnEnter();
			yield return new WaitForSeconds( 1 );
		}

		public override IEnumerator OnExit() {
			yield return base.OnExit();
			yield return new WaitForSeconds( 1 );
		}
	}
}