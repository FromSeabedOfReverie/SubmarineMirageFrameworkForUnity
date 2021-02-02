//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestBase.Test {
	using System.Collections;
	using UnityEngine;
	using Base;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class TestSMTestBody : SMStandardBase {
		BaseSMTest _owner	{ get; set; }
		SMAsyncCanceler _asyncCanceler => _owner._asyncCanceler;


		public TestSMTestBody( BaseSMTest owner )
			=> _owner = owner;



		public IEnumerator TestTask() => _owner.From( async () => {
			SMLog.Debug( $"{nameof( TestTask )} : start" );
			await UTask.Delay( _asyncCanceler, 1000 );
			SMLog.Debug( $"{nameof( TestTask )} : end" );
		} );

		public IEnumerator TestCoroutine() => _owner.From( TestCoroutineSub() );
		IEnumerator TestCoroutineSub() {
			SMLog.Debug( $"{nameof( TestCoroutine )} : start" );
			yield return new WaitForSeconds( 1 );
			SMLog.Debug( $"{nameof( TestCoroutine )} : end" );
		}


		public IEnumerator TestCancelTask() => _owner.From( async () => {
			SMLog.Debug( $"{nameof( TestCancelTask )} : start" );
			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 1000 );
				_owner.StopAsync();
			} );
			await UTask.Never( _asyncCanceler );
			SMLog.Debug( $"{nameof( TestCancelTask )} : end" );
		} );

		public IEnumerator TestCancelCoroutine() => _owner.From( TestCancelCoroutineSub() );
		IEnumerator TestCancelCoroutineSub() {
			SMLog.Debug( $"{nameof( TestCancelCoroutine )} : start" );
			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 1000 );
				_owner.StopAsync();
			} );
			yield return _owner.RunForever();
			SMLog.Debug( $"{nameof( TestCancelCoroutine )} : end" );
		}


		public IEnumerator TestDisposeTask() => _owner.From( async () => {
			SMLog.Debug( $"{nameof( TestDisposeTask )}" );
			await UTask.Never( _asyncCanceler );
			// エディタ停止ボタンは、解放されない為、F5ボタンの拡張停止を使う
		} );

		public IEnumerator TestDisposeCoroutine() => _owner.From( TestDisposeCoroutineSub() );
		IEnumerator TestDisposeCoroutineSub() {
			SMLog.Debug( $"{nameof( TestDisposeCoroutine )}" );
			yield return _owner.RunForever();
			// エディタ停止ボタンは、解放されない為、F5ボタンの拡張停止を使う
		}
	}
}