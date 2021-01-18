//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.State.Modifyler {
	using Cysharp.Threading.Tasks;
	using Task;



	// TODO : コメント追加、整頓



	public static class SMStateApplyer {
		public static void StopAsyncOnDisableAndExit( BaseSMState state ) {
			state._asyncCancelerOnDisableAndExit.Cancel();
		}



		public static async UniTask SelfInitialize( BaseSMState state, SMTaskCanceler asyncCanceler ) {
			if ( state == null )		{ return; }
			if ( state._isDispose )		{ return; }
			if ( state._isFinalizing )	{ return; }

			await state._selfInitializeEvent.Run( asyncCanceler );
		}

		public static async UniTask Initialize( BaseSMState state, SMTaskCanceler asyncCanceler ) {
			if ( state == null )	{ return; }
			if ( state._isDispose )	{ return; }
			if ( state._isFinalizing )	{ return; }

			await state._initializeEvent.Run( asyncCanceler );
			state._isInitialized = true;
		}

		public static async UniTask Finalize( BaseSMState state, SMTaskCanceler asyncCanceler ) {
			if ( state == null )	{ return; }
			if ( state._isDispose )	{ return; }

			state._isFinalizing = true;
			await state._finalizeEvent.Run( asyncCanceler );
		}


		public static void Enable( BaseSMState state ) {
			if ( state == null )		{ return; }
			if ( state._isDispose )		{ return; }
			if ( !state._isOperable )	{ return; }
			if ( state._isActive )		{ return; }

			state._enableEvent.Run();
			state._isActive = true;
		}

		public static void Disable( BaseSMState state ) {
			if ( state == null )		{ return; }
			if ( state._isDispose )		{ return; }
			if ( !state._isOperable )	{ return; }
			if ( !state._isActive )		{ return; }

			state._isActive = false;
			state._modifyler.UnregisterAll();
			StopAsyncOnDisableAndExit( state );
			state._disableEvent.Run();
		}


		public static void FixedUpdate( BaseSMState state ) {
			if ( state == null )		{ return; }
			if ( state._isDispose )		{ return; }
			if ( !state._isOperable )	{ return; }
			if ( !state._isActive )		{ return; }
			if ( state._ranState != SMStateRunState.Update )	{ return; }

			state._fixedUpdateEvent.Run();
		}

		public static void Update( BaseSMState state ) {
			if ( state == null )		{ return; }
			if ( state._isDispose )		{ return; }
			if ( !state._isOperable )	{ return; }
			if ( !state._isActive )		{ return; }
			if ( state._ranState != SMStateRunState.Update )	{ return; }

			state._updateEvent.Run();
		}

		public static void LateUpdate( BaseSMState state ) {
			if ( state == null )		{ return; }
			if ( state._isDispose )		{ return; }
			if ( !state._isOperable )	{ return; }
			if ( !state._isActive )		{ return; }
			if ( state._ranState != SMStateRunState.Update )	{ return; }

			state._lateUpdateEvent.Run();
		}
	}
}