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
	using Task;
	using MultiEvent;


	// TODO : コメント追加、整頓


	public interface ISMFSM : ISMStandardBase {
		bool _isActive				{ get; }
		string _registerEventName	{ get; }
		bool _isChangingState		{ get; }

		SMMultiAsyncEvent _loadEvent			{ get; }
		SMMultiAsyncEvent _initializeEvent	{ get; }
		SMMultiSubject _enableEvent			{ get; }
		SMMultiSubject _fixedUpdateEvent		{ get; }
		SMMultiSubject _updateEvent			{ get; }
		SMMultiSubject _lateUpdateEvent		{ get; }
		SMMultiSubject _disableEvent			{ get; }
		SMMultiAsyncEvent _finalizeEvent		{ get; }

		SMTaskCanceler _changeStateAsyncCanceler	{ get; }

		UniTask ChangeState( Type state );
	}
}