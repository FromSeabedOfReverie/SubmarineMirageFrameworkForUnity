//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using KoganeUnityLib;
	using SMTask;
	using FSM.New;
	using Singleton.New;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
	using Debug;


	// TODO : コメント追加、整頓


	public class SceneManager : Singleton<SceneManager>, IFiniteStateMachineOwner<SceneStateMachine> {
		public SceneStateMachine _fsm	{ get; private set; }
		public string _currentSceneName => _fsm._currentSceneName;
		public Scene _currentScene => _fsm._currentScene;


		new public static void CreateInstance() {
			if ( s_isCreated )	{ return; }

			s_instance._fsm = new SceneStateMachine( s_instance );
			s_instance._fsm._foreverScene.Set( s_instance );
			s_instance._disposables.AddFirst( s_instance._fsm );
			s_instance._hierarchy.SetAllData();
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


		public TProcess GetProcess<TProcess, TScene>( SMTaskType? bodyType = null )
			where TProcess : class, ISMBehavior
			where TScene : BaseScene
		{
			return _fsm.GetAllScene()
				.FirstOrDefault( s => s is TScene )
				?._hierarchies.GetProcess<TProcess>( bodyType );
		}
		public T GetProcess<T>( SMTaskType? bodyType = null ) where T : ISMBehavior {
			return _fsm.GetAllScene()
				.Select( s => s._hierarchies.GetProcess<T>( bodyType ) )
				.FirstOrDefault( p => p != null );
		}
		public ISMBehavior GetProcess( System.Type type, System.Type sceneType = null, SMTaskType? bodyType = null ) {
			if ( sceneType != null ) {
				return _fsm.GetAllScene()
					.FirstOrDefault( s => s.GetType() == sceneType )
					?._hierarchies.GetProcess( type, bodyType );
			} else {
				return _fsm.GetAllScene()
					.Select( s => s._hierarchies.GetProcess( type, bodyType ) )
					.FirstOrDefault( p => p != null );
			}
		}

		public IEnumerable<TProcess> GetProcesses<TProcess, TScene>( SMTaskType? bodyType = null )
			where TProcess : class, ISMBehavior
			where TScene : BaseScene
		{
			return _fsm.GetAllScene()
				.FirstOrDefault( s => s is TScene )
				?._hierarchies.GetProcesses<TProcess>( bodyType );
		}
		public IEnumerable<T> GetProcesses<T>( SMTaskType? bodyType = null ) where T : ISMBehavior {
			return _fsm.GetAllScene()
				.SelectMany( s => s._hierarchies.GetProcesses<T>( bodyType ) );
		}
		public IEnumerable<ISMBehavior> GetProcesses( System.Type type, System.Type sceneType = null,
													SMTaskType? bodyType = null
		) {
			if ( sceneType != null ) {
				return _fsm.GetAllScene()
					.FirstOrDefault( s => s.GetType() == sceneType )
					?._hierarchies.GetProcesses( type, bodyType );
			} else {
				return _fsm.GetAllScene()
					.SelectMany( s => s._hierarchies.GetProcesses( type, bodyType ) );
			}
		}
	}
}