//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.New {
	using MultiEvent;
	using Extension;


	// TODO : コメント追加、整頓


	public abstract class State<TFSM, TOwner> : IState<TFSM, TOwner>
		where TFSM : IFiniteStateMachine
		where TOwner : IFiniteStateMachineOwner<TFSM>
	{
		public TFSM _fsm		{ get; private set; }
		public TOwner _owner	{ get; private set; }

		public MultiAsyncEvent _loadEvent			{ get; private set; } = new MultiAsyncEvent();
		public MultiAsyncEvent _initializeEvent		{ get; private set; } = new MultiAsyncEvent();
		public MultiAsyncEvent _enableEvent			{ get; private set; } = new MultiAsyncEvent();
		public MultiAsyncEvent _enterEvent			{ get; private set; } = new MultiAsyncEvent();
		public MultiAsyncEvent _updateEvent			{ get; private set; } = new MultiAsyncEvent();
		public MultiSubject _fixedUpdateDeltaEvent	{ get; private set; } = new MultiSubject();
		public MultiSubject _updateDeltaEvent		{ get; private set; } = new MultiSubject();
		public MultiSubject _lateUpdateDeltaEvent	{ get; private set; } = new MultiSubject();
		public MultiAsyncEvent _exitEvent			{ get; private set; } = new MultiAsyncEvent();
		public MultiAsyncEvent _disableEvent		{ get; private set; } = new MultiAsyncEvent();
		public MultiAsyncEvent _finalizeEvent		{ get; private set; } = new MultiAsyncEvent();

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public State() {
			_disposables.AddLast(
				_initializeEvent,
				_enterEvent,
				_updateEvent,
				_updateDeltaEvent,
				_fixedUpdateDeltaEvent,
				_lateUpdateDeltaEvent,
				_exitEvent
			);
		}

		public void Dispose() => _disposables.Dispose();

		~State() => Dispose();


		public void Set( TOwner owner ) {
			_fsm = owner._fsm;
			_owner = owner;
		}


		public override string ToString()
			=> $"{this.GetAboutName()}( _fsm : {_fsm.GetAboutName()}, _owner : {_owner.GetAboutName()} )";
	}
}