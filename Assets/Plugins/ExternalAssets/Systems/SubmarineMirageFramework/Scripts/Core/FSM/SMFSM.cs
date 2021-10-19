//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestFSM
namespace SubmarineMirage {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;



	public class SMFSM<TState> : SMStandardBase
		where TState : BaseSMState
	{
		[SMShowLine] object _owner	{ get; set; }
		[SMShowLine] public readonly string _name = typeof( SMFSM<TState> ).GetName();

		[SMShow] readonly Dictionary<Type, TState> _states = new Dictionary<Type, TState>();
		[SMShowLine] public TState _state	{ get; private set; }

		[SMShow] public bool _isInitialized	{ get; private set; }
		bool _isInternalActive				{ get; set; }

		SMSubject _fixedUpdateEvent	{ get; set; }
		SMSubject _updateEvent		{ get; set; }
		SMSubject _lateUpdateEvent	{ get; set; }

		[SMShow] readonly SMModifyler _modifyler = new SMModifyler( string.Empty );

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
			_modifyler._name = _name;
			_isActive = false;
			_asyncCancelerOnExit.Cancel( false );

			_disposables.AddFirst( () => {
				_modifyler.Dispose();

				_asyncCancelerOnExit.Dispose();
				_asyncCancelerOnDispose.Dispose();

				_states.ForEach( pair => pair.Value.Dispose() );
				_states.Clear();
				_state = null;

				var es = new SMSubject[] { _fixedUpdateEvent, _updateEvent, _lateUpdateEvent };
#if TestFSM
				SMLog.Debug( $"{this.GetName()} Updateイベント破棄 : {es.ToShowString()}", SMLogTag.FSM );
#endif
				es
					.Where( e => e != null )
					.Where( e => !e._isDispose )
					.ForEach( e => e.Remove( _name ) );
#if TestFSM
				SMLog.Debug( $"{this.GetName()} Updateイベント破棄 : {es.ToShowString()}", SMLogTag.FSM );
#endif
				_fixedUpdateEvent = null;
				_updateEvent = null;
				_lateUpdateEvent = null;

				_owner = null;

				_isInternalActive = false;
			} );
#if TestFSM
			_disposables.AddFirst( () => {
				SMLog.Debug( $"{this.GetName()}.{nameof( Dispose )} : start\n{this}", SMLogTag.FSM );
			} );
			_disposables.AddLast( () => {
				SMLog.Debug( $"{this.GetName()}.{nameof( Dispose )} : end\n{this}", SMLogTag.FSM );
			} );
			SMLog.Debug( $"{this.GetName()}() : \n{this}", SMLogTag.FSM );
#endif
		}


		public override void Dispose()
			=> base.Dispose();


		public void Setup( object owner, IEnumerable<TState> states,
							SMSubject fixedUpdateEvent = null, SMSubject updateEvent = null,
							SMSubject lateUpdateEvent = null
		) {
			CheckDisposeError( nameof( Setup ) );
			if ( _owner != null ) {
				throw new InvalidOperationException( $"{this.GetName()}.{nameof( Setup )} : 既に実行済\n{this}" );
			}

			_owner = owner;

			var task = _owner as SMTask;
			_fixedUpdateEvent	= fixedUpdateEvent	?? task?._fixedUpdateEvent;
			_updateEvent		= updateEvent		?? task?._updateEvent;
			_lateUpdateEvent	= lateUpdateEvent	?? task?._lateUpdateEvent;
#if TestFSM
			SMLog.Debug( $"{this.GetName()} Updateイベント設定 : " + string.Join( "\n",
				_fixedUpdateEvent,
				_updateEvent,
				_lateUpdateEvent
			), SMLogTag.FSM );
#endif
			_fixedUpdateEvent	?.AddLast( _name ).Subscribe( _ => FixedUpdateState() );
			_updateEvent		?.AddLast( _name ).Subscribe( _ => UpdateState() );
			_lateUpdateEvent	?.AddLast( _name ).Subscribe( _ => LateUpdateState() );
#if TestFSM
			SMLog.Debug( $"{this.GetName()} Updateイベント設定 : " + string.Join( "\n",
				_fixedUpdateEvent,
				_updateEvent,
				_lateUpdateEvent
			), SMLogTag.FSM );
#endif
			states.ForEach( s => {
				s.Setup( _owner, this );
				var type = s.GetType();
				_states[type] = s;
			} );
			_states.ForEach( pair => pair.Value._setupEvent.Run() );

			_isActive = true;
		}

		public void Setup( object owner, IEnumerable<Type> stateTypes,
							SMSubject fixedUpdateEvent = null, SMSubject updateEvent = null,
							SMSubject lateUpdateEvent = null
		) {
			var states = stateTypes.Select( t => t.Create<TState>() );
			Setup( owner, states, fixedUpdateEvent, updateEvent, lateUpdateEvent );
		}



		public IEnumerable<TState> GetStates() {
			CheckDisposeError( nameof( GetStates ) );

			return _states.Select( pair => pair.Value );
		}

