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
	using Modifyler;
	using Extension;


	// TODO : コメント追加、整頓


	public interface ISMBehaviour : IDisposableExtension {
		SMTaskType _type			{ get; }
		SMTaskLifeSpan _lifeSpan	{ get; }

		uint _id				{ get; }
		SMObject _object		{ get; set; }
		SMBehaviourBody _body	{ get; }
		SMBehaviourModifyler _modifyler	{ get; }
		ISMBehaviour _previous	{ get; set; }
		ISMBehaviour _next		{ get; set; }

		bool _isInitialized	{ get; }
		bool _isActive		{ get; }

		MultiAsyncEvent _selfInitializeEvent	{ get; }
		MultiAsyncEvent _initializeEvent		{ get; }
		MultiSubject _enableEvent				{ get; }
		MultiSubject _fixedUpdateEvent			{ get; }
		MultiSubject _updateEvent				{ get; }
		MultiSubject _lateUpdateEvent			{ get; }
		MultiSubject _disableEvent				{ get; }
		MultiAsyncEvent _finalizeEvent			{ get; }

		UTaskCanceler _asyncCancelerOnDisable	{ get; }
		UTaskCanceler _asyncCancelerOnDispose	{ get; }

		void Create();
		void DestroyObject();
		void ChangeActiveObject( bool isActive );
		void StopAsyncOnDisable();
		string ToString();
		string ToLineString();
	}
}