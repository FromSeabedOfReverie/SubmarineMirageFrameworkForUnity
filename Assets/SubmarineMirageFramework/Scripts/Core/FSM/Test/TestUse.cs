//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Test {
	using System;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Task;
	using FSM;
	using FSM.State;



	// TODO : コメント追加、整頓



	public class Human : SMBehaviour, ISMFSMOwner<HumanFSM> {
		public bool _isInitialEnteredFSMs	{ get; set; }
		public HumanFSM _fsm	{ get; private set; }
		public override void Create() {
			_fsm = new HumanFSM( this );
		}
		public void _Owner() {
			_fsm._FSM();
		}
	}
	public class HumanFSM : SMFSM<Human, HumanFSM, HumanState> {
		public HumanFSM( Human owner ) {
			Setup( owner, new HumanState[] { new NormalHumanState(), new DeathHumanState() } );
		}
		public void _FSM() {
			_owner._Owner();
			GetStates().ForEach( s => s._State() );
		}
	}
	public abstract class HumanState : SMState<Human, HumanFSM> {
		public void _State() {
			_fsm._owner._Owner();
			_fsm._FSM();
			_fsm.GetStates().ForEach( s => s._State() );
			_fsm.ChangeState( typeof( NormalHumanState ) ).Forget();
			_fsm.ChangeState<NormalHumanState>().Forget();
		}
	}
	public class NormalHumanState : HumanState {
		public void _Normal() {
			_fsm._owner._Owner();
			_fsm._FSM();
			_fsm.GetStates().ForEach( s => s._State() );
			_State();
		}
	}
	public class DeathHumanState : HumanState {
		public void _Death() {
			_fsm._owner._Owner();
			_fsm._FSM();
			_fsm.GetStates().ForEach( s => s._State() );
			_State();
		}
	}



	public class Dragon : SMBehaviour, ISMFSMOwner<DragonFSM> {
		public bool _isInitialEnteredFSMs	{ get; set; }
		public DragonFSM _fsm	{ get; private set; }
		public DragonFSM _headFSM	{ get; private set; }
		public DragonFSM _bodyFSM	{ get; private set; }
		public override void Create() {
			_fsm = DragonFSM.Generate( this,
				new SMFSMGenerateList<DragonState> {
					{
						new DragonState[] {
							new NormalDragonHeadState(),
							new BiteDragonHeadState(),
						},
						typeof( DragonHeadState )
					}, {
						new DragonState[] {
							new NormalDragonBodyState(),
							new DeathDragonBodyState(),
							new NormalDragonHeadState(),	// コンパイルエラーにならない
//							new NormalDummyState(),			// エラー
						},
						typeof( DragonBodyState )
					}, {
						new Type[] {
							typeof( NormalDummyState ),	// コンパイルエラーにならない
						},
						typeof( DummyState )	// コンパイルエラーにならない
					}
				}
			);
			_headFSM = _fsm.GetFSM<DragonHeadState>();
			_bodyFSM = _fsm.GetFSM<DragonBodyState>();
		}
		public void _Owner() {
			_fsm._FSM();
			_headFSM.ChangeState<BiteDragonHeadState>().Forget();
			_headFSM.ChangeState<DeathDragonBodyState>().Forget();	// コンパイルエラーにならない
		}
	}
	public class DragonFSM : SMFSM<Dragon, DragonFSM, DragonState> {
		public void _FSM() {
			_owner._Owner();
			GetFSMs().ForEach( fsm => fsm._FSM() );
			GetStates().ForEach( s => s._BaseState() );
		}
	}
	public abstract class DragonState : SMState<Dragon, DragonFSM> {
		public void _BaseState() {
			_owner._Owner();
			_fsm._FSM();
			_fsm.GetStates().ForEach( s => s._BaseState() );
			_fsm.ChangeState<BiteDragonHeadState>().Forget();
			_fsm.ChangeState<DeathDragonBodyState>().Forget();
		}
	}
	public abstract class DragonHeadState : DragonState {
		public void _BaseHead() {
			_owner._Owner();
			_fsm._FSM();
			_fsm.GetStates().ForEach( s => s._BaseState() );
			_BaseState();
		}
	}
	public class NormalDragonHeadState : DragonHeadState {
		public void _NormalHead() {
			_owner._Owner();
			_fsm._FSM();
			_fsm.GetStates().ForEach( s => s._BaseState() );
			_BaseState();
		}
	}
	public class BiteDragonHeadState : DragonHeadState {
		public void _BiteHead() {
			_owner._Owner();
			_fsm._FSM();
			_fsm.GetStates().ForEach( s => s._BaseState() );
			_BaseState();
		}
	}
	public abstract class DragonBodyState : DragonState {
		public void _BaseBody() {
			_owner._Owner();
			_fsm._FSM();
			_fsm.GetStates().ForEach( s => s._BaseState() );
			_BaseState();
		}
	}
	public class NormalDragonBodyState : DragonBodyState {
		public void _NormalBody() {
			_owner._Owner();
			_fsm._FSM();
			_fsm.GetStates().ForEach( s => s._BaseState() );
			_BaseState();
		}
	}
	public class DeathDragonBodyState : DragonBodyState {
		public void _DeathBody() {
			_owner._Owner();
			_fsm._FSM();
			_fsm.GetStates().ForEach( s => s._BaseState() );
			_BaseState();
		}
	}



	public class Dummy : SMBehaviour, ISMFSMOwner<DummyFSM> {
		public bool _isInitialEnteredFSMs	{ get; set; }
		public DummyFSM _fsm	{ get; private set; }
		public override void Create() {
			_fsm = DummyFSM.Generate(
				this,
				new SMFSMGenerateList<DummyState>{
					new DummyState[] {
						new NormalDummyState()
					}
				}
			);
		}
	}
	public class DummyFSM : SMFSM<Dummy, DummyFSM, DummyState> {
		public DummyFSM() {
		}
	}
	public abstract class DummyState : SMState<Dummy, DummyFSM> {
	}
	public class NormalDummyState : DummyState {
	}
}