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
	using Task.Marker;
	using Extension;
	using Utility;
	using Debug;
	using TestBase;



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

			var markers1 = new SMTaskMarkerManager( "1" );
			SMLog.Debug( _taskManager );
			markers1.Dispose();
			SMLog.Debug( _taskManager );

			var markers2 = new SMTaskMarkerManager( "2" );
			SMLog.Debug( _taskManager );
			var markers3 = new SMTaskMarkerManager( "3" );
			SMLog.Debug( _taskManager );
			markers2.Dispose();
			SMLog.Debug( _taskManager );
			markers3.Dispose();
			SMLog.Debug( _taskManager );

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestLinkLast() => From( async () => {
			SMLog.Warning( "Start" );

			var markers1 = new SMTaskMarkerManager( "1" );
			SMLog.Debug( _taskManager );
			SMTaskManager.CREATE_TASK_TYPES.ForEach( ( t, i ) => {
				markers1.LinkLast( new SMTestTask( $"Test1_{i}", t, false, false, true ) );
				SMLog.Debug( _taskManager );
			} );

			var markers2 = new SMTaskMarkerManager( "2" );
			SMLog.Debug( _taskManager );
			SMTaskManager.CREATE_TASK_TYPES.ForEach( ( t, i ) => {
				markers2.LinkLast( new SMTestTask( $"Test2_{i}", t, false, false, true ) );
				SMLog.Debug( _taskManager );
			} );

			try {
				markers2.LinkLast( null );
			} catch ( Exception e )	{ SMLog.Error( e ); }

			markers1.Dispose();
			SMLog.Debug( _taskManager );
			markers2.Dispose();
			SMLog.Debug( _taskManager );

			using ( var t = new SMTestTask( $"Test", SMTaskRunType.Dont, false, false, true ) ) {
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

			var markers = new SMTaskMarkerManager( "1" );
			SMTaskManager.CREATE_TASK_TYPES.ForEach( t => {
				SMLog.Debug( markers.GetFirst( t, false ) );
				SMLog.Debug( markers.GetFirst( t, true ) );
				SMLog.Debug( markers.GetLast( t, false ) );
				SMLog.Debug( markers.GetLast( t, true ) );
				SMLog.Debug( markers.GetAlls( t, false ).ToShowString() );
				SMLog.Debug( markers.GetAlls( t, true ).ToShowString() );
			} );
			SMLog.Debug( markers.GetAlls( false ).ToShowString() );
			SMLog.Debug( markers.GetAlls( true ).ToShowString() );

			SMTaskManager.CREATE_TASK_TYPES.ForEach( ( t, i ) => {
				markers.LinkLast( new SMTestTask( $"Test1_{i}", t, false, false, true ) );
				SMLog.Debug( markers.GetFirst( t, false ) );
				SMLog.Debug( markers.GetFirst( t, true ) );
				SMLog.Debug( markers.GetLast( t, false ) );
				SMLog.Debug( markers.GetLast( t, true ) );
				SMLog.Debug( markers.GetAlls( t, false ).ToShowString() );
				SMLog.Debug( markers.GetAlls( t, true ).ToShowString() );
			} );
			SMLog.Debug( markers.GetAlls( false ).ToShowString() );
			SMLog.Debug( markers.GetAlls( true ).ToShowString() );

			markers.Dispose();
			SMTaskManager.CREATE_TASK_TYPES.ForEach( t => {
				SMLog.Debug( markers.GetFirst( t, false ) );
				SMLog.Debug( markers.GetFirst( t, true ) );
				SMLog.Debug( markers.GetLast( t, false ) );
				SMLog.Debug( markers.GetLast( t, true ) );
				SMLog.Debug( markers.GetAlls( t, false ).ToShowString() );
				SMLog.Debug( markers.GetAlls( t, true ).ToShowString() );
			} );
			SMLog.Debug( markers.GetAlls( false ).ToShowString() );
			SMLog.Debug( markers.GetAlls( true ).ToShowString() );

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestInitializeAndFinalizeAll() => From( async () => {
			SMLog.Warning( "Start" );

			var markers1 = new SMTaskMarkerManager( "1" );
			SMTaskManager.CREATE_TASK_TYPES.ForEach( ( t, i ) => {
				markers1.LinkLast( new SMTestTask( $"Test1_{i}", t, false, false, true ) );
			} );
			var markers2 = new SMTaskMarkerManager( "2" );
			SMTaskManager.CREATE_TASK_TYPES.ForEach( ( t, i ) => {
				markers2.LinkLast( new SMTestTask( $"Test2_{i}", t, false, false, true ) );
			} );
			SMLog.Debug( _taskManager );
			_taskManager._modifyler._isDebug = true;

			await markers1.InitializeAll();
			SMLog.Debug( _taskManager );
			await markers1.InitializeAll();
			SMLog.Debug( _taskManager );
			await markers2.InitializeAll();
			SMLog.Debug( _taskManager );

			await markers1.FinalizeAll();
			SMLog.Debug( _taskManager );
			await markers1.FinalizeAll();
			SMLog.Debug( _taskManager );
			await markers2.FinalizeAll();
			SMLog.Debug( _taskManager );

			await markers1.InitializeAll();
			SMLog.Debug( _taskManager );
			await markers1.InitializeAll();
			SMLog.Debug( _taskManager );
			await markers2.InitializeAll();
			SMLog.Debug( _taskManager );

			markers1.Dispose();
			SMLog.Debug( _taskManager );
			markers2.Dispose();
			SMLog.Debug( _taskManager );

			try {
				await markers1.InitializeAll();
			} catch ( Exception e )	{ SMLog.Error( e ); }
			try {
				await markers1.FinalizeAll();
			} catch ( Exception e ) { SMLog.Error( e ); }

			SMLog.Warning( "End" );
		} );
	}
}