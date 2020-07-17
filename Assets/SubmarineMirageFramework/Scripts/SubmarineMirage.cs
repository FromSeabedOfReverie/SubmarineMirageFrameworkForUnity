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
	using UTask;
	using SMTask;
	using Singleton;
	using Scene;


	// TODO : コメント追加、整頓


	public class SubmarineMirage : RawSingleton<SubmarineMirage> {
		public readonly MultiAsyncEvent _createTestEvent = new MultiAsyncEvent();
		public bool _isRegisterCreateTestEvent = true;

// TODO : 戻す
//		public bool _isInitialized	{ get; private set; }
// TODO : 消す
		public bool _isInitialized	{ get; set; }

		UTaskCanceler _canceler = new UTaskCanceler();


		public SubmarineMirage() {
			_disposables.AddLast( _canceler );
			_disposables.AddLast( _createTestEvent );
			_disposables.AddLast( () => {
				SMTaskRunner.DisposeInstance();
				SceneManager.DisposeInstance();
			} );
		}


// TODO : もっと良い開始名を考える
		public async UniTask TakeOff( Func<UniTask> initializePluginEvent, Func<UniTask> registerBehavioursEvent )
		{

			await Task.Delay( 1, _canceler.ToToken() );

			await initializePluginEvent();

			SceneManager.CreateInstance();
			MonoBehaviourSingletonManager.CreateInstance();
			SMTaskRunner.CreateInstance();

			await UTask.WaitWhile( _canceler, () => !_isRegisterCreateTestEvent );
			await _createTestEvent.Run( _canceler );

			await SMTaskRunner.s_instance.Create( () => registerBehavioursEvent() );

			_isInitialized = true;
		}
	}
}