//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using Base;
	using Event;
	using Utility;



	public interface ISMFSMOwner : ISMStandardBase {
		SMFSM _fsm	{ get; }

		bool _isInitialized			{ get; }
		bool _isFinalize			{ get; }
		bool _isOperable			{ get; }
		bool _isActive				{ get; }
		bool _isInitialEnteredFSMs	{ get; set; }

		SMAsyncEvent _selfInitializeEvent	{ get; }
		SMAsyncEvent _initializeEvent		{ get; }
		SMSubject _enableEvent				{ get; }
		SMSubject _fixedUpdateEvent			{ get; }
		SMSubject _updateEvent				{ get; }
		SMSubject _lateUpdateEvent			{ get; }
		SMSubject _disableEvent				{ get; }
		SMAsyncEvent _finalizeEvent			{ get; }

		SMAsyncCanceler _asyncCancelerOnDispose	{ get; }
	}
}