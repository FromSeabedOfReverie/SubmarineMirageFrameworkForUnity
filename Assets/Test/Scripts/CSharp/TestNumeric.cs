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
	using UTask;
	using Debug;
	using Test;


	public partial class TestNumeric : Test {
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
			Log.Debug( string.Join( "\n",
				$"{nameof( min )} : {min}",
				$"{nameof( max )} : {max}"
			) );

			min--;
			max++;
			Log.Debug( string.Join( "\n",
				$"{nameof( min )} : {min}",
				$"{nameof( max )} : {max}"
			) );

			min--;
			max++;
			Log.Debug( string.Join( "\n",
				$"{nameof( min )} : {min}",
				$"{nameof( max )} : {max}"
			) );

			await UTask.DontWait();
		} );
	}
}