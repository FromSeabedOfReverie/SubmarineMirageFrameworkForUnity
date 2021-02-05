//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler {
	using Cysharp.Threading.Tasks;
	using FSM.Modifyler.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public class FinalExitSMFSM : SMFSMModifyData {
		[SMShowLine] public override SMFSMModifyType _type => SMFSMModifyType.FirstRunner;


		public override async UniTask Run() {
			_owner.StopAsyncOnDisableAndExit();

			if ( _owner._stateBody != null ) {
				await _owner._stateBody.Exit();
				_owner._stateBody = null;
			}

			_modifyler.Dispose();
		}
	}
}