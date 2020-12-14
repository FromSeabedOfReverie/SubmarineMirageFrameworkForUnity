//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using System;
	using Cysharp.Threading.Tasks;
	using Base;
	using UTask;
	using MultiEvent;


	// TODO : コメント追加、整頓


	public interface IFiniteStateMachine : ISMStandardBase {
		bool _isActive				{ get; }
		string _registerEventName	{ get; }
		bool _isChangingState		{ get; }

		MultiAsyncEvent _loadEvent			{ get; }
		MultiAsyncEvent _initializeEvent	{ get; }
		MultiAsyncEvent _enableEvent		{ get; }
		MultiSubject _fixedUpdateEvent		{ get; }
		MultiSubject _updateEvent			{ get; }
		MultiSubject _lateUpdateEvent		{ get; }
		MultiAsyncEvent _disableEvent		{ get; }
		MultiAsyncEvent _finalizeEvent		{ get; }

		UTaskCanceler _changeStateAsyncCanceler	{ get; }

		UniTask ChangeState( Type state );
	}
}