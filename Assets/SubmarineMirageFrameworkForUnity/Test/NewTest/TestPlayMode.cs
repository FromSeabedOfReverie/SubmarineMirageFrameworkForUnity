using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UniRx.Async;
using DG.Tweening;
//using GameDevWare.Serialization;

namespace Tests
{
    public class TestPlayMode
    {
        [UnityTest]
        public IEnumerator TestB() => UniTask.ToCoroutine( async () => {
			var second = Time.time;
			Debug.Log( 0 );
			Assert.IsTrue( 1 == 1 );
			await UniTask.Delay( 1000 );
			Assert.IsTrue( 1 == 1 );
			Debug.Log( Time.time - second );
		} );
    }
}