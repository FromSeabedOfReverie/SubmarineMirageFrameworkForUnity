//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;



	public class Human : SMTask {
		public SMFSM<HumanState> _fsm	{ get; private set; }
		public override void Create() {
			_fsm = new SMFSM<HumanState>();
			_fsm.Setup( this, new HumanState[] { new NormalHumanState(), new DeathHumanState() } );
		}
		public void _Owner() {
			_fsm.GetStates()
				.ForEach( s => s._State() );
		}
	}
	public abstract class HumanState : SMState<Human, HumanState> {
		public void _State() {
			_owner._Owner();
			_fsm.GetStates()
				.ForEach( s => s._State() );
			_fsm.ChangeState( typeof( NormalHumanState ) ).Forget();
			_fsm.ChangeState<NormalHumanState>().Forget();
		}
	}
	public class NormalHumanState : HumanState {
		public void _Normal() {
			_owner._Owner();
			_fsm.GetStates()
				.ForEach( s => s._State() );
			_State();
		}
	}
	public class DeathHumanState : HumanState {
		public void _Death() {
			_owner._Owner();
			_fsm.GetStates()
				.ForEach( s => s._State() );
			_State();
		}
	}



	public class Dragon : SMTask {
		public SMFSM<DragonHeadState> _headFSM	{ get; private set; }
		public SMFSM<DragonBodyState> _bodyFSM	{ get; private set; }
		public SMFSM<DummyState> _dummyFSM		{ get; private set; }
		public override void Create() {
			_headFSM = new SMFSM<DragonHeadState>();
			_headFSM.Setup(
				this,
				new DragonHeadState[] { new NormalDragonHeadState(), new BiteDragonHeadState(), }
			);
			_bodyFSM = new SMFSM<DragonBodyState>();
			_bodyFSM.Setup(
				this,
				new DragonBodyState[] {
					new NormalDragonBodyState(),
					new DeathDragonBodyState(),
//					new NormalDragonHeadState(),	// エラー
//					new NormalDummyState(),			// エラー
				}
			);
			_dummyFSM = new SMFSM<DummyState>();
			_dummyFSM.Setup(
				this,   // コンパイルエラーにならない
				new Type[] { typeof( NormalDummyState ), }
			);
		}
		public void _Owner() {
			_headFSM.GetStates()
				.ForEach( s => s._BaseState() );
			_headFSM.ChangeState<BiteDragonHeadState>().Forget();
//			_headFSM.ChangeState<DeathDragonBodyState>().Forget();	// エラー
		}
	}
	public abstract class DragonState<TState> : SMState<Dragon, TState>
		where TState : DragonState<TState>
	{
		public void _BaseState() {
			_owner._Owner();
			_fsm.GetStates()
				.ForEach( s => s._BaseState() );
//			_fsm.ChangeState<BiteDragonHeadState>().Forget();	// エラー
//			_fsm.ChangeState<DeathDragonBodyState>().Forget();	// エラー
		}
	}
	public abstract class DragonHeadState : DragonState<DragonHeadState> {
		public void _BaseHead() {
			_owner._Owner();
			_fsm.GetStates()
				.ForEach( s => s._BaseState() );
			_BaseState();
		}
	}
	public class NormalDragonHeadState : DragonHeadState {
		public void _NormalHead() {
			_owner._Owner();
			_fsm.GetStates()
				.ForEach( s => s._BaseState() );
			_BaseState();
		}
	}
	public class BiteDragonHeadState : DragonHeadState {
		public void _BiteHead() {
			_owner._Owner();
			_fsm.GetStates()
				.ForEach( s => s._BaseState() );
			_BaseState();
		}
	}
	public abstract class DragonBodyState : DragonState<DragonBodyState> {
		public void _BaseBody() {
			_owner._Owner();
			_fsm.GetStates()
				.ForEach( s => s._BaseState() );
			_BaseState();
		}
	}
	public class NormalDragonBodyState : DragonBodyState {
		public void _NormalBody() {
			_owner._Owner();
			_fsm.GetStates()
				.ForEach( s => s._BaseState() );
			_BaseState();
		}
	}
	public class DeathDragonBodyState : DragonBodyState {
		public void _DeathBody() {
			_owner._Owner();
			_fsm.GetStates()
				.ForEach( s => s._BaseState() );
			_BaseState();
		}
	}



	public class Dummy : SMTask {
		public SMFSM<DummyState> _fsm	{ get; private set; }
		public override void Create() {
			_fsm = new SMFSM<DummyState>();
			_fsm.Setup(
				this,
				new DummyState[] { new NormalDummyState() }
			);
		}
	}
	public abstract class DummyState : SMState<Dummy, DummyState> {
	}
	public class NormalDummyState : DummyState {
	}
}