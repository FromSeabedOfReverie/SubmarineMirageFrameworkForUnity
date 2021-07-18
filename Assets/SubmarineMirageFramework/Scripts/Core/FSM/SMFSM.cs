//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestFSM
namespace SubmarineMirage.FSM {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;
	using Modifyler;
	using Task;
	using Extension;
	using Utility;
	using Debug;



	public class SMFSM : SMLinkNode {
		[SMShowLine] ISMFSMOwner _owner	{ get; set; }
		[SMShow] readonly Dictionary<Type, SMState> _states = new Dictionary<Type, SMState>();
		[SMShowLine] public SMState _state	{ get; private set; }

		[SMShowLine] public string _name	{ get; private set; }
		[SMShow] Type _baseStateType		{ get; set; }

		[SMShow] public bool _isInitialized	{ get; private set; }
		bool _isInternalActive				{ get; set; }

		[SMShow] readonly SMModifyler _modifyler = new SMModifyler( nameof( SMFSM ) );

		public readonly SMAsyncCanceler _asyncCancelerOnExit = new SMAsyncCanceler();



		[SMShow] public bool _isActive {
			get => _isInternalActive;
			set {
				CheckDisposeError( $"{nameof( _isActive )} = {value}" );

				_isInternalActive = value;
				_modifyler._isLock = !_isInternalActive;
			}
		}
		public bool _isAllActive {
			get => GetFSMs().All( fsm => fsm._isActive );
			set => GetFSMs().ForEach( fsm => fsm._isActive = value );
		}
		public bool _isAllInitialized
			=> GetFSMs().All( fsm => fsm._isInitialized );



#region ToString
		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _owner ), i => _toStringer.DefaultLineValue( _owner ) );
			_toStringer.SetValue( nameof( _states ), i => _toStringer.DefaultValue( _states, i, true ) );
			_toStringer.SetValue( nameof( _state ), i => _toStringer.DefaultLineValue( _state ) );

			_toStringer.SetLineValue( nameof( _owner ), () => _owner.GetAboutName() );
			_toStringer.SetLineValue( nameof( _state ), () => _state.GetAboutName() );
		}
