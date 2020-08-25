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



	public class TestChangeParentSMObject : Test {
		protected override void Create() {
			Application.targetFrameRate = 30;

			_createEvent.AddLast( async canceler => {
				Log.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestSetParent ):	CreateTestSetParent();	break;
				}
				Log.Debug( $"end {nameof( Create )}{_testName}" );

				await UTask.DontWait();
			} );
		}


/*
		・親設定テスト
		transform.SetParent、確認
*/
		void CreateTestSetParent() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestSetParent() => From( async () => {
			Log.Debug( $"{nameof( TestSetParent )}" );

			var p = new GameObject( "1p" ).transform;
			p.position = Vector3.one;
			var c = new GameObject( "1c" ).transform;
			c.position = Vector3.one * 2;
			c.SetParent( p );

			p = new GameObject( "2p" ).transform;
			p.position = Vector3.one;
			c = new GameObject( "2c" ).transform;
			c.position = Vector3.one * 2;
			c.SetParent( p, true );

			p = new GameObject( "3p" ).transform;
			p.position = Vector3.one;
			c = new GameObject( "3c" ).transform;
			c.position = Vector3.one * 2;
			c.SetParent( p, false );

			p = new GameObject( "4p" ).transform;
			p.position = Vector3.one;
			c = new GameObject( "4c" ).transform;
			c.position = Vector3.one * 2;
			c.parent = p;

			c = new GameObject( "5c" ).transform;
			c.SetParent( null );

			await UTask.Never( _asyncCanceler );
		} );
	}
}