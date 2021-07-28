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
	using File;
	using Data;
	using Extension;
	using Utility;
	using Setting;
	using Debug;
	using SystemTask = System.Threading.Tasks.Task;



	public class SubmarineMirageFramework : SMStandardBase, ISMService {
		[SMShow] public bool _isInitialized	{ get; private set; }
		readonly SMAsyncCanceler _asyncCanceler = new SMAsyncCanceler();



		public SubmarineMirageFramework() {
			SMLog.s_isEnable = SMDebugManager.IS_DEVELOP;	// 真っ先に指定しないと、ログが出ない

			_disposables.AddFirst( () => {
				_isInitialized = false;
				_asyncCanceler.Dispose();
			} );
		}

		public static void Shutdown( bool isApplicationQuit = true ) {
			try {
				SMServiceLocator.Dispose();

				if ( !isApplicationQuit )			{ return; }
				if ( SMDebugManager.s_isPlayTest )	{ return; }

				if ( SMDebugManager.IS_UNITY_EDITOR ) {
					UnityEditor.EditorApplication.isPlaying = false;
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

			_disposables.AddFirst( Observable.OnceApplicationQuit().Subscribe( _ => Shutdown( false ) ) );

			await initializePluginEvent();

			var taskManager = SMServiceLocator.Register( new SMTaskManager() );
			SMServiceLocator.Register( new SMMainSetting() );
			SMServiceLocator.Register( new SMDecorationManager() );
			SMServiceLocator.Register( new SMDebugManager() );
			SMServiceLocator.Register( new SMDisplayLog() );
			SMServiceLocator.Register( new SMFileManager() );
			SMServiceLocator.Register( new SMAllDataManager() );
			SMServiceLocator.Register( new SMCoroutineManager() );
			SMServiceLocator.Register( new SMInputManager() );
			SMServiceLocator.Register( new SMUnityTagManager() ) ;
			SMServiceLocator.Register( new SMUnityLayerManager() );
			SMServiceLocator.Register( new SMTimeManager() );
			SMServiceLocator.Register( new SMNetworkManager() );

			await registerSettingsEvent();

			new SMSplashScreenWaiter();
//			SMServiceLocator.Register( new SMSceneManager() );
			await taskManager.Initialize();

			if ( SMDebugManager.s_isPlayTest ) {
				var test = await SMServiceLocator.WaitResolve<IBaseSMTest>( _asyncCanceler );
				await test.Initialize();
			}

			_isInitialized = true;
		}
	}
}