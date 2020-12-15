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
	using Utility;
	using Debug;
	using Test;


	public class TestMultiEvent : SMStandardTest {
		SMMultiEvent _events = new SMMultiEvent();


		protected override void Create() {
			Application.targetFrameRate = 10;
			_disposables.AddLast( _events );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestModifyler() => From( async () => {
			TestMultiEventUtility.SetModifyler( _events, a => a );

			SMLog.Debug( "・実行" );
			_events.Run();
			SMLog.Debug( _events );

			await UTask.DontWait();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestChangeWhileRunning() => From( async () => {
			TestMultiEventUtility.SetChangeWhileRunning( _events, a => a );

			SMLog.Debug( "・実行 1" );
			_events.Run();
			SMLog.Debug( _events );

			SMLog.Debug( "・実行 2" );
			_events.Run();
			SMLog.Debug( _events );

			await UTask.DontWait();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( async () => {
			_disposables.AddLast(
				TestMultiEventUtility.SetKey( _events, a => a ),
				Observable.EveryUpdate().Subscribe( _ => _events.Run() )
			);

			await UTask.Never( _asyncCanceler );
		} );
	}
}