//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;
	using Modifyler;
	using Event;
	using Extension;
	using Utility;
	using Debug;



	public class SMFSM : SMLinkNode {
		[SMShowLine] public new SMFSM _previous {
			get => base._previous as SMFSM;
			set => base._previous = value;
		}
		[SMShowLine] public new SMFSM _next {
			get => base._next as SMFSM;
			set => base._next = value;
		}

		public ISMFSMOwner _owner	{ get; private set; }
		[SMShow] readonly Dictionary<Type, SMState> _states = new Dictionary<Type, SMState>();
		[SMShowLine] public SMState _state	{ get; set; }

		[SMShowLine] public Type _baseStateType			{ get; private set; }
		[SMShow] public Type _startStateType			{ get; set; }
		[SMShow] public string _registerEventName		{ get; private set; }
		[SMShow] public bool _isInitialEntered			{ get; set; }
		[SMShow] public bool _isLockBeforeInitialize	{ get; private set; }

		public bool _isInitialized	=> _owner?._isInitialized ?? false;
		public bool _isFinalize		=> _owner?._isFinalize ?? false;
		public bool _isOperable		=> _owner?._isOperable ?? false;
		public bool _isActive		=> _owner?._isActive ?? false;

		public readonly SMModifyler _modifyler = new SMModifyler( nameof( SMFSM ) );

		public SMAsyncEvent _selfInitializeEvent	=> _owner?._selfInitializeEvent;
		public SMAsyncEvent _initializeEvent		=> _owner?._initializeEvent;
		public SMSubject _enableEvent				=> _owner?._enableEvent;
		public SMSubject _fixedUpdateEvent			=> _owner?._fixedUpdateEvent;
		public SMSubject _updateEvent				=> _owner?._updateEvent;
		public SMSubject _lateUpdateEvent			=> _owner?._lateUpdateEvent;
		public SMSubject _disableEvent				=> _owner?._disableEvent;
		public SMAsyncEvent _finalizeEvent			=> _owner?._finalizeEvent;

		public readonly SMAsyncCanceler _asyncCancelerOnDisableAndExit	= new SMAsyncCanceler();
		public SMAsyncCanceler _asyncCancelerOnDispose					=> _owner?._asyncCancelerOnDispose;



#region ToString
		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _states ), i =>
				_toStringer.DefaultValue( _states, i, true ) );
			_toStringer.Add( nameof( _previous ), i => _toStringer.DefaultLineValue( _previous ) );
			_toStringer.Add( nameof( _next ), i => _toStringer.DefaultLineValue( _next ) );
		}
