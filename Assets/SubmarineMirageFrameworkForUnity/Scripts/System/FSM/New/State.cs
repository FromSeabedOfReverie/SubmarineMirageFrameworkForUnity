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

		public readonly MultiAsyncEvent _initializeEvent = new MultiAsyncEvent();
		public readonly MultiAsyncEvent _enterEvent = new MultiAsyncEvent();
		public readonly MultiAsyncEvent _updateEvent = new MultiAsyncEvent();
		public readonly MultiSubject _updateDeltaEvent = new MultiSubject();
		public readonly MultiSubject _fixedUpdateDeltaEvent = new MultiSubject();
		public readonly MultiSubject _lateUpdateDeltaEvent = new MultiSubject();
		public readonly MultiAsyncEvent _exitEvent = new MultiAsyncEvent();

		public State( TOwner owner ) {
			_owner = owner;
			_initializeEvent.AddFirst( async cancel => {
				_fsm = _owner._fsm;
				await UniTaskUtility.DontWait( cancel );
			} );
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