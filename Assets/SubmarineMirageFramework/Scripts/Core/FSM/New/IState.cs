//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.New {
	using MultiEvent;
	using Extension;


	// TODO : コメント追加、整頓


	public interface IState<TFSM, TOwner> : IDisposableExtension
		where TFSM : IFiniteStateMachine
		where TOwner : IFiniteStateMachineOwner<TFSM>
	{
		TFSM _fsm		{ get; }
		TOwner _owner	{ get; }

		MultiAsyncEvent _loadEvent			{ get; }
		MultiAsyncEvent _initializeEvent	{ get; }
		MultiAsyncEvent _enableEvent		{ get; }
		MultiAsyncEvent _enterEvent			{ get; }
		MultiAsyncEvent _updateEvent		{ get; }
		MultiSubject _fixedUpdateDeltaEvent	{ get; }
		MultiSubject _updateDeltaEvent		{ get; }
		MultiSubject _lateUpdateDeltaEvent	{ get; }
		MultiAsyncEvent _exitEvent			{ get; }
		MultiAsyncEvent _disableEvent		{ get; }
		MultiAsyncEvent _finalizeEvent		{ get; }

		void Set( TOwner owner );
	}
}