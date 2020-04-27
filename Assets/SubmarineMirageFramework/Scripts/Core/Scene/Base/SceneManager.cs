//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using System.Linq;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using KoganeUnityLib;
	using Process.New;
	using FSM.New;
	using Singleton.New;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


	// TODO : コメント追加、整頓


	public class SceneManager : Singleton<SceneManager>, IFiniteStateMachineOwner<SceneStateMachine> {
		public SceneStateMachine _fsm	{ get; private set; }
		public string _currentSceneName => _fsm._currentSceneName;
		public Scene _currentScene => _fsm._currentScene;


		public void SetupInstance() {
			_fsm = new SceneStateMachine( this );
			_disposables.AddFirst( _fsm );
			_hierarchy.ResetTop();
		}

		public override void Create() {}



		public bool IsInBuildScene() {
#if UNITY_EDITOR
			var path = _currentScene.path;
			return UnityEditor.EditorBuildSettings.scenes
				.Any( scene => scene.path == path );
#else
			return true;
#endif
		}
	}
}