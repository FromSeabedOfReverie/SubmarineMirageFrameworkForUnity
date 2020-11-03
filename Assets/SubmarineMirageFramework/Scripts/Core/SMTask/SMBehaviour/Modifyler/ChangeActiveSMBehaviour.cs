//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
	using Cysharp.Threading.Tasks;
	using UTask;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class ChangeActiveSMBehaviour : SMBehaviourModifyData {
		SMTaskActiveState _state;


		public ChangeActiveSMBehaviour( SMBehaviourBody body, SMTaskActiveState state,
										ModifyType type = ModifyType.Runner
		) : base( body ) {
			_state = state;
			_type = type;
		}

		public override void Cancel() {}


		public override UniTask Run() => ChangeActive();


		async UniTask ChangeActive() {
			if ( _body._owner._type == SMTaskType.DontWork )	{ return; }
			if ( _body._activeState == _state )					{ return; }
			if ( !_body._isInitialized )						{ return; }
			if ( _body._ranState == SMTaskRunState.Finalized )	{ return; }

#if TestSMTaskModifyler
			Log.Debug( $"{_body._owner.GetAboutName()}.{nameof( ChangeActive )} : {_state}\n{_body}" );
#endif

			switch ( _state ) {
				case SMTaskActiveState.Enable:
					_body._enableEvent.Run();
					_body._activeState = SMTaskActiveState.Enable;
					return;

				case SMTaskActiveState.Disable:
					_body.StopAsyncOnDisable();
					_body._disableEvent.Run();
					_body._activeState = SMTaskActiveState.Disable;
					return;
			}

			await UTask.DontWait();
		}


		public static async UniTask RegisterAndRun( ISMBehaviour behaviour, SMTaskActiveState state ) {
			behaviour._body._modifyler.Register( new ChangeActiveSMBehaviour( behaviour._body, state ) );
			await behaviour._body._modifyler.WaitRunning();
		}

		public static UniTask RegisterAndRun( ISMBehaviour behaviour, bool isActive )
			=> RegisterAndRun( behaviour, isActive ? SMTaskActiveState.Enable : SMTaskActiveState.Disable );

		public static UniTask RegisterAndRunInitial( ISMBehaviour behaviour )
			=> RegisterAndRun( behaviour, behaviour._body._initialActiveState );


		public override string ToString() => base.ToString().InsertLast( ", ",
			string.Join( ", ",
				_state
			)
			+ ", "
		);
	}
}