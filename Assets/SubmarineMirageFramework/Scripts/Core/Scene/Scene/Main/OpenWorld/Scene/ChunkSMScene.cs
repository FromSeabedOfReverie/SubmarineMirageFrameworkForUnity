//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using UnityEngine.SceneManagement;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Extension;
	using Utility;



	// TODO : コメント追加、整頓



	public class ChunkSMScene : SMScene {
		protected override bool _isUseUnloadUnusedAssets => false;



		public ChunkSMScene( string name ) {
			_name = name;
			ReloadRawScene();

/*
			_enterEvent.Remove( _registerEventName );
			_enterEvent.AddLast( _registerEventName, async canceler => {
				await SceneManager.LoadSceneAsync( _name, LoadSceneMode.Additive ).ToUniTask( canceler );
				ReloadRawScene();
			} );

			_exitEvent.Remove( _registerEventName );
			_exitEvent.AddLast( _registerEventName, async canceler => {
				await SceneManager.UnloadSceneAsync( _name ).ToUniTask( canceler );
				ReloadRawScene();
			} );
*/
		}


		protected override void SetSceneName() {}

		protected override void ReloadRawScene() {
			if ( _name.IsNullOrEmpty() )	{ return; }
			base.ReloadRawScene();
		}
	}
}