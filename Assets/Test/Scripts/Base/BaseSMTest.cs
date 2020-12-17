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
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Main;
	using Base;
	using Task;
	using Extension;
	using Utility;
	using Debug;
	using EditorExtension;
	using EditorUtility;


	// TODO : コメント追加、整頓


	public abstract class BaseSMTest : BaseSM, IPrebuildSetup {
		protected string _testName => TestContext.CurrentContext.Test.Name;
		protected bool _isInitialized	{ get; set; }
		[SMHide] protected readonly SMTaskCanceler _asyncCanceler = new SMTaskCanceler();


		public void Setup() {
			ConsoleEditorSMUtility.Clear();
			SubmarineMirage.DisposeInstance();
			SMTestManager.DisposeInstance();
			PlayerEditorSMExtension.s_instance._playType = PlayerEditorSMExtension.PlayType.Test;
		}

		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
		static void Main() {
			if ( PlayerEditorSMExtension.s_instance._playType == PlayerEditorSMExtension.PlayType.Test ) {
				SubmarineMirage.s_instance._isRegisterCreateTestEvent = false;
			}
		}

		[OneTimeSetUp]
		protected void Awake() {
			SubmarineMirage.s_instance._disposables.Add( () => SMTestManager.DisposeInstance() );
			SMTestManager.s_instance.Register( this );
			SubmarineMirage.s_instance._createTestEvent.AddLast( async canceler => {
				try {
					await AwakeSub();
				} catch ( OperationCanceledException ) {
					throw;
				} catch ( Exception e ) {
					SMLog.Error( e );
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
				SMTestManager.s_instance.Unregister( this );
			} catch ( OperationCanceledException ) {
				throw;
			} catch ( Exception e ) {
				SMLog.Error( e );
				throw;
			}
		}


		protected IEnumerator From( Func<UniTask> task ) => UTask.ToCoroutine( async () => {
			await UTask.WaitWhile( _asyncCanceler, () => !_isInitialized );
			try {
				await task.Invoke();
			} catch ( OperationCanceledException ) {
				throw;
			} catch ( Exception e ) {
				SMLog.Error( e );
				throw;
			}
		} );

		protected IEnumerator From( IEnumerator coroutine ) {
			while ( !_isInitialized )	{ yield return null; }

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

			while ( isRunning )	{ yield return null; }
			disposable.Dispose();
		}

		protected IEnumerator RunForever() {
			while ( true )	{ yield return null; }
		}
	}
}