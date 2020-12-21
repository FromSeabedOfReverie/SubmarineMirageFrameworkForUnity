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
	using Task;
	using Task.Behaviour;
	using Task.Object;
	using Task.Group.Modifyler;
	using Scene;
	using Utility;
	using Debug;
	using TestBase;



	// TODO : コメント追加、整頓



	public class TestRegisterSMObject : SMStandardTest {
		protected override void Create() {
			Application.targetFrameRate = 30;

			_createEvent.AddLast( async canceler => {
				SMLog.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestMoveGameObjectToScene ):	CreateTestMoveGameObjectToScene();	break;
					case nameof( TestSetRunObject ):			CreateTestSetRunObject();			break;
					case nameof( TestCancel ):					CreateTestCancel();					break;
					case nameof( TestError ):					CreateTestError();					break;
				}
				SMLog.Debug( $"end {nameof( Create )}{_testName}" );

				await UTask.DontWait();
			} );
		}


/*
		・シーン移動テスト
		Run、SMTaskLifeSpan.InScene、SMTaskLifeSpan.Forever、確認
*/
		void CreateTestMoveGameObjectToScene() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestMoveGameObjectToScene() => From( async () => {
			SMLog.Debug( $"{nameof( TestMoveGameObjectToScene )}" );

			await UTask.NextFrame( _asyncCanceler );


			SMLog.Debug( $"・{SMTaskLifeSpan.InScene}のテスト" );
			new B1();
			await UTask.NextFrame( _asyncCanceler );

			var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
			new SMObject( b.gameObject, new [] { b }, null );
			await UTask.NextFrame( _asyncCanceler );


			SMLog.Debug( $"・{SMTaskLifeSpan.Forever}のテスト" );
			new B4();
			await UTask.NextFrame( _asyncCanceler );

			b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M4" );
			new SMObject( b.gameObject, new [] { b }, null );
			await UTask.NextFrame( _asyncCanceler );


			await UTask.Never( _asyncCanceler );
		} );


/*
		・種類別登録テスト
		SetRunObject、SMTaskType.DontWork、SMTaskType.Work、SMTaskType.FirstWork、確認
*/
		void CreateTestSetRunObject() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestSetRunObject() => From( async () => {
			SMLog.Debug( $"{nameof( TestSetRunObject )}" );

			await UTask.NextFrame( _asyncCanceler );


			SMLog.Debug( $"・{SMTaskType.DontWork}のテスト" );
			new B1();
			await UTask.NextFrame( _asyncCanceler );

			var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
			new SMObject( b.gameObject, new [] { b }, null );
			await UTask.NextFrame( _asyncCanceler );


			SMLog.Debug( $"・{SMTaskType.Work}のテスト" );
			new B2();
			await UTask.NextFrame( _asyncCanceler );

			b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M2" );
			new SMObject( b.gameObject, new [] { b }, null );
			await UTask.NextFrame( _asyncCanceler );


			SMLog.Debug( $"・{SMTaskType.FirstWork}のテスト" );
			new B3();
			await UTask.NextFrame( _asyncCanceler );

			b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M3" );
			new SMObject( b.gameObject, new [] { b }, null );
			await UTask.NextFrame( _asyncCanceler );


			await UTask.Never( _asyncCanceler );
		} );


/*
		・停止テスト
		Cancel、確認
*/
		void CreateTestCancel() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCancel() => From( async () => {
			await UTask.NextFrame( _asyncCanceler );
			SMLog.Debug( $"{nameof( TestCancel )}" );


			var o = new B1( true )._object;
			o._top = o;
			o._scene = SMSceneManager.s_instance._fsm._scene;
			var d = new RegisterSMGroup( o );
			d.Dispose();

			o = new B1( true )._object;
			o._top = o;
			o._scene = SMSceneManager.s_instance._fsm._scene;
			d = new RegisterSMGroup( o );
			d.Run().Forget();
			d.Dispose();
			await UTask.NextFrame( _asyncCanceler );

			o = new B1( true )._object;
			o._top = o;
			o._scene = SMSceneManager.s_instance._fsm._scene;
			d = new RegisterSMGroup( o );
			await d.Run();
			d.Dispose();

			o = new B1( true )._object;


			var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
			o = new SMObject( b.gameObject, new [] { b }, null, true );
			o._top = o;
			o._scene = SMSceneManager.s_instance._fsm._scene;
			d = new RegisterSMGroup( o );
			d.Dispose();

			b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
			o = new SMObject( b.gameObject, new [] { b }, null, true );
			o._top = o;
			o._scene = SMSceneManager.s_instance._fsm._scene;
			d = new RegisterSMGroup( o );
			d.Run().Forget();
			d.Dispose();
			await UTask.NextFrame( _asyncCanceler );

			b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
			o = new SMObject( b.gameObject, new [] { b }, null, true );
			o._top = o;
			o._scene = SMSceneManager.s_instance._fsm._scene;
			d = new RegisterSMGroup( o );
			await d.Run();
			d.Dispose();

			b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
			o = new SMObject( b.gameObject, new [] { b }, null, true );


			await UTask.Never( _asyncCanceler );
		} );


/*
		・エラーテスト
		RegisterSMObject、top以外、確認
*/
		void CreateTestError() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestError() => From( async () => {
			await UTask.NextFrame( _asyncCanceler );
			SMLog.Debug( $"{nameof( TestError )}" );

			try {
				new RegisterSMGroup( null );
			} catch ( Exception e )	{ SMLog.Error( e ); }

			try {
				var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
				new RegisterSMGroup( o );
			} catch ( Exception e )	{ SMLog.Error( e ); }

			await UTask.Never( _asyncCanceler );
		} );
	}
}