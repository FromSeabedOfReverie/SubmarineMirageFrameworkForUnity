//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System;
	using System.Threading;
	using System.Collections;
	using NUnit.Framework;
	using UniRx;
	using UniRx.Async;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class BaseTest : IDisposable {
		protected bool _isInitialized;
		protected CancellationTokenSource _asyncCanceler = new CancellationTokenSource();
		protected CancellationToken _asyncCancel => _asyncCanceler.Token;

		[OneTimeSetUp]
		protected void Awake() {
			TestManager.Register( this );
			AwakeSub().Forget();
		}

		protected abstract UniTask AwakeSub();

		protected abstract void Create();


		[OneTimeTearDown]
		protected virtual void OnDestroy() {
			Dispose();
			TestManager.UnRegister( this );
		}

		public abstract void Dispose();

		~BaseTest() => Dispose();


		protected IEnumerator From( Func<UniTask> task ) => UniTask.ToCoroutine( async () => {
			await UniTaskUtility.WaitWhile( _asyncCancel, () => !_isInitialized );
			try										{ await task.Invoke(); }
			catch ( OperationCanceledException )	{}
			catch ( Exception e )					{ Log.Error( e ); }
		} );

		protected IEnumerator From( IEnumerator coroutine ) {
			while ( !_isInitialized )	{ yield return null; }
			var isWait = true;

			var disposable = Observable.FromCoroutine( () => coroutine )
				.DoOnError( e => {
					if ( e is OperationCanceledException )	{}
					else									{ Log.Error( e ); }
					isWait = false;
				} )
				.DoOnCompleted( () => isWait = false )
				.Subscribe();

			while ( isWait )	{ yield return null; }
			disposable.Dispose();
		}
	}
}