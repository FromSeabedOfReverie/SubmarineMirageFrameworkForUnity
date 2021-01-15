//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.FSM {
	using MultiEvent;
	using Task;
	using Base.Modifyler;
	using State;
	using Modifyler;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class BaseSMFSM : BaseSMFSMModifylerOwner<BaseSMFSM, SMFSMModifyler, SMFSMModifyData> {

		[SMHide] public abstract SMMultiAsyncEvent _selfInitializeEvent	{ get; }
		[SMHide] public abstract SMMultiAsyncEvent _initializeEvent		{ get; }
		[SMHide] public abstract SMMultiSubject _enableEvent			{ get; }
		[SMHide] public abstract SMMultiSubject _fixedUpdateEvent		{ get; }
		[SMHide] public abstract SMMultiSubject _updateEvent			{ get; }
		[SMHide] public abstract SMMultiSubject _lateUpdateEvent		{ get; }
		[SMHide] public abstract SMMultiSubject _disableEvent			{ get; }
		[SMHide] public abstract SMMultiAsyncEvent _finalizeEvent		{ get; }

		public string _registerEventName	{ get; private set; }
		public abstract SMTaskRunState _taskRunState	{ get; }
		public abstract BaseSMState _rawState	{ get; set; }

		[SMHide] public SMTaskCanceler _asyncCanceler	{ get; private set; } = new SMTaskCanceler();


		public BaseSMFSM() {
			_registerEventName = this.GetAboutName();
			_modifyler = new SMFSMModifyler( this );
			_disposables.AddLast( () => {
				_asyncCanceler.Dispose();
			} );
		}
	}
}