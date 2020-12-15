//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestCSharp {
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using KoganeUnityLib;
	using Utility;
	using Debug;
	using Test;

	public partial class TestQueue : SMStandardTest {
		protected override void Create() {
			Application.targetFrameRate = 30;
		}


/*
		・参照中に変更テスト
*/
		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestChangeAtWhile() => From( async () => {
			var q = new Queue<int>();
			q.Enqueue( 1 );
			q.Enqueue( 2 );
			q.Enqueue( 3 );
			q.Enqueue( 4 );

			while ( !q.IsEmpty() ) {
				var i = q.Dequeue();
				SMLog.Debug( i );
				if ( i % 2 == 0 ) {
					var n = Random.Range( 10, 100 );
					if ( n % 2 == 0 )	{ n += 1; }
					q.Enqueue( n );
				}
			}

			await UTask.DontWait();
		} );
	}
}