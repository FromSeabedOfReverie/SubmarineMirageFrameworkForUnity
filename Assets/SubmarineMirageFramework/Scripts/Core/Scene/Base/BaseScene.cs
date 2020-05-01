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
	using UniRx.Async;
	using DG.Tweening;
	using KoganeUnityLib;
	using Process.New;
	using FSM.New;
	using Audio;
	using Extension;
	using Utility;
	using Debug;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


	// TODO : コメント追加、整頓


	public abstract class BaseScene : State<SceneStateMachine, SceneManager> {
		public string _name	{ get; protected set; }
		protected string registerKey	{ get; private set; }

		public Scene _scene	{ get; protected set; }
		public ProcessHierarchyManager _hierarchies	{ get; private set; }


		public BaseScene() {
			_name = this.GetAboutName().RemoveAtLast( "Scene" );
			registerKey = nameof( BaseScene );
			ResetScene();
			_hierarchies = new ProcessHierarchyManager( this );
			_disposables.AddLast( _hierarchies );

			_enterEvent.AddFirst( registerKey, async cancel => {
				if ( _fsm._isSkipLoadForFirstScene ) {
					_fsm._isSkipLoadForFirstScene = false;
				} else {
					await UnitySceneManager.LoadSceneAsync( _name, LoadSceneMode.Additive )
						.ConfigureAwait( cancel );
				}
				ResetScene();
				UnitySceneManager.SetActiveScene( _scene );
				await _hierarchies.Enter();
			} );

			_exitEvent.AddFirst( registerKey, async cancel => {
				await _hierarchies.Exit();
// TODO : DOTween全停止による、音停止を、シーン内の文字列登録文だけ停止させる事で、流し続ける
//				DOTween.KillAll();
//				GameAudioManager.s_instance.StopAll();
				await UnitySceneManager.UnloadSceneAsync( _name ).ConfigureAwait( cancel );
			} );
		}


		protected virtual void ResetScene()
			=> _scene = UnitySceneManager.GetSceneByName( _name );
	}
}