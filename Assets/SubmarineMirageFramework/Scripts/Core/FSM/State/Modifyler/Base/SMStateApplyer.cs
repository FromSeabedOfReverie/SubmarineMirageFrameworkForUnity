//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.State.Modifyler.Base {
	using Cysharp.Threading.Tasks;
	using Task;
	using FSM.State.Base;



	// TODO : コメント追加、整頓



	public static class SMStateApplyer {
		public static void StopAsyncOnDisableAndExit( BaseSMState state ) {
			state._asyncCancelerOnDisableAndExit.Cancel();
		}



		public static async UniTask SelfInitialize( BaseSMState state, SMTaskCanceler asyncCanceler ) {
			await state._selfInitializeEvent.Run( asyncCanceler );
		}

		public static async UniTask Initialize( BaseSMState state, SMTaskCanceler asyncCanceler ) {
			await state._initializeEvent.Run( asyncCanceler );
		}

		public static async UniTask Finalize( BaseSMState state, SMTaskCanceler asyncCanceler ) {
			await state._finalizeEvent.Run( asyncCanceler );
		}


		public static void Enable( BaseSMState state ) {
			if ( state == null )		{ return; }

			state._enableEvent.Run();
		}

		public static void Disable( BaseSMState state ) {
			if ( state == null )		{ return; }

			state._modifyler.Reset();
			StopAsyncOnDisableAndExit( state );
			state._disableEvent.Run();
		}


		public static void FixedUpdate( BaseSMState state ) {
			if ( state == null )	{ return; }
			if ( state._ranState != SMStateRunState.Update )	{ return; }

			state._fixedUpdateEvent.Run();
		}

		public static void Update( BaseSMState state ) {
			if ( state == null )	{ return; }
			if ( state._ranState != SMStateRunState.Update )	{ return; }

			state._modifyler.Run().Forget();
			state._updateEvent.Run();
		}

		public static void LateUpdate( BaseSMState state ) {
			if ( state == null )	{ return; }
			if ( state._ranState != SMStateRunState.Update )	{ return; }

			state._lateUpdateEvent.Run();
		}
	}
}