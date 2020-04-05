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


	public class TestMultiSubject {
		MultiSubject _events = new MultiSubject();

		public TestMultiSubject() {
			Application.targetFrameRate = 10;

			Log.Debug( _events );
			_events.AddLast( "b" ).Subscribe( _ => Log.Debug( "b" ) );
			_events.AddLast( "c" ).Subscribe( _ => Log.Debug( "c" ) );
			_events.AddFirst( "a" ).Subscribe( _ => Log.Debug( "a" ) );
			Log.Debug( _events );


			Observable.EveryUpdate()
				.Subscribe( _ => _events.Run() );
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
					_events.AddLast( $"{ii}" ).Subscribe( __ => Log.Debug( $"{ii}" ) );
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

		~TestMultiSubject() => _events.Dispose();
	}
}