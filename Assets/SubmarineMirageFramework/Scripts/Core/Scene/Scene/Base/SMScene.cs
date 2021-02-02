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
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;
	using MultiEvent;
	using Task;
	using Task.Base;
	using FSM.State;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMScene : SMState<SMSceneManager, SMSceneInternalFSM> {
		[SMShowLine] public string _name	{ get; protected set; }
		public Scene _rawScene	{ get; protected set; }
		public SMGroupManagerBody _groupManagerBody	{ get; private set; }

		[SMHide] protected readonly SMMultiAsyncEvent _createBehavioursEvent = new SMMultiAsyncEvent();


		public SMScene() {
			SetSceneName();
			ReloadRawScene();

			var groupManager = new SMGroupManager( this );
			_groupManagerBody = groupManager._body;

			_disposables.AddLast( () => {
				_groupManagerBody._manager.Dispose();
			} );

			_enterEvent.AddLast( async canceler => {
// TODO : ForeverScene等、作成系シーンで、読込ガードされる？
				var isRemove = _fsm._fsm.RemoveFirstLoaded( this );
				if ( !isRemove ) {
					await SceneManager.LoadSceneAsync( _name, LoadSceneMode.Additive ).ToUniTask( canceler );
					ReloadRawScene();
				}
				if ( this is MainSMScene )	{ SceneManager.SetActiveScene( _rawScene ); }
				await _createBehavioursEvent.Run( canceler );
				await _groupManagerBody.Enter();
			} );

			_exitEvent.AddLast( async canceler => {
				await _groupManagerBody.Exit();
				await SceneManager.UnloadSceneAsync( _name ).ToUniTask( canceler );
				ReloadRawScene();
				await Resources.UnloadUnusedAssets().ToUniTask( canceler );
			} );

			_fixedUpdateEvent.AddLast().Subscribe( _ => _groupManagerBody.FixedUpdate() );
			_updateEvent.AddLast().Subscribe( _ => _groupManagerBody.Update() );
			_lateUpdateEvent.AddLast().Subscribe( _ => _groupManagerBody.LateUpdate() );
		}


		protected virtual void SetSceneName()
			=> _name = this.GetAboutName().RemoveAtLast( "SMScene" );

		protected virtual void ReloadRawScene()
			=> _rawScene = SceneManager.GetSceneByName( _name );



		public void MoveGroup( SMGroupBody groupBody )
			=> SceneManager.MoveGameObjectToScene( groupBody._gameObject, _rawScene );


		public bool IsInBuild() {
#if UNITY_EDITOR
			return UnityEditor.EditorBuildSettings.scenes
				.Any( s => s.path == _rawScene.path );
#else
			return true;
#endif
		}


		public T GetBehaviour<T>() where T : ISMBehaviour
			=> _groupManagerBody._manager.GetBehaviour<T>();

		public ISMBehaviour GetBehaviour( Type type )
			=> _groupManagerBody._manager.GetBehaviour( type );

		public IEnumerable<T> GetBehaviours<T>() where T : ISMBehaviour
			=> _groupManagerBody._manager.GetBehaviours<T>();

		public IEnumerable<ISMBehaviour> GetBehaviours( Type type )
			=> _groupManagerBody._manager.GetBehaviours( type );
	}
}