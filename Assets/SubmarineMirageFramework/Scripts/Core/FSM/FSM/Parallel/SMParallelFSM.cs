//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using System;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Base;
	using MultiEvent;
	using Task;
	using Utility;
	using Debug;



// TODO : コメント追加、整頓



	public abstract class SMParallelFSM<TOwner, TFSM> : SMStandardBase, ISMParallelFSM
		where TOwner : ISMParallelFSMOwner
		where TFSM : ISMInternalFSM
	{
		[SMHide] public bool _isActive	=> _owner._isActive;
		public string _registerEventName	{ get; private set; }

		[SMHide] public SMMultiAsyncEvent _selfInitializeEvent	=> _owner._selfInitializeEvent;
		[SMHide] public SMMultiAsyncEvent _initializeEvent		=> _owner._initializeEvent;
		[SMHide] public SMMultiSubject _enableEvent				=> _owner._enableEvent;
		[SMHide] public SMMultiSubject _fixedUpdateEvent		=> _owner._fixedUpdateEvent;
		[SMHide] public SMMultiSubject _updateEvent				=> _owner._updateEvent;
		[SMHide] public SMMultiSubject _lateUpdateEvent			=> _owner._lateUpdateEvent;
		[SMHide] public SMMultiSubject _disableEvent			=> _owner._disableEvent;
		[SMHide] public SMMultiAsyncEvent _finalizeEvent		=> _owner._finalizeEvent;

		[SMHide] public SMTaskCanceler _asyncCancelerOnChange	{ get; private set; } = new SMTaskCanceler();

		protected TOwner _owner	{ get; private set; }
		public readonly Dictionary<Type, ISMInternalFSM> _fsms = new Dictionary<Type, ISMInternalFSM>();


		public SMParallelFSM( TOwner owner, ISMInternalFSM[] fsms ) {
			_owner = owner;
			fsms.ForEach( fsm => {
				if ( !( fsm is TFSM ) )	{ throw new Exception( "違う型の状態が代入された" ); }
				fsm.Set( this );
				_fsms[fsm.GetType()] = fsm;
			} );
		}

		public UniTask ChangeState( Type state ) => UTask.DontWait();
	}
}