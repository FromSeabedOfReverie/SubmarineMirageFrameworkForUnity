//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestCSharp {
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using Utility;
	using Debug;
	using TestBase;


	public partial class TestSMNumeric : SMStandardTest {
		protected override void Create() {
			Application.targetFrameRate = 30;
		}


/*
		・上限突破テスト
*/
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestOverflow() => From( async () => {
			var min = uint.MinValue;
			var max = uint.MaxValue;
			SMLog.Debug( string.Join( "\n",
				$"{nameof( min )} : {min}",
				$"{nameof( max )} : {max}"
			) );

			min--;
			max++;
			SMLog.Debug( string.Join( "\n",
				$"{nameof( min )} : {min}",
				$"{nameof( max )} : {max}"
			) );

			min--;
			max++;
			SMLog.Debug( string.Join( "\n",
				$"{nameof( min )} : {min}",
				$"{nameof( max )} : {max}"
			) );

			await UTask.DontWait();
		} );
	}
}