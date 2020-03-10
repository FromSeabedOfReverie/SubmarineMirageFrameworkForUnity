//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process.New {
	using System;
	using System.Threading;
	using MultiEvent;


	// TODO : コメント追加、整頓


	public interface IProcess : IDisposable {
		CoreProcessManager.ExecutedState _executedState	{ get; set; }
		CoreProcessManager.ProcessType _type			{ get; }
		CoreProcessManager.ProcessLifeSpan _lifeSpan	{ get; }
		bool _isInitialized	{ get; set; }
		bool _isActive		{ get; set; }
		CancellationToken _activeAsyncCancel	{ get; }
		CancellationToken _finalizeAsyncCancel	{ get; }
		
		MultiAsyncEvent _loadEvent			{ get; }
		MultiAsyncEvent _initializeEvent	{ get; }
		MultiAsyncEvent _enableEvent		{ get; }
		MultiSubject _fixedUpdateEvent		{ get; }
		MultiSubject _updateEvent			{ get; }
		MultiSubject _lateUpdateEvent		{ get; }
		MultiAsyncEvent _disableEvent		{ get; }
		MultiAsyncEvent _finalizeEvent		{ get; }

		void Create();
		void StopActiveAsync();
		string ToString();
	}
}