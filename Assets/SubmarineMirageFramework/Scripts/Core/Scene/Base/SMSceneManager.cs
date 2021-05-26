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
	using Task;
	using FSM;
	using Service;
	using Extension;
	using Utility;
	using Debug;



	public class SMSceneManager : SMTask, ISMFSMOwner, ISMService {
		[SMShow] public SMFSM _fsm		{ get; set; }
		public SMFSM _foreverFSM		{ get; set; }
		public SMFSM _mainFSM			{ get; set; }
		public SMFSM _uiFSM				{ get; set; }
		public SMFSM _debugFSM			{ get; set; }

		public SMScene _foreverScene	{ get; set; }
		Scene _rawSceneUntilDispose		{ get; set; }
		[SMShow] public List<Scene> _firstLoadedRawScenes { get; set; } = new List<Scene>();

		public bool _isFinalDisabling	{ get; set; }
		[SMShowLine] public bool _isInitialEnteredFSMs { get; set; }



#region ToString
		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _fsm ), i =>
				_toStringer.DefaultValue( _fsm?.GetFSMs(), i, true ) );
			_toStringer.SetValue( nameof( _firstLoadedRawScenes ), i =>
				_toStringer.DefaultValue( _firstLoadedRawScenes.Select( s => s.name ), i, false ) );
		}
#endregion



		public SMSceneManager() {
			_rawSceneUntilDispose = SceneManager.CreateScene( "UntilDispose" );

			_disposables.AddLast( () => {
				_fsm.Dispose();
				SubmarineMirageFramework.Shutdown();
			} );

///*
			var test = new Test.TestSMSceneManager( this );
//			test.SetEvent();
			_disposables.AddLast( () => {
				test.Dispose();
			} );
//*/
		}

		public override void Create() {
		}

		public override void Dispose() => base.Dispose();



		public async UniTask Initialize() {
			var setting = SMServiceLocator.Resolve<ISMSceneSetting>();
			_fsm = SMFSM.Generate( this, setting._sceneFSMList );
			SMServiceLocator.Unregister<ISMSceneSetting>();

			_foreverFSM = _fsm.GetFSM<ForeverSMScene>();
			_mainFSM = _fsm.GetFSM<MainSMScene>();
			_uiFSM = _fsm.GetFSM<UISMScene>();
			_debugFSM = _fsm.GetFSM<DebugSMScene>();

			_foreverScene = _foreverFSM.GetStates()
				.Select( s => s as SMScene )
				.FirstOrDefault();

			_firstLoadedRawScenes = GetLoadedRawScenes().ToList();

			_fsm.GetFSMs().ForEach( fsm => {
				fsm._body._startStateType = fsm.GetStates()
					.Select( s => s as SMScene )
					.FirstOrDefault( s => IsFirstLoaded( s ) )
					?.GetType();
			} );
			// 不明なシーンを設定
			var scene = _mainFSM.GetState<UnknownSMScene>();
			scene.Setup();

			_isInitialized = true;
			_isOperable = true;
			_isActive = true;

			await _foreverFSM.InitialEnter( true );
			await _fsm.InitialEnter();
		}

		public async UniTask Finalize() {
			_isFinalizing = true;

			await _fsm.GetFSMs()
				.Where( fsm => fsm != _foreverFSM )
				.Select( fsm => fsm.FinalExit( true ) );
			await _foreverFSM.FinalExit( true );

			_isActive = false;

			Dispose();
		}



		public IEnumerable<Scene> GetLoadedRawScenes()
			=> Enumerable.Range( 0, SceneManager.sceneCount )
				.Select( i => SceneManager.GetSceneAt( i ) )
				.Where( rs => rs.name != _rawSceneUntilDispose.name );

		public bool RemoveFirstLoaded( SMScene scene )
			=> _firstLoadedRawScenes
				.RemoveFind( s => s.name == scene._name );

		public bool IsFirstLoaded( SMScene scene )
			=> _firstLoadedRawScenes
				.Any( s => s.name == scene._name );



		public IEnumerable<Scene> GetUnknownRawScenes() {
			var scenes = _firstLoadedRawScenes.Copy();

			_fsm.GetFSMs()
				.Where( fsm => fsm._body._startStateType != null )
				.Select( fsm => fsm.GetState( fsm._body._startStateType ) )
				.Select( s => s as SMScene )
				.ForEach( s => scenes.Remove( rs => rs.name == s._name ) );

			return scenes;
		}



		public IEnumerable<string> GetScenePathsInBuild()
			=> Enumerable.Range( 0, SceneManager.sceneCountInBuildSettings )
				.Select( i => SceneUtility.GetScenePathByBuildIndex( i ) );

		public bool IsExistSceneInBuild( string path, bool isNameOnly = false )
			=> GetScenePathsInBuild()
				.Select( p => {
					if ( isNameOnly ) { return PathSMUtility.GetName( p ); }
					return p;
				} )
				.Any( p => p == path );



		public void MoveForeverScene( GameObject gameObject )
			=> SceneManager.MoveGameObjectToScene( gameObject, _foreverScene._rawScene );



		public IEnumerable<SMScene> GetScenes()
			=> _fsm.GetFSMs()
				.SelectMany( fsm => fsm.GetStates() )
				.Select( s => s as SMScene )
				.Distinct();

		public SMScene GetScene( Scene rawScene )
			=> GetScenes()
				.FirstOrDefault( s => s._rawScene == rawScene );
	}
}