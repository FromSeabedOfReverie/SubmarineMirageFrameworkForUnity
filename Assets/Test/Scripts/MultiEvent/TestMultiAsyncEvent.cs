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
	using UTask;
	using Debug;
	using Test;


	public class TestMultiAsyncEvent : Test {
		MultiAsyncEvent _events = new MultiAsyncEvent();


		protected override void Create() {
			Application.targetFrameRate = 10;
			_disposables.AddLast( _events );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestAdd() => From( async () => {
			Log.Debug( _events );

			_events.AddLast( "b", async canceler => {
				Log.Debug( "b start" );
				await UTask.Delay( canceler, 1000 );
				Log.Debug( "b end" );
			} );
			_events.AddLast( "c", async canceler => {
				Log.Debug( "c start" );
				await UTask.Delay( canceler, 1000 );
				Log.Debug( "c end" );
			} );
			_events.AddFirst( "a", async canceler => {
				Log.Debug( "a start" );
				await UTask.Delay( canceler, 1000 );
				Log.Debug( "a end" );
			} );
			Log.Debug( _events );

			await _events.Run( _asyncCanceler );
		} );


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
					_events.AddLast( $"{i}", async canceler => {
						Log.Debug( $"{i} start" );
						await UTask.Delay( canceler, 1000 );
						Log.Debug( $"{i} end" );
					} );
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
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down Dispose" );
					Log.Debug( _events );
					_events.Dispose();
					Log.Debug( _events );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Space ) ).Subscribe( _ => {
					Log.Warning( "key down Run" );
					UTask.Void( async () => {
						Log.Debug( _events );
						await _events.Run( _asyncCanceler );
						Log.Debug( _events );
					} );
				} )
			);

			while ( true )	{ yield return null; }
		}
	}
}