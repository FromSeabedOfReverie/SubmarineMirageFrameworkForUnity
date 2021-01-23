//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using Cysharp.Threading.Tasks;
	using Utility;



	// TODO : コメント追加、整頓



	public abstract class MainSMScene : SMScene {
		public MainSMScene() {
			_enterEvent.AddFirst( _registerEventName, async canceler => {
				
			} );


			_exitEvent.AddLast( _registerEventName, async canceler => {
				await SceneManager.UnloadSceneAsync( _name ).ToUniTask( canceler );
				ReloadRawScene();
				await Resources.UnloadUnusedAssets().ToUniTask( canceler );
			} );
		}
	}
}