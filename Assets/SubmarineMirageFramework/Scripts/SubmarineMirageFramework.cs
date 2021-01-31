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
	using Task;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using SystemTask = System.Threading.Tasks.Task;



	// TODO : コメント追加、整頓



	public class SubmarineMirageFramework : SMStandardBase, ISMService {
		public static bool s_isPlayTest	{ get; set; }
		public bool _isInitialized	{ get; private set; }
		[SMHide] readonly SMTaskCanceler _asyncCanceler = new SMTaskCanceler();


		public SubmarineMirageFramework() {
			_disposables.AddLast( () => {
				s_isPlayTest = false;
				_isInitialized = false;
				_asyncCanceler.Dispose();
			} );
		}

		public static void Shutdown() {
			try {
				SMServiceLocator.Dispose();
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

			_disposables.AddLast( Observable.OnceApplicationQuit().Subscribe( _ => Shutdown() ) );

			await initializePluginEvent();
			SMServiceLocator.Register<SMIDCounter>();
			SMServiceLocator.Register<SMDecorationManager>();
			SMServiceLocator.Register<SMLog>();

			await registerSettingsEvent();

			var hoge = SMServiceLocator.Resolve<ISMSceneSetting>();
			SMLog.Debug( hoge );
			SMLog.Debug( hoge.ToLineString() );

			var scene = SMServiceLocator.Register<SMSceneManager>();
			scene._body.Setup();
//			SMMonoBehaviourSingletonManager.CreateInstance();

			await scene._body.Initialize();

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