//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System;
	using System.Collections;
	using NUnit.Framework;
	using UniRx.Async;


	// TODO : コメント追加、整頓


	public abstract class BaseTest : IDisposable {
		[OneTimeSetUp]
		protected virtual void Awake() => TestManager.Register( this );

		protected abstract void Create();


		[OneTimeTearDown]
		protected virtual void OnDestroy() {
			Dispose();
			TestManager.UnRegister( this );
		}

		public abstract void Dispose();

		~BaseTest() => Dispose();


		protected IEnumerator From( Func<UniTask> task ) => UniTask.ToCoroutine( async () => {
			try										{ await task.Invoke(); }
			catch ( OperationCanceledException )	{}
			catch ( Exception e ) { Debug.Log.Error( e ); }
		} );
	}
}