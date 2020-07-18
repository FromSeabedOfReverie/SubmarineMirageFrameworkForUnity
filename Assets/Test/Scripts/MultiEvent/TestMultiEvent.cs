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


	public class TestMultiEvent : Test {
		MultiEvent _events = new MultiEvent();


		protected override void Create() {
			Application.targetFrameRate = 10;
			_disposables.AddLast( _events );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestAdd() => From( async () => {
			Log.Debug( _events );

			_events.AddLast( "b", () => Log.Debug( "b" ) );
			_events.AddLast( "c", () => Log.Debug( "c" ) );
			_events.AddFirst( "a", () => Log.Debug( "a" ) );
			Log.Debug( _events );

			await UTask.DontWait();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( async () => {
			_disposables.AddLast(
				TestMultiEventUtility.SetKey(
					_events,
					i => _events.AddLast( $"{i}", () => Log.Debug( $"{i}" ) )
				),
				Observable.EveryUpdate().Subscribe( _ => _events.Run() )
			);

			await UTask.Never( _asyncCanceler );
		} );
	}
}