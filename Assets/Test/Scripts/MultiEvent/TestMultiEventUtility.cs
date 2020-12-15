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
		public static void SetModifyler<T>( BaseSMMultiEvent<T> events, Func<Action, T> function,
											Action<string> addEvent = null
		) {
			SMLog.Debug( "・生成" );
			SMLog.Debug( events );

			var startName = "start";
			SMLog.Debug( $"・{startName}" );
			events.AddLast( startName, function( () => SMLog.Debug( startName ) ) );
			SMLog.Debug( events );

			var ifName = $"{nameof( events.InsertFirst )}";
			SMLog.Debug( $"・{ifName}" );
			events.InsertFirst( startName, $"{ifName} 1", function( () => SMLog.Debug( $"{ifName} 1" ) ) );
			SMLog.Debug( events );
			events.InsertFirst( startName, function( () => SMLog.Debug( $"{ifName} 2" ) ) );
			SMLog.Debug( events );

			var ilName = $"{nameof( events.InsertLast )}";
			SMLog.Debug( $"・{ilName}" );
			events.InsertLast( startName, $"{ilName} 1", function( () => SMLog.Debug( $"{ilName} 1" ) ) );
			SMLog.Debug( events );
			events.InsertLast( startName, function( () => SMLog.Debug( $"{ilName} 2" ) ) );
			SMLog.Debug( events );

			var afName = $"{nameof( events.AddFirst )}";
			SMLog.Debug( $"・{afName}" );
			events.AddFirst( $"{afName} 1", function( () => SMLog.Debug( $"{afName} 1" ) ) );
			SMLog.Debug( events );
			events.AddFirst( function( () => SMLog.Debug( $"{afName} 2" ) ) );
			SMLog.Debug( events );

			var alName = $"{nameof( events.AddLast )}";
			SMLog.Debug( $"・{alName}" );
			events.AddLast( $"{alName} 1", function( () => SMLog.Debug( $"{alName} 1" ) ) );
			SMLog.Debug( events );
			events.AddLast( function( () => SMLog.Debug( $"{alName} 2" ) ) );
			SMLog.Debug( events );

			addEvent?.Invoke( startName );

			SMLog.Debug( $"・{nameof( events.Reverse )}" );
			events.Reverse();
			SMLog.Debug( events );

			SMLog.Debug( $"・{nameof( events.Remove )}" );
			events.Remove( startName );
			SMLog.Debug( events );
		}


		public static void SetChangeWhileRunning<T>( BaseSMMultiEvent<T> events, Func<Action, T> function,
														Action<string> addEvent = null
		) {
			SMLog.Debug( "・生成" );
			SMLog.Debug( events );

			SMLog.Debug( "・追加中に変更を登録" );
			var tcwrName = "TestChangeWhileRunning";
			events.AddLast( tcwrName, function( () => {
				SMLog.Debug( events );

				var startName = "start";
				events.AddLast( startName, function( () => SMLog.Debug( startName ) ) );

				var ifName = $"{nameof( events.InsertFirst )}";
				events.InsertFirst( startName, $"{ifName} 1", function( () => SMLog.Debug( $"{ifName} 1" ) ) );
				events.InsertFirst( startName, function( () => SMLog.Debug( $"{ifName} 2" ) ) );

				var ilName = $"{nameof( events.InsertLast )}";
				events.InsertLast( startName, $"{ilName} 1", function( () => SMLog.Debug( $"{ilName} 1" ) ) );
				events.InsertLast( startName, function( () => SMLog.Debug( $"{ilName} 2" ) ) );

				var afName = $"{nameof( events.AddFirst )}";
				events.AddFirst( $"{afName} 1", function( () => SMLog.Debug( $"{afName} 1" ) ) );
				events.AddFirst( function( () => SMLog.Debug( $"{afName} 2" ) ) );

				var alName = $"{nameof( events.AddLast )}";
				events.AddLast( $"{alName} 1", function( () => SMLog.Debug( $"{alName} 1" ) ) );
				events.AddLast( function( () => SMLog.Debug( $"{alName} 2" ) ) );

				addEvent?.Invoke( startName );

				events.Reverse();

				events.Remove( startName );
				events.Remove( tcwrName );

				SMLog.Debug( events );
			} ) );
			SMLog.Debug( events );
		}


		public static SMMultiDisposable SetKey<T>( BaseSMMultiEvent<T> events, Func<Action, T> function ) {
			var disposables = new SMMultiDisposable();
			var count = 0;

			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {nameof( events.AddLast )}" );
					SMLog.Debug( events );
					var i = count++;
					events.AddLast( $"{i}", function( () => SMLog.Debug( $"{i}" ) ) );
					SMLog.Debug( events );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {nameof( events.Remove )}" );
					SMLog.Debug( events );
					count--;
					events.Remove( $"{count}" );
					SMLog.Debug( events );
				} )
			);
			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {nameof( events.Dispose )}" );
					SMLog.Debug( events );
					events.Dispose();
					SMLog.Debug( events );
				} )
			);

			return disposables;
		}
	}
}