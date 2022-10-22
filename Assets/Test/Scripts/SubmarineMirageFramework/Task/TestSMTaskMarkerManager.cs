//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Test {
	using System;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using KoganeUnityLib;



	public class TestSMTaskMarkerManager : SMStandardTest {
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

			var markers1 = new SMTaskMarkerManager( "1", _taskManager );
			await _taskManager._modifyler.WaitRunning();
			SMLog.Debug( _taskManager );

			markers1.Dispose();
			await _taskManager._modifyler.WaitRunning();
			SMLog.Debug( _taskManager );


			var markers2 = new SMTaskMarkerManager( "2", _taskManager );
			await _taskManager._modifyler.WaitRunning();
			SMLog.Debug( _taskManager );

			var markers3 = new SMTaskMarkerManager( "3", _taskManager );
			await _taskManager._modifyler.WaitRunning();
			SMLog.Debug( _taskManager );

			markers2.Dispose();
			await _taskManager._modifyler.WaitRunning();
			SMLog.Debug( _taskManager );

			markers3.Dispose();
			await _taskManager._modifyler.WaitRunning();
			SMLog.Debug( _taskManager );

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestLinkLast() => From( async () => {
			SMLog.Warning( "Start" );

			var markers1 = new SMTaskMarkerManager( "1", _taskManager );
			await _taskManager._modifyler.WaitRunning();
			SMLog.Debug( _taskManager );
			SMTaskManager.CREATE_TASK_TYPES.ForEach( ( t, i ) => {
				markers1.LinkLast( new SMTestTask( $"1_{i}", t, false, false, SMTestTaskLogType.Setup ) );
				SMLog.Debug( _taskManager );
			} );

			var markers2 = new SMTaskMarkerManager( "2", _taskManager );
			await _taskManager._modifyler.WaitRunning();
			SMLog.Debug( _taskManager );
			SMTaskManager.CREATE_TASK_TYPES.ForEach( ( t, i ) => {
				markers2.LinkLast( new SMTestTask( $"2_{i}", t, false, false, SMTestTaskLogType.Setup ) );
				SMLog.Debug( _taskManager );
			} );

			try {
				markers2.LinkLast( null );
			} catch ( Exception e )	{ SMLog.Error( e ); }

			markers1.Dispose();
			await _taskManager._modifyler.WaitRunning();
			SMLog.Debug( _taskManager );

			markers2.Dispose();
			await _taskManager._modifyler.WaitRunning();
			SMLog.Debug( _taskManager );

			using ( var t = new SMTestTask( $"3_0", SMTaskRunType.Dont, false, false, SMTestTaskLogType.Setup ) ) {
				try {
					markers2.LinkLast( t );
				} catch ( Exception e )	{ SMLog.Error( e ); }
			}

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGets() => From( async () => {
			SMLog.Warning( "Start" );

			var markers = new SMTaskMarkerManager( "1", _taskManager );
			await _taskManager._modifyler.WaitRunning();
			SMLog.Debug( _taskManager );
			SMTaskManager.CREATE_TASK_TYPES.ForEach( t => {
				SMLog.Debug( markers.GetFirst( t, false ) );
				SMLog.Debug( markers.GetFirst( t, true ) );
				SMLog.Debug( markers.GetLast( t, false ) );
				SMLog.Debug( markers.GetLast( t, true ) );
				SMLog.Debug( markers.GetAlls( t, false ).ToShowString( 0, true ) );
				SMLog.Debug( markers.GetAlls( t, true ).ToShowString( 0, true ) );
			} );
			SMLog.Debug( markers.GetAlls( false ).ToShowString( 0, true ) );
			SMLog.Debug( markers.GetAlls( true ).ToShowString( 0, true ) );

			SMTaskManager.CREATE_TASK_TYPES.ForEach( ( t, i ) => {
				markers.LinkLast( new SMTestTask( $"1_{i}", t, false, false, SMTestTaskLogType.Setup ) );
				SMLog.Debug( _taskManager );
				SMLog.Debug( markers.GetFirst( t, false ) );
				SMLog.Debug( markers.GetFirst( t, true ) );
				SMLog.Debug( markers.GetLast( t, false ) );
				SMLog.Debug( markers.GetLast( t, true ) );
				SMLog.Debug( markers.GetAlls( t, false ).ToShowString( 0, true ) );
				SMLog.Debug( markers.GetAlls( t, true ).ToShowString( 0, true ) );
			} );
			SMLog.Debug( markers.GetAlls( false ).ToShowString( 0, true ) );
			SMLog.Debug( markers.GetAlls( true ).ToShowString( 0, true ) );

			SMTaskManager.CREATE_TASK_TYPES.ForEach( ( t, i ) => {
				markers.LinkLast( new SMTestTask( $"2_{i}", t, false, false, SMTestTaskLogType.Setup ) );
				SMLog.Debug( _taskManager );
				SMLog.Debug( markers.GetFirst( t, false ) );
				SMLog.Debug( markers.GetFirst( t, true ) );
				SMLog.Debug( markers.GetLast( t, false ) );
				SMLog.Debug( markers.GetLast( t, true ) );
				SMLog.Debug( markers.GetAlls( t, false ).ToShowString( 0, true ) );
				SMLog.Debug( markers.GetAlls( t, true ).ToShowString( 0, true ) );
			} );
			SMLog.Debug( markers.GetAlls( false ).ToShowString( 0, true ) );
			SMLog.Debug( markers.GetAlls( true ).ToShowString( 0, true ) );

			markers.Dispose();
			await _taskManager._modifyler.WaitRunning();
			SMLog.Debug( _taskManager );
			try {
				SMLog.Debug( markers.GetFirst( SMTaskRunType.Dont, false ) );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				SMLog.Debug( markers.GetLast( SMTaskRunType.Dont, false ) );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				SMLog.Debug( markers.GetAlls( SMTaskRunType.Dont, false ).ToShowString( 0, true ) );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				SMLog.Debug( markers.GetAlls( false ).ToShowString( 0, true ) );
			} catch ( Exception e )	{ SMLog.Error( e ); }

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestInitializeAndFinalizeAll() => From( async () => {
			SMLog.Warning( "Start" );


			var markers1 = new SMTaskMarkerManager( "1", _taskManager );
			await _taskManager._modifyler.WaitRunning();
			SMTaskManager.CREATE_TASK_TYPES.ForEach( ( t, i ) => {
				markers1.LinkLast( new SMTestTask( $"1_{i}", t, false, false, SMTestTaskLogType.Setup ) );
			} );
			var markers2 = new SMTaskMarkerManager( "2", _taskManager );
			await _taskManager._modifyler.WaitRunning();
			SMTaskManager.CREATE_TASK_TYPES.ForEach( ( t, i ) => {
				markers2.LinkLast( new SMTestTask( $"2_{i}", t, false, false, SMTestTaskLogType.Setup ) );
			} );
			SMLog.Debug( _taskManager );


			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 100 );
				try {
					await markers1.InitializeAll();
				} catch ( Exception e )	{ SMLog.Error( e ); }
				try {
					await markers1.FinalizeAll();
				} catch ( Exception e ) { SMLog.Error( e ); }
			} );
			await markers1.InitializeAll();
			SMLog.Debug( _taskManager );
			try {
				await markers1.InitializeAll();
			} catch ( Exception e )	{ SMLog.Error( e ); }


			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 100 );
				try {
					await markers1.InitializeAll();
				} catch ( Exception e ) { SMLog.Error( e ); }
				try {
					await markers1.FinalizeAll();
				} catch ( Exception e ) { SMLog.Error( e ); }
			} );
			await markers1.FinalizeAll();
			SMLog.Debug( _taskManager );
			try {
				await markers1.InitializeAll();
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				await markers1.FinalizeAll();
			} catch ( Exception e ) { SMLog.Error( e ); }


			await markers2.FinalizeAll();
			SMLog.Debug( _taskManager );


			SMLog.Warning( "End" );
		} );
	}
}