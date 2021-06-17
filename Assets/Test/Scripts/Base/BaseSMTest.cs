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
	using Utility;
	using Debug;
	using EditorExtension;
	using EditorUtility;



	public abstract class BaseSMTest : BaseSM, IBaseSMTest, IPrebuildSetup {
		[SMShow] protected string _testName => TestContext.CurrentContext.Test.Name;
		[SMShow] protected bool _isInitialized	{ get; set; }
		public readonly SMAsyncCanceler _asyncCanceler = new SMAsyncCanceler();
		protected readonly SMStopwatch _stopwatch = new SMStopwatch();



		// 実行前の呼び出し
		public void Setup() {
			ConsoleEditorSMUtility.Clear();
			SubmarineMirageFramework.Shutdown( false );	// 念の為、前回破棄し損ねたデータを削除
			PlayerEditorSMExtension.instance._type = PlayerEditorSMExtension.Type.Test;
		}

		// 実行直後、真っ先に呼び出し、インスタンス未生成
		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
		static void Main() {
			if ( PlayerEditorSMExtension.instance._type != PlayerEditorSMExtension.Type.Test )	{ return; }
			SMDebugManager.s_isPlayTest = true;
		}

		// インスタンス生成直後の呼び出し
		[OneTimeSetUp]
		protected virtual void Awake() => SMServiceLocator.Register<IBaseSMTest>( this );

		// 継承先にさせたい、初期化
		public abstract UniTask Initialize();

		protected abstract void Create();


		// インスタンス破棄時の呼び出し
		[OneTimeTearDown]
		protected void OnDestroy() => SubmarineMirageFramework.Shutdown( false );


		public void StopAsync() => _asyncCanceler.Cancel();



		public IEnumerator From( Func<UniTask> task ) => UTask.ToCoroutine( async () => {
			await UTask.WaitWhile( _asyncCanceler, () => !_isInitialized );
			try {
				await task.Invoke();
			} catch ( OperationCanceledException ) {
			} catch ( Exception e ) {
				SMLog.Error( e );
				throw;
			}
		} );

		public IEnumerator From( IEnumerator coroutine ) {
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



		public IEnumerator RunForever() {
			while ( true )	{ yield return null; }
		}
	}
}