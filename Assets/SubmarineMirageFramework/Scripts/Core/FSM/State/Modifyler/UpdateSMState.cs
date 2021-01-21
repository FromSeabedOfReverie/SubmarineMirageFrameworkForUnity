//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.State.Modifyler {
	using Cysharp.Threading.Tasks;
	using FSM.Modifyler.Base;
	using FSM.State.Modifyler.Base;
	using Utility;



	// TODO : コメント追加、整頓



	public class UpdateSMState : SMStateModifyData {
		public override SMFSMModifyType _type => SMFSMModifyType.Runner;


		public override async UniTask Run() {
			if ( _owner._ranState == SMStateRunState.Exit )	{ return; }
			if ( _owner._isUpdating )	{ return; }


			UTask.Void( async () => {
				try {
					_owner._ranState = SMStateRunState.Update;
					_owner._isUpdating = true;
					await _owner._updateAsyncEvent.Run( _owner._asyncCancelerOnDisableAndExit );
				} finally {
					_owner._isUpdating = false;
				}
			} );
			await UTask.DontWait();
		}
	}
}