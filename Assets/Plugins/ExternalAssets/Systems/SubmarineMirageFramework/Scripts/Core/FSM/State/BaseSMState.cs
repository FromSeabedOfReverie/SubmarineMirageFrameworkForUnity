//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {



	public abstract class BaseSMState : SMStandardBase {
		[SMShowLine] public SMStateRunState _ranState { get; set; }

		public readonly SMSubject _setupEvent			= new SMSubject();
		public readonly SMAsyncEvent _enterEvent		= new SMAsyncEvent();
		public readonly SMAsyncEvent _exitEvent			= new SMAsyncEvent();
		public readonly SMAsyncEvent _asyncUpdateEvent	= new SMAsyncEvent();
		public readonly SMSubject _fixedUpdateEvent		= new SMSubject();
		public readonly SMSubject _updateEvent			= new SMSubject();
		public readonly SMSubject _lateUpdateEvent		= new SMSubject();



		public BaseSMState() {
			_disposables.AddFirst( () => {
				_ranState = SMStateRunState.Exit;

				_setupEvent.Dispose();
				_enterEvent.Dispose();
				_exitEvent.Dispose();
				_asyncUpdateEvent.Dispose();
				_fixedUpdateEvent.Dispose();
				_updateEvent.Dispose();
				_lateUpdateEvent.Dispose();
			} );
		}

		public override void Dispose()
			=> base.Dispose();

		public abstract void Setup( object owner, object fsm );
	}
}