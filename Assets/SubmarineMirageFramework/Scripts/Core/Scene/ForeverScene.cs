//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


	// TODO : コメント追加、整頓


	public class ForeverScene : BaseScene {
		public ForeverScene() {
			_scene = UnitySceneManager.CreateScene( _name );

			_enterEvent.Remove( registerKey );
			_enterEvent.AddFirst( registerKey, async cancel => await _hierarchies.Enter() );
			_exitEvent.Remove( registerKey );
			_exitEvent.AddFirst( registerKey, async cancel => await _hierarchies.Exit() );
		}
	}
}