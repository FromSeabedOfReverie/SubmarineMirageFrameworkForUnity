//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Main {
	using System;
	using System.Threading.Tasks;
	using Cysharp.Threading.Tasks;
	using MultiEvent;
	using Task;
	using Singleton;
	using Scene;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class SubmarineMirage : SMRawSingleton<SubmarineMirage> {
		public readonly SMMultiAsyncEvent _createTestEvent = new SMMultiAsyncEvent();
		public bool _isRegisterCreateTestEvent	{ get; set; } = true;
		public bool _isInitialized	{ get; private set; }
		readonly SMTaskCanceler _canceler = new SMTaskCanceler();


		public SubmarineMirage() {
			_disposables.Add( () => {
				_canceler.Dispose();
				_createTestEvent.Dispose();
				SMTaskRunner.DisposeInstance();
				SMSceneManager.DisposeInstance();
				SMDecorationManager.DisposeInstance();
			} );
		}


// TODO : もっと良い開始名を考える
		public async UniTask TakeOff( Func<UniTask> initializePluginEvent, Func<UniTask> registerBehavioursEvent )
		{

			await Task.Delay( 1, _canceler.ToToken() );

			await initializePluginEvent();

			new SMLog();
			SMSceneManager.CreateInstance();
			SMMonoBehaviourSingletonManager.CreateInstance();

			await UTask.WaitWhile( _canceler, () => !_isRegisterCreateTestEvent );
			await _createTestEvent.Run( _canceler );

			await SMTaskRunner.s_instance.RunForeverTasks( () => registerBehavioursEvent() );

			_isInitialized = true;
		}
	}
}