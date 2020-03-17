//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.MultiEvent {
	using UnityEngine;
	using UniRx;
	using Debug;


	// TODO : コメント追加、整頓


	public class TestMultiDisposable {
		MultiDisposable _events = new MultiDisposable();

		public TestMultiDisposable() {
			Application.targetFrameRate = 10;

			Log.Debug( _events );
/*
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
*/
			_events.AddLast( "b", () => Log.Debug( "b" ) );
			_events.AddLast( "c", () => Log.Debug( "c" ) );
			_events.AddFirst( "a", () => Log.Debug( "a" ) );
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
					_events.AddLast( $"{ii}", () => Log.Debug( $"{ii}" ) );
					Log.Debug( _events );
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

		~TestMultiDisposable() => _events.Dispose();
	}
}