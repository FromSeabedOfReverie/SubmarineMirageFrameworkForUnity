//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestFSM
namespace SubmarineMirage.FSM {
	using Base;
	using Event;
	using Extension;
	using Utility;
	using Debug;



	public abstract class SMState : SMStandardBase {
		[SMShow] protected object _owner	{ get; private set; }
		[SMShow] protected SMFSM _fsm		{ get; private set; }

		[SMShowLine] public SMStateRunState _ranState	{ get; set; }

		public readonly SMAsyncEvent _enterEvent		= new SMAsyncEvent();
		public readonly SMAsyncEvent _exitEvent			= new SMAsyncEvent();
		public readonly SMAsyncEvent _asyncUpdateEvent	= new SMAsyncEvent();
		public readonly SMSubject _fixedUpdateEvent		= new SMSubject();
		public readonly SMSubject _updateEvent			= new SMSubject();
		public readonly SMSubject _lateUpdateEvent		= new SMSubject();

		protected SMAsyncCanceler _asyncCancelerOnExit	=> _fsm?._asyncCancelerOnExit;



#region ToString
		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _owner ), i => _toStringer.DefaultLineValue( _owner ) );
			_toStringer.SetValue( nameof( _fsm ), i => _toStringer.DefaultLineValue( _fsm ) );
		}
#endregion



		public SMState() {
			_disposables.AddLast( () => {
#if TestFSM
				SMLog.Debug( $"{this.GetAboutName()}.{nameof( Dispose )} : start\n{this}" );
#endif
				_owner = null;
				_fsm = null;

				_ranState = SMStateRunState.Exit;

				_enterEvent.Dispose();
				_exitEvent.Dispose();
				_asyncUpdateEvent.Dispose();
				_fixedUpdateEvent.Dispose();
				_updateEvent.Dispose();
				_lateUpdateEvent.Dispose();
#if TestFSM
				SMLog.Debug( $"{this.GetAboutName()}.{nameof( Dispose )} : end\n{this}" );
#endif
			} );
#if TestFSM
			SMLog.Debug( $"{this.GetAboutName()}() : \n{this}" );
#endif
		}

		public override void Dispose() => base.Dispose();

		public virtual void Setup( object owner, SMFSM fsm ) {
			CheckDisposeError( nameof( Setup ) );

			_owner = owner;
			_fsm = fsm;
		}
	}
}