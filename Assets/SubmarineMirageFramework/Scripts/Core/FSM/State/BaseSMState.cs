//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.State.Base {
	using SubmarineMirage.Base;
	using MultiEvent;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class BaseSMState : SMStandardBase {
		[SMHide] public SMStateRunState _ranState			=> _body._ranState;
		[SMHide] public SMStateUpdateState _updatedState	=> _body._updatedState;
		[SMHide] public bool _isUpdating					=> _body._isUpdating;

		[SMHide] public SMMultiAsyncEvent _selfInitializeEvent	=> _body._selfInitializeEvent;
		[SMHide] public SMMultiAsyncEvent _initializeEvent		=> _body._initializeEvent;
		[SMHide] public SMMultiSubject _enableEvent				=> _body._enableEvent;
		[SMHide] public SMMultiSubject _fixedUpdateEvent		=> _body._fixedUpdateEvent;
		[SMHide] public SMMultiSubject _updateEvent				=> _body._updateEvent;
		[SMHide] public SMMultiSubject _lateUpdateEvent			=> _body._lateUpdateEvent;
		[SMHide] public SMMultiSubject _disableEvent			=> _body._disableEvent;
		[SMHide] public SMMultiAsyncEvent _finalizeEvent		=> _body._finalizeEvent;

		[SMHide] public SMMultiAsyncEvent _enterEvent		=> _body._enterEvent;
		[SMHide] public SMMultiAsyncEvent _updateAsyncEvent	=> _body._updateAsyncEvent;
		[SMHide] public SMMultiAsyncEvent _exitEvent		=> _body._exitEvent;

		[SMHide] public abstract SMAsyncCanceler _asyncCancelerOnDisableAndExit	{ get; }
		[SMHide] public abstract SMAsyncCanceler _asyncCancelerOnDispose	{ get; }

		public BaseSMStateBody _body	{ get; private set; }



		public BaseSMState() {
			_body = new BaseSMStateBody( this );

			_disposables.AddLast( () => {
				_body.Dispose();
			} );
		}

		public override void Dispose() => base.Dispose();
	}
}