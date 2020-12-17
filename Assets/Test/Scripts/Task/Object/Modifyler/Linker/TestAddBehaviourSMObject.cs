//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask.Modifyler {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Task;
	using Task.Modifyler;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using TestBase;



	// TODO : コメント追加、整頓



	public class TestAddBehaviourSMObject : SMStandardTest {
		protected override void Create() {
			Application.targetFrameRate = 30;

			_createEvent.AddLast( async canceler => {
				SMLog.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestRun ):		CreateTestRun();	break;
					case nameof( TestCancel ):	CreateTestCancel();	break;
					case nameof( TestError ):	CreateTestError();	break;
				}
				SMLog.Debug( $"end {nameof( Create )}{_testName}" );

				await UTask.DontWait();
			} );
		}


/*
		・実行テスト
		Run、リンク設定されるか、SMObjectデータ設定されるか、modifyler設定されるか
*/
		void CreateTestRun() => TestSMBehaviourSMUtility.CreateBehaviours( @"
			M1,
			M1,
				M1,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRun() => From( async () => {
			SMLog.Debug( $"{nameof( TestRun )}" );

			async UniTask SetAdds( SMObject smObject ) {
				var data = new LinkedList< Func<UniTask> >();
				new [] { typeof( M1 ), typeof( M2 ), typeof( M3 ), typeof( M6 ) }.ForEach( t => {
					var d = new AddBehaviourSMObject( smObject, t );
					smObject._top._modifyler.Register( d );
					data.Enqueue( async () => {
						await UTask.WaitWhile( _asyncCanceler, () => d._behaviour._body == null );
						TestSMBehaviourSMUtility.SetEvent( d._behaviour );
					} );
				} );
				foreach ( var d in data ) {
					await d();
				}
				await smObject._top._modifyler.WaitRunning();
			}

			SMLog.Debug( "・親に追加テスト" );
			await SetAdds( SMSceneManager.s_instance.GetBehaviour<M1>()._object );

			SMLog.Debug( "・子に追加テスト" );
			await SetAdds( SMSceneManager.s_instance.GetBehaviour<M1, UnknownSMScene>()._object._child );

			await UTask.Never( _asyncCanceler );
		} );


/*
		・停止テスト
		Cancel、確認
*/
		void CreateTestCancel() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCancel() => From( async () => {
			SMLog.Debug( $"{nameof( TestCancel )}" );


			SMLog.Debug( "・生成直後、停止" );
			var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M3" );
			var o = new SMObject( b.gameObject, new [] { b }, null );
			var d = new AddBehaviourSMObject( o, typeof( M3 ) );
			d.Cancel();

			b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
			o = new SMObject( b.gameObject, new [] { b }, null );
			d = new AddBehaviourSMObject( o, typeof( M6 ) );
			d.Cancel();


			SMLog.Debug( "・実行中、停止" );
			b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M3" );
			o = new SMObject( b.gameObject, new [] { b }, null );
			await o._top._modifyler.WaitRunning();
			d = new AddBehaviourSMObject( o, typeof( M3 ) );
			o._top._modifyler.Register( d );
			d.Cancel();

			// _modifylerで、再登録してシーン移動するが、そもそも実行中や実行後に停止できない
			b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
			o = new SMObject( b.gameObject, new [] { b }, null );
			await o._top._modifyler.WaitRunning();
			d = new AddBehaviourSMObject( o, typeof( M6 ) );
			o._top._modifyler.Register( d );
			d.Cancel();


			SMLog.Debug( "・実行後、停止" );
			b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M3" );
			o = new SMObject( b.gameObject, new [] { b }, null );
			d = new AddBehaviourSMObject( o, typeof( M3 ) );
			await d.Run();
			d.Cancel();


			await UTask.Never( _asyncCanceler );
		} );


/*
		・エラーテスト
		AddBehaviourSMObject、SMMonoBehaviour以外、typeを変なのにする
*/
		void CreateTestError() => TestSMBehaviourSMUtility.CreateBehaviours( @"
			M3
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestError() => From( async () => {
			SMLog.Debug( $"{nameof( TestError )}" );

			try {
				var o = new B3()._object;
				TestSMBehaviourSMUtility.SetEvent( o._behaviour );
				o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.Create ) );
				o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.Initializing ) );
				o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.SelfInitializing ) );
				o._modifyler.Register( new RunInitialActiveSMObject( o ) );
				o._modifyler.Register( new AddBehaviourSMObject( o, typeof( M3 ) ) );
				await o._modifyler.WaitRunning();
				o.Dispose();
			} catch ( Exception e )	{ SMLog.Error( e ); }

			try {
				var o = SMSceneManager.s_instance.GetBehaviour<M3>()._object;
				TestSMBehaviourSMUtility.SetEvent( o._behaviour );
				o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.Create ) );
				o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.Initializing ) );
				o._modifyler.Register( new RunStateSMObject( o, SMTaskRunState.SelfInitializing ) );
				o._modifyler.Register( new RunInitialActiveSMObject( o ) );
				o._modifyler.Register( new AddBehaviourSMObject( o, typeof( B3 ) ) );
				await o._modifyler.WaitRunning();
				o.Dispose();
			} catch ( Exception e )	{ SMLog.Error( e ); }

			try {
				await new AddBehaviourSMObject( null, null ).Run();
			} catch ( Exception e )	{ SMLog.Error( e ); }

			await UTask.Never( _asyncCanceler );
		} );
	}
}