//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task {
	using System;
	using Base;
	using Event;
	using Task.Modifyler;
	using Extension;
	using Utility;



	public abstract class SMTask : SMStandardBase {
		public SMTask _previous	{ get; set; }
		public SMTask _next		{ get; set; }

		public SMTaskManager _manager	{ get; set; }

		public virtual SMTaskRunType _type		=> SMTaskRunType.Parallel;
		public SMTaskRunState _ranState			{ get; set; }
		public SMTaskActiveState _activeState	{ get; set; }

		public bool _isInitialized			=> _ranState >= SMTaskRunState.InitialEnable;
		public bool _isFinalizing			=> _ranState >= SMTaskRunState.FinalDisable;
		public bool _isOperable				=> _isInitialized && !_isFinalizing;
		public bool _isActive				=> _activeState == SMTaskActiveState.Enable;
		public bool _isRequestInitialEnable	{ get; set; } = true;
		public Func<bool> _isCanActiveEvent = () => true;

		public SMAsyncEvent _selfInitializeEvent	{ get; private set; } = new SMAsyncEvent();
		public SMAsyncEvent _initializeEvent		{ get; private set; } = new SMAsyncEvent();
		public SMAsyncEvent _finalizeEvent			{ get; private set; } = new SMAsyncEvent();
		public SMSubject _enableEvent				{ get; private set; } = new SMSubject();
		public SMSubject _disableEvent				{ get; private set; } = new SMSubject();
		public SMSubject _fixedUpdateEvent			{ get; private set; } = new SMSubject();
		public SMSubject _updateEvent				{ get; private set; } = new SMSubject();
		public SMSubject _lateUpdateEvent			{ get; private set; } = new SMSubject();

		public SMAsyncCanceler _asyncCancelerOnDisable	{ get; private set; } = new SMAsyncCanceler();
		public SMAsyncCanceler _asyncCancelerOnDispose	{ get; private set; } = new SMAsyncCanceler();



#region ToString
		public override void SetToString() {
			base.SetToString();


			_toStringer.SetValue( nameof( _id ), i => StringSMUtility.IndentSpace( i ) +
				$"{_id} ↑{_previous?._id} ↓{_next?._id}" );
			_toStringer.Add( "this", i => this.GetAboutName() );
			_toStringer.Add( nameof( _previous ), i => _toStringer.DefaultValue( _previous, i, true ) );
			_toStringer.Add( nameof( _next ), i => _toStringer.DefaultValue( _next, i, true ) );

			_toStringer.Add( nameof( _selfInitializeEvent ),	i => _selfInitializeEvent._isRunning.ToString() );
			_toStringer.Add( nameof( _initializeEvent ),		i => _initializeEvent._isRunning.ToString() );
			_toStringer.Add( nameof( _finalizeEvent ),			i => _finalizeEvent._isRunning.ToString() );


			_toStringer.AddLine( nameof( _previous ), () => $"↑{_previous?._id}" );
			_toStringer.AddLine( nameof( _next ), () => $"↓{_next?._id}" );
			_toStringer.AddLine( nameof( _ranState ), () => _toStringer.DefaultLineValue( _ranState ) );
			_toStringer.AddLine( nameof( _isActive ), () => _isActive ? "◯" : "×" );
		}
#endregion



		public SMTask() {
			_manager.Register( this );

			_disposables.AddLast( () => {
				Unlink();

				_isRequestInitialEnable = false;
				_isCanActiveEvent = null;

				_asyncCancelerOnDisable.Dispose();
				_asyncCancelerOnDispose.Dispose();

				_selfInitializeEvent.Dispose();
				_initializeEvent.Dispose();
				_finalizeEvent.Dispose();
				_enableEvent.Dispose();
				_disableEvent.Dispose();
				_fixedUpdateEvent.Dispose();
				_updateEvent.Dispose();
				_lateUpdateEvent.Dispose();
			} );
		}

		public abstract void Create();

		public override void Dispose() => base.Dispose();



		public void Link( SMTask add ) {
			var temp = _next;
			_next = add;
			add._previous = this;
			add._next = temp;
			temp._previous = add;
		}

		public void Unlink() {
			if ( _previous != null )	{ _previous._next = _next; }
			if ( _next != null )		{ _next._previous = _previous; }
			_previous = null;
			_next = null;
		}



		public void AdjustRun( SMTask previous ) {
			if (	previous._ranState >= SMTaskRunState.FinalDisable &&
					_ranState < SMTaskRunState.FinalDisable &&
					_type != SMTaskRunType.Dont
			) {
				_manager.Register( new FinalDisableSMTask( this ) );
			}
			if (	previous._ranState >= SMTaskRunState.Finalize &&
					_ranState < SMTaskRunState.Finalize
			) {
				if ( _type != SMTaskRunType.Dont ) {
					_manager.Register( new FinalizeSMTask( this ) );
				} else {
					Dispose();
				}
			}
			if ( previous._isFinalizing ) { return; }


			if (	previous._ranState >= SMTaskRunState.Create &&
					_ranState < SMTaskRunState.Create
			) {
				_manager.Register( new CreateSMTask( this ) );
			}
			if ( _type == SMTaskRunType.Dont ) { return; }


			if (	previous._ranState >= SMTaskRunState.SelfInitialize &&
					_ranState < SMTaskRunState.SelfInitialize
			) {
				_manager.Register( new SelfInitializeSMTask( this ) );
			}
			if (	previous._ranState >= SMTaskRunState.Initialize &&
					_ranState < SMTaskRunState.Initialize
			) {
				_manager.Register( new InitializeSMTask( this ) );
			}
			if (	previous._ranState >= SMTaskRunState.InitialEnable &&
					_ranState < SMTaskRunState.InitialEnable
			) {
				_manager.Register( new InitialEnableSMTask( this ) );
			}
			if ( !previous._isInitialized ) { return; }


			if ( previous.IsActiveInHierarchy() ) {
				_manager.Register( new EnableSMTask( this ) );
			} else {
				_manager.Register( new DisableSMTask( this ) );
			}
		}



		public virtual void FixedUpdate() {
			if ( !_isOperable )	{ return; }
			if ( !_isActive )	{ return; }
			if ( _ranState < SMTaskRunState.InitialEnable )	{ return; }

			_fixedUpdateEvent.Run();

			if ( _ranState == SMTaskRunState.InitialEnable ) {
				_ranState = SMTaskRunState.FixedUpdate;
			}
		}

		public virtual void Update() {
			if ( !_isOperable )	{ return; }
			if ( !_isActive )	{ return; }
			if ( _ranState < SMTaskRunState.FixedUpdate )	{ return; }

			_updateEvent.Run();

			if ( _ranState == SMTaskRunState.FixedUpdate ) {
				_ranState = SMTaskRunState.Update;
			}
		}

		public virtual void LateUpdate() {
			if ( !_isOperable )	{ return; }
			if ( !_isActive )	{ return; }
			if ( _ranState < SMTaskRunState.Update )	{ return; }

			_lateUpdateEvent.Run();

			if ( _ranState == SMTaskRunState.Update ) {
				_ranState = SMTaskRunState.LateUpdate;
			}
		}
	}
}