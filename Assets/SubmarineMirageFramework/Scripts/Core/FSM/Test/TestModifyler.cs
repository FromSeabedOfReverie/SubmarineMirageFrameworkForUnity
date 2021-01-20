//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSMTest {
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Task;
	using Extension;
	using Utility;



	// TODO : コメント追加、整頓



	public abstract class BaseSMFSMModifylerOwner<TOwner, TModifyler, TData>
		where TOwner : BaseSMFSMModifylerOwner<TOwner, TModifyler, TData>
		where TModifyler : BaseSMFSMModifyler<TOwner, TModifyler, TData>
		where TData : BaseSMFSMModifyData<TOwner, TModifyler, TData>
	{
		public TModifyler _modifyler	{ get; private set; }
	}
	public abstract class BaseSMFSMModifyData<TOwner, TModifyler, TData>
		where TOwner : BaseSMFSMModifylerOwner<TOwner, TModifyler, TData>
		where TModifyler : BaseSMFSMModifyler<TOwner, TModifyler, TData>
		where TData : BaseSMFSMModifyData<TOwner, TModifyler, TData>
	{
		protected TOwner _owner	{ get; private set; }
		protected TModifyler _modifyler	{ get; private set; }
		public virtual void Set( TOwner owner ) {
			_owner = owner;
			_modifyler = _owner._modifyler;
		}
		public abstract UniTask Run();
	}
	public abstract class BaseSMFSMModifyler<TOwner, TModifyler, TData>
		where TOwner : BaseSMFSMModifylerOwner<TOwner, TModifyler, TData>
		where TModifyler : BaseSMFSMModifyler<TOwner, TModifyler, TData>
		where TData : BaseSMFSMModifyData<TOwner, TModifyler, TData>
	{
		protected TOwner _owner	{ get; private set; }
		protected readonly LinkedList<TData> _data = new LinkedList<TData>();
		protected abstract SMTaskCanceler _asyncCanceler	{ get; }
		public BaseSMFSMModifyler( TOwner owner ) {
			_owner = owner;
		}
		public void Register( TData data ) {
			data.Set( _owner );
			_data.Enqueue( data );
		}
		public UniTask WaitRunning()
			=> UTask.WaitWhile( _asyncCanceler, () => !_data.IsEmpty() );
	}



	public abstract class SMStateModifyData
		: BaseSMFSMModifyData<BaseSMState, SMStateModifyler, SMStateModifyData>
	{}
	public class SMStateModifyler : BaseSMFSMModifyler<BaseSMState, SMStateModifyler, SMStateModifyData> {
		protected override SMTaskCanceler _asyncCanceler => _owner._asyncCancelerOnChangeOrDisable;
		public SMStateModifyler( BaseSMState owner ) : base( owner ) {}
	}
	public class EnterSMState : SMStateModifyData {
		public override async UniTask Run() {
			if ( _owner._runState == FSM.SMStateRunState.Enter )	{ return; }
			if ( _owner._runState == FSM.SMStateRunState.Update )	{ return; }
			await _owner._enterEvent.Run( _owner._asyncCancelerOnChangeOrDisable );
			_owner._runState = FSM.SMStateRunState.Enter;
		}
	}
	public class UpdateSMState : SMStateModifyData {
		public override async UniTask Run() {
			if ( _owner._runState == FSM.SMStateRunState.Exit )	{ return; }
			_owner._runState = FSM.SMStateRunState.Update;
			_owner._updateAsyncEvent.Run( _owner._asyncCancelerOnChangeOrDisable ).Forget();
			await UTask.DontWait();
		}
	}
	public class ExitSMState : SMStateModifyData {
		public override async UniTask Run() {
			if ( _owner._runState == FSM.SMStateRunState.Exit )	{ return; }
			await _owner._exitEvent.Run( _owner._asyncCancelerOnChangeOrDisable );
			_owner._runState = FSM.SMStateRunState.Exit;
		}
	}



	public abstract class SMFSMModifyData : BaseSMFSMModifyData<BaseSMFSM, SMFSMModifyler, SMFSMModifyData> {
	}
	public class SMFSMModifyler : BaseSMFSMModifyler<BaseSMFSM, SMFSMModifyler, SMFSMModifyData> {
		protected override SMTaskCanceler _asyncCanceler => new SMTaskCanceler();
		public SMFSMModifyler( BaseSMFSM owner ) : base( owner ) {}
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