//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.FSM.New {
	using System;
	using MultiEvent;
	using Utility;


	// TODO : コメント追加、整頓


	public abstract class State<TOwner, TFSM> : IDisposable
		where TOwner : IFiniteStateMachineOwner<TFSM>
		where TFSM : IFiniteStateMachine
	{
		protected TOwner _owner	{ get; private set; }
		protected TFSM _fsm	{ get; private set; }

		public MultiAsyncEvent _initializeEvent		{ get; protected set; }
		public MultiAsyncEvent _enterEvent			{ get; protected set; }
		public MultiAsyncEvent _updateEvent			{ get; protected set; }
		public MultiSubject _updateDeltaEvent		{ get; protected set; }
		public MultiSubject _fixedUpdateDeltaEvent	{ get; protected set; }
		public MultiSubject _lateUpdateDeltaEvent	{ get; protected set; }
		public MultiAsyncEvent _exitEvent			{ get; protected set; }

		public State( TOwner owner ) {
			_owner = owner;
			_initializeEvent = new MultiAsyncEvent();
			_initializeEvent.AddFirst( async cancel => {
				_fsm = _owner._fsm;
				await UniTaskUtility.DontWait( cancel );
			} );
			_enterEvent = new MultiAsyncEvent();
			_updateEvent = new MultiAsyncEvent();
			_updateDeltaEvent = new MultiSubject();
			_fixedUpdateDeltaEvent = new MultiSubject();
			_lateUpdateDeltaEvent = new MultiSubject();
			_exitEvent = new MultiAsyncEvent();
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

		~State() {
			Dispose();
		}
	}
}