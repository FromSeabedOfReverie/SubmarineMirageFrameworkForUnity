//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Base {
	using SubmarineMirage.Base;
	using Event;
	using Utility;



	// TODO : コメント追加、整頓



	public interface IBaseSMFSMOwner : ISMStandardBase {
		bool _isInitialized	{ get; }
		bool _isOperable	{ get; }
		bool _isFinalizing	{ get; }
		bool _isActive		{ get; }
		bool _isInitialEnteredFSMs	{ get; set; }

		SMAsyncEvent _selfInitializeEvent	{ get; }
		SMAsyncEvent _initializeEvent		{ get; }
		SMSubject _enableEvent				{ get; }
		SMSubject _fixedUpdateEvent		{ get; }
		SMSubject _updateEvent				{ get; }
		SMSubject _lateUpdateEvent			{ get; }
		SMSubject _disableEvent			{ get; }
		SMAsyncEvent _finalizeEvent		{ get; }

		SMAsyncCanceler _asyncCancelerOnDispose	{ get; }
	}
}