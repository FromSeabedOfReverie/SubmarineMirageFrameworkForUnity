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
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
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



			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGets() => From( async () => {
			_taskManager._modifyler._isDebug = false;
			SMLog.Warning( "Start" );
			await UTask.WaitWhile( _asyncCanceler, () => Input.GetKeyDown( KeyCode.M ) );

			SMTaskManager.CREATE_TASK_TYPES.ForEach( t => {
				SMLog.Debug( _taskManager.GetFirst( t, false ) );
				SMLog.Debug( _taskManager.GetFirst( t, true ) );
				SMLog.Debug( _taskManager.GetLast( t, false ) );
				SMLog.Debug( _taskManager.GetLast( t, true ) );
				SMLog.Debug( _taskManager.GetAlls( t ).ToShowString() );
			} );
			await UTask.WaitWhile( _asyncCanceler, () => Input.GetKeyDown( KeyCode.M ) );

			SMTaskManager.CREATE_TASK_TYPES.ForEach( ( t, i ) => {
				new SMTestTask( $"Test1_{i}", t, false );
				SMLog.Debug( _taskManager.GetFirst( t, false ) );
				SMLog.Debug( _taskManager.GetFirst( t, true ) );
				SMLog.Debug( _taskManager.GetLast( t, false ) );
				SMLog.Debug( _taskManager.GetLast( t, true ) );
				SMLog.Debug( _taskManager.GetAlls( t ).ToShowString() );
			} );
			await UTask.WaitWhile( _asyncCanceler, () => Input.GetKeyDown( KeyCode.M ) );

			var markerManager = new SMTaskMarkerManager( "Test" );
			SMTaskManager.CREATE_TASK_TYPES.ForEach( t => {
				SMLog.Debug( markerManager.GetFirst( t, false ) );
				SMLog.Debug( markerManager.GetFirst( t, true ) );
				SMLog.Debug( markerManager.GetLast( t, false ) );
				SMLog.Debug( markerManager.GetLast( t, true ) );
				SMLog.Debug( markerManager.GetAlls( t, false ).ToShowString() );
				SMLog.Debug( markerManager.GetAlls( t, true ).ToShowString() );
			} );
			await UTask.WaitWhile( _asyncCanceler, () => Input.GetKeyDown( KeyCode.M ) );

			SMTaskManager.CREATE_TASK_TYPES.ForEach( ( t, i ) => {
				new SMTestTask( $"Test2_{i}", t, false );
				SMLog.Debug( markerManager.GetFirst( t, false ) );
				SMLog.Debug( markerManager.GetFirst( t, true ) );
				SMLog.Debug( markerManager.GetLast( t, false ) );
				SMLog.Debug( markerManager.GetLast( t, true ) );
				SMLog.Debug( markerManager.GetAlls( t, false ).ToShowString() );
				SMLog.Debug( markerManager.GetAlls( t, true ).ToShowString() );
			} );
			await UTask.WaitWhile( _asyncCanceler, () => Input.GetKeyDown( KeyCode.M ) );

			markerManager.Dispose();
			SMTaskManager.CREATE_TASK_TYPES.ForEach( t => {
				SMLog.Debug( markerManager.GetFirst( t, false ) );
				SMLog.Debug( markerManager.GetFirst( t, true ) );
				SMLog.Debug( markerManager.GetLast( t, false ) );
				SMLog.Debug( markerManager.GetLast( t, true ) );
				SMLog.Debug( markerManager.GetAlls( t, false ).ToShowString() );
				SMLog.Debug( markerManager.GetAlls( t, true ).ToShowString() );
			} );
			await UTask.WaitWhile( _asyncCanceler, () => Input.GetKeyDown( KeyCode.M ) );

			SMServiceLocator.Unregister<SMTaskManager>();
			SMTaskManager.CREATE_TASK_TYPES.ForEach( t => {
				SMLog.Debug( _taskManager.GetFirst( t, false ) );
				SMLog.Debug( _taskManager.GetFirst( t, true ) );
				SMLog.Debug( _taskManager.GetLast( t, false ) );
				SMLog.Debug( _taskManager.GetLast( t, true ) );
				SMLog.Debug( _taskManager.GetAlls( t ).ToShowString() );
			} );
			await UTask.WaitWhile( _asyncCanceler, () => Input.GetKeyDown( KeyCode.M ) );

			SMLog.Warning( "End" );
		} );
	}
}