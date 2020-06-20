//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask {
	using System.Threading;
	using UniRx.Async;
	using MultiEvent;
	using Extension;


	// TODO : コメント追加、整頓


	public interface ISMBehavior : IDisposableExtension {
		SMTaskType _type			{ get; }
		SMTaskLifeSpan _lifeSpan	{ get; }

		SMHierarchy _hierarchy	{ get; set; }
		SMBehaviorBody _body	{ get; }
		ISMBehavior _previous	{ get; set; }
		ISMBehavior _next		{ get; set; }

		bool _isInitialized	{ get; }
		bool _isActive		{ get; }

		MultiAsyncEvent _loadEvent			{ get; }
		MultiAsyncEvent _initializeEvent	{ get; }
		MultiAsyncEvent _enableEvent		{ get; }
		MultiSubject _fixedUpdateEvent		{ get; }
		MultiSubject _updateEvent			{ get; }
		MultiSubject _lateUpdateEvent		{ get; }
		MultiAsyncEvent _disableEvent		{ get; }
		MultiAsyncEvent _finalizeEvent		{ get; }

		MultiSubject _activeAsyncCancelEvent	{ get; }
		CancellationToken _activeAsyncCancel	{ get; }
		CancellationToken _inActiveAsyncCancel	{ get; }

		void Create();
		void StopActiveAsync();
		UniTask RunStateEvent( SMTaskRanState state );
		UniTask ChangeActive( bool isActive );
		UniTask RunActiveEvent();
		string ToString();
	}
}