//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.MultiEvent {
	using System;
	using System.Threading;
	using UnityEngine;
	using UniRx;
	using UniRx.Async;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class TestMultiAsyncEvent {
		MultiAsyncEvent _events = new MultiAsyncEvent();
		CancellationTokenSource _canceler = new CancellationTokenSource();

		public TestMultiAsyncEvent() {
			Application.targetFrameRate = 10;

			Log.Debug( _events );
			_events.AddLast( "b", async cancel => {
				Log.Debug( "b start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( "b end" );
			} );
			_events.AddLast( "c", async cancel => {
				Log.Debug( "c start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( "c end" );
			} );
			_events.AddFirst( "a", async cancel => {
				Log.Debug( "a start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( "a end" );
			} );
			Log.Debug( _events );


			Observable.EveryUpdate()
				.Where( _ => Input.GetKeyDown( KeyCode.R ) )
				.Subscribe( _ => {
					Log.Warning( "key down R" );
					Log.Debug( _events );
					_events.Remove( "b" );
					Log.Debug( _events );
				} );
			var i = 0;
			Observable.EveryUpdate()
				.Where( _ => Input.GetKeyDown( KeyCode.A ) )
				.Subscribe( _ => {
					Log.Warning( "key down A" );
					Log.Debug( _events );
					var ii = i++;
					_events.AddLast( $"{ii}", async cancel => {
						Log.Debug( $"{ii} start" );
						await UniTaskUtility.Delay( cancel, 1000 );
						Log.Debug( $"{ii} end" );
					} );
					Log.Debug( _events );
				} );
			Observable.EveryUpdate()
				.Where( _ => Input.GetKeyDown( KeyCode.S ) )
				.Subscribe( _ => {
					Log.Warning( "key down S" );
					Log.Debug( _events );
					new Func<UniTask>( async () => {
						await _events.Run( _canceler.Token );
						Log.Debug( _events );
					} )().Forget();
				} );
			Observable.EveryUpdate()
				.Where( _ => Input.GetKeyDown( KeyCode.D ) )
				.Subscribe( _ => {
					Log.Warning( "key down D" );
					Log.Debug( _events );
					_events.Dispose();
					Log.Debug( _events );
				} );
		}

		~TestMultiAsyncEvent() => _events.Dispose();
	}
}