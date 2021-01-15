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
		public static async UniTask SelfInitialize( BaseSMState state, SMTaskCanceler asyncCanceler ) {
			if ( state == null )	{ return; }
			await state._selfInitializeEvent.Run( asyncCanceler );
		}

		public static async UniTask Initialize( BaseSMState state, SMTaskCanceler asyncCanceler ) {
			if ( state == null )	{ return; }
			await state._initializeEvent.Run( asyncCanceler );
		}

		public static async UniTask Finalize( BaseSMState state, SMTaskCanceler asyncCanceler ) {
			if ( state == null )	{ return; }
			await state._finalizeEvent.Run( asyncCanceler );
		}


		public static void Enable( BaseSMState state ) {
			if ( state == null )	{ return; }
			state._enableEvent.Run();
		}

		public static void Disable( BaseSMState state ) {
			if ( state == null )	{ return; }
			state.StopActiveAsync();
			state._disableEvent.Run();
		}


		public static void FixedUpdate( BaseSMState state ) {
			if ( state == null )	{ return; }
			if ( state._runState != SMFSMRunState.Update )	{ return; }
			state._fixedUpdateEvent.Run();
		}

		public static void Update( BaseSMState state ) {
			if ( state == null )	{ return; }
			if ( state._runState != SMFSMRunState.Update )	{ return; }
			state._modifyler.Register( new UpdateSMState() );
			state._updateEvent.Run();
		}

		public static void LateUpdate( BaseSMState state ) {
			if ( state == null )	{ return; }
			if ( state._runState != SMFSMRunState.Update )	{ return; }
			state._lateUpdateEvent.Run();
		}
	}
}