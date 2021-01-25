//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestBase {
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using Cysharp.Threading.Tasks;
	using Service;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class TestBaseSMTest : BaseSMTest {
		public bool _isDispose => _asyncCanceler._isDispose;


		protected override async UniTask AwakeSub() {
			Create();
			UTask.Void( async () => {
				var framework = SMServiceLocator.Resolve<SubmarineMirageFramework>();
				await framework.WaitInitialize();
				_isInitialized = true;
			} );
			await UTask.DontWait();
		}

		protected override void Create() => SMLog.Debug( $"{nameof( Create )}" );

		public override void Dispose() {
			_asyncCanceler.Dispose();
			SMLog.Debug( $"{nameof( Dispose )}" );
		}


		[UnityTest]
		public IEnumerator TestTask() => From( async () => {
			SMLog.Debug( $"{nameof( TestTask )} : start" );
			await UTask.Delay( _asyncCanceler, 1000 );
			SMLog.Debug( $"{nameof( TestTask )} : end" );
		} );

		[UnityTest]
		public IEnumerator TestCoroutine() => From( TestCoroutineSub() );
		IEnumerator TestCoroutineSub() {
			SMLog.Debug( $"{nameof( TestCoroutine )} : start" );
			yield return new WaitForSeconds( 1 );
			SMLog.Debug( $"{nameof( TestCoroutine )} : end" );
		}


		[UnityTest]
		public IEnumerator TestCancelTask() => From( async () => {
			SMLog.Debug( $"{nameof( TestCancelTask )} : start" );
			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 1000 );
				_asyncCanceler.Cancel();
			} );
			await UTask.WaitWhile( _asyncCanceler, () => true );
			SMLog.Debug( $"{nameof( TestCancelTask )} : end" );
		} );

		[UnityTest]
		public IEnumerator TestCancelCoroutine() => From( TestCancelCoroutineSub() );
		IEnumerator TestCancelCoroutineSub() {
			SMLog.Debug( $"{nameof( TestCancelCoroutine )} : start" );
			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 1000 );
				_asyncCanceler.Cancel();
			} );
			while ( true )	{ yield return null; }
			SMLog.Debug( $"{nameof( TestCancelCoroutine )} : end" );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestDisposeTask() => From( async () => {
			SMLog.Debug( $"{nameof( TestDisposeTask )}" );
			await UTask.WaitWhile( _asyncCanceler, () => true );
			// エディタ停止ボタンは、解放されない為、F5ボタンの拡張停止を使う
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestDisposeCoroutine() => From( TestDisposeCoroutineSub() );
		IEnumerator TestDisposeCoroutineSub() {
			SMLog.Debug( $"{nameof( TestDisposeCoroutine )}" );
			while ( true )	{ yield return null; }
			// エディタ停止ボタンは、解放されない為、F5ボタンの拡張停止を使う
		}
	}
}