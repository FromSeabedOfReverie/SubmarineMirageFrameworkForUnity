//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestUTask {
	using System;
	using System.Linq;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.Networking;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Task;
	using Utility;
	using Debug;
	using Test;


	public partial class TestUTask : SMStandardTest {
		SMTaskCanceler _canceler = new SMTaskCanceler();


		protected override void Create() {
			Application.targetFrameRate = 30;

			_disposables.AddLast( _canceler );
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Return ) ).Subscribe( _ =>
					_canceler.Cancel()
				)
			);
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestEmpty() => From( async () => {
			SMLog.Debug( 1 );
			UTask.Void( async () => {
				SMLog.Debug( 2 );
				await UTask.Empty;
				SMLog.Debug( 3 );
				await UTask.Empty;
				SMLog.Debug( 4 );
			} );
			SMLog.Debug( 5 );
			await UTask.DontWait();
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestDontWait() => From( async () => {
			SMLog.Debug( 1 );
			UTask.Void( async () => {
				SMLog.Debug( 2 );
				await UTask.DontWait();
				SMLog.Debug( 3 );
				await UTask.DontWait();
				SMLog.Debug( 4 );
			} );
			SMLog.Debug( 5 );
			await UTask.DontWait();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestFunc() => From( async () => {
			SMLog.Debug( 1 );
			var t = UTask.Func( async () => {
				SMLog.Debug( 2 );
				await UTask.NextFrame( _canceler );
				SMLog.Debug( 3 );
			} );

			SMLog.Debug( 4 );
			await t();
			SMLog.Debug( 5 );
			await t();		// UniTask.Lazyと異なり、きちんと、2回目も待機される
			SMLog.Debug( 6 );
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestFunc2() => From( async () => {
			SMLog.Debug( 1 );
			var t = UTask.Func( async () => {
				SMLog.Debug( 2 );
				await UTask.NextFrame( _canceler );
				SMLog.Debug( 3 );
				return UnityEngine.Random.Range( 10, 100 );
			} );
			SMLog.Debug( 4 );
			SMLog.Debug( await t() );
			SMLog.Debug( 5 );
			SMLog.Debug( await t() );	// UniTask.Lazyと異なり、きちんと、2回目も待機され、新規値も返される
			SMLog.Debug( 6 );
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestVoid() => From( async () => {
			SMLog.Debug( 1 );
			UTask.Void( async () => {
				SMLog.Debug( 2 );
				await UTask.NextFrame( _canceler );
				SMLog.Debug( 3 );
			} );
			SMLog.Debug( 4 );
			await UTask.NextFrame( _canceler );	// 待機しないと、エディタ終了で、途中停止
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestAction() => From( async () => {
			SMLog.Debug( 1 );
			var a = UTask.Action( async () => {
				SMLog.Debug( 2 );
				await UTask.NextFrame( _canceler );
				SMLog.Debug( 3 );
			} );
			SMLog.Debug( 4 );
			a();
			SMLog.Debug( 5 );
			a();			// 2回目も、正常実行
			SMLog.Debug( 6 );
			await UTask.NextFrame( _canceler );	// 待機しないと、エディタ終了で、途中停止
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestUnityAction() => From( async () => {
			SMLog.Debug( 1 );
			var ua = UTask.UnityAction( async () => {
				SMLog.Debug( 2 );
				await UTask.NextFrame( _canceler );
				SMLog.Debug( 3 );
			} );
			SMLog.Debug( 4 );
			ua();
			SMLog.Debug( 5 );
			ua();			// 2回目は、正常実行
			SMLog.Debug( 6 );
			await UTask.NextFrame( _canceler );	// 待機しないと、エディタ終了で、途中停止
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestNever() => From( async () => {
			SMLog.Debug( 1 );
			try {
				await UTask.Never( _canceler );
			} finally {
				SMLog.Debug( 2 );
			}
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestYield() => From( async () => {
			SMLog.Debug( 1 );
			UTask.Void( async () => {
				await UTask.Yield( _canceler );
				SMLog.Debug( 2 );
			} );
			SMLog.Debug( 3 );
			await UTask.NextFrame( _canceler );

			SMLog.Debug( 4 );
			UTask.Void( async () => {
				await UTask.Yield( _canceler );
				SMLog.Debug( 5 );
			} );
			SMLog.Debug( 6 );
			_canceler.Cancel();
			SMLog.Debug( 7 );
			await UTask.NextFrame( _asyncCanceler );
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestNextFrame() => From( async () => {
			SMLog.Debug( 1 );
			UTask.Void( async () => {
				await UTask.NextFrame( _canceler );
				SMLog.Debug( 2 );
			} );
			SMLog.Debug( 3 );
			await UTask.NextFrame( _canceler );

			SMLog.Debug( 4 );
			UTask.Void( async () => {
				await UTask.NextFrame( _canceler );
				SMLog.Debug( 5 );
			} );
			SMLog.Debug( 6 );
			_canceler.Cancel();
			SMLog.Debug( 7 );
			await UTask.NextFrame( _asyncCanceler );
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestDelay1() => From( async () => {
			SMLog.Debug( 1 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, 1000 );
				SMLog.Debug( 2 );
			} );
			SMLog.Debug( 3 );
			await UTask.Delay( _canceler, 1000 );

			SMLog.Debug( 4 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, 0 );
				SMLog.Debug( 5 );
			} );
			SMLog.Debug( 6 );
			await UTask.NextFrame( _canceler );

			SMLog.Debug( 7 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, 1000 );
				SMLog.Debug( 8 );
			} );
			SMLog.Debug( 9 );
			_canceler.Cancel();
			SMLog.Debug( 10 );
			await UTask.Delay( _asyncCanceler, 1000 );
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestDelay2() => From( async () => {
			SMLog.Debug( 1 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, TimeSpan.FromSeconds( 1 ) );
				SMLog.Debug( 2 );
			} );
			SMLog.Debug( 3 );
			await UTask.Delay( _canceler, TimeSpan.FromSeconds( 1 ) );

			SMLog.Debug( 4 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, TimeSpan.Zero );
				SMLog.Debug( 5 );
			} );
			SMLog.Debug( 6 );
			await UTask.NextFrame( _canceler );

			SMLog.Debug( 7 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, TimeSpan.FromSeconds( 1 ) );
				SMLog.Debug( 8 );
			} );
			SMLog.Debug( 9 );
			_canceler.Cancel();
			SMLog.Debug( 10 );
			await UTask.Delay( _asyncCanceler, TimeSpan.FromSeconds( 1 ) );
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestDelay3() => From( async () => {
			SMLog.Debug( 1 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, 1000, DelayType.DeltaTime );
				SMLog.Debug( 2 );
			} );
			SMLog.Debug( 3 );
			await UTask.Delay( _canceler, 1000, DelayType.DeltaTime );

			SMLog.Debug( 4 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, 0, DelayType.DeltaTime );
				SMLog.Debug( 5 );
			} );
			SMLog.Debug( 6 );
			await UTask.NextFrame( _canceler );

			SMLog.Debug( 7 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, 1000, DelayType.DeltaTime );
				SMLog.Debug( 8 );
			} );
			SMLog.Debug( 9 );
			_canceler.Cancel();
			SMLog.Debug( 10 );
			await UTask.Delay( _asyncCanceler, 1000, DelayType.DeltaTime );
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestDelay4() => From( async () => {
			SMLog.Debug( 1 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, TimeSpan.FromSeconds( 1 ), DelayType.DeltaTime );
				SMLog.Debug( 2 );
			} );
			SMLog.Debug( 3 );
			await UTask.Delay( _canceler, TimeSpan.FromSeconds( 1 ), DelayType.DeltaTime );

			SMLog.Debug( 4 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, TimeSpan.Zero, DelayType.DeltaTime );
				SMLog.Debug( 5 );
			} );
			SMLog.Debug( 6 );
			await UTask.NextFrame( _canceler );

			SMLog.Debug( 7 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, TimeSpan.FromSeconds( 1 ), DelayType.DeltaTime );
				SMLog.Debug( 8 );
			} );
			SMLog.Debug( 9 );
			_canceler.Cancel();
			SMLog.Debug( 10 );
			await UTask.Delay( _asyncCanceler, TimeSpan.FromSeconds( 1 ), DelayType.DeltaTime );
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestDelayFrame() => From( async () => {
			SMLog.Debug( 1 );
			UTask.Void( async () => {
				await UTask.DelayFrame( _canceler, 30 );
				SMLog.Debug( 2 );
			} );
			SMLog.Debug( 3 );
			await UTask.DelayFrame( _canceler, 30 );

			SMLog.Debug( 4 );
			UTask.Void( async () => {
				await UTask.DelayFrame( _canceler, 0 );
				SMLog.Debug( 5 );
			} );
			SMLog.Debug( 6 );
			await UTask.NextFrame( _canceler );

			SMLog.Debug( 7 );
			UTask.Void( async () => {
				await UTask.DelayFrame( _canceler, 30 );
				SMLog.Debug( 8 );
			} );
			SMLog.Debug( 9 );
			_canceler.Cancel();
			SMLog.Debug( 10 );
			await UTask.DelayFrame( _asyncCanceler, 30 );
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestWaitWhile() => From( async () => {
			SMLog.Debug( 1 );
			var isWait = true;
			UTask.Void( async () => {
				await UTask.Delay( _canceler, 1000 );
				isWait = false;
				SMLog.Debug( 2 );
			} );
			SMLog.Debug( 3 );
			await UTask.WaitWhile( _canceler, () => isWait );
			SMLog.Debug( 4 );

			SMLog.Debug( 5 );
			UTask.Void( async () => {
				await UTask.WaitWhile( _canceler, () => isWait );
				SMLog.Debug( 6 );
			} );
			SMLog.Debug( 7 );
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestWaitUntil() => From( async () => {
			SMLog.Debug( 1 );
			var isGo = false;
			UTask.Void( async () => {
				await UTask.Delay( _canceler, 1000 );
				isGo = true;
				SMLog.Debug( 2 );
			} );
			SMLog.Debug( 3 );
			await UTask.WaitUntil( _canceler, () => isGo );
			SMLog.Debug( 4 );

			SMLog.Debug( 5 );
			UTask.Void( async () => {
				await UTask.WaitUntil( _canceler, () => isGo );
				SMLog.Debug( 6 );
			} );
			SMLog.Debug( 7 );
		} );


		int testWaitUntilValueChangedValue = 0;
		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestWaitUntilValueChanged() => From( async () => {
			SMLog.Debug( 1 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, 1000 );
				testWaitUntilValueChangedValue = 1;
				SMLog.Debug( 2 );
			} );
			SMLog.Debug( 3 );
			await UTask.WaitUntilValueChanged( _canceler, this, t => t.testWaitUntilValueChangedValue );
			SMLog.Debug( 4 );
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestWaitUntilCanceled() => From( async () => {
			SMLog.Debug( 1 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, 1000 );
				_canceler.Cancel();
				SMLog.Debug( 2 );
			} );
			SMLog.Debug( 3 );
			await UTask.WaitUntilCanceled( _canceler );
			SMLog.Debug( 4 );
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestToUniTask2() => From( async () => {
			var go = new GameObject();
			go.AddComponent<Camera>();
			go.AddComponent<AudioListener>();
			var s = go.AddComponent<AudioSource>();

			SMLog.Debug( 1 );
			s.clip = (AudioClip)await Resources.LoadAsync( "Audio/BGM/TestTitle" ).ToUniTask( _canceler );
			s.Play();
			SMLog.Debug( 2 );
			await UTask.Delay( _canceler, 5000 );

			SMLog.Debug( 3 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, 1 );
				_canceler.Cancel();
				SMLog.Debug( 4 );
			} );
			SMLog.Debug( 5 );
			s.clip = (AudioClip)await Resources.LoadAsync( "Audio/BGM/TestBattle" ).ToUniTask( _canceler );
			s.Play();
			SMLog.Debug( 6 );
			await UTask.Delay( _asyncCanceler, 5000 );
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestToUniTask3() => From( async () => {
			var url =
"https://docs.google.com/spreadsheets/d/e/2PACX-1vQZjl0KQ3qdx1ghjDLczrLpmWQ11Ao75IdaSobLMoFHjuzhG4pTCX0bXvZgGl_P4-2fjLCdCbBKHaRE/pub?gid=1615869423&single=true&output=csv";

			SMLog.Debug( 1 );
			var r = await UnityWebRequest.Get( url ).SendWebRequest().ToUniTask( _canceler );
			SMLog.Debug( r.downloadHandler.text );
			SMLog.Debug( 2 );

			SMLog.Debug( 3 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, 1 );
				_canceler.Cancel();
				SMLog.Debug( 4 );
			} );
			SMLog.Debug( 5 );
			r = await UnityWebRequest.Get( url ).SendWebRequest().ToUniTask( _canceler );
			SMLog.Debug( r.downloadHandler.text );
			SMLog.Debug( 6 );
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestToUniTask4() => From( async () => {
			SMLog.Debug( 1 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, 1000 );
				_canceler.Cancel();
				SMLog.Debug( 2 );
			} );
			SMLog.Debug( 3 );
			await _canceler.ToUniTask();
			SMLog.Debug( 4 );
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestToUniTask5() => From( async () => {
			SMLog.Debug( 1 );
			await TestToUniTask5Coroutine().ToUniTask( _canceler );
			SMLog.Debug( 2 );

			SMLog.Debug( 3 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, 500 );
				_canceler.Cancel();
				SMLog.Debug( 4 );
			} );
			SMLog.Debug( 5 );
			await TestToUniTask5Coroutine().ToUniTask( _canceler );
			SMLog.Debug( 6 );
		} );
		IEnumerator TestToUniTask5Coroutine() {
			SMLog.Debug( 10 );
			yield return new WaitForSeconds( 1 );
			SMLog.Debug( 20 );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestToCoroutine() => From( async () => {
			var t = UTask.Func( async () => {
				try {
					SMLog.Debug( 10 );
					await UTask.Delay( _canceler, 1000 );
					SMLog.Debug( 20 );
				} catch ( OperationCanceledException ) {}	// キャンセルエラーで、タスクが残るのを防止
			} );

			SMLog.Debug( 1 );
			Observable.FromCoroutine( () => UTask.ToCoroutine( t ) )
				.Subscribe( _ => {} );
			await UTask.Delay( _canceler, 1000 );
			SMLog.Debug( 2 );

			SMLog.Debug( 3 );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, 500 );
				_canceler.Cancel();
				SMLog.Debug( 4 );
			} );
			SMLog.Debug( 5 );
			Observable.FromCoroutine( () => UTask.ToCoroutine( t ) ).Subscribe( _ => {} );
			await UTask.Delay( _asyncCanceler, 1000 );	// キャンセルエラーで、エディタ実行後もタスクが残る
			SMLog.Debug( 6 );
		} );
	}
}