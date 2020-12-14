//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTask
namespace SubmarineMirage.SMTask {
	using Base;
	using MultiEvent;
	using UTask;
	using Modifyler;
	using Extension;
	using Debug;
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public class SMBehaviourBody : SMStandardBase {
		public SMTaskRunState _ranState;
		public bool _isInitialized =>	_ranState >= SMTaskRunState.Initialized;
		public bool _isOperable =>
			SMTaskRunState.Initialized <= _ranState && _ranState <= SMTaskRunState.LateUpdate;
		public bool _isActive;
		public bool _isInitialActive	{ get; private set; }

		public ISMBehaviour _owner	{ get; private set; }
		public SMBehaviourModifyler _modifyler	{ get; private set; }

		public readonly MultiAsyncEvent _selfInitializeEvent = new MultiAsyncEvent();
		public readonly MultiAsyncEvent _initializeEvent = new MultiAsyncEvent();
		public readonly MultiSubject _enableEvent = new MultiSubject();
		public readonly MultiSubject _fixedUpdateEvent = new MultiSubject();
		public readonly MultiSubject _updateEvent = new MultiSubject();
		public readonly MultiSubject _lateUpdateEvent = new MultiSubject();
		public readonly MultiSubject _disableEvent = new MultiSubject();
		public readonly MultiAsyncEvent _finalizeEvent = new MultiAsyncEvent();

		public readonly UTaskCanceler _asyncCancelerOnDisable = new UTaskCanceler();
		public readonly UTaskCanceler _asyncCancelerOnDispose = new UTaskCanceler();


		public SMBehaviourBody( ISMBehaviour owner, bool isInitialActive ) {
			_owner = owner;
			_isInitialActive = isInitialActive;
			_modifyler = new SMBehaviourModifyler( this );

			_disposables.AddLast( () => {
				_modifyler.Dispose();

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

				SMBehaviourModifyData.UnLinkBehaviour( this );
				if ( _owner._object != null ) {
					if ( _owner._object._isGameObject )			{ UnityObject.Destroy( (SMMonoBehaviour)_owner ); }
					if ( _owner._object._behaviour == null )	{ _owner._object.Dispose(); }
				}
			} );
#if TestSMTask
			_disposables.AddLast( () =>
				Log.Debug( $"{nameof( SMBehaviourBody )}.{nameof( Dispose )} : {this}" )
			);
			Log.Debug( $"{nameof( SMBehaviourBody )}() : {this}" );
#endif
		}


		public void StopAsyncOnDisable() => _asyncCancelerOnDisable.Cancel();


		public override void SetToString() {
			base.SetToString();
			_toStringer.SetValue(
				nameof( _id ), i => $"{_id} ↑{_owner._previous?._id} ↓{_owner._next?._id}" );
			_toStringer.SetValue(
				nameof( _owner ), i => $"{_owner._id} {_owner.GetAboutName()}" );
			_toStringer.SetValue(
				nameof( _asyncCancelerOnDisable ), i => $"_isCancel : {_asyncCancelerOnDisable._isCancel}" );
			_toStringer.SetValue(
				nameof( _asyncCancelerOnDispose ), i => $"_isCancel : {_asyncCancelerOnDispose._isCancel}" );
			_toStringer.SetValue(
				nameof( _selfInitializeEvent ), i => $"_isRunning : {_selfInitializeEvent._isRunning}" );
			_toStringer.SetValue(
				nameof( _initializeEvent ), i => $"_isRunning : {_initializeEvent._isRunning}" );
			_toStringer.SetValue(
				nameof( _enableEvent ), i => $"Count : {_enableEvent._events.Count}" );
			_toStringer.SetValue(
				nameof( _fixedUpdateEvent ), i => $"Count : {_fixedUpdateEvent._events.Count}" );
			_toStringer.SetValue(
				nameof( _updateEvent ), i => $"Count : {_updateEvent._events.Count}" );
			_toStringer.SetValue(
				nameof( _lateUpdateEvent ), i => $"Count : {_lateUpdateEvent._events.Count}" );
			_toStringer.SetValue(
				nameof( _disableEvent ), i => $"Count : {_disableEvent._events.Count}" );
			_toStringer.SetValue(
				nameof( _finalizeEvent ), i => $"_isRunning : {_finalizeEvent._isRunning}" );
		}

		public static void SetBehaviourToString( ISMBehaviour behaviour ) {
			behaviour._toStringer.SetValue(
				nameof( behaviour._object ), i => behaviour._object?.ToLineString() );
			behaviour._toStringer.SetValue(
				nameof( behaviour._previous ), i => behaviour._previous?.ToLineString() );
			behaviour._toStringer.SetValue(
				nameof( behaviour._next ), i => behaviour._next?.ToLineString() );

			behaviour._toStringer.SetLineValue(
				nameof( behaviour._previous ), () => $"↑{behaviour._previous?._id}" );
			behaviour._toStringer.SetLineValue(
				nameof( behaviour._next ), () => $"↓{behaviour._next?._id}" );
			behaviour._toStringer.AddLine(
				nameof( behaviour._body._ranState ), () => $"{behaviour._body?._ranState}" );
			behaviour._toStringer.AddLine(
				nameof( behaviour._body._isActive ), () => $"{behaviour._body?._isActive}" );
			behaviour._toStringer.AddLine(
				nameof( behaviour._body._isInitialActive ), () => $"{behaviour._body?._isInitialActive}" );
		}
	}
}