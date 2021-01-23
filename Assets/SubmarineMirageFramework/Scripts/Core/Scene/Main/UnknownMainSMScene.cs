//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using UnityEngine.SceneManagement;



	// TODO : コメント追加、整頓


/*
TODO : 複数シーン読込後に起動した場合、GetActiveSceneの初期値が不明なので、保留
	public class UnknownMainSMScene : MainSMScene {
		public UnknownMainSMScene() {
		}


		protected override void SetSceneName() {
			ReloadRawScene();
			_name = _rawScene.name;
		}

		protected override void ReloadRawScene()
			=> _rawScene = SceneManager.GetActiveScene();
	}
*/
}