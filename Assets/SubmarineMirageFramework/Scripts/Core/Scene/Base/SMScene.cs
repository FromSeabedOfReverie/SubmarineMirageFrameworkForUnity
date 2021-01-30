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
	using Task.Behaviour;
	using Task.Group;
	using Task.Group.Manager;
	using Task.Group.Manager.Modifyler;
	using FSM.State;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMScene : SMState<SMSceneManager, SMSceneInternalFSM> {
		[SMShowLine] public string _name	{ get; protected set; }
		public Scene _rawScene	{ get; protected set; }
		public SMGroupManager _groups	{ get; private set; }

		[SMHide] protected readonly SMMultiAsyncEvent _createBehavioursEvent = new SMMultiAsyncEvent();


		public SMScene() {
			SetSceneName();
			ReloadRawScene();
			_groups = new SMGroupManager( this );

			_disposables.AddLast( () => {
				_groups.Dispose();
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
				await _groups.Enter();
			} );

			_exitEvent.AddLast( async canceler => {
				await _groups.Exit();
				await SceneManager.UnloadSceneAsync( _name ).ToUniTask( canceler );
				ReloadRawScene();
				await Resources.UnloadUnusedAssets().ToUniTask( canceler );
			} );

			_fixedUpdateEvent.AddLast().Subscribe( _ =>
				SMGroupManagerApplyer.FixedUpdate( _groups )
			);
			_updateEvent.AddLast().Subscribe( _ =>
				SMGroupManagerApplyer.Update( _groups )
			);
			_lateUpdateEvent.AddLast().Subscribe( _ =>
				SMGroupManagerApplyer.LateUpdate( _groups )
			);
		}


		protected virtual void SetSceneName()
			=> _name = this.GetAboutName().RemoveAtLast( "SMScene" );

		protected virtual void ReloadRawScene()
			=> _rawScene = SceneManager.GetSceneByName( _name );



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


		public T GetBehaviour<T>() where T : ISMBehaviour
			=> _groups.GetBehaviour<T>();

		public ISMBehaviour GetBehaviour( Type type )
			=> _groups.GetBehaviour( type );

		public IEnumerable<T> GetBehaviours<T>() where T : ISMBehaviour
			=> _groups.GetBehaviours<T>();

		public IEnumerable<ISMBehaviour> GetBehaviours( Type type )
			=> _groups.GetBehaviours( type );
	}
}