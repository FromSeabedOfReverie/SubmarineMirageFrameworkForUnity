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
	using Event;
	using Task;
	using Task.Base;
	using FSM.State;
	using Scene.Base;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMScene : SMState<SMSceneManager, SMSceneFSM> {
		public SMGroupManagerBody _groupManagerBody	{ get; private set; }

		[SMShow] public Scene _rawScene	{ get; protected set; }
		[SMShowLine] public string _name	{ get; protected set; }
		[SMShow] protected string _registerEventName	{ get; private set; }

		public readonly SMAsyncEvent _createBehavioursEvent = new SMAsyncEvent();


		public SMScene() {
			SetSceneName();
			ReloadRawScene();
			_registerEventName = this.GetAboutName();

			var groupManager = new SMGroupManager( this );
			_groupManagerBody = groupManager._body;

			_disposables.AddLast( () => {
				_groupManagerBody._manager.Dispose();
			} );
/*
			var test = new Test.TestSMScene( this );
			test.SetEvent();
			_disposables.AddLast( () => {
				test.Dispose();
			} );
*/


			_enterEvent.AddLast( _registerEventName, async canceler => {
				SMLog.Debug( $"start : {this.GetAboutName()}入口" );
				var isRemove = _owner._body.RemoveFirstLoaded( this );
				SMLog.Debug( $"{this.GetAboutName()}既に読まれてる？ : {isRemove}" );
				if ( !isRemove ) {
					await SceneManager.LoadSceneAsync( _name, LoadSceneMode.Additive ).ToUniTask( canceler );
					ReloadRawScene();
				}
				if ( this is MainSMScene ) {
					SMLog.Debug( $"メインシーン : {_rawScene.name}" );
					SceneManager.SetActiveScene( _rawScene );
				}
				await _createBehavioursEvent.Run( canceler );
				await _groupManagerBody.Enter();
				SMLog.Debug( $"end : {this.GetAboutName()}入口" );
			} );

			_exitEvent.AddLast( _registerEventName, async canceler => {
				SMLog.Debug( $"start : {this.GetAboutName()}出口" );
				await _groupManagerBody.Exit();
				await SceneManager.UnloadSceneAsync( _name ).ToUniTask( canceler );
				ReloadRawScene();
				await Resources.UnloadUnusedAssets().ToUniTask( canceler );
				SMLog.Debug( $"end : {this.GetAboutName()}出口" );
			} );


			_fixedUpdateEvent.AddLast( _registerEventName ).Subscribe( _ => _groupManagerBody.FixedUpdate() );
			_updateEvent.AddLast( _registerEventName ).Subscribe( _ => _groupManagerBody.Update() );
			_lateUpdateEvent.AddLast( _registerEventName ).Subscribe( _ => _groupManagerBody.LateUpdate() );
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


		public T GetBehaviour<T>() where T : SMBehaviour
			=> _groupManagerBody._manager.GetBehaviour<T>();

		public SMBehaviour GetBehaviour( Type type )
			=> _groupManagerBody._manager.GetBehaviour( type );

		public IEnumerable<T> GetBehaviours<T>() where T : SMBehaviour
			=> _groupManagerBody._manager.GetBehaviours<T>();

		public IEnumerable<SMBehaviour> GetBehaviours( Type type )
			=> _groupManagerBody._manager.GetBehaviours( type );
	}
}