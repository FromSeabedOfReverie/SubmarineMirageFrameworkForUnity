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



	public class TestReRegisterSMObject : Test {
		protected override void Create() {
			Application.targetFrameRate = 30;

			_createEvent.AddLast( async canceler => {
				Log.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestMoveGameObjectToScene ):	CreateTestMoveGameObjectToScene();	break;
					case nameof( TestChangeType ):				CreateTestChangeType();				break;
					case nameof( TestRun ):						CreateTestRun();					break;
					case nameof( TestError ):					CreateTestError();					break;
				}
				Log.Debug( $"end {nameof( Create )}{_testName}" );

				await UTask.DontWait();
			} );
		}


/*
		・シーン移動テスト
		Run、_foreverScene、_scene、確認
*/
		void CreateTestMoveGameObjectToScene() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestMoveGameObjectToScene() => From( async () => {
			await UTask.NextFrame( _asyncCanceler );
			Log.Debug( $"{nameof( TestMoveGameObjectToScene )}" );


			Log.Debug( $"・{nameof( SMBehaviour )}のテスト" );
			2.Times( () => {
				var o = new B2()._object;
				var lastType = o._type;
				var lastScene = o._scene;
				o._scene = SceneManager.s_instance._fsm._foreverScene;
				var d = new ReRegisterSMObject( o, lastType, lastScene );
				d.Run().Forget();

				lastType = o._type;
				lastScene = o._scene;
				o._scene = SceneManager.s_instance._fsm._scene;
				d = new ReRegisterSMObject( o, lastType, lastScene );
				d.Run().Forget();

				o = SceneManager.s_instance.GetBehaviour<B2>()._object;
				lastType = o._type;
				lastScene = o._scene;
				d = new ReRegisterSMObject( o, lastType, lastScene );
				d.Run().Forget();
			} );
			SceneManager.s_instance.GetBehaviour<B2>()._object.Dispose();
			SceneManager.s_instance.GetBehaviour<B2>()._object.Dispose();


			Log.Debug( $"・{nameof( SMMonoBehaviour )}のテスト" );
			2.Times( () => {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M5" );
				var o = new SMObject( b.gameObject, new [] { b }, null );
				var lastType = o._type;
				var lastScene = o._scene;
				o._scene = SceneManager.s_instance._fsm._scene;
				var d = new ReRegisterSMObject( o, lastType, lastScene );
				d.Run().Forget();

				lastType = o._type;
				lastScene = o._scene;
				o._scene = SceneManager.s_instance._fsm._foreverScene;
				d = new ReRegisterSMObject( o, lastType, lastScene );
				d.Run().Forget();

				o = SceneManager.s_instance.GetBehaviour<M5>()._object;
				lastType = o._type;
				lastScene = o._scene;
				d = new ReRegisterSMObject( o, lastType, lastScene );
				d.Run().Forget();
			} );


			await UTask.Never( _asyncCanceler );
		} );


/*
		・種類変更テスト
		Run、SMTaskType.DontWork、SMTaskType.Work、SMTaskType.FirstWork、確認
*/
		void CreateTestChangeType() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestChangeType() => From( async () => {
			await UTask.NextFrame( _asyncCanceler );
			Log.Debug( $"{nameof( TestChangeType )}" );

			new B1();
			new B1();
			new B2();
			new B3();
			var o = SceneManager.s_instance.GetBehaviour<B1>()._object;

			var lastType = o._type;
			o._type = SMTaskType.DontWork;
			var d = new ReRegisterSMObject( o, lastType, o._scene );
			d.Run().Forget();

			lastType = o._type;
			o._type = SMTaskType.Work;
			d = new ReRegisterSMObject( o, lastType, o._scene );
			d.Run().Forget();

			lastType = o._type;
			o._type = SMTaskType.FirstWork;
			d = new ReRegisterSMObject( o, lastType, o._scene );
			d.Run().Forget();

			lastType = o._type;
			o._type = SMTaskType.DontWork;
			d = new ReRegisterSMObject( o, lastType, o._scene );
			d.Run().Forget();

			await UTask.Never( _asyncCanceler );
		} );


/*
		・実行テスト
		Run、総合確認
*/
		void CreateTestRun() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRun() => From( async () => {
			await UTask.NextFrame( _asyncCanceler );
			Log.Debug( $"{nameof( TestRun )}" );

			2.Times( () => {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null );
				o = SceneManager.s_instance.GetBehaviour<M1>()._object;

				var lastType = o._type;
				var lastScene = o._scene;
				o._type = SMTaskType.Work;
				o._scene = SceneManager.s_instance._fsm._foreverScene;
				var d = new ReRegisterSMObject( o, lastType, lastScene );
				d.Run().Forget();

				lastType = o._type;
				lastScene = o._scene;
				o._type = SMTaskType.FirstWork;
				o._scene = SceneManager.s_instance._fsm._scene;
				d = new ReRegisterSMObject( o, lastType, lastScene );
				d.Run().Forget();

				lastType = o._type;
				lastScene = o._scene;
				o._type = SMTaskType.DontWork;
				o._scene = SceneManager.s_instance._fsm._foreverScene;
				d = new ReRegisterSMObject( o, lastType, lastScene );
				d.Run().Forget();
			} );

			await UTask.Never( _asyncCanceler );
		} );


/*
		・エラーテスト
		ReRegisterSMObject、top以外、lastType、lastScene、確認
*/
		void CreateTestError() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestError() => From( async () => {
			await UTask.NextFrame( _asyncCanceler );
			Log.Debug( $"{nameof( TestError )}" );

			try {
				new ReRegisterSMObject( null, default, null ).Run().Forget();
			} catch ( Exception e )	{ Log.Error( e ); }

			try {
				var o = new B1()._object;
				new ReRegisterSMObject( o, default, null ).Run().Forget();
			} catch ( Exception e )	{ Log.Error( e ); }

			try {
				var o = new B1()._object;
				new ReRegisterSMObject( o, o._type, null ).Run().Forget();
			} catch ( Exception e )	{ Log.Error( e ); }

			try {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M5" );
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
				new ReRegisterSMObject( o, o._type, o._scene ).Run().Forget();
			} catch ( Exception e )	{ Log.Error( e ); }

			await UTask.Never( _asyncCanceler );
		} );
	}
}