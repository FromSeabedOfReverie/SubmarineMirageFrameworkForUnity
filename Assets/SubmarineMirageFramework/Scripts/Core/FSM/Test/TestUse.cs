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
		public abstract class State : SMState<OwnerFSM> {
			public void _State() {
				_fsm._owner._Owner();
				_fsm._OwnerFSM();
				_fsm._states.ForEach( pair => pair.Value._State() );
				_fsm.ChangeState( typeof( NormalState ) ).Forget();
				_fsm.ChangeState<NormalState>().Forget();
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
		public enum FSMType {
			Head,
			Body,
		}
		public class OwnerFSM : SMParallelFSM<Dragon, OwnerFSM, InternalFSM, FSMType> {
			public InternalFSM _headFSM	{ get; private set; }
			public InternalFSM _bodyFSM	{ get; private set; }
			public OwnerFSM( Dragon owner ) : base( owner, new Dictionary<FSMType, InternalFSM> {
				{
					FSMType.Head,
					new InternalFSM(
						typeof( HeadState ),
						new BaseState[] { new NormalHeadState(), new BiteHeadState(), }
					)
				},
				{
					FSMType.Body,
					new InternalFSM(
						typeof( BodyState ),
						new BaseState[] { new NormalBodyState(), new DeathBodyState(),
							new NormalHeadState(),						// コンパイルエラーにならない
//							new Dummy.FSM.Internal.State.NormalState(),	// エラー
						}
					)
				},
/*
				{
					FSMType.Body,
					new Dummy.FSM.Internal.InternalFSM()	// エラー
				},
*/
			} ) {
				_headFSM = Get( FSMType.Head );
				_bodyFSM = Get( FSMType.Body );
			}
			public void _OwnerFSM() {
				_owner._Owner();
				_fsms.ForEach( pair => pair.Value._BaseFSM() );
				_headFSM.ChangeState<BiteHeadState>().Forget();
				_headFSM.ChangeState<DeathBodyState>().Forget();	// コンパイルエラーにならない
			}
		}
	}
	namespace Dragon.FSM.Internal {
		using State;
		public class InternalFSM : SMInternalFSM<OwnerFSM, BaseState> {
			public InternalFSM( Type baseStateType, BaseState[] states ) : base( baseStateType, states ) {}
			public void _BaseFSM() {
				_fsm._owner._Owner();
				_fsm._OwnerFSM();
				_states.ForEach( pair => pair.Value._BaseState() );
			}
		}
	}
	namespace Dragon.FSM.Internal.State {
		public abstract class BaseState : SMState<InternalFSM> {
			public void _BaseState() {
				_fsm._fsm._owner._Owner();
				_fsm._fsm._OwnerFSM();
				_fsm._BaseFSM();
				_fsm._states.ForEach( pair => pair.Value._BaseState() );
				_fsm.ChangeState<BiteHeadState>().Forget();
				_fsm.ChangeState<DeathBodyState>().Forget();
			}
		}
		public abstract class HeadState : BaseState {
			public void _BaseHead() {
				_fsm._fsm._owner._Owner();
				_fsm._fsm._OwnerFSM();
				_fsm._BaseFSM();
				_fsm._states.ForEach( pair => pair.Value._BaseState() );
				_BaseState();
			}
		}
		public class NormalHeadState : HeadState {
			public void _NormalHead() {
				_fsm._fsm._owner._Owner();
				_fsm._fsm._OwnerFSM();
				_fsm._BaseFSM();
				_fsm._states.ForEach( pair => pair.Value._BaseState() );
				_BaseState();
			}
		}
		public class BiteHeadState : HeadState {
			public void _BiteHead() {
				_fsm._fsm._owner._Owner();
				_fsm._fsm._OwnerFSM();
				_fsm._BaseFSM();
				_fsm._states.ForEach( pair => pair.Value._BaseState() );
				_BaseState();
			}
		}
		public abstract class BodyState : BaseState {
			public void _BaseBody() {
				_fsm._fsm._owner._Owner();
				_fsm._fsm._OwnerFSM();
				_fsm._BaseFSM();
				_fsm._states.ForEach( pair => pair.Value._BaseState() );
				_BaseState();
			}
		}
		public class NormalBodyState : BodyState {
			public void _NormalBody() {
				_fsm._fsm._owner._Owner();
				_fsm._fsm._OwnerFSM();
				_fsm._BaseFSM();
				_fsm._states.ForEach( pair => pair.Value._BaseState() );
				_BaseState();
			}
		}
		public class DeathBodyState : BodyState {
			public void _DeathBody() {
				_fsm._fsm._owner._Owner();
				_fsm._fsm._OwnerFSM();
				_fsm._BaseFSM();
				_fsm._states.ForEach( pair => pair.Value._BaseState() );
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
		public enum FSMType {
			Body,
		}
		public class OwnerFSM : SMParallelFSM<Dummy, OwnerFSM, InternalFSM, FSMType> {
			public OwnerFSM( Dummy owner ) : base( owner, new Dictionary<FSMType, InternalFSM> {
				{ FSMType.Body, new InternalFSM() },
			} ) {}
		}
	}
	namespace Dummy.FSM.Internal {
		using State;
		public class InternalFSM : SMInternalFSM<OwnerFSM, BaseState> {
			public InternalFSM() : base( typeof( BaseState ), new BaseState[] {
				new NormalState(),
			} ) {}
		}
	}
	namespace Dummy.FSM.Internal.State {
		public abstract class BaseState : SMState<InternalFSM> {
		}
		public class NormalState : BaseState {
		}
	}
}