//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestFSM {
	using System;
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Task;
	using FSM;



	public class Human : SMTask, ISMFSMOwner {
		public SMFSM _fsm	{ get; private set; }
		public override void Create() {
			_fsm = new SMFSM();
			_fsm.Setup( this, new HumanState[] { new NormalHumanState(), new DeathHumanState() } );
		}
		public void _Owner() {
			_fsm.GetStates()
				.Select( s => s as HumanState )
				.ForEach( s => s._State() );
		}
	}
	public abstract class HumanState : SMState {
		public new Human _owner { get; private set; }
		public override void Setup( ISMFSMOwner owner, SMFSM fsm ) {
			base.Setup( owner, fsm );
			_owner = base._owner as Human;
		}
		public void _State() {
			_owner._Owner();
			_fsm.GetStates()
				.Select( s => s as HumanState )
				.ForEach( s => s._State() );
			_fsm.ChangeState( typeof( NormalHumanState ) ).Forget();
			_fsm.ChangeState<NormalHumanState>().Forget();
		}
	}
	public class NormalHumanState : HumanState {
		public void _Normal() {
			_owner._Owner();
			_fsm.GetStates()
				.Select( s => s as HumanState )
				.ForEach( s => s._State() );
			_State();
		}
	}
	public class DeathHumanState : HumanState {
		public void _Death() {
			_owner._Owner();
			_fsm.GetStates()
				.Select( s => s as HumanState )
				.ForEach( s => s._State() );
			_State();
		}
	}



	public class Dragon : SMTask, ISMFSMOwner {
		public SMFSM _headFSM	{ get; private set; }
		public SMFSM _bodyFSM	{ get; private set; }
		public SMFSM _dummyFSM	{ get; private set; }
		public override void Create() {
			var fsm = SMFSM.Generate(
				this,
				new SMFSMGenerateList {
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
			_headFSM = fsm.GetFSM<DragonHeadState>();
			_bodyFSM = fsm.GetFSM<DragonBodyState>();
			_dummyFSM = fsm.GetFSM<DummyState>();   // コンパイルエラーにならない
		}
		public void _Owner() {
			_headFSM.GetStates()
				.Select( s => s as DragonState )
				.ForEach( s => s._BaseState() );
			_headFSM.ChangeState<BiteDragonHeadState>().Forget();
			_headFSM.ChangeState<DeathDragonBodyState>().Forget();	// コンパイルエラーにならない
		}
	}
	public abstract class DragonState : SMState {
		public new Dragon _owner { get; private set; }
		public override void Setup( ISMFSMOwner owner, SMFSM fsm ) {
			base.Setup( owner, fsm );
			_owner = base._owner as Dragon;
		}
		public void _BaseState() {
			_owner._Owner();
			_fsm.GetStates()
				.Select( s => s as DragonState )
				.ForEach( s => s._BaseState() );
			_fsm.ChangeState<BiteDragonHeadState>().Forget();   // コンパイルエラーにならない
			_fsm.ChangeState<DeathDragonBodyState>().Forget();  // コンパイルエラーにならない
		}
	}
	public abstract class DragonHeadState : DragonState {
		public void _BaseHead() {
			_owner._Owner();
			_fsm.GetStates()
				.Select( s => s as DragonState )
				.ForEach( s => s._BaseState() );
			_BaseState();
		}
	}
	public class NormalDragonHeadState : DragonHeadState {
		public void _NormalHead() {
			_owner._Owner();
			_fsm.GetStates()
				.Select( s => s as DragonState )
				.ForEach( s => s._BaseState() );
			_BaseState();
		}
	}
	public class BiteDragonHeadState : DragonHeadState {
		public void _BiteHead() {
			_owner._Owner();
			_fsm.GetStates()
				.Select( s => s as DragonState )
				.ForEach( s => s._BaseState() );
			_BaseState();
		}
	}
	public abstract class DragonBodyState : DragonState {
		public void _BaseBody() {
			_owner._Owner();
			_fsm.GetStates()
				.Select( s => s as DragonState )
				.ForEach( s => s._BaseState() );
			_BaseState();
		}
	}
	public class NormalDragonBodyState : DragonBodyState {
		public void _NormalBody() {
			_owner._Owner();
			_fsm.GetStates()
				.Select( s => s as DragonState )
				.ForEach( s => s._BaseState() );
			_BaseState();
		}
	}
	public class DeathDragonBodyState : DragonBodyState {
		public void _DeathBody() {
			_owner._Owner();
			_fsm.GetStates()
				.Select( s => s as DragonState )
				.ForEach( s => s._BaseState() );
			_BaseState();
		}
	}



	public class Dummy : SMTask, ISMFSMOwner {
		public SMFSM _fsm	{ get; private set; }
		public override void Create() {
			_fsm = SMFSM.Generate(
				this,
				new SMFSMGenerateList {
					{
						new DummyState[] {
							new NormalDummyState()
						},
						typeof( DummyState )
					},
				}
			);
		}
	}
	public abstract class DummyState : SMState {
		public new Dummy _owner { get; private set; }
		public override void Setup( ISMFSMOwner owner, SMFSM fsm ) {
			base.Setup( owner, fsm );
			_owner = base._owner as Dummy;
		}
	}
	public class NormalDummyState : DummyState {
	}
}