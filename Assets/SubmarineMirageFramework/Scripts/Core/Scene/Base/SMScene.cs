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
	using KoganeUnityLib;
	using Event;
	using Task;
	using Task.Modifyler;
	using Task.Marker;
	using FSM;
	using FSM.State;
	using Behaviour;
	using Extension;
	using Utility;
	using Debug;



	public abstract class SMScene : SMState {
		public new SMSceneManager _owner { get; private set; }

//		SMTaskRunType _type = SMTaskRunType.Parallel;

		public bool _isEntered	{ get; private set; }
		protected virtual bool _isUseUnloadUnusedAssets => true;

		SMTaskMarkerManager _taskMarkers;

		[SMShow] public Scene _rawScene { get; protected set; }
		[SMShowLine] public string _name { get; protected set; }
		[SMShow] protected string _registerEventName { get; private set; }
		public readonly SMAsyncEvent _createBehavioursEvent = new SMAsyncEvent();



		public SMScene() {
			SetSceneName();
			ReloadRawScene();
			_registerEventName = this.GetAboutName();

			_enterEvent.AddLast( _registerEventName, _ => Enter() );
			_exitEvent.AddLast( _registerEventName, _ => Exit() );

			_disposables.AddLast( () => {
				_taskMarkers?.Dispose();
			} );

/*
			var test = new Test.TestSMScene( this );
			test.SetEvent();
			_disposables.AddLast( () => {
				test.Dispose();
			} );
*/
		}

		public override void Setup( object owner, SMFSM fsm ) {
			base.Setup( owner, fsm );
			_owner = base._owner as SMSceneManager;
		}

		public override void Dispose() => base.Dispose();



		public async UniTask Enter() {
			var isRemove = _owner.RemoveFirstLoaded( this );
			if ( !isRemove ) {
				await SceneManager.LoadSceneAsync( _name, LoadSceneMode.Additive )
					.ToUniTask( _asyncCancelerOnDisableAndExit );
				ReloadRawScene();
			}
			if ( this is MainSMScene ) {
				SceneManager.SetActiveScene( _rawScene );
			}

			_taskMarkers = new SMTaskMarkerManager( this.GetAboutName() );
			await _createBehavioursEvent.Run( _asyncCancelerOnDisableAndExit );
			await Load();

			await _taskMarkers.InitializeAll();

			_isEntered = true;
		}

		public async UniTask Exit() {
			_isEntered = false;

			await _taskMarkers.FinalizeAll();

			await SceneManager.UnloadSceneAsync( _name )
				.ToUniTask( _asyncCancelerOnDisableAndExit );
			ReloadRawScene();
			if ( _isUseUnloadUnusedAssets ) {
				await Resources.UnloadUnusedAssets()
					.ToUniTask( _asyncCancelerOnDisableAndExit );
			}
		}

		async UniTask Load() {
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

				await UTask.NextFrame( _asyncCancelerOnDisableAndExit );
			}
		}



		protected virtual void SetSceneName()
			=> _name = this.GetAboutName().RemoveAtLast( "SMScene" );

		protected virtual void ReloadRawScene()
			=> _rawScene = SceneManager.GetSceneByName( _name );

		public void MoveGameObject( GameObject gameObject )
			=> SceneManager.MoveGameObjectToScene( gameObject, _rawScene );

		public bool IsInBuild()
			=> _owner.IsExistSceneInBuild( _rawScene.path );



		public SMTask GetFirst( SMTaskRunType type, bool isRaw = false )
			=> _taskMarkers.GetFirst( type, isRaw );

		public SMTask GetLast( SMTaskRunType type, bool isRaw = false )
			=> _taskMarkers.GetLast( type, isRaw );

		IEnumerable<SMTask> GetAlls( SMTaskRunType type )
			=> _taskMarkers.GetAlls( type, true );

		public IEnumerable<SMBehaviourBody> GetBehaviours()
			=> _taskMarkers.GetAlls()
				.Select( t => t as SMBehaviourBody );
	}
}