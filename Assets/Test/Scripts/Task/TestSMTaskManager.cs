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
			SMLog.Debug( _taskManager.GetAlls().ToShowString() );

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator Test() => From( async () => {
			SMLog.Warning( "Start" );
			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );
	}
}