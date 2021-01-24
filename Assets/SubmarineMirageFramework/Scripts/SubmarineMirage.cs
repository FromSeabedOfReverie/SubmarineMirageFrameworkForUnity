//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using Base;
	using Service;
	using MultiEvent;
	using Task;
	using Singleton;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using SystemTask = System.Threading.Tasks.Task;


	// TODO : コメント追加、整頓


	public class SubmarineMirageFramework : SMStandardBase, ISMService {
		public readonly SMMultiAsyncEvent _createTestEvent = new SMMultiAsyncEvent();
		public bool _isRegisterCreateTestEvent	{ get; set; } = true;
		public bool _isInitialized	{ get; private set; }
		readonly SMTaskCanceler _canceler = new SMTaskCanceler();


		public SubmarineMirageFramework() {
			SMServiceLocator.Register( this );

			_disposables.AddLast( () => {
				_canceler.Dispose();
				_createTestEvent.Dispose();
			} );
		}


// TODO : もっと良い開始名を考える
		public async UniTask TakeOff( Func<UniTask> initializePluginEvent, Func<UniTask> registerSettingsEvent,
										Func<UniTask> registerBehavioursEvent
		) {
			await SystemTask.Delay( 1, _canceler.ToToken() );


			_disposables.AddLast( Observable.OnceApplicationQuit().Subscribe( _ => Shutdown() ) );


			await initializePluginEvent();


			SMServiceLocator.Register( new SMDecorationManager() );
			new SMLog();


			await registerSettingsEvent();


			SMServiceLocator.Register( new SMSceneManager() );
			SMMonoBehaviourSingletonManager.CreateInstance();
			var runner = SMServiceLocator.RegisterRawBehaviour<SMTaskRunner>();


			await UTask.WaitWhile( _canceler, () => !_isRegisterCreateTestEvent );
			await _createTestEvent.Run( _canceler );


			await registerBehavioursEvent();

			await runner.Initialize();


			_isInitialized = true;
		}


		public static void Shutdown() => SMServiceLocator.Dispose();
	}
}