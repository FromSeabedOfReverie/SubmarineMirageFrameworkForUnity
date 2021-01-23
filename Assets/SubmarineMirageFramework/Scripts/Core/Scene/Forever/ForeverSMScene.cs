//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using UnityEngine.SceneManagement;


	// TODO : コメント追加、整頓


	public class ForeverSMScene : SMScene {
		public ForeverSMScene() {
		}


		protected override void ReloadRawScene()
			=> _rawScene = SceneManager.CreateScene( _name );
	}
}