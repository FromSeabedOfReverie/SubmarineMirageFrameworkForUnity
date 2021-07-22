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
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Service;
	using Task;
	using Extension;
	using Utility;
	using Debug;
	using TestBase;



	public class TestSMTaskManager : SMStandardTest {
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

			SMLog.Debug( _taskManager );
			SMLog.Debug( SMTaskManager.CREATE_TASK_TYPES.ToShowString() );
			SMLog.Debug( SMTaskManager.DISPOSE_TASK_TYPES.ToShowString() );
			SMLog.Debug( SMTaskManager.RUN_TASK_TYPES.ToShowString() );
			SMLog.Debug( SMTaskManager.DISPOSE_RUN_TASK_TYPES.ToShowString() );
			SMServiceLocator.Unregister<SMTaskManager>();
			SMLog.Debug( _taskManager );

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetAlls() => From( async () => {
			SMLog.Warning( "Start" );

			SMLog.Debug( _taskManager.GetAlls().ToShowString( 0, true ) );
			SMServiceLocator.Unregister<SMTaskManager>();
			try {
				SMLog.Debug( _taskManager.GetAlls().ToShowString( 0, true ) );
			} catch ( Exception e ) { SMLog.Error( e ); }

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRegister() => From( async () => {
			SMLog.Warning( "Start" );
			await _taskManager.Initialize();

			await _taskManager.Register(
				new SMTestTask( "1", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup ), false );
			SMLog.Debug( _taskManager );
			await _taskManager.Register(
				new SMTestTask( "2", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup ), true );
			SMLog.Debug( _taskManager );

			_taskManager.Register(
				new SMTestTask( "3", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup ), false ).Forget();
			_taskManager.Register(
				new SMTestTask( "4", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup ), false ).Forget();
			SMLog.Debug( _taskManager );
			SMServiceLocator.Unregister<SMTaskManager>();
			SMLog.Debug( _taskManager );

			var t = new SMTestTask( "5", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup );
			try {
				await _taskManager.Register( t, false );
			} catch ( Exception e ) { SMLog.Error( e ); }
			t.Dispose();
			try {
				await _taskManager.Register( t, false );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				await _taskManager.Register( null, false );
			} catch ( Exception e ) { SMLog.Error( e ); }

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestUnregister() => From( async () => {
			SMLog.Warning( "Start" );

			var t = new SMTestTask( "1", SMTaskRunType.Dont, true, false, SMTestTaskLogType.Setup );
			await _taskManager._modifyler.WaitRunning();
			SMLog.Debug( _taskManager );
			await _taskManager.Unregister( t );
			SMLog.Debug( _taskManager );
			await _taskManager.Unregister( t );
			SMLog.Debug( _taskManager );

			await _taskManager.Register( t, false );
			t.Dispose();
			await _taskManager.Unregister( t );
			await _taskManager.Unregister( t );

			try {
				await _taskManager.Unregister( null );
			} catch ( Exception e ) { SMLog.Error( e ); }
			SMServiceLocator.Unregister<SMTaskManager>();
			await _taskManager.Unregister( t );
			await _taskManager.Unregister( null );

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCreateTask() => From( async () => {
			SMLog.Warning( "Start" );

			var t1 = new SMTestTask( "1", SMTaskRunType.Dont, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );

			var t2 = new SMTestTask( "2", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t2 );
			await _taskManager.CreateTask( t2 );
			await _taskManager.SelfInitializeTask( t2 );
			await _taskManager.CreateTask( t2 );
			await _taskManager.InitializeTask( t2 );
			await _taskManager.CreateTask( t2 );
			await _taskManager.InitialEnableTask( t2 );
			await _taskManager.CreateTask( t2 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.CreateTask( t2 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.CreateTask( t2 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.CreateTask( t2 );
			await _taskManager.FinalDisableTask( t2 );
			await _taskManager.CreateTask( t2 );
			await _taskManager.FinalizeTask( t2 );
			await _taskManager.CreateTask( t2 );
			await _taskManager.DisposeTask( t2 );

			try {
				await _taskManager.CreateTask( t2 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				await _taskManager.CreateTask( null );
			} catch ( Exception e ) { SMLog.Error( e ); }
			SMServiceLocator.Unregister<SMTaskManager>();
			try {
				await _taskManager.CreateTask( t1 );
			} catch ( Exception e ) { SMLog.Error( e ); }

			t1.Dispose();
			t2.Dispose();

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestSelfInitializeTask() => From( async () => {
			SMLog.Warning( "Start" );

			var t1 = new SMTestTask( "1", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.SelfInitializeTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.SelfInitializeTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.FinalizeTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.DisposeTask( t1 );

			try {
				await _taskManager.SelfInitializeTask( t1 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				await _taskManager.SelfInitializeTask( null );
			} catch ( Exception e ) { SMLog.Error( e ); }
			var t2 = new SMTestTask( "2", SMTaskRunType.Dont, true, false, SMTestTaskLogType.Setup );
			await _taskManager._modifyler.WaitRunning();
			try {
				await _taskManager.SelfInitializeTask( t2 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			SMServiceLocator.Unregister<SMTaskManager>();
			var t3 = new SMTestTask( "3", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup );
			try {
				await _taskManager.SelfInitializeTask( t3 );
			} catch ( Exception e ) { SMLog.Error( e ); }

			t1.Dispose();
			t2.Dispose();
			t3.Dispose();

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestInitializeTask() => From( async () => {
			SMLog.Warning( "Start" );

			var t1 = new SMTestTask( "1", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.CreateTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.InitializeTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.InitializeTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.FinalizeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.DisposeTask( t1 );

			try {
				await _taskManager.InitializeTask( t1 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				await _taskManager.InitializeTask( null );
			} catch ( Exception e ) { SMLog.Error( e ); }
			var t2 = new SMTestTask( "2", SMTaskRunType.Dont, true, false, SMTestTaskLogType.Setup );
			await _taskManager._modifyler.WaitRunning();
			try {
				await _taskManager.InitializeTask( t2 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			SMServiceLocator.Unregister<SMTaskManager>();
			var t3 = new SMTestTask( "3", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup );
			try {
				await _taskManager.InitializeTask( t3 );
			} catch ( Exception e ) { SMLog.Error( e ); }

			t1.Dispose();
			t2.Dispose();
			t3.Dispose();

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestInitialEnableTask() => From( async () => {
			SMLog.Warning( "Start" );

			var t0 = new SMTestTask( "0", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			t0._isRequestInitialEnable = false;
			await _taskManager.CreateTask( t0 );
			await _taskManager.SelfInitializeTask( t0 );
			await _taskManager.InitializeTask( t0 );
			await _taskManager.InitialEnableTask( t0 );

			var t1 = new SMTestTask( "1", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.CreateTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.FinalizeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			try {
				await _taskManager.InitialEnableTask( t1 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				await _taskManager.InitialEnableTask( null );
			} catch ( Exception e ) { SMLog.Error( e ); }
			var t2 = new SMTestTask( "2", SMTaskRunType.Dont, true, false, SMTestTaskLogType.Setup );
			await _taskManager._modifyler.WaitRunning();
			try {
				await _taskManager.InitialEnableTask( t2 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			SMServiceLocator.Unregister<SMTaskManager>();
			var t3 = new SMTestTask( "3", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup );
			try {
				await _taskManager.InitialEnableTask( t3 );
			} catch ( Exception e ) { SMLog.Error( e ); }

			t0.Dispose();
			t1.Dispose();
			t2.Dispose();
			t3.Dispose();

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestFinalDisableTask() => From( async () => {
			SMLog.Warning( "Start" );

			var t1 = new SMTestTask( "1", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "2", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "3", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "4", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "5", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "6", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "7", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "8", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "9", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.FinalizeTask( t1 );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			try {
				await _taskManager.FinalDisableTask( t1 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				await _taskManager.FinalDisableTask( null );
			} catch ( Exception e ) { SMLog.Error( e ); }
			var t2 = new SMTestTask( "2", SMTaskRunType.Dont, true, false, SMTestTaskLogType.Setup );
			await _taskManager._modifyler.WaitRunning();
			try {
				await _taskManager.FinalDisableTask( t2 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			SMServiceLocator.Unregister<SMTaskManager>();
			var t3 = new SMTestTask( "3", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup );
			try {
				await _taskManager.FinalDisableTask( t3 );
			} catch ( Exception e ) { SMLog.Error( e ); }

			t1.Dispose();
			t2.Dispose();
			t3.Dispose();

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestFinalizeTask() => From( async () => {
			SMLog.Warning( "Start" );

			var t1 = new SMTestTask( "1", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.FinalizeTask( t1 );
			await _taskManager.CreateTask( t1 );
			await _taskManager.FinalizeTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.FinalizeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.FinalizeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.FinalizeTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.FinalizeTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.FinalizeTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.FinalizeTask( t1 );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.FinalizeTask( t1 );
			await _taskManager.FinalizeTask( t1 );
			await _taskManager.DisposeTask( t1 );

			try {
				await _taskManager.FinalizeTask( t1 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				await _taskManager.FinalizeTask( null );
			} catch ( Exception e ) { SMLog.Error( e ); }
			var t2 = new SMTestTask( "2", SMTaskRunType.Dont, true, false, SMTestTaskLogType.Setup );
			await _taskManager._modifyler.WaitRunning();
			try {
				await _taskManager.FinalizeTask( t2 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			SMServiceLocator.Unregister<SMTaskManager>();
			var t3 = new SMTestTask( "3", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup );
			try {
				await _taskManager.FinalizeTask( t3 );
			} catch ( Exception e ) { SMLog.Error( e ); }

			t1.Dispose();
			t2.Dispose();
			t3.Dispose();

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



// TODO : TestDisposeTaskから再開
	}
}