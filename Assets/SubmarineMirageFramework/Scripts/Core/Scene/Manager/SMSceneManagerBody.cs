//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene.Base {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;
	using Service;
	using Event;
	using Task;
	using Task.Modifyler.Base;
	using Scene.Modifyler;
	using Scene.Modifyler.Base;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMSceneManagerBody : SMTaskModifyTarget<SMSceneManagerModifyler> {
		public SMSceneManager _sceneManager	{ get; private set; }
		[SMShow] public SMSceneFSM _fsm	{ get; set; }

		public SMSceneFSM _foreverFSM	{ get; set; }
		public SMSceneFSM _mainFSM		{ get; set; }
		public SMSceneFSM _uiFSM		{ get; set; }
		public SMSceneFSM _debugFSM	{ get; set; }

		public SMScene _foreverScene	{ get; set; }
		Scene _rawSceneUntilDispose	{ get; set; }
		[SMShow] public List<Scene> _firstLoadedRawScenes	{ get; set; } = new List<Scene>();

		public readonly ReactiveProperty<bool> _isUpdating = new ReactiveProperty<bool>();
		public bool _isFinalDisabling	{ get; set; }

		public readonly SMAsyncEvent _selfInitializeEvent	= new SMAsyncEvent();
		public readonly SMAsyncEvent _initializeEvent		= new SMAsyncEvent();
		public readonly SMSubject _enableEvent			= new SMSubject();
		public readonly SMSubject _fixedUpdateEvent		= new SMSubject();
		public readonly SMSubject _updateEvent			= new SMSubject();
		public readonly SMSubject _lateUpdateEvent		= new SMSubject();
		public readonly SMSubject _disableEvent			= new SMSubject();
		public readonly SMAsyncEvent _finalizeEvent		= new SMAsyncEvent();
#if DEVELOP
		public readonly SMSubject _onGUIEvent = new SMSubject();
#endif
		public readonly SMAsyncCanceler _asyncCancelerOnDisable = new SMAsyncCanceler();
		public readonly SMAsyncCanceler _asyncCancelerOnDispose	= new SMAsyncCanceler();



#region ToString
		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _fsm ), i =>
				_toStringer.DefaultValue( _fsm?.GetFSMs(), i, true ) );
			_toStringer.SetValue( nameof( _firstLoadedRawScenes ), i =>
				_toStringer.DefaultValue( _firstLoadedRawScenes.Select( s => s.name ), i, false ) );
			_toStringer.Add( nameof( _isUpdating ), i => $"{_isUpdating.Value}" );
		}
#endregion



		public SMSceneManagerBody( SMSceneManager sceneManager ) {
			_modifyler = new SMSceneManagerModifyler( this );
			_modifyler.SetupSceneUpdating( _isUpdating );

			_sceneManager = sceneManager;
			_rawSceneUntilDispose = SceneManager.CreateScene( "UntilDispose" );


			_disposables.AddLast( () => {
				_isFinalizing = true;
				_ranState = SMTaskRunState.Finalize;

				_isUpdating.Dispose();
				_fsm.Dispose();

				_asyncCancelerOnDisable.Dispose();
				_asyncCancelerOnDispose.Dispose();

				_selfInitializeEvent.Dispose();
				_initializeEvent.Dispose();
				_enableEvent.Dispose();
				_fixedUpdateEvent.Dispose();
				_updateEvent.Dispose();
				_lateUpdateEvent.Dispose();
				_disableEvent.Dispose();
				_finalizeEvent.Dispose();
#if DEVELOP
				_onGUIEvent.Dispose();
#endif
			} );
		}

		public override void Dispose() => base.Dispose();



		public void StopAsyncOnDisable() => _asyncCancelerOnDisable.Cancel();



		public bool IsActiveInHierarchyAndComponent()
			=> _sceneManager.gameObject.activeInHierarchy && _sceneManager.enabled;



		public async UniTask Initialize() {
			await _modifyler.RegisterAndRun( new CreateSMSceneManager() );
			await _modifyler.RegisterAndRun( new SelfInitializeSMSceneManager() );
			await _modifyler.RegisterAndRun( new InitializeSMSceneManager() );
			await _modifyler.RegisterAndRun( new InitialEnableSMSceneManager() );
		}

		public async UniTask Finalize() {
			await _modifyler.RegisterAndRun( new FinalDisableSMSceneManager() );
			await _modifyler.RegisterAndRun( new FinalizeSMSceneManager() );
		}



		public void FixedUpdate() {
			if ( !_isOperable )	{ return; }
			if ( !_isActive )	{ return; }
			if ( _ranState < SMTaskRunState.InitialEnable )	{ return; }

			_isUpdating.Value = true;
			_fixedUpdateEvent.Run();
			_isUpdating.Value = false;

			if ( _ranState == SMTaskRunState.InitialEnable )	{ _ranState = SMTaskRunState.FixedUpdate; }
		}


		public void Update() {
			if ( !_isOperable )	{ return; }
			if ( !_isActive )	{ return; }
			if ( _ranState < SMTaskRunState.FixedUpdate )	{ return; }

			_isUpdating.Value = true;
			_updateEvent.Run();
			_isUpdating.Value = false;

			if ( _ranState == SMTaskRunState.FixedUpdate )	{ _ranState = SMTaskRunState.Update; }
		}


		public void LateUpdate() {
			if ( !_isOperable )	{ return; }
			if ( !_isActive )		{ return; }
			if ( _ranState < SMTaskRunState.Update )	{ return; }

			_isUpdating.Value = true;
			_lateUpdateEvent.Run();
			_isUpdating.Value = false;

			if ( _ranState == SMTaskRunState.Update )	{ _ranState = SMTaskRunState.LateUpdate; }
		}


#if DEVELOP
		public void OnGUI() {
			_onGUIEvent.Run();
		}
#endif



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
				.Select( fsm => fsm.GetScene( fsm._body._startStateType ) )
				.ForEach( s => scenes.Remove( rs => rs.name == s._name ) );

			return scenes;
		}



		public IEnumerable<string> GetScenePathsInBuild()
			=> Enumerable.Range( 0, SceneManager.sceneCountInBuildSettings )
				.Select( i => SceneUtility.GetScenePathByBuildIndex( i ) );

		public bool IsExistSceneInBuild( string path, bool isNameOnly = false )
			=> GetScenePathsInBuild()
				.Select( p => {
					if ( isNameOnly )	{ return PathSMUtility.GetName( p ); }
					return p;
				} )
				.Any( p => p == path );



		public void MoveForeverScene( GameObject gameObject )
			=> SceneManager.MoveGameObjectToScene( gameObject, _foreverScene._rawScene );



		public IEnumerable<SMScene> GetScenes()
			=> _fsm.GetFSMs()
				.SelectMany( fsm => fsm.GetScenes() )
				.Distinct();

		public SMScene GetScene( Scene rawScene )
			=> GetScenes()
				.FirstOrDefault( s => s._rawScene == rawScene );
	}
}