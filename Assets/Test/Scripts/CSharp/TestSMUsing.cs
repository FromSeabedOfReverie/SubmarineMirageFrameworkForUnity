//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestCSharp {
	using System;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using Task;
	using Utility;
	using Debug;
	using TestBase;


	public partial class TestSMUsing : SMStandardTest {
		protected override void Create() {
			Application.targetFrameRate = 30;
		}


/*
		・参照中に変更テスト
*/
		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestException() => From( async () => {
			try {
				using ( var c = new SMTaskCanceler() ) {
					c._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( "停止" ) );
					throw new Exception();
				}
			} catch {}
			SMLog.Debug( "end" );
			await UTask.DontWait();
		} );
	}
}