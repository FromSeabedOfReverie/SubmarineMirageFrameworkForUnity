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
	using Debug;


	public static class TestMultiEventUtility {
		public static void SetModifyler<T>( BaseMultiEvent<T> events, Func<Action, T> function,
											Action<string> addEvent = null
		) {
			Log.Debug( "・生成" );
			Log.Debug( events );

			var startName = "start";
			Log.Debug( $"・{startName}" );
			events.AddLast( startName, function( () => Log.Debug( startName ) ) );
			Log.Debug( events );

			var ifName = $"{nameof( events.InsertFirst )}";
			Log.Debug( $"・{ifName}" );
			events.InsertFirst( startName, $"{ifName} 1", function( () => Log.Debug( $"{ifName} 1" ) ) );
			Log.Debug( events );
			events.InsertFirst( startName, function( () => Log.Debug( $"{ifName} 2" ) ) );
			Log.Debug( events );

			var ilName = $"{nameof( events.InsertLast )}";
			Log.Debug( $"・{ilName}" );
			events.InsertLast( startName, $"{ilName} 1", function( () => Log.Debug( $"{ilName} 1" ) ) );
			Log.Debug( events );
			events.InsertLast( startName, function( () => Log.Debug( $"{ilName} 2" ) ) );
			Log.Debug( events );

			var afName = $"{nameof( events.AddFirst )}";
			Log.Debug( $"・{afName}" );
			events.AddFirst( $"{afName} 1", function( () => Log.Debug( $"{afName} 1" ) ) );
			Log.Debug( events );
			events.AddFirst( function( () => Log.Debug( $"{afName} 2" ) ) );
			Log.Debug( events );

			var alName = $"{nameof( events.AddLast )}";
			Log.Debug( $"・{alName}" );
			events.AddLast( $"{alName} 1", function( () => Log.Debug( $"{alName} 1" ) ) );
			Log.Debug( events );
			events.AddLast( function( () => Log.Debug( $"{alName} 2" ) ) );
			Log.Debug( events );

			addEvent?.Invoke( startName );

			Log.Debug( $"・{nameof( events.Reverse )}" );
			events.Reverse();
			Log.Debug( events );

			Log.Debug( $"・{nameof( events.Remove )}" );
			events.Remove( startName );
			Log.Debug( events );
		}


		public static void SetChangeWhileRunning<T>( BaseMultiEvent<T> events, Func<Action, T> function,
														Action<string> addEvent = null
		) {
			Log.Debug( "・生成" );
			Log.Debug( events );

			Log.Debug( "・追加中に変更を登録" );
			var tcwrName = "TestChangeWhileRunning";
			events.AddLast( tcwrName, function( () => {
				Log.Debug( events );

				var startName = "start";
				events.AddLast( startName, function( () => Log.Debug( startName ) ) );

				var ifName = $"{nameof( events.InsertFirst )}";
				events.InsertFirst( startName, $"{ifName} 1", function( () => Log.Debug( $"{ifName} 1" ) ) );
				events.InsertFirst( startName, function( () => Log.Debug( $"{ifName} 2" ) ) );

				var ilName = $"{nameof( events.InsertLast )}";
				events.InsertLast( startName, $"{ilName} 1", function( () => Log.Debug( $"{ilName} 1" ) ) );
				events.InsertLast( startName, function( () => Log.Debug( $"{ilName} 2" ) ) );

				var afName = $"{nameof( events.AddFirst )}";
				events.AddFirst( $"{afName} 1", function( () => Log.Debug( $"{afName} 1" ) ) );
				events.AddFirst( function( () => Log.Debug( $"{afName} 2" ) ) );

				var alName = $"{nameof( events.AddLast )}";
				events.AddLast( $"{alName} 1", function( () => Log.Debug( $"{alName} 1" ) ) );
				events.AddLast( function( () => Log.Debug( $"{alName} 2" ) ) );

				addEvent?.Invoke( startName );

				events.Reverse();

				events.Remove( startName );
				events.Remove( tcwrName );

				Log.Debug( events );
			} ) );
			Log.Debug( events );
		}


		public static MultiDisposable SetKey<T>( BaseMultiEvent<T> events, Func<Action, T> function ) {
			var disposables = new MultiDisposable();
			var count = 0;

			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( $"key down {nameof( events.AddLast )}" );
					Log.Debug( events );
					var i = count++;
					events.AddLast( $"{i}", function( () => Log.Debug( $"{i}" ) ) );
					Log.Debug( events );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( $"key down {nameof( events.Remove )}" );
					Log.Debug( events );
					count--;
					events.Remove( $"{count}" );
					Log.Debug( events );
				} )
			);
			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( $"key down {nameof( events.Dispose )}" );
					Log.Debug( events );
					events.Dispose();
					Log.Debug( events );
				} )
			);

			return disposables;
		}
	}
}