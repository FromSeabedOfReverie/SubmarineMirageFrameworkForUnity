//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using System;
	using System.Linq;
	using System.Threading;
	using Utility;
	using UnityEngine.SceneManagement;
	using UniRx.Async;
	using DG.Tweening;
	using KoganeUnityLib;
	using Process.New;
	using FSM.New;
	using Singleton.New;
	using Audio;
	using Extension;
	using Debug;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


	// TODO : コメント追加、整頓


	public class SceneManager : Singleton<SceneManager>, IFiniteStateMachineOwner<SceneStateMachine> {
		public SceneStateMachine _fsm	{ get; private set; }
		public string _currentSceneName => _fsm._currentSceneName;
		public Scene _currentScene => _fsm._currentScene;

		public override void Create() {
			_fsm = new SceneStateMachine( this );
			_disposables.AddFirst( _fsm );
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
	}


	public class SceneStateMachine : FiniteStateMachine<SceneStateMachine, SceneManager, BaseScene> {
		public BaseScene _scene => _state;
		public string _currentSceneName => _scene._name;
		public Scene _currentScene => _scene._scene;
		public bool _isSkipLoadForFirstScene = true;

		public SceneStateMachine( SceneManager owner ) : base(
			owner,
			new BaseScene[] {
				new UnknownScene(),
				new TestChangeScene1Scene(),
				new TestChangeScene2Scene(),
			}
		) {
			_startState = _states
				.Select( pair => pair.Value )
				.Where( s => !( s is UnknownScene ) )
				.FirstOrDefault( s => s._name == UnitySceneManager.GetActiveScene().name )
				?.GetType();
			if ( _startState == null )	{ _startState = typeof( UnknownScene ); }
		}

// TODO : MultiFSM実装後、複数シーン読込に対応する
		public async UniTask ChangeScene<T>() where T : BaseScene
			=> await ChangeState<T>();

		public async UniTask ChangeScene( Type stateType )
			=> await ChangeState( stateType );
	}


	public abstract class BaseScene : State<SceneStateMachine, SceneManager> {
		public string _name	{ get; protected set; }
		public Scene _scene	{ get; protected set; }

		public BaseScene() {
			_name = this.GetAboutName().RemoveAtLast( "Scene" );
			_scene = UnitySceneManager.GetSceneByName( _name );

			_enterEvent.AddFirst( async cancel => {
				if ( _fsm._isSkipLoadForFirstScene ) {
					_fsm._isSkipLoadForFirstScene = false;
					UnitySceneManager.CreateScene( ProcessBody.FOREVER_SCENE_NAME );
//					await UnitySceneManager.LoadSceneAsync( ProcessBody.FOREVER_SCENE_NAME )
//						.ConfigureAwait( cancel );
				} else {
					await UnitySceneManager.LoadSceneAsync( _name, LoadSceneMode.Additive )
						.ConfigureAwait( cancel );
				}
				Hoge();
//				await CoreProcessManager.s_instance.RunSceneProcesses();
			} );

			_exitEvent.AddFirst( async cancel => {
//				await CoreProcessManager.s_instance.DeleteSceneProcesses();
// TODO : DOTween全停止による、音停止を、シーン内の文字列登録文だけ停止させる事で、流し続ける
//				DOTween.KillAll();
//				GameAudioManager.s_instance.StopAll();
				await UnitySceneManager.UnloadSceneAsync( _name ).ConfigureAwait( cancel );
			} );
		}

		public void Hoge() {
			Log.Debug( string.Join( "\n", _scene.GetRootGameObjects().Select( go => go.name ) ) );
		}
	}


	public class UnknownScene : BaseScene {
		public UnknownScene() {
			_scene = UnitySceneManager.GetActiveScene();
			_name = _scene.name;
			Log.Debug( _name );
		}
	}


	public class TestChangeScene1Scene : BaseScene {
	}


	public class TestChangeScene2Scene : BaseScene {
	}
}