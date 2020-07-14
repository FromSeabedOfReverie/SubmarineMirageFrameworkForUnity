//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using Cysharp.Threading.Tasks;
	using Main;
	using UTask;
	using Debug;


	// TODO : コメント追加、整頓


	public class TestBaseTest : BaseTest {
		protected override async UniTask AwakeSub() {
			Create();
			UTask.Void( async () => {
				await UTask.WaitWhile( _asyncCanceler, () => !SubmarineMirage.s_instance._isInitialized );
				_isInitialized = true;
			} );
			await UTask.DontWait();
		}

		protected override void Create() => Log.Debug( $"{nameof( Create )}" );

		public override void Dispose() {
			_asyncCanceler.Dispose();
			Log.Debug( $"{nameof( Dispose )}" );
		}


		[UnityTest]
		public IEnumerator TestTask() => From( async () => {
			Log.Debug( $"{nameof( TestTask )} : start" );
			await UTask.Delay( _asyncCanceler, 1000 );
			Log.Debug( $"{nameof( TestTask )} : end" );
		} );

		[UnityTest]
		public IEnumerator TestCoroutine() => From( TestCoroutineSub() );
		IEnumerator TestCoroutineSub() {
			Log.Debug( $"{nameof( TestCoroutine )} : start" );
			yield return new WaitForSeconds( 1 );
			Log.Debug( $"{nameof( TestCoroutine )} : end" );
		}


		[UnityTest]
		public IEnumerator TestCancelTask() => From( async () => {
			Log.Debug( $"{nameof( TestCancelTask )} : start" );
			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 1000 );
				_asyncCanceler.Cancel();
			} );
			await UTask.WaitWhile( _asyncCanceler, () => true );
			Log.Debug( $"{nameof( TestCancelTask )} : end" );
		} );

		[UnityTest]
		public IEnumerator TestCancelCoroutine() => From( TestCancelCoroutineSub() );
		IEnumerator TestCancelCoroutineSub() {
			Log.Debug( $"{nameof( TestCancelCoroutine )} : start" );
			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 1000 );
				_asyncCanceler.Cancel();
			} );
			while ( true )	{ yield return null; }
			Log.Debug( $"{nameof( TestCancelCoroutine )} : end" );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestDisposeTask() => From( async () => {
			Log.Debug( $"{nameof( TestDisposeTask )}" );
			await UTask.WaitWhile( _asyncCanceler, () => true );
			// エディタ停止ボタンは、解放されない為、F5ボタンの拡張停止を使う
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestDisposeCoroutine() => From( TestDisposeCoroutineSub() );
		IEnumerator TestDisposeCoroutineSub() {
			Log.Debug( $"{nameof( TestDisposeCoroutine )}" );
			while ( true )	{ yield return null; }
			// エディタ停止ボタンは、解放されない為、F5ボタンの拡張停止を使う
		}
	}
}