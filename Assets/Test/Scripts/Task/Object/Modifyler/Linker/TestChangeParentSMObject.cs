//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask.Modifyler {
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using UnityEngine;
	using Task.Object.Modifyler;
	using Scene;
	using Utility;
	using Debug;
	using TestBase;



	// TODO : コメント追加、整頓



	public class TestChangeParentSMObject : SMStandardTest {
		protected override void Create() {
			Application.targetFrameRate = 30;

			_createEvent.AddLast( async canceler => {
				SMLog.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestSetParent1 ):			CreateTestSetParent1();				break;
					case nameof( TestSetParent2 ):			CreateTestSetParent2();				break;
					case nameof( TestChangeLink ):			CreateTestChangeLink();				break;
					case nameof( TestUnlinkParent ):		CreateTestUnlinkParent();			break;
					case nameof( TestReregisterModifyler ):	CreateTestReregisterModifyler();	break;
					case nameof( TestChangeActive ):		CreateTestChangeActive();			break;
					case nameof( TestError ):				CreateTestError();					break;
						
				}
				SMLog.Debug( $"end {nameof( Create )}{_testName}" );

				await UTask.DontWait();
			} );
		}


/*
		・親設定テスト
		transform.SetParent、確認
*/
		void CreateTestSetParent1() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestSetParent1() => From( async () => {
			SMLog.Debug( $"{nameof( TestSetParent1 )}" );

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


/*
		・親設定テスト2
		Run、transform.SetParent、確認
*/
		void CreateTestSetParent2() => TestSMBehaviourSMUtility.CreateBehaviours( @"
			M1,
			M1,
			M1,
			M1,
			M2,
				M2,
			M2,
				M2,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestSetParent2() => From( async () => {
			SMLog.Debug( $"{nameof( TestSetParent2 )}" );


			SMLog.Debug( "・子接続テスト" );
			var p = SMSceneManager.s_instance.GetBehaviour<M1>()._object;
			var c = p._next;
			p._owner.transform.position = Vector3.one;
			c._owner.transform.position = Vector3.one * 2;
			c._top._modifyler.Register( new SendChangeParentSMObject( c, p._owner.transform, true ) );
			await c._top._modifyler.WaitRunning();

			p = p._next;
			c = p._next;
			p._owner.transform.position = Vector3.one;
			c._owner.transform.position = Vector3.one * 2;
			c._top._modifyler.Register( new SendChangeParentSMObject( c, p._owner.transform, false ) );
			await c._top._modifyler.WaitRunning();


			SMLog.Debug( "・子解除テスト" );
			p = SMSceneManager.s_instance.GetBehaviour<M2>()._object;
			c = p._child;
			p._owner.transform.position = Vector3.one;
			c._owner.transform.position = Vector3.one * 2;
			c._top._modifyler.Register( new SendChangeParentSMObject( c, null, true ) );
			await c._top._modifyler.WaitRunning();

			p = p._next;
			c = p._child;
			p._owner.transform.position = Vector3.one;
			c._owner.transform.position = Vector3.one * 2;
			c._top._modifyler.Register( new SendChangeParentSMObject( c, null, false ) );
			await c._top._modifyler.WaitRunning();


			await UTask.Never( _asyncCanceler );
		} );


/*
		・リンク変更テスト
		Run、_object、parent、元_top、新_top、確認
*/
		void CreateTestChangeLink() => TestSMBehaviourSMUtility.CreateBehaviours( @"
			M1,
			M1,

			M1,
				null,
			M3,

			M1,
				null,
					null,
			M6,


			M1,
				M1,
			M1,

			M1,
				null,
				M1,
					null,
			M3,

			M1,
				M1,
				null,
				null,
					null,
						null,
					M1,
			M6,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestChangeLink() => From( async () => {
			SMLog.Debug( $"{nameof( TestChangeLink )}" );


			SMLog.Debug( "・子接続テスト" );
			var p = SMSceneManager.s_instance.GetBehaviour<M1>()._object;
			var nextP = p._next._next;
			var c = p._next;
			c._top._modifyler.Register( new SendChangeParentSMObject( c, p._owner.transform, true ) );
			await c._top._modifyler.WaitRunning();

			SMLog.Debug( "・b" );

			p = nextP;
			nextP = p._next;
			c = SMSceneManager.s_instance.GetBehaviour<M3>()._object;
			c._top._modifyler.Register( new SendChangeParentSMObject( c, p._owner.transform.GetChild( 0 ), true ) );
			await c._top._modifyler.WaitRunning();

			SMLog.Debug( "・c" );

			p = nextP;
			nextP = p._next;
			c = SMSceneManager.s_instance.GetBehaviour<M6>()._object;
			c._top._modifyler.Register( new SendChangeParentSMObject(
				c, p._owner.transform.GetChild( 0 ).GetChild( 0 ), true ) );
			await c._top._modifyler.WaitRunning();


			SMLog.Debug( "・孫接続テスト" );
			p = nextP;
			nextP = p._next._next;
			c = p._next;
			c._top._modifyler.Register( new SendChangeParentSMObject( c, p._child._owner.transform, true ) );
			await c._top._modifyler.WaitRunning();

			p = nextP;
			nextP = p._next;
			c = SMSceneManager.s_instance.GetBehaviour<M3>()._object;
			c._top._modifyler.Register( new SendChangeParentSMObject(
				c, p._child._owner.transform.GetChild( 0 ), true ) );
			await c._top._modifyler.WaitRunning();

			p = nextP;
			c = SMSceneManager.s_instance.GetBehaviour<M6>()._object;
			c._top._modifyler.Register( new SendChangeParentSMObject(
				c, p._child._next._owner.transform.parent.GetChild( 0 ).GetChild( 0 ), true ) );
			await c._top._modifyler.WaitRunning();


			await UTask.Never( _asyncCanceler );
		} );


/*
		・親解除テスト
		Run、_object、parent、元_top、新_top、確認
*/
		void CreateTestUnlinkParent() => TestSMBehaviourSMUtility.CreateBehaviours( @"
			M1,
				M1,

			M1,
				M3,

			M1,
				M6,

			M1,
			M1,
				M2,
					M1,

			M1,
			M1,
				M2,
					M3,

			M1,
			M1,
				M2,
					M6,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestUnlinkParent() => From( async () => {
			SMLog.Debug( $"{nameof( TestUnlinkParent )}" );

			await UTask.Never( _asyncCanceler );
		} );


/*
		・変更付け替えテスト
		Run、溜まっている_modifylerの付け替え、確認
*/
		void CreateTestReregisterModifyler() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestReregisterModifyler() => From( async () => {
			SMLog.Debug( $"{nameof( TestReregisterModifyler )}" );

			await UTask.Never( _asyncCanceler );
		} );


/*
		・活動状態変更テスト
		Run、ChangeActiveSMObject、確認
*/
		void CreateTestChangeActive() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestChangeActive() => From( async () => {
			SMLog.Debug( $"{nameof( TestChangeActive )}" );

			await UTask.Never( _asyncCanceler );
		} );


/*
		・エラーテスト
		生成時、SMBehaviourへ登録、null指定、Destroy済_parent指定、確認
*/
		void CreateTestError() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestError() => From( async () => {
			SMLog.Debug( $"{nameof( TestError )}" );

			await UTask.Never( _asyncCanceler );
		} );
	}
}