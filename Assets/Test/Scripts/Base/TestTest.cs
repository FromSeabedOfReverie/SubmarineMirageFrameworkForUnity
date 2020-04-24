//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using UniRx;
	using UniRx.Async;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class TestTest : Test {
		protected override void Create() {
			Log.Debug( "Create" );
			_initializeEvent.AddLast( async cancel => Log.Debug( "_initializeEvent" ) );
			_finalizeEvent.AddLast().Subscribe( _ => Log.Debug( "_finalizeEvent" ) );
			_disposables.AddLast( () => Log.Debug( "Dispose" ) );
		}


		[UnityTest]
		public IEnumerator TestDelay() => From( async () => {
			Log.Debug( 1 );
			await UniTaskUtility.Delay( _asyncCancel, 1000 );
			Log.Debug( 2 );
		} );


		[UnityTest]
		public IEnumerator TestCancel() => From( async () => {
			Log.Debug( 1 );
			UniTask.Void( async () => {
				await UniTaskUtility.Delay( _asyncCancel, 1000 );
				StopAsync();
			} );
			await UniTaskUtility.WaitWhile( _asyncCancel, () => true );
			Log.Debug( 2 );
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestDispose() => From( async () => {
			await UniTaskUtility.WaitWhile( _asyncCancel, () => true );
			// エディタ停止ボタンは、解放されない為、F5ボタンの拡張停止を使う
		} );
	}
}