#endregion



		public SMFSM() {
// TODO : フラグ、ごちゃごちゃしてるので、修正
			_modifyler._isCanRunEvent = () => {
				if ( _isFinalize )		{ return true; }
				if ( !_isInitialized )	{ return false; }
				if ( !_isActive )		{ return false; }
				return true;
			};


			_disposables.AddLast( () => {
				_modifyler.Dispose();

				_asyncCancelerOnDisableAndExit.Dispose();

				GetStates().ForEach( s => s.Dispose() );
				_states.Clear();
				_state = null;

				_next?.Dispose();
				_previous = null;
				_next = null;
			} );
		}

		public override void Dispose() => base.Dispose();


		public void Setup( ISMFSMOwner owner, IEnumerable<SMState> states,
							Type baseStateType = null, Type startStateType = null,
							bool isInitialEnter = true, bool isLockBeforeInitialize = false
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
				_state = null;
				await GetStates().Select( s => s.Finalize() );
			} );

			_enableEvent.AddLast( _registerEventName ).Subscribe( _ => {
				_state?.Enable();
			} );
			_disableEvent.AddLast( _registerEventName ).Subscribe( _ => {
				_modifyler.Reset();
				_asyncCancelerOnDisableAndExit.Cancel();
				_state?.Disable();
			} );

			_fixedUpdateEvent.AddLast( _registerEventName ).Subscribe( _ => {
				_state?.FixedUpdate();
			} );
			_updateEvent.AddLast( _registerEventName ).Subscribe( _ => {
				_modifyler.Run().Forget();
				_state?.Update();
			} );
			_lateUpdateEvent.AddLast( _registerEventName ).Subscribe( _ => {
				_state?.LateUpdate();
			} );


			_isLockBeforeInitialize = isLockBeforeInitialize;
			if ( isInitialEnter )	{ InitialEnter( true ).Forget(); }
		}

		void SetupStates( IEnumerable<SMState> states, Type baseStateType, Type startStateType ) {
			_baseStateType = baseStateType ?? typeof( SMState );
			_registerEventName = $"{typeof( SMFSM )}<{_baseStateType.GetAboutName()}>";

			states.ForEach( state => {
				var type = state.GetType();
				if ( !type.IsInheritance( _baseStateType ) ) {
					throw new InvalidOperationException(
						$"基盤状態が違う、状態を指定 : {type}, {_baseStateType}" );
				}
				_states[type] = state;
			} );

			_startStateType = startStateType ?? GetStates().First().GetType();

			GetStates()
				.ForEach( s => s.Setup( _owner, this ) );
		}



		public static SMFSM Generate( ISMFSMOwner owner, SMFSMGenerateList generateList ) {
			SMFSM first = null;
			SMFSM last = null;
			generateList.ForEach( data => {
				var current = typeof( SMFSM ).Create<SMFSM>();
				data.CreateStates();
				current.Setup(
					owner, data._states, data._baseStateType, data._startStateType,
					data._isInitialEnter, data._isLockBeforeInitialize
				);

				if ( first == null )	{ first = current; }
				last?.LinkLast( current );
				last = current;
			} );

			return first;
		}



		public void LinkLast( SMFSM add ) {
			base.LinkLast( add );
			add._owner = _owner;
		}



		public IEnumerable<SMFSM> GetFSMs()
			=> GetAlls();

		public SMFSM GetFSM( Type baseStateType )
			=> GetAlls()
				.FirstOrDefault( fsm => fsm._baseStateType == baseStateType );

		public SMFSM GetFSM<T>() where T : SMState
			=> GetFSM( typeof( T ) );


		public IEnumerable<SMState> GetStates()
			=> _states
				.Select( pair => pair.Value );

		public SMState GetState( Type stateType )
			=> _states
				.GetOrDefault( stateType );

		public T GetState<T>() where T : SMState
			=> GetState( typeof( T ) ) as T;



		public new SMFSM GetFirst()
			=> base.GetFirst() as SMFSM;

		public new SMFSM GetLast()
			=> base.GetLast() as SMFSM;

		public new IEnumerable<SMFSM> GetAlls()
			=> base.GetAlls() as IEnumerable<SMFSM>;



		public async UniTask InitialEnter( bool isRunSelfOnly = false ) {
			if ( !isRunSelfOnly ) {
				await GetAlls().Select( fsm => fsm.InitialEnter( true ) );
				return;
			}

			await _modifyler.Register(
				nameof( InitialEnter ),
				SMModifyType.Normal,
				async () => {
					if ( _isFinalize )			{ return; }
					if ( _isInitialEntered )	{ return; }

					if ( _state != null ) {
						throw new InvalidOperationException(
							$"初期状態遷移前に、既に状態設定済み : {nameof( _state )}" );
					}
					SMState state = null;
					if ( _startStateType != null ) {
						state = GetState( _startStateType );
						if ( state == null ) {
							throw new InvalidOperationException(
								$"初期状態遷移に、未所持状態を指定 : {nameof( _startStateType )}" );
						}
					}

					_state = state;
					if ( _state == null )	{ return; }

					await _state.Enter();
					_state.Enable();
					_isInitialEntered = true;

					if ( !_owner._isInitialEnteredFSMs ) {
						_owner._isInitialEnteredFSMs =
							GetAlls().All( fsm => fsm._isInitialEntered );
					}

					if ( _modifyler._isHaveData )	{ return; }

					_state.UpdateAsync();
				}
			);
		}

		public async UniTask FinalExit( bool isRunSelfOnly = false ) {
			if ( !isRunSelfOnly ) {
				await GetAlls().Select( fsm => fsm.FinalExit( true ) );
				return;
			}

			await _modifyler.Register(
				nameof( FinalExit ),
				SMModifyType.Priority,
				async () => {
					_asyncCancelerOnDisableAndExit.Cancel();

					if ( _state != null ) {
						_state.Disable();
						await _state.Exit();
						_state = null;
					}

					_modifyler.Reset();
				}
			);
		}

		public async UniTask ChangeState( Type stateType ) {
			await _modifyler.Register(
				nameof( ChangeState ),
				SMModifyType.Single,
				async () => {
					if ( _isFinalize )	{ return; }

					if ( !_isInitialEntered ) {
						if ( _isLockBeforeInitialize ) {
							throw new InvalidOperationException( string.Join( "\n",
								$"初期遷移前の状態遷移は、ロック中 : {stateType}",
								nameof( _isLockBeforeInitialize )
							) );
						}
						_startStateType = stateType;
						return;
					}

					SMState state = null;
					if ( stateType != null ) {
						state = GetState( stateType );
						if ( state == null ) {
							throw new InvalidOperationException( $"状態遷移に、未所持状態を指定 : {stateType}" );
						}
					}

					_asyncCancelerOnDisableAndExit.Cancel();

					if ( _state != null ) {
						_state.Disable();
						await _state.Exit();
					}
					if ( _modifyler._isHaveData ) {
						_state = null;
						return;
					}

					_state = state;
					if ( _state == null )	{ return; }

					await _state.Enter();
					_state.Enable();
					if ( _modifyler._isHaveData )	{ return; }

					_state.UpdateAsync();
				}
			);
		}

		public UniTask ChangeState<T>() where T : SMState
			=> ChangeState( typeof( T ) );
	}
}