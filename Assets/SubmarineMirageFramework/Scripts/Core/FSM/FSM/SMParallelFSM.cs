//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.FSM {
	using System;
	using System.Collections.Generic;
	using KoganeUnityLib;
	using MultiEvent;
	using Task;
	using Task.Behaviour;
	using State;



// TODO : コメント追加、整頓



	public abstract class SMParallelFSM<TOwner, TInternalFSM, TEnum> : BaseSMFSM, IBaseSMFSMOwner
		where TOwner : IBaseSMFSMOwner, ISMBehaviour
		where TInternalFSM : BaseSMFSM
		where TEnum : Enum
	{
		public override SMMultiAsyncEvent _selfInitializeEvent	=> ( (ISMBehaviour)_owner )._selfInitializeEvent;
		public override SMMultiAsyncEvent _initializeEvent		=> ( (ISMBehaviour)_owner )._initializeEvent;
		public override SMMultiSubject _enableEvent				=> ( (ISMBehaviour)_owner )._enableEvent;
		public override SMMultiSubject _fixedUpdateEvent		=> ( (ISMBehaviour)_owner )._fixedUpdateEvent;
		public override SMMultiSubject _updateEvent				=> ( (ISMBehaviour)_owner )._updateEvent;
		public override SMMultiSubject _lateUpdateEvent			=> ( (ISMBehaviour)_owner )._lateUpdateEvent;
		public override SMMultiSubject _disableEvent			=> ( (ISMBehaviour)_owner )._disableEvent;
		public override SMMultiAsyncEvent _finalizeEvent		=> ( (ISMBehaviour)_owner )._finalizeEvent;

		public SMTaskCanceler _asyncCancelerOnDisable => ( (ISMBehaviour)_owner )._asyncCancelerOnDisable;

		public override SMTaskRunState _taskRanState => _owner._body._ranState;

		public TOwner _owner	{ get; private set; }
		public readonly Dictionary<TEnum, TInternalFSM> _fsms = new Dictionary<TEnum, TInternalFSM>();

		public override BaseSMState _rawState {
			get { throw new InvalidOperationException( $"利用不可 : {_rawState}" ); }
			set { throw new InvalidOperationException( $"利用不可 : {_rawState}" ); }
		}


		public SMParallelFSM( TOwner owner, Dictionary<TEnum, TInternalFSM> fsms ) {
			Set( owner );
			_fsms = fsms;
			fsms.ForEach( pair => pair.Value.Set( this ) );
		}

		public override void Set( IBaseSMFSMOwner owner ) {
			_owner = (TOwner)owner;
		}


		public TInternalFSM Get( TEnum key )
			=> _fsms.GetOrDefault( key );
	}
}