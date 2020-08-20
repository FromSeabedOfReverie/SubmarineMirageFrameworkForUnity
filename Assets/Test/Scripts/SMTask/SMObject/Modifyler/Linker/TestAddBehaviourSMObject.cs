//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestSMTask.Modifyler {
	using System;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using UTask;
	using SMTask;
	using SMTask.Modifyler;
	using Debug;
	using Test;



	// TODO : コメント追加、整頓



	public class TestAddBehaviourSMObject : Test {
		protected override void Create() {
			Application.targetFrameRate = 30;

			_createEvent.AddLast( async canceler => {
				Log.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestHoge ):	CreateTestHoge();	break;
				}
				Log.Debug( $"end {nameof( Create )}{_testName}" );

				await UTask.DontWait();
			} );
		}


/*
		・テスト
*/
		void CreateTestHoge() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestHoge() => From( async () => {
			await UTask.NextFrame( _asyncCanceler );
			Log.Debug( $"{nameof( TestHoge )}" );

			await UTask.Never( _asyncCanceler );
		} );
	}
}