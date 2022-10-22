//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using UnityEngine.SceneManagement;



	public class ForeverSMScene : SMScene {
		public ForeverSMScene() {
			_exitEvent.AddFirst( _registerEventName, async canceler => {
				_owner._foreverRawScene = SceneManager.CreateScene( "UntilQuit" );
				await UTask.DontWait();
			} );
		}

		public void Setup( Scene rawScene ) {
			_rawScene = rawScene;
		}



		protected override void ReloadRawScene() {
		}
	}
}