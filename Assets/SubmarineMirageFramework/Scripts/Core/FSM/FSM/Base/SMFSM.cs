//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Base {
	using System;
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Base;
	using MultiEvent;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMFSM : SMStandardBase {
		[SMHide] public abstract bool _isInitialized	{ get; }
		[SMHide] public abstract bool _isOperable		{ get; }
		[SMHide] public abstract bool _isFinalizing		{ get; }
		[SMHide] public abstract bool _isActive			{ get; }
		[SMHide] public bool _isInitialEntered	=> _body._isInitialEntered;

		[SMHide] public abstract SMMultiAsyncEvent _selfInitializeEvent	{ get; }
		[SMHide] public abstract SMMultiAsyncEvent _initializeEvent		{ get; }
		[SMHide] public abstract SMMultiSubject _enableEvent			{ get; }
		[SMHide] public abstract SMMultiSubject _fixedUpdateEvent		{ get; }
		[SMHide] public abstract SMMultiSubject _updateEvent			{ get; }
		[SMHide] public abstract SMMultiSubject _lateUpdateEvent		{ get; }
		[SMHide] public abstract SMMultiSubject _disableEvent			{ get; }
		[SMHide] public abstract SMMultiAsyncEvent _finalizeEvent		{ get; }

		[SMHide] public SMAsyncCanceler _asyncCancelerOnDisableAndExit	=> _body._asyncCancelerOnDisableAndExit;
		[SMHide] public abstract SMAsyncCanceler _asyncCancelerOnDispose	{ get; }

		public SMFSMBody _body	{ get; private set; }



		public SMFSM() {
			_body = new SMFSMBody( this );

			_disposables.AddLast( () => {
				_body.Dispose();
			} );
		}

		public override void Dispose() => base.Dispose();


		public virtual void SetFSMType( Enum fsmType )
			=> throw new InvalidOperationException( $"{this.GetAboutName()}は非対応 : {nameof( SetFSMType )}" );

		public abstract UniTask FinalExit();
	}
}