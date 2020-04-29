//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using System;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using UniRx.Async;
	using KoganeUnityLib;
	using Process.New;
	using FSM.New;
	using Utility;
	using Debug;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
	using LifeSpan = Process.New.ProcessBody.LifeSpan;


	// TODO : コメント追加、整頓


	public class SceneStateMachine : FiniteStateMachine<SceneStateMachine, SceneManager, BaseScene> {
		public BaseScene _scene => _state;
		public string _currentSceneName => _scene._name;
		public Scene _currentScene => _scene._scene;
		public ForeverScene _foreverScene	{ get; private set; }
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

			_startState = _states
				.Select( pair => pair.Value )
				.Where( s => !( s is UnknownScene ) )
				.FirstOrDefault( s => s._name == UnitySceneManager.GetActiveScene().name )
				?.GetType();
			if ( _startState == null )	{ _startState = typeof( UnknownScene ); }
		}


		public BaseScene Get( Scene scene ) {
			var result = _states
				.Select( pair => pair.Value )
				.FirstOrDefault( s => s._scene == scene );
			if ( result == null )	{ result = _states[typeof( UnknownScene )]; }
			return result;
		}


// TODO : MultiFSM実装後、複数シーン読込に対応する
		public async UniTask ChangeScene<T>() where T : BaseScene
			=> await ChangeState<T>();

		public async UniTask ChangeScene( Type stateType )
			=> await ChangeState( stateType );
	}
}