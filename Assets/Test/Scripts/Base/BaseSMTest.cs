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
	using Base;
	using Service;
	using Task;
	using Utility;
	using Debug;
	using EditorUtility;



	// TODO : コメント追加、整頓



	public abstract class BaseSMTest : BaseSM, IBaseSMTest, IPrebuildSetup {
		protected string _testName => TestContext.CurrentContext.Test.Name;
		protected bool _isInitialized	{ get; set; }
		[SMHide] protected readonly SMTaskCanceler _asyncCanceler = new SMTaskCanceler();


		public void Setup() {
			ConsoleEditorSMUtility.Clear();
//			SubmarineMirageFramework.Shutdown();
			SubmarineMirageFramework.s_playType = SubmarineMirageFramework.PlayType.Test;
			SMServiceLocator.Register<IBaseSMTest>( this );
		}

		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
		static void Main() {
			if ( SubmarineMirageFramework.s_playType == SubmarineMirageFramework.PlayType.Test ) {
			}
		}

		[OneTimeSetUp]
		protected void Awake() {
		}

		public async UniTask AwakeTop() {
			try {
				await AwakeSub();
			} catch ( OperationCanceledException ) {
				throw;
			} catch ( Exception e ) {
				SMLog.Error( e );
				throw;
			}
		}

		protected abstract UniTask AwakeSub();

		protected abstract void Create();


		[OneTimeTearDown]
		protected void OnDestroy() {
			try {
				Dispose();
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