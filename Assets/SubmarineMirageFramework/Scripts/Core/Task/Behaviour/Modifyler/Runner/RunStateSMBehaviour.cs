//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestBehaviourModifyler
namespace SubmarineMirage.Task.Behaviour.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class RunStateSMBehaviour : SMBehaviourModifyData {
		[SMShowLine] SMTaskRunState _state	{ get; set; }


		public RunStateSMBehaviour( SMBehaviourBody target, SMTaskRunState state ) : base( target ) {
			_state = state;
			_type = new Func<SMTaskModifyType>( () => {
				switch ( _state ) {
					case SMTaskRunState.Finalizing:			return SMTaskModifyType.Interrupter;
					case SMTaskRunState.Create:
					case SMTaskRunState.SelfInitializing:
					case SMTaskRunState.Initializing:
															return SMTaskModifyType.Linker;
					default:								return SMTaskModifyType.Runner;
				}
			} )();

			switch ( _state ) {
				case SMTaskRunState.FixedUpdate:
				case SMTaskRunState.Update:
				case SMTaskRunState.LateUpdate:
					throw new ArgumentOutOfRangeException(
						$"{_state}",
						$"負荷軽減の為、静的関数 {nameof( RegisterAndRun )} 以外で、実行不可"
					);
				case SMTaskRunState.None:
				case SMTaskRunState.SelfInitialized:
				case SMTaskRunState.Initialized:
				case SMTaskRunState.Finalized:
					throw new ArgumentOutOfRangeException(
						$"{_state}", $"実行状態に、実行後の型を指定した為、実行不可" );
			}
		}

		protected override void Cancel() {}



		public override async UniTask Run() {
			switch ( _state ) {
				case SMTaskRunState.Create:				Create();				return;
				case SMTaskRunState.SelfInitializing:	await SelfInitialize();	return;
				case SMTaskRunState.Initializing:		await Initialize();		return;
				case SMTaskRunState.Finalizing:			await Finalize();		return;
			}
		}

		void Create() {
			if ( _target._ranState != SMTaskRunState.None )	{ return; }
#if TestBehaviourModifyler
			SMLog.Debug( $"{_target._owner.GetAboutName()}.{nameof(Create)} :\n{_target}" );
#endif
			_target._owner.Create();
			_target._ranState = SMTaskRunState.Create;
		}

		async UniTask SelfInitialize() {
			if ( _target._owner._type == SMTaskType.DontWork )	{ return; }
			if ( _target._ranState != SMTaskRunState.Create )	{ return; }
#if TestBehaviourModifyler
			SMLog.Debug( $"{_target._owner.GetAboutName()}.{nameof(SelfInitialize)} :\n{_target}" );
#endif
			_target._ranState = SMTaskRunState.SelfInitializing;
			try {
				await _target._selfInitializeEvent.Run( _target._asyncCancelerOnDispose );
			} catch {
				_target._ranState = SMTaskRunState.Create;
				throw;
			}
			_target._ranState = SMTaskRunState.SelfInitialized;
		}

		async UniTask Initialize() {
			if ( _target._owner._type == SMTaskType.DontWork )			{ return; }
			if ( _target._ranState != SMTaskRunState.SelfInitialized )	{ return; }
#if TestBehaviourModifyler
			SMLog.Debug( $"{_target._owner.GetAboutName()}.{nameof(Initialize)} :\n{_target}" );
#endif
			_target._ranState = SMTaskRunState.Initializing;
			try {
				await _target._initializeEvent.Run( _target._asyncCancelerOnDispose );
			} catch {
				_target._ranState = SMTaskRunState.SelfInitialized;
				throw;
			}
			_target._ranState = SMTaskRunState.Initialized;
		}

