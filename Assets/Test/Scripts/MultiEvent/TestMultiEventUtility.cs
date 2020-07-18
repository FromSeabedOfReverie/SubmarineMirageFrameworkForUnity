//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestMultiEvent {
	using System;
	using UnityEngine;
	using UniRx;
	using MultiEvent;
	using UTask;
	using Debug;


	public static class TestMultiEventUtility {
		public static MultiDisposable SetKey<T>( BaseMultiEvent<T> events, Action<int> addEvent ) {
			var disposables = new MultiDisposable();
			var count = 0;

			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( "key down Add" );
					Log.Debug( events );
					var i = count++;
					addEvent.Invoke( i );
					Log.Debug( events );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down Remove" );
					Log.Debug( events );
					count--;
					events.Remove( $"{count}" );
					Log.Debug( events );
				} )
			);
			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down Dispose" );
					Log.Debug( events );
					events.Dispose();
					Log.Debug( events );
				} )
			);

			return disposables;
		}
	}
}