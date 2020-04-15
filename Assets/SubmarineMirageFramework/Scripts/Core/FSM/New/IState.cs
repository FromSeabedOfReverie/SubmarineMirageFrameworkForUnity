//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.New {
	using System.Threading;
	using UniRx.Async;
	using Process.New;
	using Extension;


	// TODO : コメント追加、整頓


	public interface IState<TFSM, TOwner> : IDisposableExtension
		where TFSM : IFiniteStateMachine
		where TOwner : IFiniteStateMachineOwner<TFSM>
	{
		TFSM _fsm		{ get; }
		TOwner _owner	{ get; }

		FiniteStateMachineRunState _runState	{ get; }
		bool _isActive	{ get; }
		CancellationToken _activeAsyncCancel	{ get; }

		void Set( TOwner owner );
		void StopActiveAsync();
		UniTask RunStateEvent( FiniteStateMachineRunState state );
		UniTask ChangeActive( bool isActive );
		UniTask RunProcessStateEvent( ProcessBody.RanState state );
	}
}