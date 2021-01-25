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
	using Singleton;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using SystemTask = System.Threading.Tasks.Task;



	// TODO : コメント追加、整頓



	public class SubmarineMirageFramework : SMStandardBase, ISMService {
		public enum PlayType {
			Stop,
			Runtime,
			Editor,
			Test,
		}

		public static PlayType s_playType	{ get; set; }
		public bool _isInitialized	{ get; private set; }
		readonly SMTaskCanceler _canceler = new SMTaskCanceler();


		public SubmarineMirageFramework() {
			SMServiceLocator.Register( this );

			_disposables.AddLast( () => {
				s_playType = PlayType.Stop;
				_isInitialized = false;
				_canceler.Dispose();
			} );
		}

		public static void Shutdown() => SMServiceLocator.Dispose();



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


			if ( s_playType == PlayType.Test ) {
				var test = await SMServiceLocator.WaitResolve<IBaseSMTest>( _canceler );
				await test.AwakeTop();
			}


			await registerBehavioursEvent();

			await runner.Initialize();


			_isInitialized = true;
		}



		public UniTask WaitInitialize()
			=> UTask.WaitWhile( _canceler, () => !_isInitialized );
	}
}