		public TState GetState( Type stateType ) {
			CheckDisposeError( $"{nameof( GetState )}( {stateType.GetAboutName()} )" );

			return _states.GetOrDefault( stateType );
		}

		public T GetState<T>() where T : TState {
			CheckDisposeError( $"{nameof( GetState )}<{typeof( T ).GetAboutName()}>" );

			return GetState( typeof( T ) ) as T;
		}



		public void FixedUpdateState() {
#if TestFSM
//			SMLog.Debug( $"{this.GetName()}.{nameof( FixedUpdateState )}", SMLogTag.FSM );
#endif
			if ( _state == null )									{ return; }
			if ( _state._ranState < SMStateRunState.AsyncUpdate )	{ return; }

			_state._fixedUpdateEvent.Run();

			if ( _state._ranState == SMStateRunState.AsyncUpdate ) {
				_state._ranState = SMStateRunState.FixedUpdate;
#if TestFSM
				SMLog.Debug( $"{this.GetName()}.{nameof( FixedUpdateState )} : First Run\n{_state}",
					SMLogTag.FSM );
#endif
			}
		}

		public void UpdateState() {
#if TestFSM
//			SMLog.Debug( $"{this.GetName()}.{nameof( UpdateState )}", SMLogTag.FSM );
#endif
			if ( _state == null )									{ return; }
			if ( _state._ranState < SMStateRunState.FixedUpdate )	{ return; }

			_state._updateEvent.Run();

			if ( _state._ranState == SMStateRunState.FixedUpdate ) {
				_state._ranState = SMStateRunState.Update;
#if TestFSM
				SMLog.Debug( $"{this.GetName()}.{nameof( UpdateState )} : First Run\n{_state}",
					SMLogTag.FSM );
#endif
			}
		}

		public void LateUpdateState() {
#if TestFSM
//			SMLog.Debug( $"{this.GetName()}.{nameof( LateUpdateState )}", SMLogTag.FSM );
#endif
			if ( _state == null )								{ return; }
			if ( _state._ranState < SMStateRunState.Update )	{ return; }

			_state._lateUpdateEvent.Run();

			if ( _state._ranState == SMStateRunState.Update ) {
				_state._ranState = SMStateRunState.LateUpdate;
#if TestFSM
				SMLog.Debug( $"{this.GetName()}.{nameof( LateUpdateState )} : First Run\n{_state}",
					SMLogTag.FSM );
#endif
			}
		}



		public async UniTask ChangeState( Type stateType ) {
			CheckDisposeError( $"{nameof( ChangeState )}( {stateType?.GetAboutName()} )" );

			await _modifyler.Register(
				nameof( ChangeState ),
				SMModifyType.Single,
				async () => {
#if TestFSM
					SMLog.Debug( $"{this.GetName()}.{nameof( ChangeState )}( {stateType.GetAboutName()} ) : Run",
						SMLogTag.FSM );
#endif
					TState state = null;
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
#if TestFSM
					SMLog.Debug( $"{nameof( state )} : {state}", SMLogTag.FSM );
#endif


#if TestFSM
					SMLog.Debug( $"{nameof( _asyncCancelerOnExit )}.Cancel( false )", SMLogTag.FSM );
#endif
					_asyncCancelerOnExit.Cancel( false );

					if ( _state != null && _state._ranState != SMStateRunState.Exit ) {
#if TestFSM
						SMLog.Debug( $"{nameof( _state._exitEvent )}.Run", SMLogTag.FSM );
#endif
						_state._ranState = SMStateRunState.Exit;
						await _state._exitEvent.Run( _asyncCancelerOnDispose );
					}


					if ( _modifyler._isHaveData )	{ return; }

#if TestFSM
					SMLog.Debug( $"{nameof( _state )} = {state.GetAboutName()}", SMLogTag.FSM );
#endif
					_state = state;
					if ( _state == null )	{ return; }


#if TestFSM
					SMLog.Debug( $"{nameof( _asyncCancelerOnExit )}.Recreate", SMLogTag.FSM );
#endif
					_asyncCancelerOnExit.Recreate();

					if ( _state._ranState == SMStateRunState.Exit ) {
#if TestFSM
						SMLog.Debug( $"{nameof( _state._enterEvent )}.Run", SMLogTag.FSM );
#endif
						await _state._enterEvent.Run( _asyncCancelerOnExit );
						_state._ranState = SMStateRunState.Enter;
					}


					_isInitialized = true;

					if ( _modifyler._isHaveData )	{ return; }


					if ( _state._ranState == SMStateRunState.Enter ) {
						UTask.Void( async () => {
#if TestFSM
							SMLog.Debug( $"{nameof( _state._asyncUpdateEvent )}.Run", SMLogTag.FSM );
#endif
							_state._ranState = SMStateRunState.AsyncUpdate;
							await _state._asyncUpdateEvent.Run( _asyncCancelerOnExit );
						} );
					}
				}
			);
		}

		public UniTask ChangeState<T>() where T : TState
			=> ChangeState( typeof( T ) );
	}
}