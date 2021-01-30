//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using MultiEvent;
	using Task;
	using FSM.Base;
	using FSM.Modifyler;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMParallelFSM<TOwner, TInternalFSM, TEnum> : SMFSM, IBaseSMFSMOwner
		where TOwner : IBaseSMFSMOwner
		where TInternalFSM : SMFSM
		where TEnum : Enum
	{
		[SMHide] public override bool _isInitialized	=> _owner._isInitialized;
		[SMHide] public override bool _isOperable	=> _owner._isOperable;
		[SMHide] public override bool _isFinalizing	=> _owner._isFinalizing;
		[SMHide] public override bool _isActive		=> _owner._isActive;

		[SMHide] public override SMMultiAsyncEvent _selfInitializeEvent	=> _owner._selfInitializeEvent;
		[SMHide] public override SMMultiAsyncEvent _initializeEvent		=> _owner._initializeEvent;
		[SMHide] public override SMMultiSubject _enableEvent				=> _owner._enableEvent;
		[SMHide] public override SMMultiSubject _fixedUpdateEvent		=> _owner._fixedUpdateEvent;
		[SMHide] public override SMMultiSubject _updateEvent				=> _owner._updateEvent;
		[SMHide] public override SMMultiSubject _lateUpdateEvent			=> _owner._lateUpdateEvent;
		[SMHide] public override SMMultiSubject _disableEvent			=> _owner._disableEvent;
		[SMHide] public override SMMultiAsyncEvent _finalizeEvent		=> _owner._finalizeEvent;

		[SMHide] public override SMTaskCanceler _asyncCancelerOnDispose	=> _owner._asyncCancelerOnDispose;

		[SMHide] public TOwner _owner	{ get; private set; }
		public readonly Dictionary<TEnum, TInternalFSM> _fsms = new Dictionary<TEnum, TInternalFSM>();


		public SMParallelFSM( TOwner owner, Dictionary<TEnum, TInternalFSM> fsms ) {
			_fsms = fsms;
			_fsms.ForEach( pair => pair.Value.SetFSMType( pair.Key ) );
			Set( owner, owner );

			_disposables.AddLast( () => {
				_fsms.ForEach( pair => pair.Value.Dispose() );
				_fsms.Clear();
			} );
		}

		public override void Set( IBaseSMFSMOwner topOwner, IBaseSMFSMOwner owner ) {
			_owner = (TOwner)owner;
			base.Set( topOwner, owner );
			_fsms.ForEach( pair => pair.Value.Set( topOwner, this ) );

			_modifyler.Register( new InitialEnterSMParallelFSM<TOwner, TInternalFSM, TEnum>() );
		}


		public IEnumerable<TInternalFSM> GetFSMs()
			=> _fsms.Select( pair => pair.Value );

		public TInternalFSM GetFSM( TEnum type )
			=> _fsms.GetOrDefault( type );


		public override UniTask FinalExit()
			=> _modifyler.RegisterAndRun( new FinalExitSMParallelFSM<TOwner, TInternalFSM, TEnum>() );



		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _fsms ), i => {
				var arrayI = StringSMUtility.IndentSpace( i + 1 );
				return "\n" + string.Join( ",\n", _fsms.Select( pair =>
					$"{arrayI}{pair.Key} : {pair.Value.ToString( i + 2 )}"
				) );
			} );
		}
	}
}