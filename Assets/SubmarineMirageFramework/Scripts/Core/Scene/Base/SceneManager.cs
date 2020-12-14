//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestSMTaskModifyler
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
// TODO : ここで、SetAllDataする意味が分からない・・・
			s_instance._object._group.SetAllData();
		}

#if TestSMTaskModifyler
		public SceneManager() => Log.Debug( $"{nameof( SceneManager )}() : {this}" );
#endif

		public override void Create() {
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( Create )} : {this}" );
#endif
		}


		public bool IsInBuildScene() {
#if UNITY_EDITOR
			var path = _currentScene.path;
			return UnityEditor.EditorBuildSettings.scenes
				.Any( scene => scene.path == path );
#else
			return true;
#endif
		}


		public T GetBehaviour<T>( SMTaskType? taskType = null )
			where T : ISMBehaviour
			=> GetBehaviours<T>( taskType )
				.FirstOrDefault();

		public TBehaviour GetBehaviour<TBehaviour, TScene>( SMTaskType? taskType = null )
			where TBehaviour : ISMBehaviour
			where TScene : BaseScene
			=> GetBehaviours<TBehaviour, TScene>( taskType )
				.FirstOrDefault();

		public ISMBehaviour GetBehaviour( Type type, Type sceneType = null, SMTaskType? taskType = null )
			=> GetBehaviours( type, sceneType, taskType )
				.FirstOrDefault();

		public IEnumerable<T> GetBehaviours<T>( SMTaskType? taskType = null )
			where T : ISMBehaviour
			=> GetBehaviours( typeof( T ), null, taskType )
				.Select( b => (T)b );

		public IEnumerable<TBehaviour> GetBehaviours<TBehaviour, TScene>( SMTaskType? taskType = null )
			where TBehaviour : ISMBehaviour
			where TScene : BaseScene
			=> GetBehaviours( typeof( TBehaviour ), typeof( TScene ), taskType )
				.Select( b => (TBehaviour)b );

		public IEnumerable<ISMBehaviour> GetBehaviours( Type type, Type sceneType = null,
														SMTaskType? taskType = null
		) {
			var scenes = Enumerable.Empty<BaseScene>();
			if ( sceneType != null ) {
				var scene = _fsm.GetAllScene().FirstOrDefault( s => s.GetType() == sceneType );
				if ( scene != null )	{ scenes = new [] { scene }; }
			} else {
				scenes = _fsm.GetAllScene();
			}
			var currents = new Queue<SMObject>( scenes.SelectMany( s => {
				return s._groups.GetAllGroups( taskType );
			} ) );
			while ( !currents.IsEmpty() ) {
				var o = currents.Dequeue();
				foreach ( var b in o.GetBehaviours( type ) ) {
					yield return b;
				}
				o.GetChildren().ForEach( c => currents.Enqueue( c ) );
			}
		}
	}
}