//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using FSM.Modifyler.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public class FinalExitSMFSM : SMFSMModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.FirstRunner;


		public override async UniTask Run() {
			_target.StopAsyncOnDisableAndExit();

			if ( _target._stateBody != null ) {
				await _target._stateBody.Exit();
				_target._stateBody = null;
			}

			_modifyler.Dispose();
		}
	}
}