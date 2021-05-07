//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Task;
	using Debug;


	public class FinalizeSMSceneManager : SMSceneManagerModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.FirstRunner;


		public override async UniTask Run() {
			if ( _target._ranState != SMTaskRunState.FinalDisable )	{ return; }


			await _target._finalizeEvent.Run( _target._asyncCancelerOnDispose );

			_target._ranState = SMTaskRunState.Finalize;
			_target._sceneManager.Dispose();
		}
	}
}