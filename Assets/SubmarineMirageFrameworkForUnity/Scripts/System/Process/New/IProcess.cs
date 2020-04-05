//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process.New {
	using System.Threading;
	using UniRx.Async;
	using MultiEvent;
	using Extension;


	// TODO : コメント追加、整頓


	public interface IProcess : IDisposableExtension {
		ProcessBody.Type _type			{ get; }
		ProcessBody.LifeSpan _lifeSpan	{ get; }

		ProcessHierarchy _hierarchy	{ get; set; }
		ProcessBody _process		{ get; }

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

		CancellationToken _activeAsyncCancel	{ get; }
		CancellationToken _inActiveAsyncCancel	{ get; }

		void Create();
		void StopActiveAsync();
		UniTask RunStateEvent( ProcessBody.RanState state );
		UniTask ChangeActive( bool isActive );
		UniTask RunActiveEvent();
		string ToString();
	}
}