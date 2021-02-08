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
	using SubmarineMirage.Base;
	using Service;
	using Event;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMSceneManagerBody : SMStandardBase {
		public SMSceneManager _owner	{ get; private set; }
		[SMShow] public SMSceneFSM _fsm	{ get; private set; }

		public SMSceneFSM _foreverFSM	{ get; private set; }
		public SMSceneFSM _mainFSM		{ get; private set; }
		public SMSceneFSM _uiFSM		{ get; private set; }
		public SMSceneFSM _debugFSM	{ get; private set; }

		public SMScene _foreverScene	{ get; private set; }
		Scene _rawSceneUntilDispose	{ get; set; }

		[SMShow] List<Scene> _firstLoadedRawScenes	{ get; set; } = new List<Scene>();

		[SMShow] public bool _isInitialized	{ get; set; }
		[SMShow] public bool _isOperable		=> _isInitialized && !_isFinalizing;
		[SMShow] public bool _isFinalizing	{ get; set; }
		[SMShow] public bool _isActive		{ get; set; }
		public readonly ReactiveProperty<bool> _isUpdating = new ReactiveProperty<bool>();
		bool _isCalledFinalize	{ get; set; }

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



		public SMSceneManagerBody( SMSceneManager owner ) {
			_owner = owner;
			_rawSceneUntilDispose = SceneManager.CreateScene( "UntilDispose" );


			_disposables.AddLast( () => {
				_isActive = false;
				_isFinalizing = true;

				_isUpdating.Dispose();
				_fsm.Dispose();

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



		public async UniTask Initialize() {
			Setup();
//			return;

			await _selfInitializeEvent.Run( _asyncCancelerOnDispose );
			await _initializeEvent.Run( _asyncCancelerOnDispose );
			_enableEvent.Run();
			_isActive = true;
			_isInitialized = true;

			await _foreverFSM.InitialEnter( true );
			await _fsm.InitialEnter();
		}

		// SMScene内部で、SMServiceLocatorから自身を参照する為、Body生成後に、Sceneを遅延生成する
		void Setup() {
			var setting = SMServiceLocator.Resolve<ISMSceneSetting>();
			_fsm = SMSceneFSM.Generate( _owner, setting._sceneFSMList );
			SMServiceLocator.Unregister<ISMSceneSetting>();

			_foreverFSM = _fsm.GetFSM<ForeverSMScene>();
			_mainFSM = _fsm.GetFSM<MainSMScene>();
			_uiFSM = _fsm.GetFSM<UISMScene>();
			_debugFSM = _fsm.GetFSM<DebugSMScene>();
			_foreverScene = _foreverFSM.GetScenes().FirstOrDefault();

			_firstLoadedRawScenes = GetLoadedRawScenes().ToList();

			_fsm.GetFSMs().ForEach( fsm => {
				fsm._body._startStateType = fsm.GetScenes()
					.FirstOrDefault( s => IsFirstLoaded( s ) )
					?.GetType();
//				SMLog.Debug( fsm._body._startStateType?.GetAboutName() );
			} );
			// 不明なシーンを設定
			var scene = _mainFSM.GetScene<UnknownMainSMScene>();
			scene.Setup();
		}


		public async UniTask Finalize() {
			if ( _isCalledFinalize )	{ return; }
			_isCalledFinalize = true;
//			SMLog.Debug( "Finalize実行中" );

			await UTask.WaitWhile( _asyncCancelerOnDispose, () => !_isInitialized );

			await _fsm.GetFSMs()
				.Where( fsm => fsm != _foreverFSM )
				.Select( fsm => fsm.FinalExit( true ) );
			await _foreverFSM.FinalExit( true );

			_isFinalizing = true;
			_disableEvent.Run();
			_isActive = false;
			await _finalizeEvent.Run( _asyncCancelerOnDispose );

			SubmarineMirageFramework.Shutdown();
		}



		public void FixedUpdate() {
			return;
			if ( _isDispose )	{ return; }
			if ( _isUpdating.Value ) {
				throw new InvalidOperationException( $"更新中に更新され、被った : {nameof( FixedUpdate )}" );
			}

			_isUpdating.Value = true;
// TODO : Foreverが先に実行されるか、確認する
			_fixedUpdateEvent.Run();
			_isUpdating.Value = false;
		}

		public void Update() {
			return;
			if ( _isDispose )	{ return; }
			if ( _isUpdating.Value ) {
				throw new InvalidOperationException( $"更新中に更新され、被った : {nameof( Update )}" );
			}

			_isUpdating.Value = true;
// TODO : Foreverが先に実行されるか、確認する
			_updateEvent.Run();
			_isUpdating.Value = false;
		}

		public void LateUpdate() {
			return;
			if ( _isDispose )	{ return; }
			if ( _isUpdating.Value ) {
				throw new InvalidOperationException( $"更新中に更新され、被った : {nameof( LateUpdate )}" );
			}

			_isUpdating.Value = true;
// TODO : Foreverが先に実行されるか、確認する
			_lateUpdateEvent.Run();
			_isUpdating.Value = false;
		}

#if DEVELOP
		public void OnGUI() {
			return;
			if ( _isDispose )	{ return; }
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

		public IEnumerable<Scene> GetUnknownScenes() {
			var scenes = _firstLoadedRawScenes.Copy();

			_fsm.GetFSMs()
				.Where( fsm => fsm._body._startStateType != null )
				.Select( fsm => fsm.GetScene( fsm._body._startStateType ) )
				.ForEach( s => scenes.Remove( rs => rs.name == s._name ) );

			return scenes;
		}



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