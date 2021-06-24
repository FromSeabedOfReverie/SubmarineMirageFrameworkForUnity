//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#if false
namespace SubmarineMirage.TestEvent {
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using Event;
	using Utility;
	using Debug;
	using TestBase;



	public class TestSMEvent : SMUnitTest {
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestChangeWhileRunning() => From( async () => {
			SMLog.Debug( "・生成テスト" );
			var e = new SMEvent();
			SMLog.Debug( e );

			SMLog.Debug( "・追加テスト" );
			e.AddLast( "b", () => SMLog.Debug( "b" ) );
			SMLog.Debug( e );

			SMLog.Debug( "・追加中に変更を登録" );
			e.AddLast( "start", () => {
				e.AddLast( "c", () => SMLog.Debug( "c" ) );
				e.AddFirst( "a", () => SMLog.Debug( "a" ) );
				e.InsertLast( "b", "b.5", () => SMLog.Debug( "b.5" ) );
				e.InsertFirst( "b", "a.5", () => SMLog.Debug( "a.5" ) );
				e.Reverse();
				e.Remove( "start" );
				SMLog.Debug( e );
			} );
			SMLog.Debug( e );

			SMLog.Debug( "・実行1" );
			e.Run();
			SMLog.Debug( e );

			SMLog.Debug( "・実行2" );
			e.Run();
			SMLog.Debug( e );

			e.Dispose();
			await UTask.DontWait();
		} );



		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestModifyler() => From( async () => {
			TestSMEventSMUtility.SetModifyler( _events, a => a );

			SMLog.Debug( "・実行" );
			_events.Run();
			SMLog.Debug( _events );

			await UTask.DontWait();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestChangeWhileRunning() => From( async () => {
			TestSMEventSMUtility.SetChangeWhileRunning( _events, a => a );

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
				TestSMEventSMUtility.SetKey( _events, a => a ),
				Observable.EveryUpdate().Subscribe( _ => _events.Run() )
			);

			await UTask.Never( _asyncCanceler );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( async () => {
			_disposables.AddLast( TestSMEventSMUtility.SetKey( _events, a => a ) );

			await UTask.Never( _asyncCanceler );
		} );
	}
}
#endif