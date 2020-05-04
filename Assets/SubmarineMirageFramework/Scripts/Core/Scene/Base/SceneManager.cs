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
	using Process.New;
	using FSM.New;
	using Singleton.New;
	using Type = Process.New.ProcessBody.Type;
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


		public TProcess GetProcess<TProcess, TScene>( Type? bodyType = null )
			where TProcess : class, IProcess
			where TScene : BaseScene
		{
			var a = _fsm.GetAllScene()
//.Where( s => { Log.Debug(s); return true; } )
				.FirstOrDefault( s => s is TScene );
//Log.Debug( a );
			return a
				?._hierarchies.GetProcess<TProcess>( bodyType );
		}
		public T GetProcess<T>( Type? bodyType = null ) where T : IProcess {
			return _fsm.GetAllScene()
				.Select( s => s._hierarchies.GetProcess<T>( bodyType ) )
				.FirstOrDefault( p => p != null );
		}
		public IProcess GetProcess( System.Type type, System.Type sceneType = null, Type? bodyType = null ) {
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

		public List<TProcess> GetProcesses<TProcess, TScene>( Type? bodyType = null )
			where TProcess : class, IProcess
			where TScene : BaseScene
		{
			return _fsm.GetAllScene()
				.FirstOrDefault( s => s is TScene )
				?._hierarchies.GetProcesses<TProcess>( bodyType );
		}
		public List<T> GetProcesses<T>( Type? bodyType = null ) where T : IProcess {
			return _fsm.GetAllScene()
				.SelectMany( s => s._hierarchies.GetProcesses<T>( bodyType ) )
				.ToList();
		}
		public List<IProcess> GetProcesses( System.Type type, System.Type sceneType = null, Type? bodyType = null )
		{
			if ( sceneType != null ) {
				return _fsm.GetAllScene()
					.FirstOrDefault( s => s.GetType() == sceneType )
					?._hierarchies.GetProcesses( type, bodyType );
			} else {
				return _fsm.GetAllScene()
					.SelectMany( s => s._hierarchies.GetProcesses( type, bodyType ) )
					.ToList();
			}
		}
	}
}