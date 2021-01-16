//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.State.Modifyler {
	using Cysharp.Threading.Tasks;



	// TODO : コメント追加、整頓



	public class EnterSMState : SMStateModifyData {
		public override SMFSMModifyType _type => SMFSMModifyType.Runner;


		public override async UniTask Run() {
			if ( !_owner._isOperable )	{ return; }
			if ( !_owner._isActive )	{ return; }
			if ( _owner._ranState != SMStateRunState.Exit )	{ return; }

			await _owner._enterEvent.Run( _owner._asyncCancelerOnChangeOrDisable );
			_owner._ranState = SMStateRunState.Enter;
		}
	}
}