		async UniTask Finalize() {
			if ( _target._owner._type == SMTaskType.DontWork )		{ return; }
			if ( _target._ranState == SMTaskRunState.Finalized )	{ return; }
			if ( _target._isActive ) {
#if TestBehaviourModifyler
				SMLog.Debug( $"{_target._owner.GetAboutName()}.{nameof(Finalize)} : Register Disable :\n{_target}" );
#endif
				_modifyler.Register( new ChangeActiveSMBehaviour( _target, false, SMTaskModifyType.Interrupter ) );
				_modifyler.Register( new RunStateSMBehaviour( _target, SMTaskRunState.Finalizing ) );
				return;
			}

			_target.StopAsyncOnDisable();	// 念の為
#if TestBehaviourModifyler
			SMLog.Debug( $"{_target._owner.GetAboutName()}.{nameof(Finalize)} :\n{_target}" );
#endif
			switch ( _target._ranState ) {
				case SMTaskRunState.SelfInitialized:
				case SMTaskRunState.Initialized:
				case SMTaskRunState.FixedUpdate:
				case SMTaskRunState.Update:
				case SMTaskRunState.LateUpdate:
					var lastRanState = _target._ranState;
					_target._ranState = SMTaskRunState.Finalizing;
					try {
						await _target._finalizeEvent.Run( _target._asyncCancelerOnDispose );
					} catch {
						_target._ranState = lastRanState;
						throw;
					}
					break;
			}
			_target._ranState = SMTaskRunState.Finalized;
			_modifyler.UnregisterAll();
		}

		static void FixedUpdate( SMBehaviourBody target ) {
			if ( target._owner._type == SMTaskType.DontWork )	{ return; }
			if ( !target._isActive )							{ return; }
			if ( target._ranState == SMTaskRunState.Initialized )	{ target._ranState = SMTaskRunState.FixedUpdate; }
			switch ( target._ranState ) {
				case SMTaskRunState.FixedUpdate:
				case SMTaskRunState.Update:
				case SMTaskRunState.LateUpdate:
#if TestBehaviourModifyler
					SMLog.Debug( $"{target._owner.GetAboutName()}.{nameof(FixedUpdate)} :\n{target}" );
#endif
					target._fixedUpdateEvent.Run();
					return;
			}
		}

		static void Update( SMBehaviourBody target ) {
			if ( target._owner._type == SMTaskType.DontWork )	{ return; }
			if ( !target._isActive )							{ return; }
			if ( target._ranState == SMTaskRunState.FixedUpdate )	{ target._ranState = SMTaskRunState.Update; }
			switch ( target._ranState ) {
				case SMTaskRunState.Update:
				case SMTaskRunState.LateUpdate:
#if TestBehaviourModifyler
					SMLog.Debug( $"{target._owner.GetAboutName()}.{nameof(Update)} :\n{target}" );
#endif
					target._updateEvent.Run();
					return;
			}
		}

		static void LateUpdate( SMBehaviourBody target ) {
			if ( target._owner._type == SMTaskType.DontWork )	{ return; }
			if ( !target._isActive )							{ return; }
			if ( target._ranState == SMTaskRunState.Update )	{ target._ranState = SMTaskRunState.LateUpdate; }
			switch ( target._ranState ) {
				case SMTaskRunState.LateUpdate:
#if TestBehaviourModifyler
					SMLog.Debug( $"{target._owner.GetAboutName()}.{nameof(LateUpdate)} :\n{target}" );
#endif
					target._lateUpdateEvent.Run();
					return;
			}
		}



		public static async UniTask RegisterAndRun( ISMBehaviour target, SMTaskRunState state ) {
			switch ( state ) {
				// 駄目元で、即時実行
				case SMTaskRunState.FixedUpdate:		FixedUpdate( target._body );	return;
				case SMTaskRunState.Update:				Update( target._body );			return;
				case SMTaskRunState.LateUpdate:			LateUpdate( target._body );		return;

				default:
					target._body._modifyler.Register( new RunStateSMBehaviour( target._body, state ) );
					await target._body._modifyler.WaitRunning();
					return;
			}
		}
	}
}