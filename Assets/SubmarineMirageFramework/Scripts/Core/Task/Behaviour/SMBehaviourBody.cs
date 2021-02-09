//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Base {
	using System.Linq;
	using System.Collections.Generic;
	using KoganeUnityLib;
	using Event;
	using Task.Modifyler;
	using Task.Modifyler.Base;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMBehaviourBody : BaseSMTaskModifylerOwner<SMBehaviourModifyler> {
		public SMBehaviour _behaviour	{ get; private set; }
		public SMObjectBody _objectBody	{ get; set; }

		public SMBehaviourBody _previous	{ get; set; }
		public SMBehaviourBody _next		{ get; set; }

		public SMTaskType _type	=> _behaviour._type;
		[SMShow] public bool _isRunInitialActive	{ get; set; }
		[SMShow] public bool _isRunFinalize	{ get; set; }

		public readonly SMAsyncEvent _selfInitializeEvent = new SMAsyncEvent();
		public readonly SMAsyncEvent _initializeEvent = new SMAsyncEvent();
		public readonly SMSubject _enableEvent = new SMSubject();
		public readonly SMSubject _fixedUpdateEvent = new SMSubject();
		public readonly SMSubject _updateEvent = new SMSubject();
		public readonly SMSubject _lateUpdateEvent = new SMSubject();
		public readonly SMSubject _disableEvent = new SMSubject();
		public readonly SMAsyncEvent _finalizeEvent = new SMAsyncEvent();

		public readonly SMAsyncCanceler _asyncCancelerOnDisable = new SMAsyncCanceler();
		public readonly SMAsyncCanceler _asyncCancelerOnDispose = new SMAsyncCanceler();



#region ToString
		public override void SetToString() {
			base.SetToString();


			_toStringer.SetValue( nameof( _id ), i => StringSMUtility.IndentSpace( i ) +
				$"{_id} ↑{_previous?._id} ↓{_next?._id}" );
			_toStringer.Add( nameof( _behaviour ), i =>
				_toStringer.DefaultValue( _behaviour, i, true ) );
			_toStringer.Add( nameof( _objectBody ), i => _toStringer.DefaultValue( _objectBody, i, true ) );

			_toStringer.Add( nameof( _previous ), i => _toStringer.DefaultValue( _previous, i, true ) );
			_toStringer.Add( nameof( _next ), i => _toStringer.DefaultValue( _next, i, true ) );

			_toStringer.Add( nameof( _selfInitializeEvent ), i =>
				$"_isRunning : {_selfInitializeEvent._isRunning}" );
			_toStringer.Add( nameof( _initializeEvent ), i => $"_isRunning : {_initializeEvent._isRunning}" );
			_toStringer.Add( nameof( _finalizeEvent ), i => $"_isRunning : {_finalizeEvent._isRunning}" );


			_toStringer.AddLine( nameof( _previous ), () => $"↑{_previous?._id}" );
			_toStringer.AddLine( nameof( _next ), () => $"↓{_next?._id}" );
		}
#endregion



		public SMBehaviourBody( SMBehaviour behaviour ) {
			_modifyler = new SMBehaviourModifyler( this );
			_behaviour = behaviour;

			_isRunInitialActive = _type != SMTaskType.DontWork && IsActiveInHierarchyAndComponent();

			_disposables.AddLast( () => {
				_isFinalizing = true;
				_ranState = SMTaskRunState.Finalize;

				_asyncCancelerOnDisable.Dispose();
				_asyncCancelerOnDispose.Dispose();

				_selfInitializeEvent.Dispose();
				_initializeEvent.Dispose();
				_enableEvent.Dispose();
				_fixedUpdateEvent.Dispose();
				_updateEvent.Dispose();
				_lateUpdateEvent.Dispose();
				_disableEvent.Dispose();
				_finalizeEvent.Dispose();
			} );
		}

		public override void Dispose() => base.Dispose();

		public void DisposeBrothers( SMTaskType type ) {
			GetBrothers()
				.Where( b => b._type == type )
				.Reverse()
				.ForEach( b => b._behaviour.Dispose() );
		}

		public void SetupObject( SMObjectBody objectBody ) {
			_objectBody = objectBody;
			_asyncCancelerOnDisable.SetParent( _objectBody._asyncCanceler );
			_asyncCancelerOnDispose.SetParent( _objectBody._asyncCanceler );
		}



		public void Link( SMBehaviourBody add ) {
			var last = GetLast();
			last._next = add;
			add._previous = last;
			add._objectBody = _objectBody;
		}

		public void Unlink() {
			if ( _objectBody._behaviourBody == this )	{ _objectBody._behaviourBody = _next; }
			if ( _previous != null )					{ _previous._next = _next; }
			if ( _next != null )						{ _next._previous = _previous; }
			_previous = null;
			_next = null;
		}



		public void StopAsyncOnDisable() => _asyncCancelerOnDisable.Cancel();



		public bool IsActiveInComponent()
			=> _behaviour.enabled;

		public bool IsActiveInHierarchyAndComponent()
			=> _behaviour.gameObject.activeInHierarchy && _behaviour.enabled;



		public void RegisterRunEventToOwner() {
			if (	_objectBody._ranState >= SMTaskRunState.FinalDisable &&
					_ranState < SMTaskRunState.FinalDisable &&
					_type != SMTaskType.DontWork
			) {
				var isActive = _objectBody.IsActiveInHierarchy();
				_modifyler.Register( new FinalDisableSMBehaviour( isActive ) );
			}

			if (	_objectBody._ranState >= SMTaskRunState.Finalize &&
					_ranState < SMTaskRunState.Finalize
			) {
				if ( _type != SMTaskType.DontWork ) {
					_modifyler.Register( new FinalizeSMBehaviour() );
				} else {
					_behaviour.Dispose();
				}
			}

			if ( _objectBody._isFinalizing )	{ return; }


			if (	_objectBody._ranState >= SMTaskRunState.Create &&
					_ranState < SMTaskRunState.Create
			) {
				_modifyler.Register( new CreateSMBehaviour() );
			}

			if ( _type == SMTaskType.DontWork )	{ return; }


			if (	_objectBody._ranState >= SMTaskRunState.SelfInitialize &&
					_ranState < SMTaskRunState.SelfInitialize
			) {
				_modifyler.Register( new SelfInitializeSMBehaviour() );
			}

			if (	_objectBody._ranState >= SMTaskRunState.Initialize &&
					_ranState < SMTaskRunState.Initialize
			) {
				_modifyler.Register( new InitializeSMBehaviour() );
			}

			if (	_objectBody._ranState >= SMTaskRunState.InitialEnable &&
					_ranState < SMTaskRunState.InitialEnable
			) {
				var isActive = _objectBody.IsActiveInHierarchy();
				_modifyler.Register( new InitialEnableSMBehaviour( isActive ) );
			}

			if ( !_objectBody._isInitialized )	{ return; }


			if ( _objectBody.IsActiveInHierarchy() ) {
				_modifyler.Register( new EnableSMBehaviour() );

			} else {
				_modifyler.Register( new DisableSMBehaviour() );
			}
		}



		public void FixedUpdate() {
			if ( !_isOperable )	{ return; }
			if ( !_isActive )	{ return; }
			if ( _ranState < SMTaskRunState.InitialEnable )	{ return; }

			_fixedUpdateEvent.Run();

			if ( _ranState == SMTaskRunState.InitialEnable )	{ _ranState = SMTaskRunState.FixedUpdate; }
		}

		public void Update() {
			if ( !_isOperable )	{ return; }
			if ( !_isActive )	{ return; }
			if ( _ranState < SMTaskRunState.FixedUpdate )	{ return; }

			_updateEvent.Run();

			if ( _ranState == SMTaskRunState.FixedUpdate )	{ _ranState = SMTaskRunState.Update; }
		}

		public void LateUpdate() {
			if ( !_isOperable )	{ return; }
			if ( !_isActive )		{ return; }
			if ( _ranState < SMTaskRunState.Update )	{ return; }

			_lateUpdateEvent.Run();

			if ( _ranState == SMTaskRunState.Update )	{ _ranState = SMTaskRunState.LateUpdate; }
		}



		public SMBehaviourBody GetFirst()
			=> _objectBody._behaviourBody;

		public SMBehaviourBody GetLast() {
			SMBehaviourBody current = null;
			for ( current = this; current._next != null; current = current._next )	{}
			return current;
		}

		public IEnumerable<SMBehaviourBody> GetBrothers() {
			for ( var current = GetFirst(); current != null; current = current._next ) {
				yield return current;
			}
		}
	}
}