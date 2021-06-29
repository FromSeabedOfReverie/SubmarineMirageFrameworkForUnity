//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestBase {
	using System;
	using System.Collections;
	using NUnit.Framework;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Base;
	using Utility;
	using Debug;



	public abstract class SMUnitTest : SMStandardBase {
		protected readonly SMAsyncCanceler _asyncCanceler = new SMAsyncCanceler();
		protected readonly SMStopwatch _stopwatch = new SMStopwatch();



		[OneTimeSetUp]
		protected void Awake() {
			SMLog.s_isEnable = true;
			_disposables.AddFirst( () => {
				_asyncCanceler.Dispose();
				_stopwatch.Dispose();
			} );
		}

		[OneTimeTearDown]
		protected void OnDestroy()
			=> Dispose();



		protected IEnumerator From( Func<UniTask> task ) => UTask.ToCoroutine( async () => {
			try {
				await task.Invoke();
			} catch ( OperationCanceledException ) {
			}
		} );

		public IEnumerator From( IEnumerator coroutine ) {
			var isRunning = true;
			_asyncCanceler._cancelEvent.AddLast().Subscribe( _ => isRunning = false );

			var disposable = Observable.FromCoroutine( () => coroutine )
				.DoOnError( e => {
					if ( !( e is OperationCanceledException ) ) {
						SMLog.Error( e );
					}
					isRunning = false;
				} )
				.DoOnCompleted( () => isRunning = false )
				.Subscribe();

			while ( isRunning ) { yield return null; }
			disposable.Dispose();
		}



		public IEnumerator RunForever() {
			while ( true ) { yield return null; }
		}
	}
}