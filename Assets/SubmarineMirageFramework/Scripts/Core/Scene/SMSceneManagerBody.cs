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
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;
	using Base;
	using Service;
	using MultiEvent;
	using Task;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMSceneManagerBody : SMStandardBase {
		public bool _isInitialized	{ get; set; }
		public bool _isOperable		=> _isInitialized && !_isFinalizing;
		public bool _isFinalizing	{ get; set; }
		public bool _isActive		{ get; set; }
		public readonly ReactiveProperty<bool> _isUpdating = new ReactiveProperty<bool>();

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
		[SMHide] public readonly SMTaskCanceler _asyncCancelerOnDispose	= new SMTaskCanceler();

		[SMHide] public SMSceneManager _owner	{ get; private set; }
		public SMSceneFSM _fsm	{ get; private set; }



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

// TODO : 将来的に、Initialize内部から呼び出す
		// SMScene内部で、SMServiceLocatorから自身を参照する為、Body生成後に、Sceneを遅延生成する
		public void Setup() {
			var setting = SMServiceLocator.Resolve<ISMSceneSetting>();
			var fsms = setting._fsmSceneTypes.ToDictionary(
				pair => pair.Key,
				pair => {
					var scenes = pair.Value.Select( t => (SMScene)t.Create() );
					switch ( pair.Key ) {
						case SMSceneType.Forever:
							return new SMSceneInternalFSM( scenes, typeof( ForeverSMScene ) );
						case SMSceneType.UI:
							return new SMSceneInternalFSM( scenes, typeof( UISMScene ) );
						case SMSceneType.Debug:
							return new SMSceneInternalFSM( scenes, typeof( DebugSMScene ) );
						case SMSceneType.Main:
							return new SMSceneInternalFSM( scenes, typeof( MainSMScene ) );
						default:
							return null;
					}
				}
			);
			if ( !setting._chunkSceneTypes.IsNullOrEmpty() ) {
				var scenes = setting._chunkSceneTypes.Select( t => (SMScene)t.Create() );
				fsms[SMSceneType.FieldChunk1]
					= new SMSceneInternalFSM( scenes, typeof( FieldChunkSMScene ), false );
				fsms[SMSceneType.FieldChunk2]
					= new SMSceneInternalFSM( scenes, typeof( FieldChunkSMScene ), false );
				fsms[SMSceneType.FieldChunk3]
					= new SMSceneInternalFSM( scenes, typeof( FieldChunkSMScene ), false );
				fsms[SMSceneType.FieldChunk4]
					= new SMSceneInternalFSM( scenes, typeof( FieldChunkSMScene ), false );
			}
			SMServiceLocator.Unregister<ISMSceneSetting>();

			_fsm = new SMSceneFSM( _owner, fsms );
		}



		public async UniTask Initialize() {
			return;

			await _selfInitializeEvent.Run( _asyncCancelerOnDispose );
			await _initializeEvent.Run( _asyncCancelerOnDispose );
			_enableEvent.Run();
			_isActive = true;
			_isInitialized = true;

			await _fsm._foreverFSM.InitialEnter();
			await _fsm.GetFSMs()
				.Where( fsm => fsm._fsmType != SMSceneType.Forever )
				.Select( fsm => fsm.InitialEnter() );
		}


		public async UniTask Finalize() {
			return;

			await _fsm.GetFSMs()
				.Where( fsm => fsm._fsmType != SMSceneType.Forever )
				.Select( fsm => fsm.FinalExit() );
			await _fsm.FinalExit();

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
	}
}