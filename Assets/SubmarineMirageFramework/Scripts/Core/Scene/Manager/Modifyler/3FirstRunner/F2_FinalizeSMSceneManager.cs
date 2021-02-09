//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene.Modifyler {
	using Cysharp.Threading.Tasks;
	using Task;
	using Task.Modifyler.Base;
	using Scene.Modifyler.Base;
	using Debug;


	// TODO : コメント追加、整頓


	public class FinalizeSMSceneManager : SMSceneManagerModifyData {
		[SMShowLine] public override SMTaskModifyType _type => SMTaskModifyType.FirstRunner;


		public override async UniTask Run() {
			if ( _owner._ranState != SMTaskRunState.FinalDisable )	{ return; }


			await _owner._finalizeEvent.Run( _owner._asyncCancelerOnDispose );

			_owner._ranState = SMTaskRunState.Finalize;
			_owner._sceneManager.Dispose();
		}
	}
}