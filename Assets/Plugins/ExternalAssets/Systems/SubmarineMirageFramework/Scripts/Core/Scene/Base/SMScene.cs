//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestScene
namespace SubmarineMirage {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ シーンのクラス
	/// </summary>
	///====================================================================================================
	public abstract class SMScene : SMState<SMSceneManager, SMScene> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>シーン名</summary>
		[SMShowLine] public string _name { get; protected set; }
		/// <summary>登録イベント名</summary>
		[SMShow] protected string _registerEventName { get; private set; }

		/// <summary>シーン実行中か？</summary>
		[SMShow] public bool _isEntered	{ get; private set; }
		/// <summary>シーン遷移時に、未使用アセットを破棄するか？</summary>
		[SMShow] protected virtual bool _isUseUnloadUnusedAssets => true;

		/// <summary>タスクの印の管理者</summary>
		SMTaskMarkerManager _taskMarkers { get; set; }

		/// <summary>生のUnityシーン</summary>
		[SMShow] public Scene _rawScene { get; protected set; }

		/// <summary>モノ動作を作成するイベント</summary>
		public readonly SMAsyncEvent _createBehavioursEvent = new SMAsyncEvent();



		public SMScene() {
			SetSceneName();
			ReloadRawScene();
			_registerEventName = this.GetName();

			var uiFade = SMServiceLocator.Resolve<SMUIFade>();
			var audioManager = SMServiceLocator.Resolve<SMAudioManager>();
			var gameServer = SMServiceLocator.Resolve<SMNetworkManager>()._gameServerModel;
			var isMainScene = this is MainSMScene;

			_enterEvent.AddLast( _registerEventName, async canceler => {
				var isRemove = _owner.RemoveFirstLoaded( this );
				if ( !isRemove ) {
					await SceneManager.LoadSceneAsync( _name, LoadSceneMode.Additive )
						.ToUniTask( canceler );
					ReloadRawScene();
				}
				if ( isMainScene ) {
					SceneManager.SetActiveScene( _rawScene );
				}

//				var taskManager = SMServiceLocator.Resolve<SMTaskManager>();
//				_taskMarkers = new SMTaskMarkerManager( _registerEventName, taskManager );
				await _createBehavioursEvent.Run( canceler );
				await LoadBehaviour( canceler );

// TODO : 循環待機になり、永遠に終わらない
//				await _taskMarkers.InitializeAll();

				if ( isMainScene ) {
					UTask.Void( async () => {
						await UTask.Delay( _asyncCancelerOnExit, 500 );
						await uiFade.In();
					} );
					if ( gameServer != null ) {
						gameServer._isActive = true;
					}
				}

				_isEntered = true;
			} );


			_exitEvent.AddLast( _registerEventName, async canceler => {
				_isEntered = false;

				if ( isMainScene ) {
					if ( gameServer != null ) {
						gameServer._isActive = false;
					}
					await UniTask.WhenAll(
						uiFade.Out(),
						audioManager.StopAll()
					);
				}

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


			_disposables.AddFirst( () => {
				_taskMarkers?.Dispose();
				_createBehavioursEvent.Dispose();

				_isEntered = false;
			} );
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
			=> _name = this.GetName().RemoveAtLast( "SMScene" );

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