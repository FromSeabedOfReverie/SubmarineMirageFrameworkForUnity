//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.State.Modifyler {
	using Cysharp.Threading.Tasks;
	using FSM.State.Base;
	using Utility;



	// TODO : コメント追加、整頓



	public static class SMStateApplyer {
		public static async UniTask SelfInitialize( BaseSMState state ) {
			await state._selfInitializeEvent.Run( state._asyncCancelerOnDispose );
		}

		public static async UniTask Initialize( BaseSMState state ) {
			await state._initializeEvent.Run( state._asyncCancelerOnDispose );
		}

		public static async UniTask Finalize( BaseSMState state ) {
			await state._finalizeEvent.Run( state._asyncCancelerOnDispose );
		}



		public static void Enable( BaseSMState state ) {
			if ( state == null )		{ return; }

			state._enableEvent.Run();
		}

		public static void Disable( BaseSMState state ) {
			if ( state == null )		{ return; }

			state._updatedState = SMStateUpdateState.Disable;
			state._disableEvent.Run();
		}



		public static void FixedUpdate( BaseSMState state ) {
			if ( state == null )	{ return; }
			if ( state._ranState != SMStateRunState.Update )	{ return; }
			if ( state._updatedState < SMStateUpdateState.Disable )	{ return; }

			state._fixedUpdateEvent.Run();

			if ( state._updatedState == SMStateUpdateState.Disable ) {
				state._updatedState = SMStateUpdateState.FixedUpdate;
			}
		}

		public static void Update( BaseSMState state ) {
			if ( state == null )	{ return; }
			if ( state._ranState != SMStateRunState.Update )	{ return; }
			if ( state._updatedState < SMStateUpdateState.FixedUpdate )	{ return; }

			state._updateEvent.Run();

			if ( state._updatedState == SMStateUpdateState.FixedUpdate ) {
				state._updatedState = SMStateUpdateState.Update;
			}
		}

		public static void LateUpdate( BaseSMState state ) {
			if ( state == null )	{ return; }
			if ( state._ranState != SMStateRunState.Update )	{ return; }
			if ( state._updatedState < SMStateUpdateState.Update )	{ return; }

			state._lateUpdateEvent.Run();

			if ( state._updatedState == SMStateUpdateState.Update ) {
				state._updatedState = SMStateUpdateState.LateUpdate;
			}
		}



		public static async UniTask Enter( BaseSMState state ) {
			if ( state._ranState != SMStateRunState.Exit )	{ return; }

			await state._enterEvent.Run( state._asyncCancelerOnDisableAndExit );
			state._ranState = SMStateRunState.Enter;
		}

		public static async UniTask Exit( BaseSMState state ) {
			if ( state._ranState == SMStateRunState.Exit )	{ return; }

			state._updatedState = SMStateUpdateState.Disable;
			state._ranState = SMStateRunState.Enter;

			await state._exitEvent.Run( state._asyncCancelerOnDisableAndExit );
			state._ranState = SMStateRunState.Exit;
		}

		public static void UpdateAsync( BaseSMState state ) {
			if ( state._ranState == SMStateRunState.Exit )	{ return; }
			if ( state._isUpdating )	{ return; }

			UTask.Void( async () => {
				try {
					state._ranState = SMStateRunState.Update;
					state._isUpdating = true;
					await state._updateAsyncEvent.Run( state._asyncCancelerOnDisableAndExit );
				} finally {
					state._isUpdating = false;
				}
			} );
		}
	}
}