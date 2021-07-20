//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask {
	using System;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using KoganeUnityLib;
	using Service;
	using Task;
	using Utility;
	using Debug;
	using TestBase;



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

			using ( var t = new SMTestTask( "1", SMTaskRunType.Dont, false, false, true ) ) {
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

			SMTaskManager.CREATE_TASK_TYPES.ForEach( ( t, i ) => {
				new SMTestTask( $"1_{i}", t, true, true, true );
				SMLog.Debug( _taskManager );
			} );

			SMTaskManager.CREATE_TASK_TYPES.ForEach( ( t, i ) => {
				new SMTestTask( $"2_{i}", t, true, false, true );
				SMLog.Debug( _taskManager );
			} );

			SMTaskManager.CREATE_TASK_TYPES.ForEach( ( t, i ) => {
				using ( var task = new SMTestTask( $"3_{i}", t, false, false, true ) ) {
					SMLog.Debug( _taskManager );
				}
			} );

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestLink() => From( async () => {
			SMLog.Warning( "Start" );

			var t1 = new SMTestTask( "1", SMTaskRunType.Dont, false, false, true );
			var t2 = new SMTestTask( "2", SMTaskRunType.Dont, false, false, true );
			t1.Link( t2 );
			t1.Unlink();

			t1.Dispose();
			try {
				t1.Link( t2 );
			} catch ( Exception e )	{ SMLog.Error( e ); }

			t2.Dispose();
			try {
				t1.Link( t2 );
			} catch ( Exception e ) { SMLog.Error( e ); }

			t1 = new SMTestTask( "1", SMTaskRunType.Dont, false, false, true );
			try {
				t1.Link( t2 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			t1.Dispose();

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDestroy() => From( async () => {
			SMLog.Warning( "Start" );

			var t = new SMTestTask( "1", SMTaskRunType.Sequential, true, false, true );
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

			var t = new SMTestTask( "1", SMTaskRunType.Sequential, true, false, true );
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