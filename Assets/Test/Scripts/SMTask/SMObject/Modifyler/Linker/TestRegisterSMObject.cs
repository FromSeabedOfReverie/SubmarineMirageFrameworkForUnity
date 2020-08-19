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
	using KoganeUnityLib;
	using UTask;
	using SMTask;
	using SMTask.Modifyler;
	using Scene;
	using Debug;
	using Test;



	// TODO : コメント追加、整頓



	public class TestRegisterSMObject : Test {
		protected override void Create() {
			Application.targetFrameRate = 30;

			_createEvent.AddLast( async canceler => {
				Log.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestMoveGameObjectToScene ):	CreateTestMoveGameObjectToScene();	break;
					case nameof( TestSetRunObject ):			CreateTestSetRunObject();			break;
					case nameof( TestCancel ):					CreateTestCancel();					break;
					case nameof( TestError ):					CreateTestError();					break;
				}
				Log.Debug( $"end {nameof( Create )}{_testName}" );

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
			Log.Debug( $"{nameof( TestMoveGameObjectToScene )}" );

			await UTask.NextFrame( _asyncCanceler );


			Log.Debug( $"・{SMTaskLifeSpan.InScene}のテスト" );
			new B1();
			await UTask.NextFrame( _asyncCanceler );

			var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
			new SMObject( b.gameObject, new [] { b }, null );
			await UTask.NextFrame( _asyncCanceler );


			Log.Debug( $"・{SMTaskLifeSpan.Forever}のテスト" );
			new B4();
			await UTask.NextFrame( _asyncCanceler );

			b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M4" );
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
			Log.Debug( $"{nameof( TestSetRunObject )}" );

			await UTask.NextFrame( _asyncCanceler );


			Log.Debug( $"・{SMTaskType.DontWork}のテスト" );
			new B1();
			await UTask.NextFrame( _asyncCanceler );

			var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
			new SMObject( b.gameObject, new [] { b }, null );
			await UTask.NextFrame( _asyncCanceler );


			Log.Debug( $"・{SMTaskType.Work}のテスト" );
			new B2();
			await UTask.NextFrame( _asyncCanceler );

			b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M2" );
			new SMObject( b.gameObject, new [] { b }, null );
			await UTask.NextFrame( _asyncCanceler );


			Log.Debug( $"・{SMTaskType.FirstWork}のテスト" );
			new B3();
			await UTask.NextFrame( _asyncCanceler );

			b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M3" );
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
			Log.Debug( $"{nameof( TestCancel )}" );


			var o = new B1( true )._object;
			o._top = o;
			o._scene = SceneManager.s_instance._fsm._scene;
			var d = new RegisterSMObject( o );
			d.Cancel();

			o = new B1( true )._object;
			o._top = o;
			o._scene = SceneManager.s_instance._fsm._scene;
			d = new RegisterSMObject( o );
			d.Run().Forget();
			d.Cancel();
			await UTask.NextFrame( _asyncCanceler );

			o = new B1( true )._object;
			o._top = o;
			o._scene = SceneManager.s_instance._fsm._scene;
			d = new RegisterSMObject( o );
			await d.Run();
			d.Cancel();

			o = new B1( true )._object;


			var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
			o = new SMObject( b.gameObject, new [] { b }, null, true );
			o._top = o;
			o._scene = SceneManager.s_instance._fsm._scene;
			d = new RegisterSMObject( o );
			d.Cancel();

			b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
			o = new SMObject( b.gameObject, new [] { b }, null, true );
			o._top = o;
			o._scene = SceneManager.s_instance._fsm._scene;
			d = new RegisterSMObject( o );
			d.Run().Forget();
			d.Cancel();
			await UTask.NextFrame( _asyncCanceler );

			b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
			o = new SMObject( b.gameObject, new [] { b }, null, true );
			o._top = o;
			o._scene = SceneManager.s_instance._fsm._scene;
			d = new RegisterSMObject( o );
			await d.Run();
			d.Cancel();

			b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
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
			Log.Debug( $"{nameof( TestError )}" );

			try {
				new RegisterSMObject( null );
			} catch ( Exception e )	{ Log.Error( e ); }

			try {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
				new RegisterSMObject( o );
			} catch ( Exception e )	{ Log.Error( e ); }

			await UTask.Never( _asyncCanceler );
		} );
	}
}