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
	using KoganeUnityLib;
	using Task;
	using Task.Modifyler;
	using Scene;
	using Utility;
	using Debug;
	using TestBase;



	// TODO : コメント追加、整頓



	public class TestDestroySMObject : SMStandardTest {
		protected override void Create() {
			Application.targetFrameRate = 30;

			_createEvent.AddLast( async canceler => {
				SMLog.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestUnLink ):	CreateTestUnLink();	break;
					case nameof( TestRun1 ):	CreateTestRun1();	break;
					case nameof( TestRun2 ):	CreateTestRun2();	break;
					case nameof( TestRun3 ):	CreateTestRun3();	break;
					case nameof( TestError ):	CreateTestError();	break;
				}
				SMLog.Debug( $"end {nameof( Create )}{_testName}" );

				await UTask.DontWait();
			} );
		}


/*
		・変更者テスト
		Run、_object、_top、_modifyler、確認
*/
		void CreateTestUnLink() => TestSMBehaviourSMUtility.CreateBehaviours( @"
			M1,
				M2,
					M6,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestUnLink() => From( async () => {
			await UTask.NextFrame( _asyncCanceler );
			SMLog.Debug( $"{nameof( TestUnLink )}" );

			var o = SMSceneManager.s_instance.GetBehaviour<M1>()._object;
			TestSMBehaviourSMUtility.SetEvent( o._behaviour );
			TestSMBehaviourSMUtility.SetEvent( o._child._behaviour );
			TestSMBehaviourSMUtility.SetEvent( o._child._child._behaviour );
			o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.Create ) );
			o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.Initializing ) );
			o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.SelfInitializing ) );
			o._modifyler.Register( new RunInitialActiveSMObject( o ) );
			await o._modifyler.WaitRunning();

			3.Times( () => o._modifyler.Register( new RunData( o ) ) );
			3.Times( () => o._modifyler.Register( new RunData( o._child ) ) );
			3.Times( () => o._modifyler.Register( new RunData( o._child._child ) ) );

			o._modifyler.Register( new DestroySMObject( o._child._child ) );
			o._modifyler.Register( new DestroySMObject( o._child ) );
			o._modifyler.Register( new DestroySMObject( o ) );

			await UTask.Never( _asyncCanceler );
		} );


/*
		・実行物テスト
		RunObject、_modifyler、逆順でもちゃんと消される？
*/
		void CreateTestRun1() => TestSMBehaviourSMUtility.CreateBehaviours( @"
			M3,
				M3,
					M3,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRun1() => From( async () => {
			await UTask.NextFrame( _asyncCanceler );
			SMLog.Debug( $"{nameof( TestRun1 )}" );

			var o = SMSceneManager.s_instance.GetBehaviour<M3>()._object;
			TestSMBehaviourSMUtility.SetEvent( o._behaviour );
			TestSMBehaviourSMUtility.SetEvent( o._child._behaviour );
			TestSMBehaviourSMUtility.SetEvent( o._child._child._behaviour );
			o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.Create ) );
			o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.Initializing ) );
			o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.SelfInitializing ) );
			o._modifyler.Register( new RunInitialActiveSMObject( o ) );
			await o._modifyler.WaitRunning();

			3.Times( () => o._modifyler.Register( new RunData( o ) ) );
			3.Times( () => o._modifyler.Register( new RunData( o._child ) ) );
			3.Times( () => o._modifyler.Register( new RunData( o._child._child ) ) );

			o._modifyler.Register( new DestroySMObject( o._child ) );
			o._modifyler.Register( new DestroySMObject( o._child._child ) );
			o._modifyler.Register( new DestroySMObject( o ) );

			await UTask.Never( _asyncCanceler );
		} );


