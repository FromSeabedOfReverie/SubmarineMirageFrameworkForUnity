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
		public enum PlayType {
			Stop,
			Runtime,
			Editor,
			Test,
		}

		public static PlayType s_playTypeBody;
		public static PlayType s_playType {
			get => s_playTypeBody;
			set {
				s_playTypeBody = value;
				UnityEngine.Debug.Log( $"{nameof( s_playType )} : {s_playTypeBody}" );
			}
		}
		public bool _isInitialized	{ get; private set; }
		readonly SMTaskCanceler _asyncCanceler = new SMTaskCanceler();


		public SubmarineMirageFramework() {
			_disposables.AddLast( () => {
				s_playType = PlayType.Stop;
				_isInitialized = false;
				_asyncCanceler.Dispose();
			} );
		}

		public static void Shutdown() => SMServiceLocator.Dispose();



// TODO : もっと良い開始名を考える
		public async UniTask TakeOff( Func<UniTask> initializePluginEvent, Func<UniTask> registerSettingsEvent ) {
			await SystemTask.Delay( 1, _asyncCanceler.ToToken() );


			_disposables.AddLast( Observable.OnceApplicationQuit().Subscribe( _ => Shutdown() ) );

			await initializePluginEvent();
			SMServiceLocator.Register<BaseSMManager>();
			SMServiceLocator.Register<SMDecorationManager>();
			new SMLog();

			await registerSettingsEvent();
			var scene = SMServiceLocator.Register<SMSceneManager>();
			scene = SMServiceLocator.Resolve<SMSceneManager>();
			scene.Setup();
//			SMMonoBehaviourSingletonManager.CreateInstance();

			if ( s_playType == PlayType.Test ) {
				var test = await SMServiceLocator.WaitResolve<IBaseSMTest>( _asyncCanceler );
				await test.AwakeTop();
			}

			await scene._body.Initialize();
			_isInitialized = true;
		}



		public UniTask WaitInitialize()
			=> UTask.WaitWhile( _asyncCanceler, () => !_isInitialized );
	}
}