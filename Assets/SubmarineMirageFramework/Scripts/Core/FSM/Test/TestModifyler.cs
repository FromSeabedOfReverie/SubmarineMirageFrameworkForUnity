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
		protected BaseSMState _owner	{ get; private set; }
		protected SMStateModifyler _modifyler	{ get; private set; }
		public virtual void Set( BaseSMState owner ) {
			_owner = owner;
			_modifyler = _owner._modifyler;
		}
		public abstract UniTask Run();
	}
	public class SMStateModifyler {
		protected BaseSMState _owner	{ get; private set; }
		protected readonly LinkedList<SMStateModifyData> _data = new LinkedList<SMStateModifyData>();
		public SMStateModifyler( BaseSMState owner ) {
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
		protected BaseSMFSM _owner	{ get; private set; }
		protected SMFSMModifyler _modifyler	{ get; private set; }
		public virtual void Set( BaseSMFSM owner ) {
			_owner = owner;
			_modifyler = _owner._modifyler;
		}
		public abstract UniTask Run();
	}
	public class SMFSMModifyler {
		protected BaseSMFSM _owner	{ get; private set; }
		protected readonly LinkedList<SMFSMModifyData> _data = new LinkedList<SMFSMModifyData>();
		public SMFSMModifyler( BaseSMFSM owner ) {
			_owner = owner;
		}
		public void Register( SMFSMModifyData data ) {
			data.Set( _owner );
			_data.Enqueue( data );
		}
		public UniTask WaitRunning()
			=> UTask.WaitWhile( new SMTaskCanceler(), () => !_data.IsEmpty() );
	}
	public class ChangeStateSMFSM : SMFSMModifyData {
		BaseSMState _state	{ get; set; }
		public ChangeStateSMFSM( BaseSMState state ) {
			_state = state;
		}
		public override async UniTask Run() {
			if ( _owner._rawState != null ) {
				_owner._rawState._modifyler.Register( new ExitSMState() );
				await _owner._rawState._modifyler.WaitRunning();
			}
			_owner._rawState = _state;
			if ( _owner._rawState == null )	{ return; }
			_owner._rawState._modifyler.Register( new EnterSMState() );
			await _owner._rawState._modifyler.WaitRunning();
			_owner._rawState._modifyler.Register( new UpdateSMState() );
		}
	}
}