//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class RunStateSMBehaviour : SMBehaviourModifyData {
		SMTaskRunState _state;


		public RunStateSMBehaviour( SMBehaviourBody body, SMTaskRunState state ) : base( body ) {
			_state = state;
			_type = new Func<ModifyType>( () => {
				switch ( _state ) {
					case SMTaskRunState.Create:
					case SMTaskRunState.SelfInitializing:
					case SMTaskRunState.Initializing:
															return ModifyType.Initializer;
					case SMTaskRunState.Finalizing:			return ModifyType.Finalizer;
					default:								return ModifyType.Operator;
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

		public override void Cancel() {}



		public override async UniTask Run() {
			switch ( _state ) {
				case SMTaskRunState.Create:				Create();				return;
				case SMTaskRunState.SelfInitializing:	await SelfInitialize();	return;
				case SMTaskRunState.Initializing:		await Initialize();		return;
				case SMTaskRunState.Finalizing:			await Finalize();		return;
			}
		}

		void Create() {
			if ( _body._ranState != SMTaskRunState.None )	{ return; }
#if TestSMTaskModifyler
			Log.Debug( $"{_body._owner.GetAboutName()}.{nameof(Create)} :\n{_body}" );
#endif
			_body._owner.Create();
			_body._ranState = SMTaskRunState.Create;
		}

		async UniTask SelfInitialize() {
			if ( _body._owner._type == SMTaskType.DontWork )	{ return; }
			if ( _body._ranState != SMTaskRunState.Create )		{ return; }
#if TestSMTaskModifyler
			Log.Debug( $"{_body._owner.GetAboutName()}.{nameof(SelfInitialize)} :\n{_body}" );
#endif
			_body._ranState = SMTaskRunState.SelfInitializing;
			try {
				await _body._selfInitializeEvent.Run( _body._asyncCancelerOnDispose );
			} catch {
				_body._ranState = SMTaskRunState.Create;
				throw;
			}
			_body._ranState = SMTaskRunState.SelfInitialized;
		}

		async UniTask Initialize() {
			if ( _body._owner._type == SMTaskType.DontWork )			{ return; }
			if ( _body._ranState != SMTaskRunState.SelfInitialized )	{ return; }
#if TestSMTaskModifyler
			Log.Debug( $"{_body._owner.GetAboutName()}.{nameof(Initialize)} :\n{_body}" );
#endif
			_body._ranState = SMTaskRunState.Initializing;
			try {
				await _body._initializeEvent.Run( _body._asyncCancelerOnDispose );
			} catch {
				_body._ranState = SMTaskRunState.SelfInitialized;
				throw;
			}
			_body._ranState = SMTaskRunState.Initialized;
		}

		async UniTask Finalize() {
			if ( _body._owner._type == SMTaskType.DontWork )	{ return; }
			if ( _body._ranState == SMTaskRunState.Finalized )	{ return; }
			if ( _body._isActive ) {
#if TestSMTaskModifyler
				Log.Debug( $"{_body._owner.GetAboutName()}.{nameof(Finalize)} : Register Disable :\n{_body}" );
#endif
				_owner.Register( new ChangeActiveSMBehaviour( _body, false, ModifyType.Finalizer ) );
				_owner.Register( new RunStateSMBehaviour( _body, SMTaskRunState.Finalizing ) );
				return;
			}

			_body.StopAsyncOnDisable();	// 念の為
#if TestSMTaskModifyler
			Log.Debug( $"{_body._owner.GetAboutName()}.{nameof(Finalize)} :\n{_body}" );
#endif
			switch ( _body._ranState ) {
				case SMTaskRunState.SelfInitialized:
				case SMTaskRunState.Initialized:
				case SMTaskRunState.FixedUpdate:
				case SMTaskRunState.Update:
				case SMTaskRunState.LateUpdate:
					var lastRanState = _body._ranState;
					_body._ranState = SMTaskRunState.Finalizing;
					try {
						await _body._finalizeEvent.Run( _body._asyncCancelerOnDispose );
					} catch {
						_body._ranState = lastRanState;
						throw;
					}
					break;
			}
			_body._ranState = SMTaskRunState.Finalized;
			_owner.UnregisterAll();
		}

		static void FixedUpdate( SMBehaviourBody body ) {
			if ( body._owner._type == SMTaskType.DontWork )	{ return; }
			if ( !body._isActive )							{ return; }
			if ( body._ranState == SMTaskRunState.Initialized )	{ body._ranState = SMTaskRunState.FixedUpdate; }
			switch ( body._ranState ) {
				case SMTaskRunState.FixedUpdate:
				case SMTaskRunState.Update:
				case SMTaskRunState.LateUpdate:
#if TestSMTaskModifyler
					Log.Debug( $"{body._owner.GetAboutName()}.{nameof(FixedUpdate)} :\n{body}" );
#endif
					body._fixedUpdateEvent.Run();
					return;
			}
		}

		static void Update( SMBehaviourBody body ) {
			if ( body._owner._type == SMTaskType.DontWork )	{ return; }
			if ( !body._isActive )							{ return; }
			if ( body._ranState == SMTaskRunState.FixedUpdate )	{ body._ranState = SMTaskRunState.Update; }
			switch ( body._ranState ) {
				case SMTaskRunState.Update:
				case SMTaskRunState.LateUpdate:
#if TestSMTaskModifyler
					Log.Debug( $"{body._owner.GetAboutName()}.{nameof(Update)} :\n{body}" );
#endif
					body._updateEvent.Run();
					return;
			}
		}

		static void LateUpdate( SMBehaviourBody body ) {
			if ( body._owner._type == SMTaskType.DontWork )	{ return; }
			if ( !body._isActive )							{ return; }
			if ( body._ranState == SMTaskRunState.Update )	{ body._ranState = SMTaskRunState.LateUpdate; }
			switch ( body._ranState ) {
				case SMTaskRunState.LateUpdate:
#if TestSMTaskModifyler
					Log.Debug( $"{body._owner.GetAboutName()}.{nameof(LateUpdate)} :\n{body}" );
#endif
					body._lateUpdateEvent.Run();
					return;
			}
		}



		public static async UniTask RegisterAndRun( ISMBehaviour behaviour, SMTaskRunState state ) {
			switch ( state ) {
				// 駄目元で、即時実行
				case SMTaskRunState.FixedUpdate:		FixedUpdate( behaviour._body );	return;
				case SMTaskRunState.Update:				Update( behaviour._body );		return;
				case SMTaskRunState.LateUpdate:			LateUpdate( behaviour._body );	return;

				default:
					behaviour._body._modifyler.Register( new RunStateSMBehaviour( behaviour._body, state ) );
					await behaviour._body._modifyler.WaitRunning();
					return;
			}
		}


		public override string ToString() => base.ToString().InsertLast( ", ",
			string.Join( ", ",
				_state
			)
			+ ", "
		);
	}
}