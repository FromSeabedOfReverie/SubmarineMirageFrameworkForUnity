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
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public interface ISMParallelFSMOwner<TOwnerFSM> : ISMBehaviour
		where TOwnerFSM : ISMParallelFSM
	{
		TOwnerFSM _fsm { get; }
	}



	public interface ISMState<TOwnerFSM, TFSM>
		where TOwnerFSM : ISMParallelFSM
		where TFSM : ISMInternalFSM<TOwnerFSM>
	{
		TFSM _fsm	{ get; }
		void Set( TFSM fsm );
	}
	public abstract class SMState<TOwnerFSM, TFSM> : ISMState<TOwnerFSM, TFSM>
		where TOwnerFSM : ISMParallelFSM
		where TFSM : ISMInternalFSM<TOwnerFSM>
	{
		public TFSM _fsm	{ get; private set; }
		public void Set( TFSM fsm ) {
			_fsm = fsm;
		}
	}



	public interface ISMInternalFSM<TOwnerFSM>
		where TOwnerFSM : ISMParallelFSM
	{
		TOwnerFSM _fsm	{ get; }
		void Set( TOwnerFSM fsm );
		UniTask ChangeState( Type stateType );
	}
	public abstract class SMInternalFSM<TOwnerFSM, TFSM, TState> : ISMInternalFSM<TOwnerFSM>
		where TOwnerFSM : ISMParallelFSM
		where TFSM : ISMInternalFSM<TOwnerFSM>
		where TState : class, ISMState<TOwnerFSM, TFSM>
	{
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



	public interface ISMParallelFSM {
	}
	public abstract class SMParallelFSM<TOwner, TOwnerFSM, TFSM> : ISMParallelFSM
		where TOwner : ISMParallelFSMOwner<TOwnerFSM>
		where TOwnerFSM : ISMParallelFSM
		where TFSM : ISMInternalFSM<TOwnerFSM>
	{
		public TOwner _owner	{ get; private set; }
		public readonly Dictionary<Type, TFSM> _fsms = new Dictionary<Type, TFSM>();
		public SMParallelFSM( TOwner owner, IEnumerable< ISMInternalFSM<TOwnerFSM> > fsms ) {
			_owner = owner;
			fsms.ForEach( fsm => {
				fsm.Set( (TOwnerFSM)(ISMParallelFSM)this );
				_fsms[fsm.GetType()] = (TFSM)fsm;
			} );
		}
		public T Get<T>() where T : ISMInternalFSM<TOwnerFSM>
			=> (T)(ISMInternalFSM<TOwnerFSM>)_fsms.GetOrDefault( typeof( T ) );
	}



	namespace Dragon {
		using FSM;

		public class Dragon : SMBehaviour, ISMParallelFSMOwner<OwnerFSM> {
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
		public abstract class State<TFSM> : SMState<OwnerFSM, TFSM>
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
}