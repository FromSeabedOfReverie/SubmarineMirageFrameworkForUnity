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
	using MultiEvent;
	using Task;
	using Task.Behaviour;
	using Extension;
	using Utility;
	using Debug;
	using RunState = FSM.SMFSMRunState;



	// TODO : コメント追加、整頓



	public abstract class SMStateModifyData {
		protected ISMState _owner	{ get; private set; }
		protected SMStateModifyler _modifyler	{ get; private set; }
		public virtual void Set( ISMState owner ) {
			_owner = owner;
			_modifyler = _owner._modifyler;
		}
		public abstract UniTask Run();
	}
	public class SMStateModifyler {
		protected ISMState _owner	{ get; private set; }
		protected readonly LinkedList<SMStateModifyData> _data = new LinkedList<SMStateModifyData>();
		public SMStateModifyler( ISMState owner ) {
			_owner = owner;
		}
		public void Register( SMStateModifyData data ) {
			data.Set( _owner );
			_data.Enqueue( data );
		}
		public UniTask WaitRunning()
			=> UTask.WaitWhile( _owner._asyncCancelerOnChangeOrDisable, () => !_data.IsEmpty() );
	}
	public class EnterSMState : SMStateModifyData {
		public override async UniTask Run() {
			if ( _owner._runState == RunState.Enter )	{ return; }
			if ( _owner._runState == RunState.Update )	{ return; }
			await _owner._enterEvent.Run( _owner._asyncCancelerOnChangeOrDisable );
			_owner._runState = RunState.Enter;
		}
	}
	public class UpdateSMState : SMStateModifyData {
		public override async UniTask Run() {
			if ( _owner._runState == RunState.Exit )	{ return; }
			_owner._runState = RunState.Update;
			_owner._updateAsyncEvent.Run( _owner._asyncCancelerOnChangeOrDisable ).Forget();
			await UTask.DontWait();
		}
	}
	public class ExitSMState : SMStateModifyData {
		public override async UniTask Run() {
			if ( _owner._runState == RunState.Exit )	{ return; }
			await _owner._exitEvent.Run( _owner._asyncCancelerOnChangeOrDisable );
			_owner._runState = RunState.Exit;
		}
	}



	public abstract class SMFSMModifyData {
		protected ISMFSM _owner	{ get; private set; }
		protected SMFSMModifyler _modifyler	{ get; private set; }
		public virtual void Set( ISMFSM owner ) {
			_owner = owner;
			_modifyler = _owner._modifyler;
		}
		public abstract UniTask Run();
	}
	public class SMFSMModifyler {
		protected ISMFSM _owner	{ get; private set; }
		protected readonly LinkedList<SMFSMModifyData> _data = new LinkedList<SMFSMModifyData>();
		public SMFSMModifyler( ISMFSM owner ) {
			_owner = owner;
		}
		public void Register( SMFSMModifyData data ) {
			data.Set( _owner );
			_data.Enqueue( data );
		}
	}
	public class ChangeStateSMFSM : SMFSMModifyData {
		ISMState _state	{ get; set; }
		public ChangeStateSMFSM( ISMState state ) {
			_state = state;
		}
		public override async UniTask Run() {
			if ( _owner._state != null ) {
				_owner._state._modifyler.Register( new ExitSMState() );
				await _owner._state._modifyler.WaitRunning();
			}
			if ( _owner._ranState == SMTaskRunState.Finalize )	{ _state = null; }
			_owner._state = _state;
			if ( _state == null )	{ return; }

			_state._modifyler.Register( new EnterSMState() );
			await _state._modifyler.WaitRunning();

			if ( _owner._ranState == SMTaskRunState.Finalize ) {
				_owner._modifyler.Register( new ChangeStateSMFSM( null ) );
				return;
			}
			_state._modifyler.Register( new UpdateSMState() );
		}
	}



	public interface ISMFSMOwner<TOwnerFSM> : ISMBehaviour
		where TOwnerFSM : ISMOwnerFSM
	{
		TOwnerFSM _fsm { get; }
	}



	public interface ISMState {
		RunState _runState	{ get; set; }
		SMStateModifyler _modifyler	{ get; }
		SMMultiAsyncEvent _enterEvent		{ get; }
		SMMultiAsyncEvent _updateAsyncEvent	{ get; }
		SMMultiAsyncEvent _exitEvent		{ get; }
		SMTaskCanceler _asyncCancelerOnChangeOrDisable	{ get; }
	}
	public interface ISMParallelState<TOwnerFSM, TFSM> : ISMState
		where TOwnerFSM : ISMOwnerFSM
		where TFSM : ISMInternalFSM<TOwnerFSM>
	{
		TFSM _fsm	{ get; }
		void Set( TFSM fsm );
	}
	public abstract class SMParallelState<TOwnerFSM, TFSM> : ISMParallelState<TOwnerFSM, TFSM>
		where TOwnerFSM : ISMOwnerFSM
		where TFSM : ISMInternalFSM<TOwnerFSM>
	{
		public RunState _runState	{ get; set; }
		public SMStateModifyler _modifyler	{ get; protected set; }
		public SMMultiAsyncEvent _enterEvent		{ get; private set; }
		public SMMultiAsyncEvent _updateAsyncEvent	{ get; private set; }
		public SMMultiAsyncEvent _exitEvent			{ get; private set; }
		public SMTaskCanceler _asyncCancelerOnChangeOrDisable	{ get; private set; }
		public TFSM _fsm	{ get; private set; }
		public void Set( TFSM fsm ) {
			_fsm = fsm;
		}
	}
	public interface ISMSingleState<TOwnerFSM> : ISMState
		where TOwnerFSM : ISMOwnerFSM
	{
		TOwnerFSM _fsm	{ get; }
		void Set( TOwnerFSM fsm );
	}
	public abstract class SMSingleState<TOwnerFSM> : ISMSingleState<TOwnerFSM>
		where TOwnerFSM : ISMOwnerFSM
	{
		public RunState _runState	{ get; set; }
		public SMStateModifyler _modifyler	{ get; protected set; }
		public SMMultiAsyncEvent _enterEvent		{ get; private set; }
		public SMMultiAsyncEvent _updateAsyncEvent	{ get; private set; }
		public SMMultiAsyncEvent _exitEvent			{ get; private set; }
		public SMTaskCanceler _asyncCancelerOnChangeOrDisable	{ get; private set; }
		public TOwnerFSM _fsm	{ get; private set; }
		public void Set( TOwnerFSM fsm ) {
			_fsm = fsm;
		}
	}



	public interface ISMFSM {
		SMFSMModifyler _modifyler	{ get; }
	}
	public interface ISMOwnerFSM : ISMFSM {
	}


	public interface ISMInternalFSM<TOwnerFSM> : ISMFSM
		where TOwnerFSM : ISMOwnerFSM
	{
		TOwnerFSM _fsm	{ get; }
		void Set( TOwnerFSM fsm );
		UniTask ChangeState( Type stateType );
	}
	public abstract class SMInternalFSM<TOwnerFSM, TFSM, TState> : ISMInternalFSM<TOwnerFSM>
		where TOwnerFSM : ISMOwnerFSM
		where TFSM : ISMInternalFSM<TOwnerFSM>
		where TState : class, ISMParallelState<TOwnerFSM, TFSM>
	{
		public SMFSMModifyler _modifyler	{ get; private set; }
		public TOwnerFSM _fsm	{ get; private set; }
		public readonly Dictionary<Type, TState> _states = new Dictionary<Type, TState>();
		public SMInternalFSM( IEnumerable<TState> states ) {
			states.ForEach( s => {
				s.Set( (TFSM)(ISMInternalFSM<TOwnerFSM>)this );
				_states[s.GetType()] = s;
			} );
		}
		public void Set( TOwnerFSM fsm ) {
			_fsm = fsm;
		}
		public async UniTask ChangeState( Type stateType ) {
			if ( !( stateType is TState ) )	{ throw new Exception( "違うステートを入れた" ); }
			await UTask.DontWait();
		}
		public UniTask ChangeState<T>() where T : TState
			=> ChangeState( typeof( T ) );
	}


	public abstract class SMParallelFSM<TOwner, TOwnerFSM, TFSM> : ISMOwnerFSM
		where TOwner : ISMFSMOwner<TOwnerFSM>
		where TOwnerFSM : ISMOwnerFSM
		where TFSM : ISMInternalFSM<TOwnerFSM>
	{
		public SMFSMModifyler _modifyler	{ get; private set; }
		public TOwner _owner	{ get; private set; }
		public readonly Dictionary<Type, TFSM> _fsms = new Dictionary<Type, TFSM>();
		public SMParallelFSM( TOwner owner, IEnumerable< ISMInternalFSM<TOwnerFSM> > fsms ) {
			_owner = owner;
			fsms.ForEach( fsm => {
				fsm.Set( (TOwnerFSM)(ISMOwnerFSM)this );
				_fsms[fsm.GetType()] = (TFSM)fsm;
			} );
		}
		public T Get<T>() where T : ISMInternalFSM<TOwnerFSM>
			=> (T)(ISMInternalFSM<TOwnerFSM>)_fsms.GetOrDefault( typeof( T ) );
	}


	public abstract class SMSingleFSM<TOwner, TOwnerFSM, TState> : ISMOwnerFSM
		where TOwner : ISMFSMOwner<TOwnerFSM>
		where TOwnerFSM : ISMOwnerFSM
		where TState : class, ISMSingleState<TOwnerFSM>
	{
		public SMFSMModifyler _modifyler	{ get; private set; }
		public TOwner _owner	{ get; private set; }
		public readonly Dictionary<Type, TState> _states = new Dictionary<Type, TState>();
		public SMSingleFSM( TOwner owner, IEnumerable<TState> states ) {
			_owner = owner;
			states.ForEach( s => {
				s.Set( (TOwnerFSM)(ISMOwnerFSM)this );
				_states[s.GetType()] = s;
			} );
		}
		public async UniTask ChangeState( Type stateType ) {
			if ( !( stateType is TState ) )	{ throw new Exception( "違うステートを入れた" ); }
			await UTask.DontWait();
		}
		public UniTask ChangeState<T>() where T : TState
			=> ChangeState( typeof( T ) );
	}



	namespace Human {
		using FSM;
		public class Human : SMBehaviour, ISMFSMOwner<OwnerFSM> {
			public OwnerFSM _fsm	{ get; private set; }
			public Human() => _fsm = new OwnerFSM( this );
			public override void Create() {}
			public void _Owner() {
				_fsm._OwnerFSM();
			}
		}
	}
	namespace Human.FSM {
		using State;
		public class OwnerFSM : SMSingleFSM<Human, OwnerFSM, State.State> {
			public OwnerFSM( Human owner ) : base( owner, new State.State[] {
				new NormalState(),
				new DeathState(),
			} ) {}
			public void _OwnerFSM() {
				_owner._Owner();
				_states.ForEach( pair => pair.Value._State() );
			}
		}
	}
	namespace Human.FSM.State {
		public abstract class State : SMSingleState<OwnerFSM> {
			public void _State() {
				_fsm._owner._Owner();
				_fsm._OwnerFSM();
				_fsm._states.ForEach( pair => pair.Value._State() );
				_fsm.ChangeState( typeof( NormalState ) ).Forget();
				_fsm.ChangeState<NormalState>();
			}
		}
		public class NormalState : State {
			public void _Normal() {
				_fsm._owner._Owner();
				_fsm._OwnerFSM();
				_fsm._states.ForEach( pair => pair.Value._State() );
				_State();
			}
		}
		public class DeathState : State {
			public void _Death() {
				_fsm._owner._Owner();
				_fsm._OwnerFSM();
				_fsm._states.ForEach( pair => pair.Value._State() );
				_State();
			}
		}
	}



	namespace Dragon {
		using FSM;
		public class Dragon : SMBehaviour, ISMFSMOwner<OwnerFSM> {
			public OwnerFSM _fsm	{ get; private set; }
			public Dragon() => _fsm = new OwnerFSM( this );
			public override void Create() {}
			public void _Owner() {
				_fsm._OwnerFSM();
			}
		}
	}
	namespace Dragon.FSM {
		using Internal;
		using Internal.State;
		public class OwnerFSM
			: SMParallelFSM<
				Dragon,
				OwnerFSM,
				InternalFSM< ISMInternalFSM<OwnerFSM>, State< ISMInternalFSM<OwnerFSM> > >
			>
		{
			public HeadFSM _headFSM	{ get; private set; }
			public BodyFSM _bodyFSM	{ get; private set; }
			public OwnerFSM( Dragon owner ) : base( owner, new ISMInternalFSM<OwnerFSM>[] {
				new HeadFSM(),
				new BodyFSM(),
			} ) {
				_headFSM = Get<HeadFSM>();
				_bodyFSM = Get<BodyFSM>();
//				_bodyFSM = Get<Dummy.FSM.Internal.InternalFSM>();	// エラー
			}
			public void _OwnerFSM() {
				_owner._Owner();
				_fsms.ForEach( pair => pair.Value._BaseFSM() );

				_headFSM.ChangeState<BiteHeadState>();
//				_headFSM.ChangeState<DeathBodyState>();	// エラー
			}
		}
	}
	namespace Dragon.FSM.Internal {
		using State;
		public abstract class InternalFSM<TFSM, TState> : SMInternalFSM<OwnerFSM, TFSM, TState>
			where TFSM : ISMInternalFSM<OwnerFSM>
			where TState : State<TFSM>
		{
			public InternalFSM( TState[] states ) : base( states ) {}
			public void _BaseFSM() {
				_fsm._owner._Owner();
				_fsm._OwnerFSM();
				_states.ForEach( pair => pair.Value._BaseState() );
			}
		}
		public class HeadFSM : InternalFSM<HeadFSM, HeadState> {
			public HeadFSM() : base( new HeadState[] {
				new NormalHeadState(),
				new BiteHeadState(),
			} ) {}
			public void _HeadFSM() {
				_fsm._owner._Owner();
				_fsm._OwnerFSM();
				_states.ForEach( pair => pair.Value._BaseState() );
				_states.ForEach( pair => pair.Value._BaseHead() );
				_BaseFSM();
			}
		}
		public class BodyFSM : InternalFSM<BodyFSM, BodyState> {
			public BodyFSM() : base( new BodyState[] {
				new NormalBodyState(),
				new DeathBodyState(),
			} ) {}
			public void _BodyFSM() {
				_fsm._owner._Owner();
				_fsm._OwnerFSM();
				_states.ForEach( pair => pair.Value._BaseState() );
				_states.ForEach( pair => pair.Value._BaseBody() );
				_BaseFSM();
			}
		}
	}
	namespace Dragon.FSM.Internal.State {
		public abstract class State<TFSM> : SMParallelState<OwnerFSM, TFSM>
			where TFSM : ISMInternalFSM<OwnerFSM>
		{
			new InternalFSM< TFSM, State<TFSM> > _fsm
				=> (InternalFSM< TFSM, State<TFSM> >)(ISMInternalFSM<OwnerFSM>)base._fsm;
			public void _BaseState() {
				base._fsm._fsm._owner._Owner();
				base._fsm._fsm._OwnerFSM();
//				base._fsm._BaseFSM();	// 参照不可エラー
//				base._fsm._states.ForEach( pair => pair.Value._BaseState() );	// 参照不可エラー
				_fsm._BaseFSM();
				_fsm._states.ForEach( pair => pair.Value._BaseState() );
				base._fsm._fsm._fsms
					.Select( pair => pair.Value )
					.Where( fsm => fsm == (ISMInternalFSM<OwnerFSM>)base._fsm )
					.FirstOrDefault()
					?._BaseFSM();
				base._fsm._fsm._fsms
					.Select( pair => pair.Value )
					.Where( fsm => fsm == (ISMInternalFSM<OwnerFSM>)base._fsm )
					.FirstOrDefault()
					?._states.ForEach( pair => pair.Value._BaseState() );
				base._fsm.ChangeState( typeof( BiteHeadState ) );
				base._fsm._fsm._headFSM.ChangeState<BiteHeadState>();
			}
		}
		public abstract class HeadState : State<HeadFSM> {
			public void _BaseHead() {
				_fsm._fsm._owner._Owner();
				_fsm._fsm._OwnerFSM();
				_fsm._BaseFSM();
				_fsm._HeadFSM();
				_fsm._states.ForEach( pair => pair.Value._BaseHead() );
				_BaseState();
			}
		}
		public class NormalHeadState : HeadState {
			public void _NormalHead() {
				_fsm._fsm._owner._Owner();
				_fsm._fsm._OwnerFSM();
				_fsm._BaseFSM();
				_fsm._HeadFSM();
				_fsm._states.ForEach( pair => pair.Value._BaseHead() );
				_BaseState();
			}
		}
		public class BiteHeadState : HeadState {
			public void _BiteHead() {
				_fsm._fsm._owner._Owner();
				_fsm._fsm._OwnerFSM();
				_fsm._BaseFSM();
				_fsm._HeadFSM();
				_fsm._states.ForEach( pair => pair.Value._BaseHead() );
				_BaseState();
			}
		}
		public abstract class BodyState : State<BodyFSM> {
			public void _BaseBody() {
				_fsm._fsm._owner._Owner();
				_fsm._fsm._OwnerFSM();
				_fsm._BaseFSM();
				_fsm._BodyFSM();
				_fsm._states.ForEach( pair => pair.Value._BaseBody() );
				_BaseState();
			}
		}
		public class NormalBodyState : BodyState {
			public void _NormalBody() {
				_fsm._fsm._owner._Owner();
				_fsm._fsm._OwnerFSM();
				_fsm._BaseFSM();
				_fsm._BodyFSM();
				_fsm._states.ForEach( pair => pair.Value._BaseBody() );
				_BaseState();
			}
		}
		public class DeathBodyState : BodyState {
			public void _DeathBody() {
				_fsm._fsm._owner._Owner();
				_fsm._fsm._OwnerFSM();
				_fsm._BaseFSM();
				_fsm._BodyFSM();
				_fsm._states.ForEach( pair => pair.Value._BaseBody() );
				_BaseState();
			}
		}
	}



	namespace Dummy {
		using FSM;
		public class Dummy : SMBehaviour, ISMFSMOwner<OwnerFSM> {
			public OwnerFSM _fsm	{ get; private set; }
			public Dummy() => _fsm = new OwnerFSM( this );
			public override void Create() {}
		}
	}
	namespace Dummy.FSM {
		using Internal;
		public class OwnerFSM : SMParallelFSM<Dummy, OwnerFSM, InternalFSM> {
			public OwnerFSM( Dummy owner ) : base( owner, new ISMInternalFSM<OwnerFSM>[] {
				new InternalFSM(),
			} ) {}
		}
	}
	namespace Dummy.FSM.Internal {
		using State;
		public class InternalFSM : SMInternalFSM<OwnerFSM, InternalFSM, State.State> {
			public InternalFSM() : base( new State.State[] {
				new NormalState(),
			} ) {}
		}
	}
	namespace Dummy.FSM.Internal.State {
		public abstract class State : SMParallelState<OwnerFSM, InternalFSM> {
		}
		public class NormalState : State {
		}
	}
}