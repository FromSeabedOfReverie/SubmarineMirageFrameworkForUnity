//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using Base;
	using Service;
	using Task;
	using Extension;
	using Utility;
	using Debug;
	using SystemTask = System.Threading.Tasks.Task;



	public class SubmarineMirageFramework : SMStandardBase, ISMService {
		[SMShow] public bool _isInitialized	{ get; private set; }
		readonly SMAsyncCanceler _asyncCanceler = new SMAsyncCanceler();



		public SubmarineMirageFramework() {
			SMLog.s_isEnable = SMDebugManager.IS_DEVELOP;   // 真っ先に指定しないと、ログが出ない

			_disposables.AddLast( () => {
				_isInitialized = false;
				_asyncCanceler.Dispose();
			} );
		}

		public static void Shutdown( bool isApplicationQuit = true ) {
			try {
				SMServiceLocator.Dispose();

				if ( !isApplicationQuit )	{ return; }

				if ( SMDebugManager.s_isPlayTest ) { return; }

				if ( SMDebugManager.IS_UNITY_EDITOR ) {
#if UNITY_EDITOR
					UnityEditor.EditorApplication.isPlaying = false;
#endif
				} else {
					Application.Quit();
				}

			} catch ( OperationCanceledException ) {
			} catch ( Exception e ) {
				SMLog.Error( e );
				throw;
			}
		}



// TODO : もっと良い開始名を考える
		public async UniTask TakeOff( Func<UniTask> initializePluginEvent, Func<UniTask> registerSettingsEvent )
		{
			try {
				await Initialize( initializePluginEvent, registerSettingsEvent );
			} catch ( OperationCanceledException ) {
			} catch ( Exception e ) {
				SMLog.Error( e );
				throw;
			}
		}

		async UniTask Initialize( Func<UniTask> initializePluginEvent, Func<UniTask> registerSettingsEvent )
		{
			await SystemTask.Delay( 1, _asyncCanceler.ToToken() );

			_disposables.AddLast( Observable.OnceApplicationQuit().Subscribe( _ => Shutdown( false ) ) );

			await initializePluginEvent();

			var taskManager = SMServiceLocator.Register( new SMTaskManager() );
			taskManager.Create();
			SMServiceLocator.Register( new SMDecorationManager() );
			SMServiceLocator.Register( new SMDebugManager() );
//			SMServiceLocator.Register( new SMTagManager() ) ;
//			SMServiceLocator.Register( new SMLayerManager() );
//			SMServiceLocator.Register( new SMTimeManager() );

			await registerSettingsEvent();

//			SMServiceLocator.Register( new SMSceneManager() );
//			SMServiceLocator.Register( new SMDisplayLog() );
			await taskManager.Initialize();

			if ( SMDebugManager.s_isPlayTest ) {
				var test = await SMServiceLocator.WaitResolve<IBaseSMTest>( _asyncCanceler );
				await test.Initialize();
			}

			_isInitialized = true;
		}
	}
}