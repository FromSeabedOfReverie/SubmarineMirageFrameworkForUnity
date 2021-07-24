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
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Base;
	using Utility;
	using Debug;



	public abstract class SMUnitTest : SMStandardBase {
		protected readonly SMStopwatch _stopwatch = new SMStopwatch();



		[OneTimeSetUp]
		protected void Awake() {
			SMLog.s_isEnable = true;

			_disposables.AddFirst(
				Observable.EveryUpdate()
					.Where( _ => !LogAssert.ignoreFailingMessages )
					.Subscribe( _ => LogAssert.ignoreFailingMessages = true )
			);
			_disposables.AddFirst( () => {
				_stopwatch.Dispose();
			} );

			Create();
		}

		protected abstract void Create();

		[OneTimeTearDown]
		protected void OnDestroy()
			=> Dispose();



		protected IEnumerator From( Func<UniTask> task ) => UTask.ToCoroutine( async () => {
			try {
				SMLog.Warning( "Start" );
				await task.Invoke();
				SMLog.Warning( "End" );
			} catch ( OperationCanceledException ) {
			} catch ( Exception e ) {
				SMLog.Error( e );
				throw;
			}
		} );

		public IEnumerator From( IEnumerator coroutine ) {
			var isRunning = true;

			SMLog.Warning( "Start" );
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
			SMLog.Warning( "End" );
		}



		public IEnumerator RunForever() {
			while ( true ) { yield return null; }
		}
	}
}