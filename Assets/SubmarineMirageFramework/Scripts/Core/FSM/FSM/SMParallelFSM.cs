//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.FSM {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using KoganeUnityLib;
	using MultiEvent;
	using Task;
	using State;



// TODO : コメント追加、整頓



	public abstract class SMParallelFSM<TOwner, TInternalFSM, TEnum> : BaseSMFSM, IBaseSMFSMOwner
		where TOwner : IBaseSMFSMOwner
		where TInternalFSM : BaseSMFSM
		where TEnum : Enum
	{
		public override bool _isInitialized	=> _owner._isInitialized;
		public override bool _isOperable	=> _owner._isOperable;
		public override bool _isFinalizing	=> _owner._isFinalizing;
		public override bool _isActive		=> _owner._isActive;

		bool _isInitialEnteredCache	{ get; set; }
		public override bool _isInitialEntered	{
			get {
				if ( _isInitialEnteredCache )	{ return true; }
				_isInitialEnteredCache = _fsms.All( pair => pair.Value._isInitialEntered );
				return _isInitialEnteredCache;
			}
			set => throw new InvalidOperationException( $"利用不可 : {nameof( _isInitialEntered )}" );
		}

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

		public override BaseSMState _rawState {
			get => throw new InvalidOperationException( $"利用不可 : {nameof( _rawState )}" );
			set => throw new InvalidOperationException( $"利用不可 : {nameof( _rawState )}" );
		}


		public SMParallelFSM( TOwner owner, Dictionary<TEnum, TInternalFSM> fsms ) {
			_fsms = fsms;
			Set( owner );
		}

		public override void Set( IBaseSMFSMOwner owner ) {
			_owner = (TOwner)owner;
			_fsms.ForEach( pair => pair.Value.Set( this ) );
		}


		public TInternalFSM GetFSM( TEnum key )
			=> _fsms.GetOrDefault( key );


		public override BaseSMState GetRawState( Type stateType )
			=> throw new InvalidOperationException( $"利用不可 : {nameof( GetRawState )}" );
	}
}