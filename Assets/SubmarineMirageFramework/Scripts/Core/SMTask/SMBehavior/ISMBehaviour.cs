//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask {
	using Cysharp.Threading.Tasks;
	using MultiEvent;
	using UTask;
	using Extension;


	// TODO : コメント追加、整頓


	public interface ISMBehaviour : IDisposableExtension {
		SMTaskType _type			{ get; }
		SMTaskLifeSpan _lifeSpan	{ get; }

		SMObject _object		{ get; set; }
		SMBehaviourBody _body	{ get; }
		uint _id				{ get; }
		ISMBehaviour _previous	{ get; set; }
		ISMBehaviour _next		{ get; set; }

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

		UTaskCanceler _activeAsyncCanceler		{ get; }
		UTaskCanceler _inActiveAsyncCanceler	{ get; }

		void Create();
		void DestroyObject();
		void StopActiveAsync();
		UniTask RunStateEvent( SMTaskRanState state );
		UniTask ChangeActive( bool isActive );
		UniTask RunActiveEvent();
		string ToString();
		string ToLineString();
	}
}