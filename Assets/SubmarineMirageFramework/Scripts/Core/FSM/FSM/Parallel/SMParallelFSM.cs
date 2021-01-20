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
	using UniRx;
	using KoganeUnityLib;
	using MultiEvent;
	using Task;
	using FSM.Base;
	using FSM.Modifyler;



// TODO : コメント追加、整頓



	public abstract class SMParallelFSM<TOwner, TInternalFSM, TEnum> : SMFSM, IBaseSMFSMOwner
		where TOwner : IBaseSMFSMOwner
		where TInternalFSM : SMFSM
		where TEnum : Enum
	{
		public override bool _isInitialized	=> _owner._isInitialized;
		public override bool _isOperable	=> _owner._isOperable;
		public override bool _isFinalizing	=> _owner._isFinalizing;
		public override bool _isActive		=> _owner._isActive;

		public override SMMultiAsyncEvent _selfInitializeEvent	=> _owner._selfInitializeEvent;
		public override SMMultiAsyncEvent _initializeEvent		=> _owner._initializeEvent;
		public override SMMultiSubject _enableEvent				=> _owner._enableEvent;
		public override SMMultiSubject _fixedUpdateEvent		=> _owner._fixedUpdateEvent;
		public override SMMultiSubject _updateEvent				=> _owner._updateEvent;
		public override SMMultiSubject _lateUpdateEvent			=> _owner._lateUpdateEvent;
		public override SMMultiSubject _disableEvent			=> _owner._disableEvent;
		public override SMMultiAsyncEvent _finalizeEvent		=> _owner._finalizeEvent;

		public override SMTaskCanceler _asyncCancelerOnDisable	=> _owner._asyncCancelerOnDisable;
		public override SMTaskCanceler _asyncCancelerOnDispose	=> _owner._asyncCancelerOnDispose;

		public TOwner _owner	{ get; private set; }
		public readonly Dictionary<TEnum, TInternalFSM> _fsms = new Dictionary<TEnum, TInternalFSM>();


		public SMParallelFSM( TOwner owner, Dictionary<TEnum, TInternalFSM> fsms ) {
			_fsms = fsms;
			Set( owner );

			_disposables.AddLast( () => {
				_fsms.ForEach( pair => pair.Value.Dispose() );
				_fsms.Clear();
			} );
		}

		public override void Set( IBaseSMFSMOwner owner ) {
			_owner = (TOwner)owner;
			_fsms.ForEach( pair => pair.Value.Set( this ) );

			_modifyler.Register( new InitialEnterSMParallelFSM<TOwner, TInternalFSM, TEnum>() );

			_disableEvent.AddFirst( _registerEventName ).Subscribe( _ => {
				_modifyler.Reset();
			} );
			_updateEvent.AddLast( _registerEventName ).Subscribe( _ => {
				_modifyler.Run().Forget();
			} );
		}


		public TInternalFSM GetFSM( TEnum type )
			=> _fsms.GetOrDefault( type );


		public override UniTask FinalExit()
			=> _modifyler.RegisterAndRun( new FinalExitSMParallelFSM<TOwner, TInternalFSM, TEnum>() );
	}
}