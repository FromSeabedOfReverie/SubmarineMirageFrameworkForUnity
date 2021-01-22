//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Base {
	using SubmarineMirage.Base;
	using MultiEvent;
	using Task;



	// TODO : コメント追加、整頓



	public interface IBaseSMFSMOwner : ISMStandardBase {
		bool _isInitialized	{ get; }
		bool _isOperable	{ get; }
		bool _isFinalizing	{ get; }
		bool _isActive		{ get; }

		SMMultiAsyncEvent _selfInitializeEvent	{ get; }
		SMMultiAsyncEvent _initializeEvent		{ get; }
		SMMultiSubject _enableEvent				{ get; }
		SMMultiSubject _fixedUpdateEvent		{ get; }
		SMMultiSubject _updateEvent				{ get; }
		SMMultiSubject _lateUpdateEvent			{ get; }
		SMMultiSubject _disableEvent			{ get; }
		SMMultiAsyncEvent _finalizeEvent		{ get; }

		SMTaskCanceler _asyncCancelerOnDispose	{ get; }
	}
}