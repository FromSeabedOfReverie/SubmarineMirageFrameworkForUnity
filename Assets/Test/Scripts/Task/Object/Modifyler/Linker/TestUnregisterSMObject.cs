//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask.Modifyler {
	using System;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using Task.Behaviour;
	using Task.Object;
	using Task.Group.Modifyler;
	using Utility;
	using Debug;
	using TestBase;



	// TODO : コメント追加、整頓



	public class TestUnregisterSMObject : SMStandardTest {
		protected override void Create() {
			Application.targetFrameRate = 30;

			_createEvent.AddLast( async canceler => {
				SMLog.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestRun ):		CreateTestRun();	break;
					case nameof( TestError ):	CreateTestError();	break;
				}
				SMLog.Debug( $"end {nameof( Create )}{_testName}" );

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
			SMLog.Debug( $"{nameof( TestRun )}" );


			var o = new B1()._object;
			o._modifyler.Register( new UnregisterSMGroup( o ) );
			await UTask.DelayFrame( _asyncCanceler, 3 );


			var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
			o = new SMObject( b.gameObject, new [] { b }, null );
			new UnregisterSMGroup( o ).Run().Forget();


			b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( @"
				M1,
					M1,
			" );
			o = new SMObject( b.gameObject, new [] { b }, null );
			new UnregisterSMGroup( o ).Run().Forget();


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
			SMLog.Debug( $"{nameof( TestError )}" );

			try {
				new UnregisterSMGroup( null ).Run().Forget();
			} catch ( Exception e )	{ SMLog.Error( e ); }

			try {
				var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null );
				o._top = null;
				new UnregisterSMGroup( o ).Run().Forget();
			} catch ( Exception e )	{ SMLog.Error( e ); }

			await UTask.Never( _asyncCanceler );
		} );
	}
}