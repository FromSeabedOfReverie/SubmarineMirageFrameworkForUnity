//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestScene
namespace SubmarineMirage.Scene {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Service;
	using Event;
	using Task;
	using Task.Marker;
	using FSM;
	using Extension;
	using Utility;
	using Debug;



	public abstract class SMScene : SMState {
		public new SMSceneManager _owner { get; private set; }

		[SMShowLine] public string _name { get; protected set; }
		[SMShow] protected string _registerEventName { get; private set; }

		[SMShow] public bool _isEntered	{ get; private set; }
		[SMShow] protected virtual bool _isUseUnloadUnusedAssets => true;

		SMTaskMarkerManager _taskMarkers { get; set; }

		[SMShow] public Scene _rawScene { get; protected set; }

		public readonly SMAsyncEvent _createBehavioursEvent = new SMAsyncEvent();



		public SMScene() {
			SetSceneName();
			ReloadRawScene();
			_registerEventName = this.GetAboutName();


			_enterEvent.AddLast( _registerEventName, async canceler => {
				var isRemove = _owner.RemoveFirstLoaded( this );
				if ( !isRemove ) {
					await SceneManager.LoadSceneAsync( _name, LoadSceneMode.Additive )
						.ToUniTask( canceler );
					ReloadRawScene();
				}
				if ( this is MainSMScene ) {
					SceneManager.SetActiveScene( _rawScene );
				}

				var taskManager = SMServiceLocator.Resolve<SMTaskManager>();
//				_taskMarkers = new SMTaskMarkerManager( _registerEventName, taskManager );
				await _createBehavioursEvent.Run( canceler );
				await LoadBehaviour( canceler );

// TODO : 循環待機になり、永遠に終わらない
//				await _taskMarkers.InitializeAll();

				_isEntered = true;
			} );


			_exitEvent.AddLast( _registerEventName, async canceler => {
				_isEntered = false;

//				await _taskMarkers.FinalizeAll();
				_taskMarkers = null;

				await SceneManager.UnloadSceneAsync( _name )
					.ToUniTask( canceler );
				ReloadRawScene();
				if ( _isUseUnloadUnusedAssets ) {
					await Resources.UnloadUnusedAssets()
						.ToUniTask( canceler );
				}
			} );


			_disposables.AddLast( () => {
				_taskMarkers?.Dispose();
				_createBehavioursEvent.Dispose();

				_isEntered = false;
			} );
		}

		public override void Setup( object owner, SMFSM fsm ) {
			base.Setup( owner, fsm );
			_owner = base._owner as SMSceneManager;
		}

		public override void Dispose() => base.Dispose();



		async UniTask LoadBehaviour( SMAsyncCanceler canceler ) {
			await UTask.DontWait();
/*
// TODO : SMBehaviour修正後、コメントアウト

			foreach ( var go in _rawScene.GetRootGameObjects() ) {
				var bs = go.GetComponentsInChildren<SMBehaviour>();
				if ( bs.IsEmpty() )	{ continue; }

				var top = bs.First();
				bs.ForEach( b => {
					if ( b._type != top._type ) {
						throw new InvalidOperationException(
							$"トップ以下の実行型が、不整合 : top {top._type} : other {b._type}\n{b}\n{top}" );
					}
					b.Constructor( this );
				} );

				await UTask.NextFrame( canceler );
			}
*/
		}



		protected virtual void SetSceneName()
			=> _name = this.GetAboutName().RemoveAtLast( "SMScene" );

		protected virtual void ReloadRawScene()
			=> _rawScene = SceneManager.GetSceneByName( _name );

		public void MoveGameObject( GameObject gameObject )
			=> SceneManager.MoveGameObjectToScene( gameObject, _rawScene );

		public bool IsInBuild()
			=> _owner.IsExistSceneInBuild( _rawScene.path );

/*
		public IEnumerable<SMBehaviourBody> GetBehaviours()
			=> _taskMarkers.GetAlls( type, false )
				.Select( t => t as SMBehaviourBody );
*/
	}
}