//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Test {
	using System;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Task.Behaviour;
	using FSM;
	using FSM.State;



	// TODO : コメント追加、整頓



	public class Human : SMBehaviour, ISMFSMOwner<HumanFSM> {
		public HumanFSM _fsm	{ get; private set; }
		public Human() => _fsm = new HumanFSM( this );
		public override void Create() {}
		public void _Owner() {
			_fsm._OwnerFSM();
		}
	}

	public class HumanFSM : SMSingleFSM<Human, HumanState> {
		public HumanFSM( Human owner ) : base(
			owner,
			new HumanState[] {
				new NormalHumanState(),
				new DeathHumanState(),
			}
		) {}
		public void _OwnerFSM() {
			_owner._Owner();
			_states.ForEach( pair => pair.Value._State() );
		}
	}

	public abstract class HumanState : SMState<Human, HumanFSM> {
		public void _State() {
			_fsm._owner._Owner();
			_fsm._OwnerFSM();
			_fsm._states.ForEach( pair => pair.Value._State() );
			_fsm.ChangeState( typeof( NormalHumanState ) ).Forget();
			_fsm.ChangeState<NormalHumanState>().Forget();
		}
	}
	public class NormalHumanState : HumanState {
		public void _Normal() {
			_fsm._owner._Owner();
			_fsm._OwnerFSM();
			_fsm._states.ForEach( pair => pair.Value._State() );
			_State();
		}
	}
	public class DeathHumanState : HumanState {
		public void _Death() {
			_fsm._owner._Owner();
			_fsm._OwnerFSM();
			_fsm._states.ForEach( pair => pair.Value._State() );
			_State();
		}
	}



	public class Dragon : SMBehaviour, ISMFSMOwner<DragonFSM> {
		public DragonFSM _fsm	{ get; private set; }
		public Dragon() => _fsm = new DragonFSM( this );
		public override void Create() {}
		public void _Owner() {
			_fsm._OwnerFSM();
		}
	}

	public enum DragonFSMType {
		Head,
		Body,
	}
	public class DragonFSM : SMParallelFSM<Dragon, DragonInternalFSM, DragonFSMType> {
		public DragonInternalFSM _headFSM	{ get; private set; }
		public DragonInternalFSM _bodyFSM	{ get; private set; }
		public DragonFSM( Dragon owner ) : base( owner, new Dictionary<DragonFSMType, DragonInternalFSM> {
			{
				DragonFSMType.Head,
				new DragonInternalFSM(
					DragonFSMType.Head,
					new DragonState[] { new NormalDragonHeadState(), new BiteDragonHeadState(), },
					typeof( DragonHeadState )
				)
			},
			{
				DragonFSMType.Body,
				new DragonInternalFSM(
					DragonFSMType.Body,
					new DragonState[] { new NormalDragonBodyState(), new DeathDragonBodyState(),
						new NormalDragonHeadState(),	// コンパイルエラーにならない
//							new NormalDummyState(),		// エラー
					},
					typeof( DragonBodyState )
				)
			},
/*
			{
				DragonFSMType.Body,
				new DummyInternalFSM()	// エラー
			},
*/
		} ) {
			_headFSM = GetFSM( DragonFSMType.Head );
			_bodyFSM = GetFSM( DragonFSMType.Body );
		}
		public void _OwnerFSM() {
			_owner._Owner();
			_fsms.ForEach( pair => pair.Value._BaseFSM() );
			_headFSM.ChangeState<BiteDragonHeadState>().Forget();
			_headFSM.ChangeState<DeathDragonBodyState>().Forget();	// コンパイルエラーにならない
		}
	}

	public class DragonInternalFSM : SMInternalFSM<Dragon, DragonFSM, DragonState, DragonFSMType> {
		public DragonInternalFSM( DragonFSMType fsmType, IEnumerable<DragonState> states, Type baseStateType )
			: base( fsmType, states, baseStateType )
		{}
		public void _BaseFSM() {
			_topOwner._Owner();
			_fsm._OwnerFSM();
			_states.ForEach( pair => pair.Value._BaseState() );
		}
	}

	public abstract class DragonState : SMState<Dragon, DragonInternalFSM> {
		public void _BaseState() {
			_topOwner._Owner();
			_fsm._fsm._OwnerFSM();
			_fsm._BaseFSM();
			_fsm._states.ForEach( pair => pair.Value._BaseState() );
			_fsm.ChangeState<BiteDragonHeadState>().Forget();
			_fsm.ChangeState<DeathDragonBodyState>().Forget();
		}
	}
	public abstract class DragonHeadState : DragonState {
		public void _BaseHead() {
			_topOwner._Owner();
			_fsm._fsm._OwnerFSM();
			_fsm._BaseFSM();
			_fsm._states.ForEach( pair => pair.Value._BaseState() );
			_BaseState();
		}
	}
	public class NormalDragonHeadState : DragonHeadState {
		public void _NormalHead() {
			_topOwner._Owner();
			_fsm._fsm._OwnerFSM();
			_fsm._BaseFSM();
			_fsm._states.ForEach( pair => pair.Value._BaseState() );
			_BaseState();
		}
	}
	public class BiteDragonHeadState : DragonHeadState {
		public void _BiteHead() {
			_topOwner._Owner();
			_fsm._fsm._OwnerFSM();
			_fsm._BaseFSM();
			_fsm._states.ForEach( pair => pair.Value._BaseState() );
			_BaseState();
		}
	}
	public abstract class DragonBodyState : DragonState {
		public void _BaseBody() {
			_topOwner._Owner();
			_fsm._fsm._OwnerFSM();
			_fsm._BaseFSM();
			_fsm._states.ForEach( pair => pair.Value._BaseState() );
			_BaseState();
		}
	}
	public class NormalDragonBodyState : DragonBodyState {
		public void _NormalBody() {
			_topOwner._Owner();
			_fsm._fsm._OwnerFSM();
			_fsm._BaseFSM();
			_fsm._states.ForEach( pair => pair.Value._BaseState() );
			_BaseState();
		}
	}
	public class DeathDragonBodyState : DragonBodyState {
		public void _DeathBody() {
			_topOwner._Owner();
			_fsm._fsm._OwnerFSM();
			_fsm._BaseFSM();
			_fsm._states.ForEach( pair => pair.Value._BaseState() );
			_BaseState();
		}
	}



	public class Dummy : SMBehaviour, ISMFSMOwner<DummyFSM> {
		public DummyFSM _fsm	{ get; private set; }
		public Dummy() => _fsm = new DummyFSM( this );
		public override void Create() {}
	}

	public enum DummyFSMType {
		Body,
	}
	public class DummyFSM : SMParallelFSM<Dummy, DummyInternalFSM, DummyFSMType> {
		public DummyFSM( Dummy owner ) : base( owner, new Dictionary<DummyFSMType, DummyInternalFSM> {
			{ DummyFSMType.Body, new DummyInternalFSM() },
		} ) {}
	}

	public class DummyInternalFSM : SMInternalFSM<Dummy, DummyFSM, DummyState, DummyFSMType> {
		public DummyInternalFSM( DummyFSMType fsmType ) : base(
			fsmType,
			new DummyState[] {
				new NormalDummyState(),
			},
			typeof( DummyState )
		) {}
	}

	public abstract class DummyState : SMState<Dummy, DummyInternalFSM> {
	}
	public class NormalDummyState : DummyState {
	}
}