/*
		・実行物テスト
		RunObject、実行されるか？、実行中停止でもfinally呼ばれる？
*/
		void CreateTestRun2() => TestSMBehaviourSMUtility.CreateBehaviours( @"
			M3,
				M3,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRun2() => From( async () => {
			await UTask.NextFrame( _asyncCanceler );
			SMLog.Debug( $"{nameof( TestRun2 )}" );

			var o = SMSceneManager.s_instance.GetBehaviour<M3>()._object;
			TestSMBehaviourSMUtility.SetEvent( o._behaviour );
			TestSMBehaviourSMUtility.SetEvent( o._child._behaviour );
			o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.Create ) );
			o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.Initializing ) );
			o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.SelfInitializing ) );
			o._modifyler.Register( new RunInitialActiveSMObject( o ) );
			await o._modifyler.WaitRunning();

			3.Times( () => o._modifyler.Register( new RunData( o ) ) );
			3.Times( () => o._modifyler.Register( new RunData( o._child ) ) );

			UTask.Void( async () => {
				var c = o._child;
				o._modifyler.Register( new DestroySMObject( o._child ) );
				await UTask.Delay( _asyncCanceler, 1500 );
				SMLog.Debug( "停止" );
				c._behaviour._asyncCancelerOnDispose.Cancel();
				await UTask.NextFrame( _asyncCanceler );
				SMLog.Debug( "停止" );
				c._behaviour._asyncCancelerOnDispose.Cancel();
			} );

			o._modifyler.Register( new DestroySMObject( o ) );

			await UTask.Never( _asyncCanceler );
		} );


/*
		・実行物テスト
		RunObject、実行中に親Disposeでもfinally呼ばれる？
*/
		void CreateTestRun3() => TestSMBehaviourSMUtility.CreateBehaviours( @"
			M3,
				M3,
					M3,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRun3() => From( async () => {
			await UTask.NextFrame( _asyncCanceler );
			SMLog.Debug( $"{nameof( TestRun3 )}" );

			var o = SMSceneManager.s_instance.GetBehaviour<M3>()._object;
			TestSMBehaviourSMUtility.SetEvent( o._behaviour );
			TestSMBehaviourSMUtility.SetEvent( o._child._behaviour );
			TestSMBehaviourSMUtility.SetEvent( o._child._child._behaviour );
			o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.Create ) );
			o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.Initializing ) );
			o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.SelfInitializing ) );
			o._modifyler.Register( new RunInitialActiveSMObject( o ) );
			await o._modifyler.WaitRunning();

			3.Times( () => o._modifyler.Register( new RunData( o ) ) );
			3.Times( () => o._modifyler.Register( new RunData( o._child ) ) );
			3.Times( () => o._modifyler.Register( new RunData( o._child._child ) ) );

			UTask.Void( async () => {
				o._modifyler.Register( new DestroySMObject( o._child._child ) );
				await UTask.Delay( _asyncCanceler, 1500 );
				SMLog.Debug( "親解放" );
				o.Dispose();
			} );

			await UTask.Never( _asyncCanceler );
		} );


/*
		・失敗テスト
		DestroySMObject、null、既にリンク切れのobject、確認
*/
		void CreateTestError() => TestSMBehaviourSMUtility.CreateBehaviours( @"
			M3,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestError() => From( async () => {
			await UTask.NextFrame( _asyncCanceler );
			SMLog.Debug( $"{nameof( TestError )}" );

			var o = SMSceneManager.s_instance.GetBehaviour<M3>()._object;
			TestSMBehaviourSMUtility.SetEvent( o._behaviour );
			o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.Create ) );
			o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.Initializing ) );
			o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.SelfInitializing ) );
			o._modifyler.Register( new RunInitialActiveSMObject( o ) );
			await o._modifyler.WaitRunning();

			try {
				o.Dispose();
				await new DestroySMObject( o ).Run();
			} catch ( Exception e )	{ SMLog.Error( e ); }

			try {
				await new DestroySMObject( null ).Run();
			} catch ( Exception e )	{ SMLog.Error( e ); }

			await UTask.Never( _asyncCanceler );
		} );
	}
}