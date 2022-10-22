//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Test {
	using System;
	using System.Linq;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;



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
			SMLog.Debug( _taskManager );
			SMLog.Debug( SMTaskManager.CREATE_TASK_TYPES.ToShowString() );
			SMLog.Debug( SMTaskManager.DISPOSE_TASK_TYPES.ToShowString() );
			SMLog.Debug( SMTaskManager.RUN_TASK_TYPES.ToShowString() );
			SMLog.Debug( SMTaskManager.DISPOSE_RUN_TASK_TYPES.ToShowString() );
			SMServiceLocator.Unregister<SMTaskManager>();
			SMLog.Debug( _taskManager );

			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetAlls() => From( async () => {
			SMLog.Debug( _taskManager.GetAlls().ToShowString( 0, true ) );
			SMServiceLocator.Unregister<SMTaskManager>();
			SMLog.Debug( _taskManager.GetAlls().ToShowString( 0, true ) );

			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRegister() => From( async () => {
			await _taskManager.Register(
				new SMTestTask( "1", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup ), false );
			SMLog.Debug( _taskManager );

			await _taskManager.Initialize();
			await _taskManager.Register(
				new SMTestTask( "2", SMTaskRunType.Parallel, false, false, SMTestTaskLogType.Setup ), true );
			SMLog.Debug( _taskManager );

			_taskManager.Register(
				new SMTestTask( "3", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup ), false
			).Forget();
			_taskManager.Register(
				new SMTestTask( "4", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup ), false
			).Forget();
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
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestUnregister() => From( async () => {
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
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCreateTask() => From( async () => {
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
			var t3 = new SMTestTask( "3", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			try {
				await _taskManager.CreateTask( t3 );
			} catch ( Exception e ) { SMLog.Error( e ); }

			t1.Dispose();
			t2.Dispose();
			t3.Dispose();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestSelfInitializeTask() => From( async () => {
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
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestInitializeTask() => From( async () => {
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
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestInitialEnableTask() => From( async () => {
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
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestFinalDisableTask() => From( async () => {
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
			var t2 = new SMTestTask( "2_1", SMTaskRunType.Dont, true, false, SMTestTaskLogType.Setup );
			await _taskManager._modifyler.WaitRunning();
			try {
				await _taskManager.FinalDisableTask( t2 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			SMServiceLocator.Unregister<SMTaskManager>();
			var t3 = new SMTestTask( "3_1", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup );
			try {
				await _taskManager.FinalDisableTask( t3 );
			} catch ( Exception e ) { SMLog.Error( e ); }

			t1.Dispose();
			t2.Dispose();
			t3.Dispose();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestFinalizeTask() => From( async () => {
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
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDisposeTask() => From( async () => {
			var t1 = new SMTestTask( "1", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "2", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "3", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "4", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "5", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "6", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "7", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "8", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
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
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "10", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.FinalizeTask( t1 );
			await _taskManager.DisposeTask( t1 );

			try {
				await _taskManager.DisposeTask( t1 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				await _taskManager.DisposeTask( null );
			} catch ( Exception e ) { SMLog.Error( e ); }
			SMServiceLocator.Unregister<SMTaskManager>();
			var t2 = new SMTestTask( "2_1", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup );
			try {
				await _taskManager.DisposeTask( t2 );
			} catch ( Exception e ) { SMLog.Error( e ); }

			t1.Dispose();
			t2.Dispose();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestUpdate() => From( async () => {
			var t1 = new SMTestTask( "1", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.All );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.DelayFrame( _asyncCanceler, 3 );
			await _taskManager.DisableTask( t1 );
			await UTask.DelayFrame( _asyncCanceler, 10 );
			await _taskManager.EnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.FinalizeTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "2", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.All );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.DelayFrame( _asyncCanceler, 3 );
			t1._updateEvent.AddLast().Subscribe( _ => SMServiceLocator.Unregister<SMTaskManager>() );
			await UTask.DelayFrame( _asyncCanceler, 3 );
			SMLog.Debug( "待機完了" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestEnableTask() => From( async () => {
			var t1 = new SMTestTask( "1", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.EnableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "2", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.EnableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "3", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.EnableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "4", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.EnableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "5", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.EnableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "6", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.EnableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "7", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.EnableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "8", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.EnableTask( t1 );
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
			await _taskManager.EnableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "10", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.FinalizeTask( t1 );
			await _taskManager.EnableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "11", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.EnableTask( t1 );
			await _taskManager.EnableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "12", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.DisableTask( t1 );
			await _taskManager.EnableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "13", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.EnableTask( t1 );
			await _taskManager.DisableTask( t1 );
			await _taskManager.EnableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "14", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			t1._isRequestInitialEnable = false;
			await _taskManager.EnableTask( t1 );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "15", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			t1._isRequestInitialEnable = false;
			await _taskManager._modifyler.WaitRunning();
			try {
				await UTask.NextFrame( t1._asyncCancelerOnDisable );
			} catch ( Exception e ) { SMLog.Error( e ); }
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			try {
				await UTask.NextFrame( t1._asyncCancelerOnDisable );
			} catch ( Exception e ) { SMLog.Error( e ); }
			await _taskManager.EnableTask( t1 );
			await UTask.NextFrame( t1._asyncCancelerOnDisable );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "16", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			t1._isRequestInitialEnable = false;
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.ChangeActiveTask( t1, true );
			await _taskManager.DisposeTask( t1 );

			try {
				await _taskManager.EnableTask( t1 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				await _taskManager.EnableTask( null );
			} catch ( Exception e ) { SMLog.Error( e ); }
			var t2 = new SMTestTask( "2_1", SMTaskRunType.Dont, true, false, SMTestTaskLogType.Setup );
			await _taskManager._modifyler.WaitRunning();
			try {
				await _taskManager.EnableTask( t2 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			SMServiceLocator.Unregister<SMTaskManager>();
			var t3 = new SMTestTask( "3_1", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup );
			try {
				await _taskManager.EnableTask( t3 );
			} catch ( Exception e ) { SMLog.Error( e ); }

			t1.Dispose();
			t2.Dispose();
			t3.Dispose();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDisableTask() => From( async () => {
			var t1 = new SMTestTask( "1", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.DisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "2", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.DisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "3", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.DisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "4", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.DisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "5", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.DisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "6", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.DisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "7", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.DisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "8", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.DisableTask( t1 );
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
			await _taskManager.DisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "10", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.FinalizeTask( t1 );
			await _taskManager.DisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "11", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.DisableTask( t1 );
			await _taskManager.DisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "12", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.EnableTask( t1 );
			await _taskManager.DisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "13", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.DisableTask( t1 );
			await _taskManager.EnableTask( t1 );
			await _taskManager.DisableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "14", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			t1._isRequestInitialEnable = true;
			await _taskManager.DisableTask( t1 );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "15", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			t1._isRequestInitialEnable = true;
			await _taskManager._modifyler.WaitRunning();
			try {
				await UTask.NextFrame( t1._asyncCancelerOnDisable );
			} catch ( Exception e ) { SMLog.Error( e ); }
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( t1._asyncCancelerOnDisable );
			await _taskManager.DisableTask( t1 );
			try {
				await UTask.NextFrame( t1._asyncCancelerOnDisable );
			} catch ( Exception e ) { SMLog.Error( e ); }
			await _taskManager.DisposeTask( t1 );

			t1 = new SMTestTask( "16", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			t1._isRequestInitialEnable = true;
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.ChangeActiveTask( t1, false );
			await _taskManager.DisposeTask( t1 );

			try {
				await _taskManager.DisableTask( t1 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				await _taskManager.DisableTask( null );
			} catch ( Exception e ) { SMLog.Error( e ); }
			var t2 = new SMTestTask( "2_1", SMTaskRunType.Dont, true, false, SMTestTaskLogType.Setup );
			await _taskManager._modifyler.WaitRunning();
			try {
				await _taskManager.DisableTask( t2 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			SMServiceLocator.Unregister<SMTaskManager>();
			var t3 = new SMTestTask( "3_1", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup );
			try {
				await _taskManager.DisableTask( t3 );
			} catch ( Exception e ) { SMLog.Error( e ); }

			t1.Dispose();
			t2.Dispose();
			t3.Dispose();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestAdjustRunTask() => From( async () => {
			var t1 = new SMTestTask( "1_1", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			var t2 = new SMTestTask( "2_1", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.DisposeTask( t2 );

			t2 = new SMTestTask( "2_2", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.DisposeTask( t2 );

			t2 = new SMTestTask( "2_3", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.DisposeTask( t2 );

			t2 = new SMTestTask( "2_4", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.DisposeTask( t2 );

			t2 = new SMTestTask( "2_5", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.DisposeTask( t2 );

			t2 = new SMTestTask( "2_6", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.DisposeTask( t2 );

			t2 = new SMTestTask( "2_7", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.DisposeTask( t2 );

			t2 = new SMTestTask( "2_8", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.DisposeTask( t2 );

			t2 = new SMTestTask( "2_9", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.DisableTask( t1 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.DisposeTask( t2 );

			t2 = new SMTestTask( "2_10", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.EnableTask( t1 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.DisposeTask( t2 );

			t2 = new SMTestTask( "2_11", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.CreateTask( t2 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.DisposeTask( t2 );

			t2 = new SMTestTask( "2_12", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.FinalizeTask( t1 );
			await _taskManager.CreateTask( t2 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.AdjustRunTask( t2 );
			await _taskManager.DisposeTask( t2 );

			t2 = new SMTestTask( "2_13", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			_taskManager.DisposeTask( t1 ).Forget();
			await _taskManager.AdjustRunTask( t2 );

			t2 = new SMTestTask( "2_14", SMTaskRunType.Parallel, true, false, SMTestTaskLogType.Setup );
			var t3 = new SMTestTask( "3_1", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t3 );
			await _taskManager.SelfInitializeTask( t3 );
			await _taskManager.InitializeTask( t3 );
			await _taskManager.InitialEnableTask( t3 );
			await _taskManager.AdjustRunTask( t2, t3 );
			await _taskManager.DisposeTask( t2 );

			t2 = new SMTestTask( "2_15", SMTaskRunType.Dont, true, false, SMTestTaskLogType.Setup );
			await _taskManager.AdjustRunTask( t2, t3 );
			await _taskManager.AdjustRunTask( t2, t3 );
			await _taskManager.DisposeTask( t2 );

			t2 = new SMTestTask( "2_16", SMTaskRunType.Dont, true, false, SMTestTaskLogType.Setup );
			_taskManager.DisposeTask( t3 ).Forget();
			await _taskManager.AdjustRunTask( t2, t3 );

			try {
				await _taskManager.AdjustRunTask( t1 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				await _taskManager.AdjustRunTask( null );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				await _taskManager.AdjustRunTask( _taskManager.GetAlls().FirstOrDefault() );
			} catch ( Exception e ) { SMLog.Error( e ); }
			SMServiceLocator.Unregister<SMTaskManager>();
			var t4 = new SMTestTask( "4_1", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup );
			try {
				await _taskManager.AdjustRunTask( t4 );
			} catch ( Exception e ) { SMLog.Error( e ); }

			t1.Dispose();
			t2.Dispose();
			t3.Dispose();
			t4.Dispose();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDestroyTask() => From( async () => {
			var t1 = new SMTestTask( "1", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.DestroyTask( t1 );

			t1 = new SMTestTask( "2", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.DestroyTask( t1 );

			t1 = new SMTestTask( "3", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.DestroyTask( t1 );

			t1 = new SMTestTask( "4", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.DestroyTask( t1 );

			t1 = new SMTestTask( "5", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await _taskManager.DestroyTask( t1 );

			t1 = new SMTestTask( "6", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.DestroyTask( t1 );

			t1 = new SMTestTask( "7", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.DestroyTask( t1 );

			t1 = new SMTestTask( "8", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.DestroyTask( t1 );

			t1 = new SMTestTask( "9", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.DestroyTask( t1 );

			t1 = new SMTestTask( "10", SMTaskRunType.Sequential, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t1 );
			await _taskManager.SelfInitializeTask( t1 );
			await _taskManager.InitializeTask( t1 );
			await _taskManager.InitialEnableTask( t1 );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await UTask.NextFrame( _asyncCanceler );
			await _taskManager.FinalDisableTask( t1 );
			await _taskManager.FinalizeTask( t1 );
			await _taskManager.DestroyTask( t1 );

			var t2 = new SMTestTask( "2_1", SMTaskRunType.Dont, true, false, SMTestTaskLogType.Setup );
			await _taskManager.DestroyTask( t2 );

			t2 = new SMTestTask( "2_2", SMTaskRunType.Dont, true, false, SMTestTaskLogType.Setup );
			await _taskManager.CreateTask( t2 );
			await _taskManager.DestroyTask( t2 );

			try {
				await _taskManager.DestroyTask( t1 );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				await _taskManager.DestroyTask( null );
			} catch ( Exception e ) { SMLog.Error( e ); }
			SMServiceLocator.Unregister<SMTaskManager>();
			var t3 = new SMTestTask( "3_1", SMTaskRunType.Sequential, false, false, SMTestTaskLogType.Setup );
			try {
				await _taskManager.DestroyTask( t3 );
			} catch ( Exception e ) { SMLog.Error( e ); }

			t1.Dispose();
			t2.Dispose();
			t3.Dispose();
		} );
	}
}