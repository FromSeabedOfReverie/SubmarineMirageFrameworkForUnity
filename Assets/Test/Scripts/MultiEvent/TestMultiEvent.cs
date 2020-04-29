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


	public class TestMultiEvent : Test {
		MultiEvent _events = new MultiEvent();


		protected override void Create() {
			Application.targetFrameRate = 10;
			_disposables.AddLast( _events );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestAdd() => From( TestAddSub() );
		IEnumerator TestAddSub() {
			Log.Debug( _events );

			_events.AddLast( "b", () => Log.Debug( "b" ) );
			_events.AddLast( "c", () => Log.Debug( "c" ) );
			_events.AddFirst( "a", () => Log.Debug( "a" ) );
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
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down Dispose" );
					Log.Debug( _events );
					_events.Dispose();
					Log.Debug( _events );
				} ),
				Observable.EveryUpdate().Where( _ => !_events._isDispose ).Subscribe( _ =>
					_events.Run()
				)
			);

			while ( true )	{ yield return null; }
		}
	}
}