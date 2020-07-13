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
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Main.New;
	using UTask;
	using Editor;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class BaseTest : IPrebuildSetup, IDisposable {
		protected string _testName => TestContext.CurrentContext.Test.Name;
		protected bool _isInitialized;
		protected CancellationTokenSource _asyncCanceler = new CancellationTokenSource();
		protected CancellationToken _asyncCancel => _asyncCanceler.Token;


		public void Setup() {
			ConsoleEditorUtility.Clear();
			SubmarineMirage.DisposeInstance();
			TestManager.DisposeInstance();
			PlayerExtensionEditorManager.instance._playType = PlayerExtensionEditor.PlayType.Test;
		}

		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
		static void Main() {
			if ( PlayerExtensionEditorManager.instance._playType == PlayerExtensionEditor.PlayType.Test ) {
				SubmarineMirage.s_instance._isRegisterCreateTestEvent = false;
			}
		}

		[OneTimeSetUp]
		protected void Awake() {
			SubmarineMirage.s_instance._disposables.AddLast( () => TestManager.DisposeInstance() );
			TestManager.s_instance.Register( this );
			SubmarineMirage.s_instance._createTestEvent.AddLast( async cancel => {
				try {
					await AwakeSub();
				} catch ( OperationCanceledException ) {
					throw;
				} catch ( Exception e ) {
					Log.Error( e );
					throw;
				}
			} );
			SubmarineMirage.s_instance._isRegisterCreateTestEvent = true;
		}

		protected abstract UniTask AwakeSub();

		protected abstract void Create();


		[OneTimeTearDown]
		protected void OnDestroy() {
			try {
				Dispose();
				TestManager.s_instance.Unregister( this );
			} catch ( OperationCanceledException ) {
				throw;
			} catch ( Exception e ) {
				Log.Error( e );
				throw;
			}
		}

		public abstract void Dispose();


		protected IEnumerator From( Func<UniTask> task ) => UTask.ToCoroutine( async () => {
			await UTask.WaitWhile( _asyncCancel, () => !_isInitialized );
			try {
				await task.Invoke();
			} catch ( OperationCanceledException ) {
				throw;
			} catch ( Exception e ) {
				Log.Error( e );
				throw;
			}
		} );

		protected IEnumerator From( IEnumerator coroutine ) {
			while ( !_isInitialized )	{ yield return null; }

			var isRunning = true;
			_asyncCancel.Register( () => isRunning = false );

			var disposable = Observable.FromCoroutine( () => coroutine )
				.DoOnError( e => {
					if ( !( e is OperationCanceledException ) ) {
						Log.Error( e );
					}
					isRunning = false;
				} )
				.DoOnCompleted( () => isRunning = false )
				.Subscribe();

			while ( isRunning )	{ yield return null; }
			disposable.Dispose();
		}

		protected IEnumerator RunForever() {
			while ( true )	{ yield return null; }
		}
	}
}