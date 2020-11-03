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
			_type = _state == SMTaskRunState.Finalizing ? ModifyType.Interrupter : ModifyType.Runner;

			switch ( state ) {
				case SMTaskRunState.FixedUpdate:
				case SMTaskRunState.Update:
				case SMTaskRunState.LateUpdate:
					throw new ArgumentOutOfRangeException(
						$"{state}",
						$"負荷軽減の為、静的関数 {nameof( RegisterAndRun )} 以外で、実行不可"
					);
			}
		}

		public override void Cancel() {}


		public override UniTask Run() => RunState( _body, _state );


		static async UniTask RunState( SMBehaviourBody body, SMTaskRunState state ) {
			switch ( state ) {
				case SMTaskRunState.Create:
					switch ( body._ranState ) {
						case SMTaskRunState.None:
#if TestSMTaskModifyler
							Log.Debug( $"{body._owner.GetAboutName()}.{nameof(RunState)} : {state}\n{body}" );
#endif
							body._owner.Create();
							body._ranState = SMTaskRunState.Create;
							return;
					}
					return;

				case SMTaskRunState.SelfInitializing:
					if ( body._owner._type == SMTaskType.DontWork )	{ return; }
					switch ( body._ranState ) {
						case SMTaskRunState.Create:
#if TestSMTaskModifyler
							Log.Debug( $"{body._owner.GetAboutName()}.{nameof(RunState)} : {state}\n{body}" );
#endif
							body._ranState = SMTaskRunState.SelfInitializing;
							try {
								await body._selfInitializeEvent.Run( body._asyncCancelerOnDisable );
							} catch {
								body._ranState = SMTaskRunState.Create;
								throw;
							}
							body._ranState = SMTaskRunState.SelfInitialized;
							return;
					}
					return;

				case SMTaskRunState.Initializing:
					if ( body._owner._type == SMTaskType.DontWork )	{ return; }
					switch ( body._ranState ) {
						case SMTaskRunState.SelfInitialized:
#if TestSMTaskModifyler
							Log.Debug( $"{body._owner.GetAboutName()}.{nameof(RunState)} : {state}\n{body}" );
#endif
							body._ranState = SMTaskRunState.Initializing;
							try {
								await body._initializeEvent.Run( body._asyncCancelerOnDisable );
							} catch {
								body._ranState = SMTaskRunState.SelfInitialized;
								throw;
							}
							body._ranState = SMTaskRunState.Initialized;
							return;
					}
					return;

				case SMTaskRunState.FixedUpdate:
					if ( body._owner._type == SMTaskType.DontWork )			{ return; }
					if ( body._activeState == SMTaskActiveState.Disable )	{ return; }
					switch ( body._ranState ) {
						case SMTaskRunState.Initialized:
							body._ranState = SMTaskRunState.FixedUpdate;
							break;
					}
					switch ( body._ranState ) {
						case SMTaskRunState.FixedUpdate:
						case SMTaskRunState.Update:
						case SMTaskRunState.LateUpdate:
#if TestSMTaskModifyler
							Log.Debug( $"{body._owner.GetAboutName()}.{nameof(RunState)} : {state}\n{body}" );
#endif
							body._fixedUpdateEvent.Run();
							return;
					}
					return;

				case SMTaskRunState.Update:
					if ( body._owner._type == SMTaskType.DontWork )			{ return; }
					if ( body._activeState == SMTaskActiveState.Disable )	{ return; }
					switch ( body._ranState ) {
						case SMTaskRunState.FixedUpdate:
							body._ranState = SMTaskRunState.Update;
							break;
					}
					switch ( body._ranState ) {
						case SMTaskRunState.Update:
						case SMTaskRunState.LateUpdate:
#if TestSMTaskModifyler
							Log.Debug( $"{body._owner.GetAboutName()}.{nameof(RunState)} : {state}\n{body}" );
#endif
							body._updateEvent.Run();
							return;
					}
					return;

				case SMTaskRunState.LateUpdate:
					if ( body._owner._type == SMTaskType.DontWork )			{ return; }
					if ( body._activeState == SMTaskActiveState.Disable )	{ return; }
					switch ( body._ranState ) {
						case SMTaskRunState.Update:
							body._ranState = SMTaskRunState.LateUpdate;
							break;
					}
					switch ( body._ranState ) {
						case SMTaskRunState.LateUpdate:
#if TestSMTaskModifyler
							Log.Debug( $"{body._owner.GetAboutName()}.{nameof(RunState)} : {state}\n{body}" );
#endif
							body._lateUpdateEvent.Run();
							return;
					}
					return;

				case SMTaskRunState.Finalizing:
					if ( body._owner._type == SMTaskType.DontWork )		{ return; }
					if ( body._ranState == SMTaskRunState.Finalized )	{ return; }
					if ( body._activeState == SMTaskActiveState.Enable ) {
#if TestSMTaskModifyler
						Log.Debug( string.Join( "\n",
							$"{body._owner.GetAboutName()}.{nameof(RunState)} : {state} : Register Disable",
							$"{body}"
						) );
#endif
						body._modifyler.Register(
							new ChangeActiveSMBehaviour( body, SMTaskActiveState.Disable, ModifyType.Interrupter )
						);
						body._modifyler.Register( new RunStateSMBehaviour( body, SMTaskRunState.Finalizing ) );
						return;
					}

					body.StopAsyncOnDisable();	// 念の為
#if TestSMTaskModifyler
					Log.Debug( $"{body._owner.GetAboutName()}.{nameof(RunState)} : {state}\n{body}" );
#endif
					switch ( body._ranState ) {
						case SMTaskRunState.SelfInitialized:
						case SMTaskRunState.Initialized:
						case SMTaskRunState.FixedUpdate:
						case SMTaskRunState.Update:
						case SMTaskRunState.LateUpdate:
							var lastRanState = body._ranState;
							body._ranState = SMTaskRunState.Finalizing;
							try {
								await body._finalizeEvent.Run( body._asyncCancelerOnDispose );
							} catch {
								body._ranState = lastRanState;
								throw;
							}
							break;
					}
					body._ranState = SMTaskRunState.Finalized;
					body._modifyler.UnregisterAll();
					return;

				case SMTaskRunState.None:
				case SMTaskRunState.SelfInitialized:
				case SMTaskRunState.Initialized:
				case SMTaskRunState.Finalized:
					throw new ArgumentOutOfRangeException(
						$"{state}", $"実行状態に、実行後の型を指定した為、実行不可" );
			}
		}


		public static async UniTask RegisterAndRun( ISMBehaviour behaviour, SMTaskRunState state ) {
			switch ( state ) {
				case SMTaskRunState.FixedUpdate:
				case SMTaskRunState.Update:
				case SMTaskRunState.LateUpdate:
					// 駄目で元々、即時実行する
					RunState( behaviour._body, state ).Forget();
					return;

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