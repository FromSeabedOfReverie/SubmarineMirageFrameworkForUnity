//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestFSM
namespace SubmarineMirage.FSM {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;
	using Modifyler;
	using Event;
	using Task;
	using Extension;
	using Utility;
	using Debug;



	public class SMFSM : SMLinkNode {
		[SMShowLine] object _owner	{ get; set; }
		[SMShow] readonly Dictionary<Type, SMState> _states = new Dictionary<Type, SMState>();
		[SMShowLine] public SMState _state	{ get; private set; }

		[SMShowLine] public string _name	{ get; private set; }
		[SMShow] Type _baseStateType		{ get; set; }

		[SMShow] public bool _isInitialized	{ get; private set; }
		bool _isInternalActive				{ get; set; }

		SMSubject _fixedUpdateEvent	{ get; set; }
		SMSubject _updateEvent		{ get; set; }
		SMSubject _lateUpdateEvent	{ get; set; }

		[SMShow] readonly SMModifyler _modifyler = new SMModifyler( nameof( SMFSM ) );

		public readonly SMAsyncCanceler _asyncCancelerOnExit	= new SMAsyncCanceler();
		public readonly SMAsyncCanceler _asyncCancelerOnDispose	= new SMAsyncCanceler();



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
			set {
				CheckDisposeError( $"{nameof( _isAllActive )} = {value}" );

				GetFSMs().ForEach( fsm => fsm._isActive = value );
			}
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
				_asyncCancelerOnDispose.Dispose();

				_states.ForEach( pair => pair.Value.Dispose() );
				_states.Clear();
				_state = null;

				new SMSubject[] { _fixedUpdateEvent, _updateEvent, _lateUpdateEvent }
					.Where( e => e != null )
					.Where( e => !e._isDispose )
					.ForEach( e => e.Remove( _name ) );
				_fixedUpdateEvent = null;
				_updateEvent = null;
				_lateUpdateEvent = null;

				_owner = null;

				_isInternalActive = false;

				_next?.Dispose();
				_previous = null;
				_next = null;
			} );
		}

		public override void Dispose() => base.Dispose();

		public void Setup( object owner, IEnumerable<SMState> states, Type baseStateType = null,
							SMSubject fixedUpdateEvent = null, SMSubject updateEvent = null,
							SMSubject lateUpdateEvent = null
		) {
			CheckDisposeError( nameof( Setup ) );
			if ( _owner != null ) {
				throw new InvalidOperationException( $"既に実行済 : {nameof( Setup )}\n{this}" );
			}

			_owner = owner;
			_baseStateType = baseStateType ?? typeof( SMState );
			_name = $"{nameof( SMFSM )}<{_baseStateType.GetAboutName()}>";
			_modifyler._name = _name;

			var task = _owner as SMTask;
			_fixedUpdateEvent	= fixedUpdateEvent	?? task?._fixedUpdateEvent;
			_updateEvent		= updateEvent		?? task?._updateEvent;
			_lateUpdateEvent	= lateUpdateEvent	?? task?._lateUpdateEvent;
			_fixedUpdateEvent	?.AddLast( _name ).Subscribe( _ => FixedUpdateState() );
			_updateEvent		?.AddLast( _name ).Subscribe( _ => UpdateState() );
			_lateUpdateEvent	?.AddLast( _name ).Subscribe( _ => LateUpdateState() );

			states.ForEach( s => {
				var type = s.GetType();
				if ( !type.IsInheritance( _baseStateType ) ) {
					throw new InvalidOperationException( string.Join( "\n",
						$"異なる種類の状態は、設定不可 : ",
						$"{nameof( type )} : {type}",
						$"{nameof( _baseStateType )} : {_baseStateType}"
					) );
				}
				s.Setup( _owner, this );
				_states[type] = s;
			} );
		}



		public static SMFSM Generate( object owner, SMFSMGenerateList generateDatas,
										SMSubject fixedUpdateEvent = null, SMSubject updateEvent = null,
										SMSubject lateUpdateEvent = null
		) {
			SMFSM first = null;
			SMFSM last = null;
			generateDatas.ForEach( data => {
				data.CreateStates();
				var current = new SMFSM();
				current.Setup(
					owner, data._states, data._baseStateType, fixedUpdateEvent, updateEvent, lateUpdateEvent );

				if ( first == null )	{ first = current; }
				last?.LinkLast( current );
				last = current;
			} );

			return first;
		}



		public IEnumerable<SMFSM> GetFSMs() {
			CheckDisposeError( nameof( GetFSMs ) );

			foreach ( var node in GetAlls() ) {
				yield return node as SMFSM;
			}
		}

		public SMFSM GetFSM( Type baseStateType ) {
			CheckDisposeError( $"{nameof( GetFSM )}( {baseStateType.GetAboutName()} )" );

			return GetFSMs().FirstOrDefault( fsm => fsm._baseStateType == baseStateType );
		}

		public SMFSM GetFSM<T>() where T : SMState {
			CheckDisposeError( $"{nameof( GetFSM )}<{typeof( T ).GetAboutName()}>" );

			return GetFSM( typeof( T ) );
		}



		public IEnumerable<SMState> GetStates() {
			CheckDisposeError( nameof( GetStates ) );

			return _states.Select( pair => pair.Value );
		}

		public SMState GetState( Type stateType ) {
			CheckDisposeError( $"{nameof( GetState )}( {stateType.GetAboutName()} )" );

			return _states.GetOrDefault( stateType );
		}

		public T GetState<T>() where T : SMState {
			CheckDisposeError( $"{nameof( GetState )}<{typeof( T ).GetAboutName()}>" );

			return GetState( typeof( T ) ) as T;
		}



		public void FixedUpdateState() {
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

		public void UpdateState() {
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

		public void LateUpdateState() {
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
			CheckDisposeError( $"{nameof( ChangeState )}( {stateType.GetAboutName()} )" );

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

					_asyncCancelerOnExit.Cancel( false );
					if ( _state != null && _state._ranState != SMStateRunState.Exit ) {
						await _state._exitEvent.Run( _asyncCancelerOnDispose );
						_state._ranState = SMStateRunState.Exit;
					}
					if ( _modifyler._isHaveData )	{ return; }

					_state = state;
					if ( _state == null )	{ return; }

					_asyncCancelerOnExit.Recreate();
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

		public async UniTask ChangeState<T>() where T : SMState {
			CheckDisposeError( $"{nameof( ChangeState )}<{typeof( T ).GetAboutName()}>" );

			await ChangeState( typeof( T ) );
		}

		public async UniTask AllChangeNullState( SMTaskRunType type ) {
			CheckDisposeError( $"{nameof( AllChangeNullState )}( {type} )" );

			switch ( type ) {
				case SMTaskRunType.Sequential:
					foreach ( var fsm in GetFSMs().Reverse() ) {
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