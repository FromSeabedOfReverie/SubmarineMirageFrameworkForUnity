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
	using State;



// TODO : コメント追加、整頓



	public abstract class SMParallelFSM<TOwner, TOwnerFSM, TInternalFSM, TEnum> : BaseSMFSM
		where TOwner : ISMFSMOwner<TOwnerFSM>
		where TOwnerFSM : BaseSMFSM
		where TInternalFSM : BaseSMInternalFSM
		where TEnum : Enum
	{
		public override SMMultiAsyncEvent _selfInitializeEvent	=> _owner._selfInitializeEvent;
		public override SMMultiAsyncEvent _initializeEvent		=> _owner._initializeEvent;
		public override SMMultiSubject _enableEvent				=> _owner._enableEvent;
		public override SMMultiSubject _fixedUpdateEvent		=> _owner._fixedUpdateEvent;
		public override SMMultiSubject _updateEvent				=> _owner._updateEvent;
		public override SMMultiSubject _lateUpdateEvent			=> _owner._lateUpdateEvent;
		public override SMMultiSubject _disableEvent			=> _owner._disableEvent;
		public override SMMultiAsyncEvent _finalizeEvent		=> _owner._finalizeEvent;

		public TOwner _owner	{ get; private set; }
		public readonly Dictionary<TEnum, TInternalFSM> _fsms = new Dictionary<TEnum, TInternalFSM>();

		public override BaseSMState _rawState {
			get { throw new Exception( $"存在しないエラー : {_rawState}" ); }
			set { throw new Exception( $"存在しないエラー : {_rawState}" ); }
		}


		public SMParallelFSM( TOwner owner, Dictionary<TEnum, TInternalFSM> fsms ) {
			_owner = owner;
			_fsms = fsms;
			fsms.ForEach( pair => pair.Value.Set( this ) );
		}


		public TInternalFSM Get( TEnum key )
			=> _fsms.GetOrDefault( key );
	}
}