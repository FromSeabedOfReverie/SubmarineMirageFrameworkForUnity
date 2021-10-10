//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;



	public class TestSMTask : SMStandardTest {
		SMTaskManager _taskManager { get; set; }



		protected override void Create() {
			_initializeEvent.AddLast( async c => {
				_taskManager = SMServiceLocator.Resolve<SMTaskManager>();
				await UTask.DontWait();
			} );
		}



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCreate() => From( async () => {
			SMLog.Warning( "Start" );

			using ( var t = new SMTestTask( "1", SMTaskRunType.Dont, false, false, SMTestTaskLogType.Setup ) ) {
				SMLog.Debug( _taskManager );
			}

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRegister() => From( async () => {
			SMLog.Warning( "Start" );

			await _taskManager.Initialize();
			SMLog.Debug( _taskManager );

			for ( var i = 0; i < SMTaskManager.CREATE_TASK_TYPES.Length; i++ ) {
				var t = SMTaskManager.CREATE_TASK_TYPES[i];
				new SMTestTask( $"1_{i}", t, true, true, SMTestTaskLogType.Setup );
				await _taskManager._modifyler.WaitRunning();
				SMLog.Debug( _taskManager );
			}

			for ( var i = 0; i < SMTaskManager.CREATE_TASK_TYPES.Length; i++ ) {
				var t = SMTaskManager.CREATE_TASK_TYPES[i];
				new SMTestTask( $"2_{i}", t, true, false, SMTestTaskLogType.Setup );
				await _taskManager._modifyler.WaitRunning();
				SMLog.Debug( _taskManager );
			}

			for ( var i = 0; i < SMTaskManager.CREATE_TASK_TYPES.Length; i++ ) {
				var t = SMTaskManager.CREATE_TASK_TYPES[i];
				using ( var task = new SMTestTask( $"3_{i}", t, false, false, SMTestTaskLogType.Setup ) ) {
					await _taskManager._modifyler.WaitRunning();
					SMLog.Debug( _taskManager );
				}
			}

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestLink() => From( async () => {
			SMLog.Warning( "Start" );

			var t1 = new SMTestTask( "1", SMTaskRunType.Dont, false, false, SMTestTaskLogType.Setup );
			var t2 = new SMTestTask( "2", SMTaskRunType.Dont, false, false, SMTestTaskLogType.Setup );
			t1.Link( t2 );
			SMLog.Debug( t1 );
			SMLog.Debug( t2 );
			t1.Unlink();

			t1.Dispose();
			try {
				t1.Link( t2 );
				SMLog.Debug( t1 );
				SMLog.Debug( t2 );
			} catch ( Exception e )	{ SMLog.Error( e ); }

			t2.Dispose();
			try {
				t1.Link( t2 );
				SMLog.Debug( t1 );
				SMLog.Debug( t2 );
			} catch ( Exception e ) { SMLog.Error( e ); }

			t1 = new SMTestTask( "1", SMTaskRunType.Dont, false, false, SMTestTaskLogType.Setup );
			try {
				t1.Link( t2 );
				SMLog.Debug( t1 );
				SMLog.Debug( t2 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			t1.Dispose();

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDestroy() => From( async () => {
			SMLog.Warning( "Start" );

			var t = new SMTestTask( "1", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t );
			await t.Destroy();

			try {
				await t.Destroy();
			} catch ( Exception e ) { SMLog.Error( e ); }

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestChangeActive() => From( async () => {
			SMLog.Warning( "Start" );

			var t = new SMTestTask( "1", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await t.ChangeActive( true );

			await t.Destroy();
			try {
				await t.ChangeActive( true );
			} catch ( Exception e ) { SMLog.Error( e ); }

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );
	}
}