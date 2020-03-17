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


	public class TestEventModifyler {
		MultiEvent _events = new MultiEvent();

		public TestEventModifyler() {
			Application.targetFrameRate = 10;

			Log.Debug( _events );
//			_events.SetIsInvoking( true );
			_events.AddLast( "b", () => Log.Debug( "b" ) );
			_events.AddLast( "c", () => Log.Debug( "c" ) );
			_events.AddFirst( "a", () => Log.Debug( "a" ) );
			_events.InsertLast( "b", "b.5", () => Log.Debug( "b.5" ) );
			_events.InsertFirst( "b", "a.5", () => Log.Debug( "b.5" ) );
			Log.Debug( _events );
//			_events.SetIsInvoking( false );
			Log.Debug( _events );

			Observable.EveryUpdate()
				.Where( _ => Input.GetKeyDown( KeyCode.R ) )
				.Subscribe( _ => {
					Log.Warning( "key down R" );
					Log.Debug( _events );
//					_events.SetIsInvoking( true );
					_events.Remove( "b" );
					Log.Debug( _events );
//					_events.SetIsInvoking( false );
					Log.Debug( _events );
				} );
			var i = 0;
			Observable.EveryUpdate()
				.Where( _ => Input.GetKeyDown( KeyCode.A ) )
				.Subscribe( _ => {
					Log.Warning( "key down A" );
					Log.Debug( _events );
//					_events.SetIsInvoking( true );
					var ii = i++;
					_events.AddLast( $"{ii}", () => Log.Debug( $"{ii}" ) );
					Log.Debug( _events );
//					_events.SetIsInvoking( false );
					Log.Debug( _events );
				} );
			Observable.EveryUpdate()
				.Where( _ => Input.GetKeyDown( KeyCode.D ) )
				.Subscribe( _ => {
					Log.Warning( "key down D" );
					Log.Debug( _events );
//					_events.SetIsInvoking( true );
					_events.Dispose();
					Log.Debug( _events );
//					_events.SetIsInvoking( false );
					Log.Debug( _events );
				} );
		}

		~TestEventModifyler() => _events.Dispose();
	}
}