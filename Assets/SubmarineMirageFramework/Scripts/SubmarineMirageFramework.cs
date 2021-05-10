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
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using SystemTask = System.Threading.Tasks.Task;



	public class SubmarineMirageFramework : SMStandardBase, ISMService {
		[SMShow] public static bool s_isPlayTest	{ get; set; }
		[SMShow] public bool _isInitialized	{ get; private set; }
		readonly SMAsyncCanceler _asyncCanceler = new SMAsyncCanceler();


		public SubmarineMirageFramework() {
			_disposables.AddLast( () => {
				_isInitialized = false;
				_asyncCanceler.Dispose();
			} );
		}

		public static void Shutdown( bool isApplicationQuit = true ) {
			try {
				SMServiceLocator.Dispose();

				if ( !isApplicationQuit )	{ return; }
#if UNITY_EDITOR
				if ( s_isPlayTest )	{ return; }
				UnityEditor.EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
			} catch ( OperationCanceledException ) {
			} catch ( Exception e ) {
				SMLog.Error( e );
				throw;
			}
		}


// TODO : もっと良い開始名を考える
		public async UniTask TakeOff( Func<UniTask> initializePluginEvent, Func<UniTask> registerSettingsEvent ) {
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
			DebugSetter.Initialize();

			await SystemTask.Delay( 1, _asyncCanceler.ToToken() );

			_disposables.AddLast( Observable.OnceApplicationQuit().Subscribe( _ => Shutdown( false ) ) );

			await initializePluginEvent();
			SMServiceLocator.Register<SMDecorationManager>();
			SMServiceLocator.Register<SMLog>();
			SMServiceLocator.Register<MainThreadDispatcherSMExtension>();
//			SMServiceLocator.Register<SMTagManager>();
//			SMServiceLocator.Register<SMLayerManager>();
//			SMServiceLocator.Register<SMTimeManager>();

			await registerSettingsEvent();

			var scene = SMServiceLocator.Register<SMSceneManager>();
			await scene._body.Initialize();
			SMServiceLocator.Register<DebugDisplay>();

			if ( s_isPlayTest ) {
				var test = await SMServiceLocator.WaitResolve<IBaseSMTest>( _asyncCanceler );
				await test.Initialize();
			}

			_isInitialized = true;
		}



		public UniTask WaitInitialize()
			=> UTask.WaitWhile( _asyncCanceler, () => !_isInitialized );
	}
}