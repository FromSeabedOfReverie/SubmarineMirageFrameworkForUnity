//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTask
namespace SubmarineMirage.SMTask {
	using Cysharp.Threading.Tasks;
	using MultiEvent;
	using UTask;
	using Modifyler;
	using Extension;
	using Debug;
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public class SMBehaviourBody : IDisposableExtension {
		static uint s_idCount;
		public uint _id			{ get; private set; }

		public SMTaskRunState _ranState;
		public SMTaskActiveState _activeState;
		public SMTaskActiveState _initialActiveState	{ get; private set; }

		public bool _isInitialized =>	_ranState >= SMTaskRunState.Initialized;
		public bool _isOperable =>
			SMTaskRunState.Initialized <= _ranState && _ranState <= SMTaskRunState.LateUpdate;
		public bool _isActive =>		_activeState == SMTaskActiveState.Enable;
		public bool _isDispose =>		_disposables._isDispose;

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

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public SMBehaviourBody( ISMBehaviour owner, SMTaskActiveState initialActiveState ) {
			_id = ++s_idCount;

			_owner = owner;
			_initialActiveState = initialActiveState;
			_modifyler = new SMBehaviourModifyler( this );

			_disposables.AddLast( _modifyler );
			_disposables.AddLast(
				_asyncCancelerOnDisable,
				_asyncCancelerOnDispose,
				_selfInitializeEvent,
				_initializeEvent,
				_enableEvent,
				_fixedUpdateEvent,
				_updateEvent,
				_lateUpdateEvent,
				_disableEvent,
				_finalizeEvent
			);
			_disposables.AddLast( () => {
				SMBehaviourModifyData.UnLinkBehaviour( this );
				if ( _owner._object != null ) {
					if ( _owner._object._owner != null )	{ UnityObject.Destroy( (SMMonoBehaviour)_owner ); }
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

		~SMBehaviourBody() => Dispose();

		public void Dispose() => _disposables.Dispose();

		public void StopAsyncOnDisable() => _asyncCancelerOnDisable.Cancel();


		public override string ToString() => string.Join( "\n",
			$"    {nameof( SMBehaviourBody )}(",
			$"        {nameof( _id )} : {_id} ↑{_owner._previous?._id} ↓{_owner._next?._id}",
			$"        {nameof( _owner )} : {_owner._id} {_owner.GetAboutName()}",
			$"        {nameof( _ranState )} : {_ranState}",
			$"        {nameof( _activeState )} : {_activeState}",
			$"        {nameof( _initialActiveState )} : {_initialActiveState}",
			"",
			$"        {nameof( _asyncCancelerOnDisable )}._isCancel : {_asyncCancelerOnDisable._isCancel}",
			$"        {nameof( _asyncCancelerOnDispose )}._isCancel : {_asyncCancelerOnDispose._isCancel}",
			$"        {nameof( _isDispose )} : {_isDispose}",
			"",
			$"        {nameof( _selfInitializeEvent )}._isRunning : {_selfInitializeEvent._isRunning}",
			$"        {nameof( _initializeEvent )}._isRunning : {_initializeEvent._isRunning}",
			$"        {nameof( _enableEvent )}.Count : {_enableEvent._events.Count}",
			$"        {nameof( _fixedUpdateEvent )}.Count : {_fixedUpdateEvent._events.Count}",
			$"        {nameof( _updateEvent )}.Count : {_updateEvent._events.Count}",
			$"        {nameof( _lateUpdateEvent )}.Count : {_lateUpdateEvent._events.Count}",
			$"        {nameof( _disableEvent )}.Count : {_disableEvent._events.Count}",
			$"        {nameof( _finalizeEvent )}._isRunning : {_finalizeEvent._isRunning}",
			"",
			$"    {nameof( _modifyler )} : {_modifyler}",
			"    )"
		);

		public static string BehaviourToString( ISMBehaviour behaviour ) => string.Join( "\n",
			$"{behaviour.GetAboutName()}(",
			$"    {nameof( behaviour._type )} : {behaviour._type}",
			$"    {nameof( behaviour._lifeSpan )} : {behaviour._lifeSpan}",
			$"    {nameof( behaviour._object._owner )} : {behaviour._object?.ToLineString()}",
			$"    {nameof( behaviour._previous )} : {behaviour._previous?.ToLineString()}",
			$"    {nameof( behaviour._next )} : {behaviour._next?.ToLineString()}",
			$"    {nameof( behaviour._body )} : {behaviour._body}",
			")"
		);

		public static string BehaviourToLineString( ISMBehaviour behaviour ) => string.Join( " ",
			behaviour._id,
			behaviour.GetAboutName(),
			behaviour._body?._ranState,
			behaviour._body?._activeState,
			behaviour._body?._initialActiveState,
			$"↑{behaviour._previous?._id}",
			$"↓{behaviour._next?._id}",
			behaviour._isDispose ? "Dispose" : ""
		);
	}
}