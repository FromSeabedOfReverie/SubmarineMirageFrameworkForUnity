//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.State.Modifyler {
	using Cysharp.Threading.Tasks;



	// TODO : コメント追加、整頓



	public class ExitSMState : SMStateModifyData {
		public override SMFSMModifyType _type => SMFSMModifyType.FirstRunner;


		public override async UniTask Run() {
			if ( !_owner._isOperable )	{ return; }
			if ( !_owner._isActive )	{ return; }
			if ( _owner._ranState == SMStateRunState.Exit )	{ return; }


			SMStateApplyer.StopAsyncOnDisableAndExit( _owner );
			await _owner._exitEvent.Run( _owner._asyncCancelerOnDisableAndExit );
			_owner._ranState = SMStateRunState.Exit;
			_modifyler.UnregisterAll();
		}
	}
}