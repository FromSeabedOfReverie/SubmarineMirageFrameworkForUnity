//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Base {
	using System;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using SubmarineMirage.Base;
	using MultiEvent;
	using Task;
	using FSM.Modifyler;
	using FSM.Modifyler.Base;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMFSM : SMStandardBase {
		public abstract bool _isInitialized	{ get; }
		public abstract bool _isOperable	{ get; }
		public abstract bool _isFinalizing	{ get; }
		public abstract bool _isActive		{ get; }

		[SMHide] public abstract SMMultiAsyncEvent _selfInitializeEvent	{ get; }
		[SMHide] public abstract SMMultiAsyncEvent _initializeEvent		{ get; }
		[SMHide] public abstract SMMultiSubject _enableEvent			{ get; }
		[SMHide] public abstract SMMultiSubject _fixedUpdateEvent		{ get; }
		[SMHide] public abstract SMMultiSubject _updateEvent			{ get; }
		[SMHide] public abstract SMMultiSubject _lateUpdateEvent		{ get; }
		[SMHide] public abstract SMMultiSubject _disableEvent			{ get; }
		[SMHide] public abstract SMMultiAsyncEvent _finalizeEvent		{ get; }

		[SMHide] public SMTaskCanceler _asyncCancelerOnDisableAndExit	{ get; private set; }
			= new SMTaskCanceler();
		[SMHide] public abstract SMTaskCanceler _asyncCancelerOnDispose	{ get; }

		public SMFSMModifyler _modifyler	{ get; private set; }
		public string _registerEventName	{ get; private set; }
		public bool _isInitialEntered	{ get; set; }



		public SMFSM() {
			_registerEventName = this.GetAboutName();
			_modifyler = new SMFSMModifyler( this );

			_disposables.AddLast( () => {
				_modifyler.Dispose();
				_asyncCancelerOnDisableAndExit.Dispose();
			} );
		}

		public override void Dispose() => base.Dispose();


		public virtual void Set( IBaseSMFSMOwner topOwner, IBaseSMFSMOwner owner ) {
			_disableEvent.AddLast( _registerEventName ).Subscribe( _ => {
				_modifyler.Reset();
				SMFSMApplyer.StopAsyncOnDisableAndExit( this );
			} );
			_updateEvent.AddLast( _registerEventName ).Subscribe( _ => {
				_modifyler.Run().Forget();
			} );
		}

		public virtual void SetFSMType( Enum fsmType )
			=> throw new InvalidOperationException( $"{this.GetAboutName()}は非対応 : {nameof( SetFSMType )}" );


		public abstract UniTask FinalExit();
	}
}