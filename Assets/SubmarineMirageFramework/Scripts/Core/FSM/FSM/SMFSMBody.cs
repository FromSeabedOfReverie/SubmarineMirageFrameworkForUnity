//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Base {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using SubmarineMirage.Base;
	using Event;
	using FSM.Modifyler;
	using FSM.Modifyler.Base;
	using FSM.State.Base;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMFSMBody : SMStandardBase {
		public BaseSMFSM _fsm			{ get; private set; }
		public IBaseSMFSMOwner _owner	{ get; private set; }
		[SMShow] Dictionary<Type, SMStateBody> _stateBodies	{ get; set; }
		[SMShowLine] public SMStateBody _stateBody	{ get; set; }

		public SMFSMBody _previous	{ get; set; }
		public SMFSMBody _next		{ get; set; }

		[SMShow] public SMFSMModifyler _modifyler	{ get; private set; }

		[SMShowLine] public Type _baseStateType	{ get; protected set; }
		[SMShow] public Type _startStateType	{ get; set; }
		[SMShow] public string _registerEventName	{ get; private set; }
		[SMShow] public bool _isInitialEntered		{ get; set; }

		public bool _isInitialized	=> _owner._isInitialized;
		public bool _isOperable	=> _owner._isOperable;
		public bool _isFinalizing	=> _owner._isFinalizing;
		public bool _isActive		=> _owner._isActive;

		public SMAsyncEvent _selfInitializeEvent	=> _owner._selfInitializeEvent;
		public SMAsyncEvent _initializeEvent		=> _owner._initializeEvent;
		public SMSubject _enableEvent				=> _owner._enableEvent;
		public SMSubject _fixedUpdateEvent		=> _owner._fixedUpdateEvent;
		public SMSubject _updateEvent				=> _owner._updateEvent;
		public SMSubject _lateUpdateEvent			=> _owner._lateUpdateEvent;
		public SMSubject _disableEvent			=> _owner._disableEvent;
		public SMAsyncEvent _finalizeEvent		=> _owner._finalizeEvent;

		public readonly SMAsyncCanceler _asyncCancelerOnDisableAndExit = new SMAsyncCanceler();
		public SMAsyncCanceler _asyncCancelerOnDispose	=> _owner._asyncCancelerOnDispose;



#region ToString
		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _stateBodies ), i =>
				_toStringer.DefaultValue( _stateBodies, i, true ) );
			_toStringer.Add( nameof( _previous ), i => _toStringer.DefaultLineValue( _previous ) );
			_toStringer.Add( nameof( _next ), i => _toStringer.DefaultLineValue( _next ) );
		}
#endregion



		public SMFSMBody( BaseSMFSM fsm ) {
			_fsm = fsm;
			_modifyler = new SMFSMModifyler( this );
			_registerEventName = _fsm.GetAboutName();


			_disposables.AddLast( () => {
				_modifyler.Dispose();
				_asyncCancelerOnDisableAndExit.Dispose();

				GetStates().ForEach( s => s._state.Dispose() );
				_stateBodies.Clear();
				_stateBody = null;

				_next?._fsm.Dispose();
				_previous = null;
				_next = null;
			} );
		}

		public override void Dispose() => base.Dispose();



		public void Setup( IBaseSMFSMOwner owner, IEnumerable<BaseSMState> states,
							Type baseStateType, Type startStateType, bool isInitialEnter
		) {
			_owner = owner;
			SetupStates( states, baseStateType, startStateType );


			_selfInitializeEvent.AddLast( _registerEventName, async canceler => {
				await GetStates().Select( s => s.SelfInitialize() );
			} );
			_initializeEvent.AddLast( _registerEventName, async canceler => {
				await GetStates().Select( s => s.Initialize() );
			} );
			_finalizeEvent.AddFirst( _registerEventName, async canceler => {
				_stateBody = null;
				await GetStates().Select( s => s.Finalize() );
			} );

			_enableEvent.AddLast( _registerEventName ).Subscribe( _ => {
				_stateBody?.Enable();
			} );
			_disableEvent.AddLast( _registerEventName ).Subscribe( _ => {
				_modifyler.Reset();
				StopAsyncOnDisableAndExit();
				_stateBody?.Disable();
			} );

			_fixedUpdateEvent.AddLast( _registerEventName ).Subscribe( _ => {
				_stateBody?.FixedUpdate();
			} );
			_updateEvent.AddLast( _registerEventName ).Subscribe( _ => {
				_modifyler.Run().Forget();
				_stateBody?.Update();
			} );
			_lateUpdateEvent.AddLast( _registerEventName ).Subscribe( _ => {
				_stateBody?.LateUpdate();
			} );


			if ( isInitialEnter )	{ InitialEnter( true ).Forget(); }
		}

		void SetupStates( IEnumerable<BaseSMState> states, Type baseStateType, Type startStateType ) {
			_baseStateType = baseStateType;

			_stateBodies = states.ToDictionary(
				state => {
					var type = state.GetType();
					if ( !type.IsInheritance( _baseStateType ) ) {
						throw new InvalidOperationException(
							$"基盤状態が違う、状態を指定 : {type}, {_baseStateType}" );
					}
					return type;
				},
				state => state._body
			);

			_startStateType = startStateType ?? GetStates().First()._state.GetType();

			GetStates().ForEach( s => s.Setup( _owner, this ) );
		}



		public void Link( SMFSMBody add ) {
			var last = GetLast();
			last._next = add;
			add._previous = last;
			add._owner = _owner;
		}



		public void StopAsyncOnDisableAndExit()
			=> _asyncCancelerOnDisableAndExit.Cancel();


		public async UniTask InitialEnter( bool isRunSelfOnly ) {
			if ( isRunSelfOnly ) {
				await _modifyler.RegisterAndRun( new InitialEnterSMFSM( _startStateType ) );
				return;
			}
			await GetBrothers().Select( fsm => fsm.InitialEnter( true ) );
		}

		public async UniTask FinalExit( bool isRunSelfOnly ) {
			if ( isRunSelfOnly ) {
				await _modifyler.RegisterAndRun( new FinalExitSMFSM() );
				return;
			}
			await GetBrothers().Select( fsm => fsm.FinalExit( true ) );
		}

		public UniTask ChangeState( Type stateType )
			=> _modifyler.RegisterAndRun( new ChangeStateSMFSM( stateType ) );



		public SMFSMBody GetFSM( Type baseStateType )
			=> GetBrothers()
				.FirstOrDefault( fsm => fsm._baseStateType == baseStateType );

		public IEnumerable<SMStateBody> GetStates()
			=> _stateBodies
				.Select( pair => pair.Value );

		public SMStateBody GetState( Type stateType )
			=> _stateBodies
				.GetOrDefault( stateType );



		public SMFSMBody GetFirst() {
			SMFSMBody current = null;
			for ( current = this; current._previous != null; current = current._previous )	{}
			return current;
		}

		public SMFSMBody GetLast() {
			SMFSMBody current = null;
			for ( current = this; current._next != null; current = current._next )	{}
			return current;
		}

		public IEnumerable<SMFSMBody> GetBrothers() {
			for ( var current = GetFirst(); current != null; current = current._next ) {
				yield return current;
			}
		}
	}
}