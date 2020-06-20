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
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using UniRx.Async;
	using KoganeUnityLib;
	using FSM.New;
	using Utility;
	using Debug;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


	// TODO : コメント追加、整頓


	public class SceneStateMachine : FiniteStateMachine<SceneStateMachine, SceneManager, BaseScene> {
		public ForeverScene _foreverScene	{ get; private set; }
		public BaseScene _startScene	{ get; private set; }
		public BaseScene _scene => _state;
		public Scene _currentScene => _scene._scene;
		public string _currentSceneName => _scene._name;
		public bool _isSkipLoadForFirstScene = true;


		public SceneStateMachine( SceneManager owner ) : base(
			owner,
			new BaseScene[] {
				new UnknownScene(),
				new TestChange1Scene(),
				new TestChange2Scene(),
			}
		) {
			_foreverScene = new ForeverScene();
			_disposables.AddLast( _foreverScene );

			_startScene = _states
				.Select( pair => pair.Value )
				.Where( s => !( s is UnknownScene ) )
				.FirstOrDefault( s => s._name == UnitySceneManager.GetActiveScene().name );
			if ( _startScene == null )	{ _startScene = _states[typeof( UnknownScene )]; }
			_startState = _startScene.GetType();
		}


		public BaseScene Get( Scene scene ) {
			var result = _states
				.Select( pair => pair.Value )
				.FirstOrDefault( s => s._scene == scene );
			if ( result == null )	{ result = _states[typeof( UnknownScene )]; }
			return result;
		}

		public IEnumerable<BaseScene> GetAllScene() {
			yield return _foreverScene;
			foreach ( var pair in _states ) {
				yield return pair.Value;
			}
		}


// TODO : MultiFSM実装後、複数シーン読込に対応する
		public async UniTask ChangeScene<T>() where T : BaseScene
			=> await ChangeState<T>();

		public async UniTask ChangeScene( Type stateType )
			=> await ChangeState( stateType );
	}
}