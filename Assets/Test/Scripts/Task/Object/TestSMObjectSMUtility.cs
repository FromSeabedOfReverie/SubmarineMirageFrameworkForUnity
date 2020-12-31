//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using MultiEvent;
	using Task;
	using Task.Behaviour;
	using Task.Object;
	using Task.Object.Modifyler;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public static class TestSMObjectSMUtility {
		public static SMMultiDisposable SetRunKey( SMObject smObject ) {
			var disposables = new SMMultiDisposable();

			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown(KeyCode.Alpha1 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskRunState.Create}" );
					RunStateSMGroup.RegisterAndRun( smObject, SMTaskRunState.Create );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown(KeyCode.Alpha2 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskRunState.SelfInitialize}" );
					RunStateSMGroup.RegisterAndRun( smObject, SMTaskRunState.SelfInitialize );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown(KeyCode.Alpha3 ) ).Subscribe( (Action<long>)(_ => {
					SMLog.Warning( $"key down {SMTaskRunState.Initialize}" );
					RunStateSMGroup.RegisterAndRun( smObject, (SMTaskRunState)SMTaskRunState.Initialize );
				}) ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown(KeyCode.Alpha4 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskRunState.FixedUpdate}" );
					RunStateSMGroup.RegisterAndRun( smObject, SMTaskRunState.FixedUpdate );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown(KeyCode.Alpha5 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskRunState.Update}" );
					RunStateSMGroup.RegisterAndRun( smObject, SMTaskRunState.Update );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown(KeyCode.Alpha6 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskRunState.LateUpdate}" );
					RunStateSMGroup.RegisterAndRun( smObject, SMTaskRunState.LateUpdate );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown(KeyCode.Alpha7 ) ).Subscribe( (Action<long>)(_ => {
					SMLog.Warning( $"key down {SMTaskRunState.Finalize}" );
					RunStateSMGroup.RegisterAndRun( smObject, (SMTaskRunState)SMTaskRunState.Finalize );
				}) )
			);
			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.A ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskActiveState.Enable} Owner" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, true, true ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.S ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskActiveState.Disable} Owner" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, false, true ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Z ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskActiveState.Enable}" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, true, false ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.X ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskActiveState.Disable}" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, false, false ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.C ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {nameof( RunInitialActiveSMObject )}" );
					smObject._top._modifyler.Register( new RunInitialActiveSMObject( smObject ) );
				} )
			);
			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {nameof( smObject.Dispose )}" );
					smObject.Dispose();
					smObject = null;
				} )
			);

			return disposables;
		}



		public static SMMultiDisposable SetChangeActiveKey( SMObject smObject ) {
			var disposables = new SMMultiDisposable();

			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.D ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskActiveState.Enable} Child Owner" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, true, true ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.F ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskActiveState.Disable} Child Owner" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, false, true ) );
				} )
			);

			return disposables;
		}



		public static void LogObject( string text, SMObject smObject )
			=> SMLog.Debug( $"{text} : {smObject?.ToLineString() ?? "null"}" );

		public static void LogObjects( string text, IEnumerable<SMObject> smObjects ) {
			var os = smObjects.ToArray();
			SMLog.Debug( string.Join( "\n",
				$"{text} : {os.Count()}",
				string.Join( "\n", os.Select( o => o?.ToLineString() ?? "null" ) )
			) );
		}


		public static async UniTask LoadObjectsInScene( SMTaskCanceler canceler ) {
			var currents = new Queue<Transform>();
			SceneManager.GetActiveScene().GetRootGameObjects()
				.ForEach( go => currents.Enqueue( go.transform ) );
			while ( !currents.IsEmpty() ) {
				var current = currents.Dequeue();
				var bs = current.GetComponents<SMMonoBehaviour>();
				if ( !bs.IsEmpty() ) {
					new SMObject( current.gameObject, bs, null );
					await UTask.NextFrame( canceler );
				} else {
					foreach ( Transform child in current ) {
						currents.Enqueue( child );
					}
				}
			}
		}
	}
}