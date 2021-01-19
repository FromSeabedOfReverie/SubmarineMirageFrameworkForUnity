//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSMTest {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Task.Behaviour;
	using Extension;



	// TODO : コメント追加、整頓



	public interface ISMFSMOwner<TFSM> : ISMBehaviour
		where TFSM : BaseSMFSM
	{
		TFSM _fsm { get; }
	}



	public abstract class BaseSMFSM : BaseSMFSMModifylerOwner<BaseSMFSM, SMFSMModifyler, SMFSMModifyData> {
		public abstract BaseSMState _rawState	{ get; set; }
	}


	public abstract class BaseSMInternalFSM : BaseSMFSM {
		public abstract void Set( BaseSMFSM fsm );
	}
	public abstract class SMInternalFSM<TOwnerFSM, TState> : BaseSMInternalFSM
		where TOwnerFSM : BaseSMFSM
		where TState : BaseSMState
	{
		public override BaseSMState _rawState {
			get => _state;
			set => _state = (TState)value;
		}
		public TOwnerFSM _fsm	{ get; private set; }
		Type _baseStateType	{ get; set; }
		public TState _state	{ get; set; }
		public readonly Dictionary<Type, TState> _states = new Dictionary<Type, TState>();
		public SMInternalFSM( Type baseStateType, IEnumerable<TState> states ) {
			_baseStateType = baseStateType;
			_states = states.ToDictionary( s => s.GetType() );
			_states.ForEach( pair => {
				if ( pair.Value.GetType().IsInheritance( _baseStateType ) ) {
					throw new Exception( "違うステートを入れた" );
				}
				pair.Value.Set( this );
			} );
		}
		public override void Set( BaseSMFSM fsm ) {
			_fsm = (TOwnerFSM)fsm;
		}
		public async UniTask ChangeState( Type stateType ) {
			if ( stateType.IsInheritance( _baseStateType ) ) {
				throw new Exception( "違うステートを入れた" );
			}
			_modifyler.Register( new ChangeStateSMFSM( _states.GetOrDefault( stateType ) ) );
			await _modifyler.WaitRunning();
		}
		public async UniTask ChangeState<T>() where T : TState
			=> await ChangeState( typeof( T ) );
	}


	public abstract class SMParallelFSM<TOwner, TOwnerFSM, TInternalFSM, TEnum> : BaseSMFSM
		where TOwner : ISMFSMOwner<TOwnerFSM>
		where TOwnerFSM : BaseSMFSM
		where TInternalFSM : BaseSMInternalFSM
		where TEnum : Enum
	{
		public override BaseSMState _rawState {
			get => throw new Exception( $"存在しないエラー : {_rawState}" );
			set => throw new Exception( $"存在しないエラー : {_rawState}" );
		}
		public TOwner _owner	{ get; private set; }
		public readonly Dictionary<TEnum, TInternalFSM> _fsms = new Dictionary<TEnum, TInternalFSM>();
		public SMParallelFSM( TOwner owner, Dictionary<TEnum, TInternalFSM> fsms ) {
			_owner = owner;
			_fsms = fsms;
			_fsms.ForEach( pair => pair.Value.Set( this ) );
		}
		public TInternalFSM Get( TEnum key )
			=> _fsms.GetOrDefault( key );
	}


	public abstract class SMSingleFSM<TOwner, TFSM, TState> : BaseSMFSM
		where TOwner : ISMFSMOwner<TFSM>
		where TFSM : BaseSMFSM
		where TState : BaseSMState
	{
		public override BaseSMState _rawState {
			get => _state;
			set => _state = (TState)value;
		}
		public TOwner _owner	{ get; private set; }
		public TState _state	{ get; set; }
		public readonly Dictionary<Type, TState> _states = new Dictionary<Type, TState>();
		public SMSingleFSM( TOwner owner, IEnumerable<TState> states ) {
			_owner = owner;
			states.ForEach( s => {
				s.Set( this );
				_states[s.GetType()] = s;
			} );
		}
		public async UniTask ChangeState( Type stateType ) {
			if ( !( stateType is TState ) )	{ throw new Exception( "違うステートを入れた" ); }
			_modifyler.Register( new ChangeStateSMFSM( _states.GetOrDefault( stateType ) ) );
			await _modifyler.WaitRunning();
		}
		public async UniTask ChangeState<T>() where T : TState
			=> await ChangeState( typeof( T ) );
	}
}