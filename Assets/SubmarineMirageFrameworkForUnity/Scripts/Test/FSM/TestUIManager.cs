//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Test.FSM {
	using System.Collections;
	using SubmarineMirageFramework.FSM;



	// 有限状態機械のテスト



	public class TestUIManager<TOwner> : FiniteStateMachine< TOwner, TestUIManager<TOwner> >
		where TOwner : IFiniteStateMachineOwner< TestUIManager<TOwner> >
	{

		public bool _isActive;

		public TestUIManager( TOwner owner, State< TOwner, TestUIManager<TOwner> >[] states )
				: base( owner, states )
		{
		}
	}


	public class UIFade<TOwner> : State< TOwner, TestUIManager<TOwner> >
		where TOwner : IFiniteStateMachineOwner< TestUIManager<TOwner> >
	{

		public UIFade( TOwner owner ) : base( owner ) {
		}

		public override IEnumerator OnEnter() {
			var s = _owner._fsm._state;
			_owner._fsm._isActive = false;
			yield break;
		}
	}
}