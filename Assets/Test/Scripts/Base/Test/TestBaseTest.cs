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
	using Main.New;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class TestBaseTest : BaseTest {
		protected override async UniTask AwakeSub() {
			Create();
			UniTask.Void( async () => {
				await UniTaskUtility.WaitWhile( _asyncCancel, () => !SubmarineMirage.s_instance._isInitialized );
				_isInitialized = true;
			} );
			await UniTaskUtility.DontWait();
		}

		protected override void Create() => Log.Debug( "Create" );

		public override void Dispose() {
			_asyncCanceler.Cancel();
			_asyncCanceler.Dispose();
			Log.Debug( "Dispose" );
		}


		[UnityTest]
		public IEnumerator TestTask() => From( async () => {
			Log.Debug( "start TestTask" );
			await UniTaskUtility.Delay( _asyncCancel, 1000 );
			Log.Debug( "end TestTask" );
		} );

		[UnityTest]
		public IEnumerator TestCoroutine() => From( TestCoroutineSub() );
		IEnumerator TestCoroutineSub() {
			Log.Debug( "start TestCoroutine" );
			yield return new WaitForSeconds( 1 );
			Log.Debug( "end TestCoroutine" );
		}


		[UnityTest]
		public IEnumerator TestCancelTask() => From( async () => {
			Log.Debug( "start TestCancelTask" );
			UniTask.Void( async () => {
				await UniTaskUtility.Delay( _asyncCancel, 1000 );
				_asyncCanceler.Cancel();
			} );
			await UniTaskUtility.WaitWhile( _asyncCancel, () => true );
			Log.Debug( "end TestCancelTask" );
		} );

		[UnityTest]
		public IEnumerator TestCancelCoroutine() => From( TestCancelCoroutineSub() );
		IEnumerator TestCancelCoroutineSub() {
			Log.Debug( "start TestCancelCoroutine" );
			UniTask.Void( async () => {
				await UniTaskUtility.Delay( _asyncCancel, 1000 );
				_asyncCanceler.Cancel();
			} );
			while ( true )	{ yield return null; }
			Log.Debug( "end TestCancelCoroutine" );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestDisposeTask() => From( async () => {
			Log.Debug( "TestDisposeTask" );
			await UniTaskUtility.WaitWhile( _asyncCancel, () => true );
			// エディタ停止ボタンは、解放されない為、F5ボタンの拡張停止を使う
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestDisposeCoroutine() => From( TestDisposeCoroutineSub() );
		IEnumerator TestDisposeCoroutineSub() {
			Log.Debug( "TestDisposeCoroutine" );
			while ( true )	{ yield return null; }
			// エディタ停止ボタンは、解放されない為、F5ボタンの拡張停止を使う
		}
	}
}