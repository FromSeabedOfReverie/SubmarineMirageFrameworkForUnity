//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Base {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using KoganeUnityLib;
	using MultiEvent;
	using Task.Modifyler;
	using Task.Modifyler.Base;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMBehaviourBody : BaseSMTaskModifylerOwner<SMBehaviourModifyler> {
		public ISMBehaviour _behaviour	{ get; private set; }
		public SMObjectBody _objectBody	{ get; set; }

		[SMShowLine] public SMBehaviourBody _previous	{ get; set; }
		[SMShowLine] public SMBehaviourBody _next		{ get; set; }

		public SMTaskType _type			=> _behaviour._type;
		public SMTaskLifeSpan _lifeSpan	=> _behaviour._lifeSpan;
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

		public readonly SMAsyncCanceler _asyncCancelerOnDisable = new SMAsyncCanceler();
		public readonly SMAsyncCanceler _asyncCancelerOnDispose = new SMAsyncCanceler();


		public SMBehaviourBody( ISMBehaviour behaviour ) {
			_modifyler = new SMBehaviourModifyler( this );
			_behaviour = behaviour;

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

		public void DisposeBrothers( SMTaskType type ) {
			GetBrothers()
				.Where( b => b._type == type )
				.Reverse()
				.ForEach( b => b._behaviour.Dispose() );
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



		public bool IsActiveInMonoBehaviour() {
			if ( !_objectBody._isGameObject )	{ return true; }

			var mb = (SMMonoBehaviour)_behaviour;
			return mb.enabled;
		}

		public bool IsActiveInHierarchyAndMonoBehaviour() {
			if ( !_objectBody._isGameObject )	{ return true; }

			var mb = (SMMonoBehaviour)_behaviour;
			return _objectBody._gameObject.activeInHierarchy && mb.enabled;
		}



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



		public static T Generate<T>() where T : ISMBehaviour
			=> (T)Generate( typeof( T ) );

		public static ISMBehaviour Generate( Type type ) {
			if ( type.IsInheritance( typeof( SMBehaviour ) ) ) {
				var b = type.Create<SMBehaviour>();
				b.Setup();
				return b;

			} else if ( type.IsInheritance( typeof( SMMonoBehaviour ) ) ) {
				var go = new GameObject( type.GetAboutName() );
				var b = (ISMBehaviour)go.AddComponent( type );
				new SMGroup( go, new ISMBehaviour[] { b } );
				return b;

			} else {
				throw new InvalidOperationException(
					$"{nameof( ISMBehaviour )}でない為、作成不可 : {type.GetAboutName()}" );
			}
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


		public T GetBehaviour<T>() where T : ISMBehaviour
			=> (T)GetBehaviour( typeof( T ) );

		public ISMBehaviour GetBehaviour( Type type )
			=> GetBehaviours( type ).FirstOrDefault();


		public IEnumerable<T> GetBehaviours<T>() where T : ISMBehaviour
			=> GetBehaviours( typeof( T ) )
				.Select( b => (T)b );

		public IEnumerable<ISMBehaviour> GetBehaviours( Type type )
			=> GetBrothers()
				.Select( b => b._behaviour )
				.Where( b => b.GetType().IsInheritance( type ) );



		public override void SetToString() {
			base.SetToString();


			_toStringer.SetValue( nameof( _id ), i => $"{_id} ↑{_previous?._id} ↓{_next?._id}" );
			_toStringer.SetValue( nameof( _behaviour ), i => $"{_behaviour._id} {_behaviour.GetAboutName()}" );
			_toStringer.SetValue( nameof( _objectBody ), i => _toStringer.DefaultValue( _objectBody, i, true ) );

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