#endregion



		public SMFSM() {
			_isActive = true;

			_disposables.AddLast( () => {
				_modifyler.Dispose();

				_asyncCancelerOnExit.Dispose();

				GetStates().ForEach( s => s.Dispose() );
				_states.Clear();
				_state = null;
				_owner = null;

				_isInternalActive = false;

				_next?.Dispose();
				_previous = null;
				_next = null;
			} );
		}

		public override void Dispose() => base.Dispose();

		public void Setup( ISMFSMOwner owner, IEnumerable<SMState> states, Type baseStateType = null ) {
			_owner = owner;
			_baseStateType = baseStateType ?? typeof( SMState );
			_name = $"{typeof( SMFSM )}<{_baseStateType.GetAboutName()}>";
			_modifyler._name = _name;

			states.ForEach( state => {
				var type = state.GetType();
				if ( !type.IsInheritance( _baseStateType ) ) {
					throw new InvalidOperationException( string.Join( "\n",
						$"異なる種類の状態は、設定不可 : ",
						$"{nameof( type )} : {type}",
						$"{nameof( _baseStateType )} : {_baseStateType}"
					) );
				}
				state.Setup( _owner, this );
				_states[type] = state;
			} );

			_owner._fixedUpdateEvent	.AddLast( _name ).Subscribe( _ => FixedUpdateState() );
			_owner._updateEvent			.AddLast( _name ).Subscribe( _ => UpdateState() );
			_owner._lateUpdateEvent		.AddLast( _name ).Subscribe( _ => LateUpdateState() );
		}



		public static SMFSM Generate( ISMFSMOwner owner, SMFSMGenerateList generateDatas ) {
			SMFSM first = null;
			SMFSM last = null;
			generateDatas.ForEach( data => {
				data.CreateStates();
				var current = new SMFSM();
				current.Setup( owner, data._states, data._baseStateType );

				if ( first == null )	{ first = current; }
				last?.LinkLast( current );
				last = current;
			} );

			return first;
		}



		public IEnumerable<SMFSM> GetFSMs()
			=> GetAlls() as IEnumerable<SMFSM>;

		public SMFSM GetFSM( Type baseStateType )
			=> GetFSMs()
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



		void FixedUpdateState() {
			if ( _state == null )									{ return; }
			if ( _state._ranState < SMStateRunState.AsyncUpdate )	{ return; }

			_state._fixedUpdateEvent.Run();

			if ( _state._ranState == SMStateRunState.AsyncUpdate ) {
				_state._ranState = SMStateRunState.FixedUpdate;
#if TestFSM
				SMLog.Debug( $"{nameof( SMFSM )}.{nameof( FixedUpdateState )} : First Run\n{_state}" );
#endif
			}
		}

		void UpdateState() {
			if ( _state == null )									{ return; }
			if ( _state._ranState < SMStateRunState.FixedUpdate )	{ return; }

			_state._updateEvent.Run();

			if ( _state._ranState == SMStateRunState.FixedUpdate ) {
				_state._ranState = SMStateRunState.Update;
#if TestFSM
				SMLog.Debug( $"{nameof( SMFSM )}.{nameof( UpdateState )} : First Run\n{_state}" );
#endif
			}
		}

		void LateUpdateState() {
			if ( _state == null )								{ return; }
			if ( _state._ranState < SMStateRunState.Update )	{ return; }

			_state._lateUpdateEvent.Run();

			if ( _state._ranState == SMStateRunState.Update ) {
				_state._ranState = SMStateRunState.LateUpdate;
#if TestFSM
				SMLog.Debug( $"{nameof( SMFSM )}.{nameof( LateUpdateState )} : First Run\n{_state}" );
#endif
			}
		}



		public async UniTask ChangeState( Type stateType ) {
			await _modifyler.Register(
				nameof( ChangeState ),
				SMModifyType.Single,
				async () => {
					SMState state = null;
					if ( stateType != null ) {
						state = GetState( stateType );
						if ( state == null ) {
							throw new InvalidOperationException( string.Join( "\n",
								$"状態遷移に、未所持状態を指定 : \n",
								$"{nameof( stateType )} : {stateType}",
								$"{nameof( _states )} : {_states.ToShowString( 0, true, false, false )}"
							) );
						}
					}

					_asyncCancelerOnExit.Cancel();


					if ( _state != null && _state._ranState != SMStateRunState.Exit ) {
						await _state._exitEvent.Run( _asyncCancelerOnExit );
						_state._ranState = SMStateRunState.Exit;
					}
					if ( _modifyler._isHaveData )	{ return; }


					_state = state;
					if ( _state == null )	{ return; }

					if ( _state._ranState == SMStateRunState.Exit ) {
						await _state._enterEvent.Run( _asyncCancelerOnExit );
						_state._ranState = SMStateRunState.Enter;
					}
					_isInitialized = true;
					if ( _modifyler._isHaveData )	{ return; }


					if ( _state._ranState == SMStateRunState.Enter ) {
						UTask.Void( async () => {
							_state._ranState = SMStateRunState.AsyncUpdate;
							await _state._asyncUpdateEvent.Run( _asyncCancelerOnExit );
						} );
					}
				}
			);
		}

		public UniTask ChangeState<T>() where T : SMState
			=> ChangeState( typeof( T ) );

		public async UniTask AllChangeNullState( SMTaskRunType type ) {
			switch ( type ) {
				case SMTaskRunType.Sequential:
					foreach ( var fsm in GetFSMs() ) {
						await fsm.ChangeState( null );
					}
					break;

				case SMTaskRunType.Parallel:
					await GetFSMs().Select( fsm => fsm.ChangeState( null ) );
					break;
			}
		}
	}
}