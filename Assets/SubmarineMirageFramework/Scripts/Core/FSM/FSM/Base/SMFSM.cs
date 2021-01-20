//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Base {
	using Cysharp.Threading.Tasks;
	using MultiEvent;
	using Task;
	using FSM.Modifyler.Base;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMFSM : BaseSMFSMModifylerOwner<SMFSM, SMFSMModifyler, SMFSMModifyData> {
		[SMHide] public abstract SMMultiAsyncEvent _selfInitializeEvent	{ get; }
		[SMHide] public abstract SMMultiAsyncEvent _initializeEvent		{ get; }
		[SMHide] public abstract SMMultiSubject _enableEvent			{ get; }
		[SMHide] public abstract SMMultiSubject _fixedUpdateEvent		{ get; }
		[SMHide] public abstract SMMultiSubject _updateEvent			{ get; }
		[SMHide] public abstract SMMultiSubject _lateUpdateEvent		{ get; }
		[SMHide] public abstract SMMultiSubject _disableEvent			{ get; }
		[SMHide] public abstract SMMultiAsyncEvent _finalizeEvent		{ get; }

		[SMHide] public abstract SMTaskCanceler _asyncCancelerOnDisable	{ get; }
		[SMHide] public abstract SMTaskCanceler _asyncCancelerOnDispose	{ get; }

		public string _registerEventName	{ get; private set; }
		public bool _isInitialEntered	{ get; set; }



		public SMFSM() {
			_registerEventName = this.GetAboutName();
			_modifyler = new SMFSMModifyler( this );
		}

		public abstract void Set( IBaseSMFSMOwner owner );


		public abstract UniTask FinalExit();
	}
}