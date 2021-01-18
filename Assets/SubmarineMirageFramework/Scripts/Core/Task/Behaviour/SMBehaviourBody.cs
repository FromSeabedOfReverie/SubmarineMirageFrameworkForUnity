//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestBehaviour
namespace SubmarineMirage.Task.Behaviour {
	using MultiEvent;
	using Modifyler;
	using Task.Modifyler;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMBehaviourBody : BaseSMTaskModifylerOwner<SMBehaviourModifyler> {
		public ISMBehaviour _behaviour	{ get; private set; }
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


		public SMBehaviourBody( ISMBehaviour behaviour, bool isRunInitialActive ) {
			_modifyler = new SMBehaviourModifyler( this );
			_behaviour = behaviour;
			_isRunInitialActive = isRunInitialActive;

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
#if TestBehaviour
			_disposables.AddLast( () =>
				SMLog.Debug( $"{nameof( SMBehaviourBody )}.{nameof( Dispose )} : {this}" )
			);
			SMLog.Debug( $"{nameof( SMBehaviourBody )}() : {this}" );
#endif
		}

		public override void Dispose() => base.Dispose();



		public override void SetToString() {
			base.SetToString();
			_toStringer.SetValue( nameof( _id ), i =>
				$"{_id} ↑{_behaviour._previous?._id} ↓{_behaviour._next?._id}" );
			_toStringer.SetValue( nameof( _behaviour ), i =>
				$"{_behaviour._id} {_behaviour.GetAboutName()}" );
			_toStringer.SetValue( nameof( _asyncCancelerOnDisable ), i =>
				$"_isCancel : {_asyncCancelerOnDisable._isCancel}" );
			_toStringer.SetValue( nameof( _asyncCancelerOnDispose ), i =>
				$"_isCancel : {_asyncCancelerOnDispose._isCancel}" );
			_toStringer.SetValue( nameof( _selfInitializeEvent ), i =>
				$"_isRunning : {_selfInitializeEvent._isRunning}" );
			_toStringer.SetValue( nameof( _initializeEvent ), i =>
				$"_isRunning : {_initializeEvent._isRunning}" );
			_toStringer.SetValue( nameof( _enableEvent ), i =>
				$"Count : {_enableEvent._events.Count}" );
			_toStringer.SetValue( nameof( _fixedUpdateEvent ), i =>
				$"Count : {_fixedUpdateEvent._events.Count}" );
			_toStringer.SetValue( nameof( _updateEvent ), i =>
				$"Count : {_updateEvent._events.Count}" );
			_toStringer.SetValue( nameof( _lateUpdateEvent ), i =>
				$"Count : {_lateUpdateEvent._events.Count}" );
			_toStringer.SetValue( nameof( _disableEvent ), i =>
				$"Count : {_disableEvent._events.Count}" );
			_toStringer.SetValue( nameof( _finalizeEvent ), i =>
				$"_isRunning : {_finalizeEvent._isRunning}" );
		}

		public static void SetBehaviourToString( ISMBehaviour behaviour ) {
			behaviour._toStringer.SetValue( nameof( behaviour._object ), i =>
				behaviour._object?.ToLineString() );
			behaviour._toStringer.SetValue( nameof( behaviour._previous ), i =>
				behaviour._previous?.ToLineString() );
			behaviour._toStringer.SetValue( nameof( behaviour._next ), i =>
				behaviour._next?.ToLineString() );

			behaviour._toStringer.SetLineValue( nameof( behaviour._previous ), () =>
				$"↑{behaviour._previous?._id}" );
			behaviour._toStringer.SetLineValue( nameof( behaviour._next ), () =>
				$"↓{behaviour._next?._id}" );
			behaviour._toStringer.AddLine( nameof( behaviour._body._ranState ), () =>
				$"{behaviour._body?._ranState}" );
			behaviour._toStringer.AddLine( nameof( behaviour._body._activeState ), () =>
				$"{behaviour._body?._activeState}" );
			behaviour._toStringer.AddLine( nameof( behaviour._body._isRunInitialActive ), () =>
				$"{behaviour._body?._isRunInitialActive}" );
		}
	}
}