//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Scene {
	using System;
	using System.Linq;
	using System.Threading;
	using UniRx.Async;
	using DG.Tweening;
	using KoganeUnityLib;
	using Process.New;
	using FSM.New;
	using Singleton.New;
	using Test.Audio;
	using Extension;
	using Utility;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


	// TODO : コメント追加、整頓


	public class SceneManager : Singleton<SceneManager>, IFiniteStateMachineOwner<SceneStateMachine> {
		public SceneStateMachine _fsm	{ get; private set; }
		public string _currentSceneName => _fsm._currentSceneName;

		public override void Create() {
			_fsm = new SceneStateMachine( this );
		}

		public override void Dispose() {
			_fsm.Dispose();
			base.Dispose();
		}
	}


	public class SceneStateMachine : FiniteStateMachine<SceneManager, SceneStateMachine> {
		public BaseScene _scene => (BaseScene)_state;
		public string _currentSceneName => _scene?._name;
		public bool _isSkipLoadForFirstScene = true;

		public SceneStateMachine( SceneManager owner ) : base(
			owner,
			new BaseScene[] {
				new TestChangeScene1Scene( owner ),
				new TestChangeScene2Scene( owner ),
			}
		) {
			var firstScene = _states
				.Select( pair => pair.Value )
				.Select( s => (BaseScene)s )
				.FirstOrDefault( s => s._name == UnitySceneManager.GetActiveScene().name );
			if ( firstScene != null ) {
				ChangeScene( _owner._activeAsyncCancel, firstScene.GetType() ).Forget();
			}
		}

		public async UniTask ChangeScene<TState>( CancellationToken cancel )
			=> await ChangeState<TState>( cancel );

		public async UniTask ChangeScene( CancellationToken cancel, Type stateType )
			=> await ChangeState( cancel, stateType );
	}


	public abstract class BaseScene : State<SceneManager, SceneStateMachine> {
		public string _name	{ get; private set; }

		public BaseScene( SceneManager owner ) : base( owner ) {
			_name = this.GetAboutName().RemoveAtLast( "Scene" );
			_enterEvent.AddFirst( async cancel => {
				if ( _fsm._isSkipLoadForFirstScene ) {
					_fsm._isSkipLoadForFirstScene = false;
				} else {
					await UnitySceneManager.LoadSceneAsync( _name ).ConfigureAwait( cancel );
				}
				await CoreProcessManager.s_instance.RunSceneProcesses();
			} );
			_exitEvent.AddFirst( async cancel => {
				await CoreProcessManager.s_instance.DeleteSceneProcesses();
// TODO : DOTween全停止による、音停止を、シーン内の文字列登録文だけ停止させる事で、流し続ける
				DOTween.KillAll();
				TestAudioManager.s_instance.StopAll();
				await UnitySceneManager.UnloadSceneAsync( _name ).ConfigureAwait( cancel );
			} );
		}
	}


	public class TestChangeScene1Scene : BaseScene {
		public TestChangeScene1Scene( SceneManager owner ) : base( owner ) {
		}
	}


	public class TestChangeScene2Scene : BaseScene {
		public TestChangeScene2Scene( SceneManager owner ) : base( owner ) {
		}
	}
}