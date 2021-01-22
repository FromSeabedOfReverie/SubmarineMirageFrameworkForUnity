//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine.SceneManagement;
	using Cysharp.Threading.Tasks;
	using DG.Tweening;
	using KoganeUnityLib;
	using Task.Group;
	using Task.Group.Manager;
	using FSM.State;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMScene : SMState<SMSceneManager, SMSceneInternalFSM> {
		public string _name				{ get; protected set; }
		protected string _registerKey	{ get; private set; }

		public Scene _rawScene	{ get; protected set; }
		public SMGroupManager _groups	{ get; private set; }


		public SMScene() {
			_name = this.GetAboutName().RemoveAtLast( "SMScene" );
			_registerKey = nameof( SMScene );
			ReloadRawScene();
			_groups = new SMGroupManager( this );
			_disposables.AddLast( _groups );

			_enterEvent.AddFirst( _registerKey, async canceler => {
				if ( !_fsm._fsm.IsFirstLoaded( _name ) ) {
					await SceneManager.LoadSceneAsync( _name, LoadSceneMode.Additive ).ToUniTask( canceler );
				}
				ReloadRawScene();
				SceneManager.SetActiveScene( _rawScene );
				await _groups.Enter();
			} );

			_exitEvent.AddFirst( _registerKey, async canceler => {
				await _groups.Exit();
// TODO : DOTween全停止による、音停止を、シーン内の文字列登録文だけ停止させる事で、流し続ける
//				DOTween.KillAll();
//				GameAudioManager.s_instance.StopAll();
				await SceneManager.UnloadSceneAsync( _name ).ToUniTask( canceler );
			} );
		}


		protected virtual void ReloadRawScene()
			=> _rawScene = SceneManager.GetSceneByName( _name );


		public void MoveGroup( SMGroup group )
			=> SceneManager.MoveGameObjectToScene( group._gameObject, _rawScene );


		public bool IsInBuild() {
#if UNITY_EDITOR
			return UnityEditor.EditorBuildSettings.scenes
				.Any( scene => scene.path == _rawScene.path );
#else
			return true;
#endif
		}
	}
}