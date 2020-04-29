//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestMultiEvent {
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using MultiEvent;
	using Debug;
	using Test;


	// TODO : コメント追加、整頓


	public class TestMultiDisposable : Test {
		MultiDisposable _events = new MultiDisposable();


		protected override void Create() {
			Application.targetFrameRate = 10;
			_disposables.AddLast( _events );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestInsert() => From( TestInsertSub() );
		IEnumerator TestInsertSub() {
			Log.Debug( _events );

			_events.AddLast( "0", () => Log.Debug( "0" ) );
			_events.InsertLast( "0", () => Log.Debug( "1" ) );
			_events.InsertLast( "0", Disposable.Create( () => Log.Debug( "2" ) ) );
			_events.InsertLast( "0",
				Disposable.Create( () => Log.Debug( "3" ) ),
				Disposable.Create( () => Log.Debug( "4" ) )
			);
			_events.InsertLast( "0", "5", () => Log.Debug( "5" ) );
			_events.InsertLast( "0", "6", Disposable.Create( () => Log.Debug( "6" ) ) );
			_events.InsertLast( "0", "7",
				Disposable.Create( () => Log.Debug( "7" ) ),
				Disposable.Create( () => Log.Debug( "8" ) )
			);
			_events.AddFirst( "-1", () => Log.Debug( "-1" ) );
			Log.Debug( _events );

			yield break;
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( TestManualSub() );
		IEnumerator TestManualSub() {
			var count = 0;
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( "key down Add" );
					Log.Debug( _events );
					var i = count++;
					_events.AddLast( $"{i}", () => Log.Debug( $"{i}" ) );
					Log.Debug( _events );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down Remove" );
					Log.Debug( _events );
					count--;
					_events.Remove( $"{count}" );
					Log.Debug( _events );
				} )
			);

			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Z ) ).Subscribe( _ => {
					Log.Warning( "key down UnLock" );
					Log.Debug( _events );
					_events._isLock = false;
					Log.Debug( _events );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.X ) ).Subscribe( _ => {
					Log.Warning( "key down Lock" );
					Log.Debug( _events );
					_events._isLock = true;
					Log.Debug( _events );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down Dispose" );
					Log.Debug( _events );
					_events.Dispose();
					Log.Debug( _events );
				} )
			);

			while ( true )	{ yield return null; }
		}
	}
}