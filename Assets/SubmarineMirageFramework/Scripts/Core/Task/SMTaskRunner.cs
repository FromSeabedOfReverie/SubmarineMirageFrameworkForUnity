//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestTaskRunner
namespace SubmarineMirage.Task {
	using System;
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;
	using Base;
	using Service;
	using MultiEvent;
	using Behaviour.Modifyler;
	using Group.Manager.Modifyler;
	using Scene;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMTaskRunner : MonoBehaviourSMExtension, ISMRawBase, ISMService {
		[SMHide] public CompositeDisposable _disposables	{ get; private set; } = new CompositeDisposable();
		public bool _isDispose => _disposables.IsDisposed;

		SMSceneManager _sceneManager	{ get; set; }
		public readonly ReactiveProperty<bool> _isUpdating = new ReactiveProperty<bool>();
#if DEVELOP
		public readonly SMMultiSubject _onGUIEvent = new SMMultiSubject();
#endif



		protected override void Awake() {
			base.Awake();
			_sceneManager = SMServiceLocator.Resolve<SMSceneManager>();

			_disposables.Add( () => {
				_isUpdating.Dispose();
#if DEVELOP
				_onGUIEvent.Dispose();
#endif
				Destroy( gameObject );
			} );
		}

		public override void Dispose() => _disposables.Dispose();



		public async UniTask Initialize() {
			await _sceneManager._modifyler.RegisterAndRun( new CreateSMBehaviour() );
			await _sceneManager._modifyler.RegisterAndRun( new SelfInitializeSMBehaviour() );
			await _sceneManager._modifyler.RegisterAndRun( new InitializeSMBehaviour() );
			await _sceneManager._modifyler.RegisterAndRun( new InitialEnableSMBehaviour( true ) );

			await _sceneManager._fsm._foreverFSM.InitialEnter();
			await _sceneManager._fsm.GetFSMs()
				.Where( fsm => fsm._fsmType != SMSceneType.Forever )
				.Select( fsm => fsm.InitialEnter() );
		}


		public async UniTask Finalize() {
			await _sceneManager._fsm.GetFSMs()
				.Where( fsm => fsm._fsmType != SMSceneType.Forever )
				.Select( fsm => fsm.FinalExit() );
			await _sceneManager._fsm.FinalExit();

			await _sceneManager._modifyler.RegisterAndRun( new FinalDisableSMBehaviour( true ) );
			await _sceneManager._modifyler.RegisterAndRun( new FinalizeSMBehaviour() );
			SubmarineMirageFramework.Shutdown();
		}


		void FixedUpdate() {
			return;
			if ( _isDispose )	{ return; }
			if ( _isUpdating.Value ) {
				throw new InvalidOperationException( $"更新中に更新され、被った : {nameof( FixedUpdate )}" );
			}

			_isUpdating.Value = true;
// TODO : Foreverが先に実行されるか、確認する
			_sceneManager._fsm.GetFSMs().ForEach( fsm =>
				SMGroupManagerApplyer.FixedUpdate( fsm._state?._groups ) );
			_isUpdating.Value = false;
		}


		void Update() {
			return;
			if ( _isDispose )	{ return; }
			if ( _isUpdating.Value ) {
				throw new InvalidOperationException( $"更新中に更新され、被った : {nameof( Update )}" );
			}

			_isUpdating.Value = true;
// TODO : Foreverが先に実行されるか、確認する
			_sceneManager._fsm.GetFSMs().ForEach( fsm =>
				SMGroupManagerApplyer.Update( fsm._state?._groups ) );
			_isUpdating.Value = false;
		}


		void LateUpdate() {
			return;
			if ( _isDispose )	{ return; }
			if ( _isUpdating.Value ) {
				throw new InvalidOperationException( $"更新中に更新され、被った : {nameof( LateUpdate )}" );
			}

			_isUpdating.Value = true;
// TODO : Foreverが先に実行されるか、確認する
			_sceneManager._fsm.GetFSMs().ForEach( fsm =>
				SMGroupManagerApplyer.LateUpdate( fsm._state?._groups ) );
			_isUpdating.Value = false;
		}


#if DEVELOP
		void OnGUI() {
			if ( _isDispose )	{ return; }
			_onGUIEvent.Run();
		}
#endif
	}
}