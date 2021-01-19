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
	using Task;
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
		public override bool _isInitialized	=> _owner._isInitialized;
		public override bool _isOperable	=> _owner._isOperable;
		public override bool _isFinalizing	=> _owner._isFinalizing;
		public override bool _isActive		=> _owner._isActive;
		public override bool _isInitialEntered	{ get; set; }

		public override SMMultiAsyncEvent _selfInitializeEvent	=> _owner._selfInitializeEvent;
		public override SMMultiAsyncEvent _initializeEvent		=> _owner._initializeEvent;
		public override SMMultiSubject _enableEvent				=> _owner._enableEvent;
		public override SMMultiSubject _fixedUpdateEvent		=> _owner._fixedUpdateEvent;
		public override SMMultiSubject _updateEvent				=> _owner._updateEvent;
		public override SMMultiSubject _lateUpdateEvent			=> _owner._lateUpdateEvent;
		public override SMMultiSubject _disableEvent			=> _owner._disableEvent;
		public override SMMultiAsyncEvent _finalizeEvent		=> _owner._finalizeEvent;

		public override SMTaskCanceler _asyncCancelerOnDisable	=> _owner._asyncCancelerOnDisable;
		public override SMTaskCanceler _asyncCancelerOnDispose	=> _owner._asyncCancelerOnDispose;

		public TOwner _owner	{ get; private set; }
		public TState _state	{ get; set; }
		public readonly Dictionary<Type, TState> _states = new Dictionary<Type, TState>();
		Type _startStateType	{ get; set; }

		public override BaseSMState _rawState {
			get => _state;
			set => _state = (TState)value;
		}


		public SMFSM( IEnumerable<TState> states, Type startStateType = null ) {
			_states = states.ToDictionary( state => state.GetType() );
			_startStateType = startStateType ?? _states.First().Value.GetType();

			_disposables.AddLast( () => {
				_states.ForEach( pair => pair.Value.Dispose() );
				_states.Clear();
				_state = null;
			} );
		}


		public override void Set( IBaseSMFSMOwner owner ) {
			_owner = (TOwner)owner;
			_states.ForEach( pair => pair.Value.Set( this ) );

			_selfInitializeEvent.AddLast( _registerEventName, async canceler => {
				await _states.Select( pair => SMStateApplyer.SelfInitialize( pair.Value, canceler ) );
			} );
			_initializeEvent.AddLast( _registerEventName, async canceler => {
				await _states.Select( pair => SMStateApplyer.Initialize( pair.Value, canceler ) );
			} );
			_finalizeEvent.AddFirst( _registerEventName, async canceler => {
// 終了は、何処に置く？
				await _modifyler.RegisterAndRun( new FinalExitSMFSM() );
				await _states.Select( pair => SMStateApplyer.Finalize( pair.Value, canceler ) );
			} );

			_enableEvent.AddLast( _registerEventName ).Subscribe( _ => {
				SMStateApplyer.Enable( _state );
			} );
			_disableEvent.AddFirst( _registerEventName ).Subscribe( _ => {
// TODO : 後ろの変更データを削除するタイミングは？
				_modifyler.UnregisterAll();
				SMStateApplyer.Disable( _state );
			} );

			_fixedUpdateEvent.AddLast( _registerEventName ).Subscribe( _ => SMStateApplyer.FixedUpdate( _state ) );
			_updateEvent.AddLast( _registerEventName ).Subscribe( _ => {
				if ( _startStateType != null ) {
					_modifyler.Register( new InitialEnterSMFSM( _startStateType ) );
					_startStateType = null;
				}
				SMStateApplyer.Update( _state );
			} );
			_lateUpdateEvent.AddLast( _registerEventName ).Subscribe( _ => SMStateApplyer.LateUpdate( _state ) );
		}


		public override BaseSMState GetRawState( Type stateType )
			=> _states.GetOrDefault( stateType );


		public UniTask ChangeState( Type stateType )
			=> _modifyler.RegisterAndRun( new ChangeStateSMFSM( stateType ) );

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