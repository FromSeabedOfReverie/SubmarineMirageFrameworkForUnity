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
	using MultiEvent;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMSceneManagerBody : SMStandardBase {
		[SMHide] public SMSceneManager _owner	{ get; private set; }
		public SMSceneFSM _fsm	{ get; private set; }

		[SMHide] public SMSceneFSM _foreverFSM	{ get; private set; }
		[SMHide] public SMSceneFSM _mainFSM		{ get; private set; }
		[SMHide] public SMSceneFSM _uiFSM		{ get; private set; }
		[SMHide] public SMSceneFSM _debugFSM	{ get; private set; }
		[SMHide] public SMScene _foreverScene	{ get; private set; }

		List<Scene> _firstLoadedRawScenes	{ get; set; }

		public bool _isInitialized	{ get; set; }
		public bool _isOperable		=> _isInitialized && !_isFinalizing;
		public bool _isFinalizing	{ get; set; }
		public bool _isActive		{ get; set; }
		[SMHide] public readonly ReactiveProperty<bool> _isUpdating = new ReactiveProperty<bool>();

		[SMHide] public readonly SMMultiAsyncEvent _selfInitializeEvent	= new SMMultiAsyncEvent();
		[SMHide] public readonly SMMultiAsyncEvent _initializeEvent		= new SMMultiAsyncEvent();
		[SMHide] public readonly SMMultiSubject _enableEvent			= new SMMultiSubject();
		[SMHide] public readonly SMMultiSubject _fixedUpdateEvent		= new SMMultiSubject();
		[SMHide] public readonly SMMultiSubject _updateEvent			= new SMMultiSubject();
		[SMHide] public readonly SMMultiSubject _lateUpdateEvent		= new SMMultiSubject();
		[SMHide] public readonly SMMultiSubject _disableEvent			= new SMMultiSubject();
		[SMHide] public readonly SMMultiAsyncEvent _finalizeEvent		= new SMMultiAsyncEvent();
#if DEVELOP
		[SMHide] public readonly SMMultiSubject _onGUIEvent = new SMMultiSubject();
#endif
		[SMHide] public readonly SMAsyncCanceler _asyncCancelerOnDispose	= new SMAsyncCanceler();



		public SMSceneManagerBody( SMSceneManager owner ) {
			_owner = owner;


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
			return;

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
		}


		public async UniTask Finalize() {
			return;

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
				.Select( i => SceneManager.GetSceneAt( i ) );

		public bool RemoveFirstLoaded( SMScene scene ) {
			var count = _firstLoadedRawScenes
				.RemoveAll( s => s.name == scene._name );
			return count > 0;
		}

		public bool IsFirstLoaded( SMScene scene )
			=> _firstLoadedRawScenes
				.Any( s => s.name == scene._name );



		public void MoveForeverScene( GameObject gameObject )
			=> SceneManager.MoveGameObjectToScene( gameObject, _foreverScene._rawScene );



		public IEnumerable<SMScene> GetScenes()
			=> _fsm.GetFSMs()
				.SelectMany( fsm => fsm.GetScenes() )
				.Distinct();

		public SMScene GetScene( Scene rawScene )
			=> GetScenes()
				.FirstOrDefault( s => s._rawScene == rawScene );



		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _firstLoadedRawScenes ),
				i => _toStringer.DefaultValue( _firstLoadedRawScenes.Select( s => s.name ), i, false )
			);
			_toStringer.Add( nameof( _isUpdating ), i => $"{_isUpdating.Value}" );
		}
	}
}