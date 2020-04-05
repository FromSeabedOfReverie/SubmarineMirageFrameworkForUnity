using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UniRx.Async;
using System;

namespace Tests {
    public class TestTest {
        [Test]
        public void TestA() {
			Debug.Log( 1 );
			Assert.IsTrue( 1 == 1 );
        }

        [UnityTest]
        public IEnumerator TestB() => UniTask.ToCoroutine( async () => {
			Debug.Log( 2 );
			Assert.IsTrue( 1 == 1 );
			var isWait = true;
			UniTask.Delay( 1000 ).ContinueWith( () => isWait = false ).Forget();
			await UniTask.WaitWhile( () => isWait );
			Assert.IsTrue( 1 == 1 );
		} );

		[UnityTest]
        public IEnumerator TestC() {
			var isWait = true;
			UniTask.Void( async () => {
				Debug.Log( 2 );
				Assert.IsTrue( 1 == 1 );
				await UniTask.Delay( 1000 );
				Assert.IsTrue( 1 == 1 );
				isWait = false;
			} );
			while ( isWait ) { yield return null; }
			Debug.Log( 3 );
		}

		[UnityTest]
		public IEnumerator DelayIgnore() => UniTask.ToCoroutine(async () => {
			var time = Time.realtimeSinceStartup;

			Time.timeScale = 0.5f;
			try {
				await UniTask.Delay(TimeSpan.FromSeconds(3), true);
				var elapsed = Time.realtimeSinceStartup - time;
				Assert.AreEqual(3,
					(int)Math.Round(TimeSpan.FromSeconds(elapsed).TotalSeconds, MidpointRounding.ToEven));
			}
			finally {
				Time.timeScale = 1.0f;
			}
		});

		public async UniTask TargetTask() {
			await UniTask.Delay( 1000 );
		}
		[UnityTest]
		public IEnumerator TestTask() {
			var task = TargetTask();
			Debug.Log( 1 );
			yield return new WaitUntil(() => task.IsCompleted);
			Debug.Log( 2 );
		}
    }
}
