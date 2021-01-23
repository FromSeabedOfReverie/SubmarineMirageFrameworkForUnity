//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using System.Linq;
	using UnityEngine.SceneManagement;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Task;
	using Task.Group;
	using Task.Group.Manager;
	using FSM.State;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMScene : SMState<SMSceneManager, SMSceneInternalFSM> {
		public string _name				{ get; protected set; }
		protected string _registerEventName	{ get; private set; }

		public Scene _rawScene	{ get; protected set; }
		public SMGroupManager _groups	{ get; private set; }


		public SMScene() {
			SetSceneName();
			_registerEventName = nameof( SMScene );
			ReloadRawScene();

			_groups = new SMGroupManager( this );
			_disposables.AddLast( () => {
				_groups.Dispose();
			} );

			_enterEvent.AddLast( _registerEventName, async canceler => {
				var isRemove = _fsm._fsm.RemoveFirstLoaded( this );
				if ( !isRemove ) {
					await SceneManager.LoadSceneAsync( _name, LoadSceneMode.Additive ).ToUniTask( canceler );
					ReloadRawScene();
				}
				SceneManager.SetActiveScene( _rawScene );
				await _groups.Enter();
			} );
			_exitEvent.AddLast( _registerEventName, async canceler => {
				await _groups.Exit();
			} );
		}


		protected virtual void SetSceneName()
			=> _name = this.GetAboutName().RemoveAtLast( "SMScene" );

		protected virtual void ReloadRawScene()
			=> _rawScene = SceneManager.GetSceneByName( _name );


		protected async UniTask LoadScene( SMTaskCanceler canceler ) {
			
		}


		public void MoveGroup( SMGroup group )
			=> SceneManager.MoveGameObjectToScene( group._gameObject, _rawScene );


		public bool IsInBuild() {
#if UNITY_EDITOR
			return UnityEditor.EditorBuildSettings.scenes
				.Any( s => s.path == _rawScene.path );
#else
			return true;
#endif
		}
	}
}