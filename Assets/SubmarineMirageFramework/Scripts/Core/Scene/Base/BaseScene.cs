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
	using Cysharp.Threading.Tasks;
	using DG.Tweening;
	using KoganeUnityLib;
	using UTask;
	using SMTask;
	using FSM;
	using Extension;
	using Utility;
	using Debug;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


	// TODO : コメント追加、整頓


	public abstract class BaseScene : State<SceneStateMachine, SceneManager> {
		public string _name				{ get; protected set; }
		protected string _registerKey	{ get; private set; }

		public Scene _scene	{ get; protected set; }
		public SMObjectManager _objects	{ get; private set; }


		public BaseScene() {
			_name = this.GetAboutName().RemoveAtLast( "Scene" );
			_registerKey = nameof( BaseScene );
			ResetScene();
			_objects = new SMObjectManager( this );
			_disposables.AddLast( _objects );

			_enterEvent.AddFirst( _registerKey, async cancel => {
				if ( _fsm._isSkipLoadForFirstScene ) {
					_fsm._isSkipLoadForFirstScene = false;
				} else {
					await UnitySceneManager.LoadSceneAsync( _name, LoadSceneMode.Additive )
						.ToUniTask( cancel );
				}
				ResetScene();
				UnitySceneManager.SetActiveScene( _scene );
				await _objects.Enter();
			} );

			_exitEvent.AddFirst( _registerKey, async cancel => {
				await _objects.Exit();
// TODO : DOTween全停止による、音停止を、シーン内の文字列登録文だけ停止させる事で、流し続ける
//				DOTween.KillAll();
//				GameAudioManager.s_instance.StopAll();
				await UnitySceneManager.UnloadSceneAsync( _name ).ToUniTask( cancel );
			} );
		}


		protected virtual void ResetScene()
			=> _scene = UnitySceneManager.GetSceneByName( _name );
	}
}