//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Base {
	using System.Collections.Generic;
	using MultiEvent;
	using Task.Modifyler.Base;
	using Task.Modifyler;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMBehaviourBody : BaseSMTaskModifylerOwner<SMBehaviourModifyler> {
		public SMTaskType _type			=> _behaviour._type;
		public SMTaskLifeSpan _lifeSpan	=> _behaviour._lifeSpan;

		public ISMBehaviour _behaviour	{ get; private set; }
		public SMObjectBody _object		{ get; set; }

		[SMShowLine] public SMBehaviourBody _previous	{ get; set; }
		[SMShowLine] public SMBehaviourBody _next		{ get; set; }

		public bool _isRunInitialActive	{ get; set; }
		public bool _isRunFinalize	{ get; set; }

		public readonly SMMultiAsyncEvent _selfInitializeEvent = new SMMultiAsyncEvent();
		public readonly SMMultiAsyncEvent _initializeEvent = new SMMultiAsyncEvent();
		public readonly SMMultiSubject _enableEvent = new SMMultiSubject();
		public readonly SMMultiSubject _fixedUpdateEvent = new SMMultiSubject();
		public readonly SMMultiSubject _updateEvent = new SMMultiSubject();
		public readonly SMMultiSubject _lateUpdateEvent = new SMMultiSubject();
		public readonly SMMultiSubject _disableEvent = new SMMultiSubject();
		public readonly SMMultiAsyncEvent _finalizeEvent = new SMMultiAsyncEvent();

		public readonly SMTaskCanceler _asyncCancelerOnDisable = new SMTaskCanceler();
		public readonly SMTaskCanceler _asyncCancelerOnDispose = new SMTaskCanceler();


		public SMBehaviourBody( ISMBehaviour behaviour ) {
			_modifyler = new SMBehaviourModifyler( this );
			_behaviour = behaviour;

			if ( _behaviour is SMBehaviour ) {
				var o = new SMObject( null, new ISMBehaviour[] { _behaviour }, null );
				_object = o._body;
			}

			_isRunInitialActive = _type != SMTaskType.DontWork && IsActiveInHierarchyAndMonoBehaviour();

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



		public void Link( SMObjectBody smObject ) {
			var last = smObject._behaviour.GetBehaviourAtLast();
			last._next = this;
			_previous = last;
			_object = smObject;
		}


		public void Unlink() {
			if ( _object._behaviour == this )	{ _object._behaviour = _next; }
			if ( _previous != null )			{ _previous._next = _next; }
			if ( _next != null )				{ _next._previous = _previous; }
			_previous = null;
			_next = null;
		}



		public void StopAsyncOnDisable() {
			_asyncCancelerOnDisable.Cancel();
		}



		public bool IsActiveInMonoBehaviour() {
			if ( !_object._isGameObject )	{ return true; }

			var mb = (SMMonoBehaviour)_behaviour;
			return mb.enabled;
		}

		public bool IsActiveInHierarchyAndMonoBehaviour() {
			if ( !_object._isGameObject )	{ return true; }

			var mb = (SMMonoBehaviour)_behaviour;
			return _object._gameObject.activeInHierarchy && mb.enabled;
		}



		public void RegisterRunEventToOwner( SMObjectBody smObject ) {
			var add = this;

			if (	smObject._ranState >= SMTaskRunState.FinalDisable &&
					add._ranState < SMTaskRunState.FinalDisable &&
					add._type != SMTaskType.DontWork
			) {
				var isActive = smObject.IsActiveInHierarchy();
				add._modifyler.Register( new FinalDisableSMBehaviour( isActive ) );
			}

			if (	smObject._ranState >= SMTaskRunState.Finalize &&
					add._ranState < SMTaskRunState.Finalize
			) {
				if ( add._type != SMTaskType.DontWork ) {
					add._modifyler.Register( new FinalizeSMBehaviour() );
				} else {
					add.Dispose();
				}
			}

			if ( smObject._isFinalizing )	{ return; }


			if (	smObject._ranState >= SMTaskRunState.Create &&
					add._ranState < SMTaskRunState.Create
			) {
				add._modifyler.Register( new CreateSMBehaviour() );
			}

			if ( add._type == SMTaskType.DontWork )	{ return; }


			if (	smObject._ranState >= SMTaskRunState.SelfInitialize &&
					add._ranState < SMTaskRunState.SelfInitialize
			) {
				add._modifyler.Register( new SelfInitializeSMBehaviour() );
			}

			if (	smObject._ranState >= SMTaskRunState.Initialize &&
					add._ranState < SMTaskRunState.Initialize
			) {
				add._modifyler.Register( new InitializeSMBehaviour() );
			}

			if (	smObject._ranState >= SMTaskRunState.InitialEnable &&
					add._ranState < SMTaskRunState.InitialEnable
			) {
				var isActive = smObject.IsActiveInHierarchy();
				add._modifyler.Register( new InitialEnableSMBehaviour( isActive ) );
			}

			if ( !smObject._isInitialized )	{ return; }


			if ( smObject.IsActiveInHierarchy() ) {
				add._modifyler.Register( new EnableSMBehaviour() );

			} else {
				add._modifyler.Register( new DisableSMBehaviour() );
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



		public SMBehaviourBody GetBehaviourAtLast() {
			SMBehaviourBody current = null;
			for ( current = this; current._next != null; current = current._next )	{}
			return current;
		}

		public IEnumerable<SMBehaviourBody> GetBehaviours() {
			for ( var current = _object._behaviour; current != null; current = current._next ) {
				yield return current;
			}
		}



		public override void SetToString() {
			base.SetToString();


			_toStringer.SetValue( nameof( _id ), i => $"{_id} ↑{_previous?._id} ↓{_next?._id}" );
			_toStringer.SetValue( nameof( _behaviour ), i => $"{_behaviour._id} {_behaviour.GetAboutName()}" );
			_toStringer.SetValue( nameof( _object ), i => _toStringer.DefaultValue( _object, i, true ) );

			_toStringer.SetValue( nameof( _previous ), i => _toStringer.DefaultValue( _previous, i, true ) );
			_toStringer.SetValue( nameof( _next ), i => _toStringer.DefaultValue( _next, i, true ) );

			_toStringer.SetValue( nameof( _asyncCancelerOnDisable ), i =>
				$"_isCancel : {_asyncCancelerOnDisable._isCancel}" );
			_toStringer.SetValue( nameof( _asyncCancelerOnDispose ), i =>
				$"_isCancel : {_asyncCancelerOnDispose._isCancel}" );
			_toStringer.SetValue( nameof( _selfInitializeEvent ), i =>
				$"_isRunning : {_selfInitializeEvent._isRunning}" );

			_toStringer.SetValue( nameof( _initializeEvent ), i => $"_isRunning : {_initializeEvent._isRunning}" );
			_toStringer.SetValue( nameof( _enableEvent ), i => $"Count : {_enableEvent._events.Count}" );
			_toStringer.SetValue( nameof( _fixedUpdateEvent ), i => $"Count : {_fixedUpdateEvent._events.Count}" );
			_toStringer.SetValue( nameof( _updateEvent ), i => $"Count : {_updateEvent._events.Count}" );
			_toStringer.SetValue( nameof( _lateUpdateEvent ), i => $"Count : {_lateUpdateEvent._events.Count}" );
			_toStringer.SetValue( nameof( _disableEvent ), i => $"Count : {_disableEvent._events.Count}" );
			_toStringer.SetValue( nameof( _finalizeEvent ), i => $"_isRunning : {_finalizeEvent._isRunning}" );


			_toStringer.SetLineValue( nameof( _previous ), () => $"↑{_previous?._id}" );
			_toStringer.SetLineValue( nameof( _next ), () => $"↓{_next?._id}" );
		}
	}
}