//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using Cysharp.Threading.Tasks;
	using Base;
	using MultiEvent;
	using Task;


	// TODO : コメント追加、整頓


	public interface ISMState<TFSM, TOwner> : ISMStandardBase
		where TFSM : ISMFSM
		where TOwner : ISMFSMOwner<TFSM>
	{
		TFSM _fsm		{ get; }
		TOwner _owner	{ get; }

		SMFSMRunState _runState	{ get; }

		SMMultiAsyncEvent _selfInitializeEvent	{ get; }
		SMMultiAsyncEvent _initializeEvent		{ get; }
		SMMultiSubject _enableEvent				{ get; }
		SMMultiSubject _fixedUpdateEvent		{ get; }
		SMMultiSubject _updateEvent				{ get; }
		SMMultiSubject _lateUpdateEvent			{ get; }
		SMMultiSubject _disableEvent			{ get; }
		SMMultiAsyncEvent _finalizeEvent		{ get; }

		SMTaskCanceler _asyncCancelerOnChangeOrDisable	{ get; }

		void Set( TOwner owner );
		void StopActiveAsync();
	}
}