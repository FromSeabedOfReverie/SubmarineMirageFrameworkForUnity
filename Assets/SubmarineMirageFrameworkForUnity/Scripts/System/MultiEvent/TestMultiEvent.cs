//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.MultiEvent {
	using System.Threading;
	using UnityEngine;
	using UniRx;
	using UniRx.Async;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class TestMultiEvent {
		MultiAsyncEvent _events = new MultiAsyncEvent();
		CancellationTokenSource _canceler = new CancellationTokenSource();

		public TestMultiEvent() {
			Application.targetFrameRate = 10;

			_events.AddLast( "b", async cancel => {
				Log.Debug( "b start" );
				await UniTaskUtility.Delay( cancel, 500 );
				Log.Debug( "b end" );
			} );
			_events.AddLast( "c", async cancel => {
				Log.Debug( "c start" );
				await UniTaskUtility.Delay( cancel, 500 );
				Log.Debug( "c end" );
			} );
			_events.AddFirst( "a", async cancel => {
				Log.Debug( "a start" );
				await UniTaskUtility.Delay( cancel, 500 );
				Log.Debug( "a end" );
			} );

			Observable.EveryUpdate()
				.Where( _ => Input.GetKeyDown( KeyCode.S ) )
				.Subscribe( _ => _events.Invoke( _canceler.Token ).Forget() );

			Observable.EveryUpdate()
				.Where( _ => Input.GetKeyDown( KeyCode.Return ) )
				.Subscribe( _ => {
					Log.Warning( "key down return" );
					_events.Remove( "b" );
				} );
			Observable.EveryUpdate()
				.Where( _ => Input.GetKeyDown( KeyCode.Space ) )
				.Subscribe( _ => {
					Log.Warning( "key down space" );
					_events.Dispose();
				} );

			var i = 0;
			Observable.EveryUpdate()
				.Where( _ => Input.GetKeyDown( KeyCode.Backspace ) )
				.Subscribe( _ => {
					Log.Warning( "key down backspace" );
					var ii = i++;
					_events.AddLast( $"{ii}", async cancel => {
						Log.Debug( $"{ii} start" );
						await UniTaskUtility.Delay( cancel, 500 );
						Log.Debug( $"{ii} end" );
					} );
				} );

			Observable.EveryUpdate()
				.Where( _ => Input.GetKeyDown( KeyCode.D ) )
				.Subscribe( _ => Log.Debug( _events ) );
		}

		~TestMultiEvent() => _events.Dispose();
	}
}