//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestSMTask {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using MultiEvent;
	using UTask;
	using SMTask;
	using SMTask.Modifyler;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public static class TestSMObjectUtility {
		public static MultiDisposable SetRunKey( SMObject smObject ) {
			var disposables = new MultiDisposable();

			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( "key down Creating" );
					smObject._top._modifyler.Register( new RunStateSMObject( smObject, SMTaskRanState.Creating ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down Loading" );
					smObject._top._modifyler.Register( new RunStateSMObject( smObject, SMTaskRanState.Loading ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha3 ) ).Subscribe( _ => {
					Log.Warning( "key down Initializing" );
					smObject._top._modifyler.Register( new RunStateSMObject( smObject, SMTaskRanState.Initializing ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha4 ) ).Subscribe( _ => {
					Log.Warning( "key down FixedUpdate" );
					smObject._top._modifyler.Register( new RunStateSMObject( smObject, SMTaskRanState.FixedUpdate ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha5 ) ).Subscribe( _ => {
					Log.Warning( "key down Update" );
					smObject._top._modifyler.Register( new RunStateSMObject( smObject, SMTaskRanState.Update ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha6 ) ).Subscribe( _ => {
					Log.Warning( "key down LateUpdate" );
					smObject._top._modifyler.Register( new RunStateSMObject( smObject, SMTaskRanState.LateUpdate ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha7 ) ).Subscribe( _ => {
					Log.Warning( "key down Finalizing" );
					smObject._top._modifyler.Register( new RunStateSMObject( smObject, SMTaskRanState.Finalizing ) );
				} )
			);
			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.A ) ).Subscribe( _ => {
					Log.Warning( "key down Enabling Owner" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, true, true ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.S ) ).Subscribe( _ => {
					Log.Warning( "key down Disabling Owner" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, false, true ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Z ) ).Subscribe( _ => {
					Log.Warning( "key down Enabling" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, true, false ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.X ) ).Subscribe( _ => {
					Log.Warning( "key down Disabling" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, false, false ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.C ) ).Subscribe( _ => {
					Log.Warning( "key down RunActiveEvent" );
					smObject._top._modifyler.Register( new RunActiveSMObject( smObject ) );
				} )
			);
			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down Dispose" );
					smObject.Dispose();
					smObject = null;
				} )
			);

			return disposables;
		}



		public static MultiDisposable SetChangeActiveKey( SMObject smObject ) {
			var disposables = new MultiDisposable();

			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.D ) ).Subscribe( _ => {
					Log.Warning( "key down Enabling Child Owner" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, true, true ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.F ) ).Subscribe( _ => {
					Log.Warning( "key down Disabling Child Owner" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, false, true ) );
				} )
			);

			return disposables;
		}



		public static void LogObject( string text, SMObject smObject ) {
			if ( smObject == null ) {
				Log.Debug( $"{text} : null" );
				return;
			}
			var name = smObject._owner != null ? smObject._owner.name : null;
			Log.Debug(
				$"{text} : "
					+ string.Join( ", ", smObject.GetBehaviours().Select( b => b.GetAboutName() ) )
					+ $" : {name}"
			);
		}

		public static void LogObjects( string text, IEnumerable<SMObject> smObjects ) {
			Log.Debug(
				$"{text} :\n"
					+ string.Join( "\n",
						smObjects.Select( o => {
							var name = o._owner != null ? o._owner.name : null;
							return string.Join( ", ", o.GetBehaviours().Select( b => b.GetAboutName() ) )
								+ $" : {name}";
						} )
					)
			);
		}
	}
}