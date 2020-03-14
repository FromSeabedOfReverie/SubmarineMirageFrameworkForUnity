//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.FSM.New {
	using MultiEvent;


	// TODO : コメント追加、整頓


	public abstract class State<TFSM, TOwner> : IState<TFSM, TOwner>
		where TFSM : IFiniteStateMachine
		where TOwner : IFiniteStateMachineOwner<TFSM>
	{
		public TFSM _fsm		{ get; private set; }
		public TOwner _owner	{ get; private set; }

		public MultiAsyncEvent _initializeEvent		{ get; private set; } = new MultiAsyncEvent();
		public MultiAsyncEvent _enterEvent			{ get; private set; } = new MultiAsyncEvent();
		public MultiAsyncEvent _updateEvent			{ get; private set; } = new MultiAsyncEvent();
		public MultiSubject _updateDeltaEvent		{ get; private set; } = new MultiSubject();
		public MultiSubject _fixedUpdateDeltaEvent	{ get; private set; } = new MultiSubject();
		public MultiSubject _lateUpdateDeltaEvent	{ get; private set; } = new MultiSubject();
		public MultiAsyncEvent _exitEvent			{ get; private set; } = new MultiAsyncEvent();

		public void Set( TOwner owner ) {
			_fsm = owner._fsm;
			_owner = owner;
		}

		public virtual void Dispose() {
			_initializeEvent.Dispose();
			_enterEvent.Dispose();
			_updateEvent.Dispose();
			_updateDeltaEvent.Dispose();
			_fixedUpdateDeltaEvent.Dispose();
			_lateUpdateDeltaEvent.Dispose();
			_exitEvent.Dispose();
		}

		~State() => Dispose();
	}
}