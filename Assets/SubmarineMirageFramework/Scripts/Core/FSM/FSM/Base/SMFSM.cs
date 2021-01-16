//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.FSM {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using MultiEvent;
	using State;
	using State.Modifyler;
	using Modifyler;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMFSM<TOwner, TState> : BaseSMFSM
		where TOwner : IBaseSMFSMOwner
		where TState : BaseSMState
	{
		public override SMMultiAsyncEvent _selfInitializeEvent	=> _owner._selfInitializeEvent;
		public override SMMultiAsyncEvent _initializeEvent		=> _owner._initializeEvent;
		public override SMMultiSubject _enableEvent				=> _owner._enableEvent;
		public override SMMultiSubject _fixedUpdateEvent		=> _owner._fixedUpdateEvent;
		public override SMMultiSubject _updateEvent				=> _owner._updateEvent;
		public override SMMultiSubject _lateUpdateEvent			=> _owner._lateUpdateEvent;
		public override SMMultiSubject _disableEvent			=> _owner._disableEvent;
		public override SMMultiAsyncEvent _finalizeEvent		=> _owner._finalizeEvent;

		public TOwner _owner	{ get; private set; }
		public TState _state	{ get; set; }
		public readonly Dictionary<Type, TState> _states = new Dictionary<Type, TState>();
		Type _baseStateType	{ get; set; }

		public override BaseSMState _rawState {
			get { return _state; }
			set { _state = (TState)value; }
		}


		public SMFSM( IEnumerable<TState> states, Type baseStateType, Type startState = null ) {
			_baseStateType = baseStateType;

			_states = states.ToDictionary( state => {
				var type = state.GetType();
				if ( type.IsInheritance( _baseStateType ) ) {
					throw new InvalidOperationException( $"違う基盤状態を指定 : {type}, {_baseStateType}" );
				}
				state.Set( this );
				return type;
			} );
			if ( startState == null )	{ startState = _states.First().Value.GetType(); }


			_disposables.AddLast( () => {
				_states.ForEach( pair => pair.Value.Dispose() );
				_states.Clear();
				_state = null;
			} );


			_selfInitializeEvent.AddLast( _registerEventName, async canceler => {
				await _states.Select( pair => SMStateApplyer.SelfInitialize( pair.Value, canceler ) );
			} );
			_initializeEvent.AddLast( _registerEventName, async canceler => {
				await _states.Select( pair => SMStateApplyer.Initialize( pair.Value, canceler ) );
			} );
			_finalizeEvent.AddFirst( _registerEventName, async canceler => {
				await ChangeState( null );
				await _states.Select( pair => SMStateApplyer.Finalize( pair.Value, canceler ) );
			} );

			_enableEvent.AddLast( _registerEventName ).Subscribe( _ => SMStateApplyer.Enable( _state ) );
			_disableEvent.AddFirst( _registerEventName ).Subscribe( _ => SMStateApplyer.Disable( _state ) );

			_fixedUpdateEvent.AddLast( _registerEventName ).Subscribe( _ => SMStateApplyer.FixedUpdate( _state ) );
			_updateEvent.AddLast( _registerEventName ).Subscribe( _ => {
				if ( startState != null ) {
					ChangeState( startState ).Forget();
					startState = null;
				}
				SMStateApplyer.Update( _state );
			} );
			_lateUpdateEvent.AddLast( _registerEventName ).Subscribe( _ => SMStateApplyer.LateUpdate( _state ) );
		}


		public override void Set( IBaseSMFSMOwner owner ) {
			_owner = (TOwner)owner;
		}


		public async UniTask ChangeState( Type stateType ) {
			if ( stateType == null ) {
				await _modifyler.RegisterAndRun( new ChangeStateSMFSM( null ) );
				return;
			}

			if ( stateType.IsInheritance( _baseStateType ) ) {
				throw new InvalidOperationException( $"違う基盤状態を指定 : {stateType}, {_baseStateType}" );
			}
			var state = _states.GetOrDefault( stateType );
			if ( state == null ) {
				throw new InvalidOperationException( $"未所持状態を指定 : {stateType}" );
			}
			await _modifyler.RegisterAndRun( new ChangeStateSMFSM( state ) );
		}

		public UniTask ChangeState<T>() where T : TState
			=> ChangeState( typeof( T ) );


		public override void SetToString() {
			base.SetToString();
			_toStringer.SetValue( nameof( _owner ), i => _owner.GetAboutName() );
			_toStringer.SetValue( nameof( _states ), i => {
				var arrayI = StringSMUtility.IndentSpace( i + 1 );
				return "\n" + string.Join( ",\n", _states.Select( pair =>
					$"{arrayI}{pair.Key} : {pair.Value.ToLineString()}"
				) );
			} );
		}
	}
}