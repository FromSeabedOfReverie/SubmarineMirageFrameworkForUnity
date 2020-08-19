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



	public class TestUnregisterSMObject : Test {
		protected override void Create() {
			Application.targetFrameRate = 30;

			_createEvent.AddLast( async canceler => {
				Log.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestRun ):		CreateTestRun();	break;
					case nameof( TestError ):	CreateTestError();	break;
				}
				Log.Debug( $"end {nameof( Create )}{_testName}" );

				await UTask.DontWait();
			} );
		}


/*
		・実行テスト
		Run、確認
*/
		void CreateTestRun() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRun() => From( async () => {
			await UTask.NextFrame( _asyncCanceler );
			Log.Debug( $"{nameof( TestRun )}" );


			var o = new B1()._object;
			o._modifyler.Register( new UnregisterSMObject( o ) );
			await UTask.DelayFrame( _asyncCanceler, 3 );


			var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
			o = new SMObject( b.gameObject, new [] { b }, null );
			new UnregisterSMObject( o ).Run().Forget();


			b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( @"
				M1,
					M1,
			" );
			o = new SMObject( b.gameObject, new [] { b }, null );
			new UnregisterSMObject( o ).Run().Forget();


			await UTask.Never( _asyncCanceler );
		} );


/*
		・エラーテスト
		UnregisterSMObject、top以外、確認
*/
		void CreateTestError() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestError() => From( async () => {
			await UTask.NextFrame( _asyncCanceler );
			Log.Debug( $"{nameof( TestError )}" );

			try {
				new UnregisterSMObject( null ).Run().Forget();
			} catch ( Exception e )	{ Log.Error( e ); }

			try {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null );
				o._top = null;
				new UnregisterSMObject( o ).Run().Forget();
			} catch ( Exception e )	{ Log.Error( e ); }

			await UTask.Never( _asyncCanceler );
		} );
	}
}