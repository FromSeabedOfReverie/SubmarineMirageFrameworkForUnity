//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestModifyler {
	using System;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Modifyler;
	using Utility;
	using Debug;
	using TestBase;



	public class TestSMModifyler : SMUnitTest {
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCreate() => From( async () => {


			await UTask.DontWait();
		} );
	}
}