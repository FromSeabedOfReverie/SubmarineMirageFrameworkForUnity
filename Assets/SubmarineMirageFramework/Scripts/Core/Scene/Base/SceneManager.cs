//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine.SceneManagement;
	using KoganeUnityLib;
	using SMTask;
	using SMTask.Modifyler;
	using FSM;
	using Singleton;
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
			SMObjectModifyData.SetAllObjectData( s_instance._object._top );
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


		public TBehaviour GetBehaviour<TBehaviour, TScene>( SMTaskType? bodyType = null )
			where TBehaviour : class, ISMBehaviour
			where TScene : BaseScene
			=> _fsm.GetAllScene()
				.FirstOrDefault( s => s is TScene )
				?._objects.GetBehaviour<TBehaviour>( bodyType );

		public T GetBehaviour<T>( SMTaskType? bodyType = null ) where T : ISMBehaviour
			=> _fsm.GetAllScene()
				.Select( s => s._objects.GetBehaviour<T>( bodyType ) )
				.FirstOrDefault( b => b != null );

		public ISMBehaviour GetBehaviour( Type type, Type sceneType = null, SMTaskType? bodyType = null ) {
			if ( sceneType != null ) {
				return _fsm.GetAllScene()
					.FirstOrDefault( s => s.GetType() == sceneType )
					?._objects.GetBehaviour( type, bodyType );
			} else {
				return _fsm.GetAllScene()
					.Select( s => s._objects.GetBehaviour( type, bodyType ) )
					.FirstOrDefault( b => b != null );
			}
		}

		public IEnumerable<TBehaviour> GetBehaviours<TBehaviour, TScene>( SMTaskType? bodyType = null )
			where TBehaviour : class, ISMBehaviour
			where TScene : BaseScene
			=> _fsm.GetAllScene()
				.FirstOrDefault( s => s is TScene )
				?._objects.GetBehaviours<TBehaviour>( bodyType );

		public IEnumerable<T> GetBehaviours<T>( SMTaskType? bodyType = null ) where T : ISMBehaviour
			=> _fsm.GetAllScene()
				.SelectMany( s => s._objects.GetBehaviours<T>( bodyType ) );

		public IEnumerable<ISMBehaviour> GetBehaviours( Type type, Type sceneType = null,
														SMTaskType? bodyType = null
		) {
			if ( sceneType != null ) {
				return _fsm.GetAllScene()
					.FirstOrDefault( s => s.GetType() == sceneType )
					?._objects.GetBehaviours( type, bodyType );
			} else {
				return _fsm.GetAllScene()
					.SelectMany( s => s._objects.GetBehaviours( type, bodyType ) );
			}
		}
	}
}