//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Main.New {
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using UniRx.Async;
	using SMTask;
	using Singleton.New;
	using MultiEvent;
	using Scene;
	using Utility;


	// TODO : コメント追加、整頓


	public class SubmarineMirage : RawSingleton<SubmarineMirage> {
		public readonly MultiAsyncEvent _createTestEvent = new MultiAsyncEvent();
		public bool _isRegisterCreateTestEvent = true;
		public bool _isInitialized	{ get; private set; }
		CancellationTokenSource _canceler = new CancellationTokenSource();


		public SubmarineMirage() {
			_disposables.AddLast( () => {
				_canceler.Cancel();
				_canceler.Dispose();
			} );
			_disposables.AddLast( _createTestEvent );
			_disposables.AddLast( () => {
				SMTaskRunner.DisposeInstance();
				SceneManager.DisposeInstance();
			} );
		}


// TODO : もっと良い開始名を考える
		public async UniTask TakeOff( Func<UniTask> initializePluginEvent, Func<UniTask> registerBehavioursEvent )
		{
			await Task.Delay( 1, _canceler.Token );

			await initializePluginEvent();

			SceneManager.CreateInstance();
			MonoBehaviourSingletonManager.CreateInstance();
			SMTaskRunner.CreateInstance();

			await UniTaskUtility.WaitWhile( _canceler.Token, () => !_isRegisterCreateTestEvent );
			await _createTestEvent.Run( _canceler.Token );

			await SMTaskRunner.s_instance.Create( () => registerBehavioursEvent() );

			_isInitialized = true;
		}
	}
}