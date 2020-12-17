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
	using MultiEvent;
	using Utility;
	using Debug;
	using TestBase;


	public class TestSMEventModifyler : SMStandardTest {
		readonly SMMultiEvent _events = new SMMultiEvent();


		protected override void Create() {
			Application.targetFrameRate = 10;
			_disposables.AddLast( _events );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestChangeWhileRunning() => From( async () => {
			SMLog.Debug( "・生成テスト" );
			SMLog.Debug( _events );

			SMLog.Debug( "・追加テスト" );
			_events.AddLast( "b", () => SMLog.Debug( "b" ) );
			SMLog.Debug( _events );

			SMLog.Debug( "・追加中に変更を登録" );
			_events.AddLast( "start", () => {
				_events.AddLast( "c", () => SMLog.Debug( "c" ) );
				_events.AddFirst( "a", () => SMLog.Debug( "a" ) );
				_events.InsertLast( "b", "b.5", () => SMLog.Debug( "b.5" ) );
				_events.InsertFirst( "b", "a.5", () => SMLog.Debug( "a.5" ) );
				_events.Reverse();
				_events.Remove( "start" );
				SMLog.Debug( _events );
			} );
			SMLog.Debug( _events );

			SMLog.Debug( "・実行1" );
			_events.Run();
			SMLog.Debug( _events );

			SMLog.Debug( "・実行2" );
			_events.Run();
			SMLog.Debug( _events );

			await UTask.DontWait();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( async () => {
			_disposables.AddLast( TestSMMultiEventSMUtility.SetKey( _events, a => a ) );

			await UTask.Never( _asyncCanceler );
		} );
	}
}