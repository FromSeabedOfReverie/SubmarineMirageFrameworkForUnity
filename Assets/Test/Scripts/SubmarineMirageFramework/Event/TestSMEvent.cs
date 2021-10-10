//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using Cysharp.Threading.Tasks;



	public class TestSMEvent : SMUnitTest {
		readonly SMAsyncCanceler _asyncCanceler = new SMAsyncCanceler();



		protected override void Create() {
			UniTaskScheduler.UnobservedExceptionWriteLogType = LogType.Error;

			_disposables.AddLast( () => {
				_asyncCanceler.Dispose();
			} );
		}



		Action TestWait( string name )
			=> () => SMLog.Warning( $"実行 : \n{name}" );

		Action TestError( string name )
			=> () => throw new Exception( $"失敗 : \n{name}" );

		Action TestDispose( SMEvent @event, string name )
			=> () => {
				SMLog.Warning( $"解放 : \n{name}" );
				@event.Dispose();
			};



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDispose() => From( async () => {
			await UTask.DelayFrame( _asyncCanceler, 2 );
			SMLog.Warning( "Start" );

			var es = new SMEvent();
			es.Dispose();

			es.Remove( "hoge" );
			es.Reverse();
			es.InsertFirst( "hoge", () => {} );
			es.InsertLast( "hoge", () => {} );
			es.AddFirst( () => {} );
			es.AddLast( () => {} );
			es.Run();

			es = new SMEvent();
			es.AddLast( TestWait( "1" ) );
			es.AddLast( TestDispose( es, "2" ) );
			es.AddLast( TestWait( "3" ) );
			es.Run();

			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRegister() => From( async () => {
			SMLog.Warning( "Start" );

			var es = new SMEvent();

			es.AddLast( "1", TestWait( "1" ) );
			SMLog.Debug( es );

			es.InsertFirst( "1", "2", TestWait( "2" ) );
			SMLog.Debug( es );
			es.InsertFirst( "1", TestWait( "3" ) );
			SMLog.Debug( es );

			es.InsertLast( "1", "4", TestWait( "4" ) );
			SMLog.Debug( es );
			es.InsertLast( "1", TestWait( "5" ) );
			SMLog.Debug( es );

			es.AddFirst( "6", TestWait( "6" ) );
			SMLog.Debug( es );
			es.AddFirst( TestWait( "7" ) );
			SMLog.Debug( es );

			es.AddLast( "8", TestWait( "8" ) );
			SMLog.Debug( es );
			es.AddLast( TestWait( "9" ) );
			SMLog.Debug( es );

			es.AddLast( "1", TestWait( "1" ) );
			SMLog.Debug( es );

			es.Reverse();
			SMLog.Debug( es );
			es.Remove( "1" );
			SMLog.Debug( es );

			es._isDebug = true;
			es.Run();
			await UTask.WaitWhile( _asyncCanceler, () => es._isRunning );

			es.Dispose();

			es = new SMEvent();
			es.InsertFirst( "hoge", "10", TestWait( "10" ) );
			es.InsertLast( "hoge", "11", TestWait( "11" ) );
			es.Dispose();

			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRun() => From( async () => {
			SMLog.Warning( "Start" );


			var es = new SMEvent();
			es.AddLast( TestWait( "1" ) );
			es.AddLast( TestWait( "2" ) );
			es.Run();
			es.Run();
			SMLog.Debug( $"{nameof( es.Run )} : end" );
			es.Dispose();


			es = new SMEvent();
			es.AddLast( "1", () => {
				es.AddLast( "2", TestWait( "2" ) );
				SMLog.Debug( es );
				es.AddLast( "3", TestWait( "3" ) );
				SMLog.Debug( es );

				es.Remove( "2" );
				SMLog.Debug( es );
				es.Remove( "1" );
				SMLog.Debug( es );
			} );
			es.AddLast( "1.5", TestWait( "1.5" ) );
			es.Run();
			es.Run();
			SMLog.Debug( $"{nameof( es.Run )} : end" );
			es.Dispose();


			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDisposeRun() => From( async () => {
			await UTask.DelayFrame( _asyncCanceler, 2 );
			SMLog.Warning( "Start" );


			var es = new SMEvent();
			es.AddLast( TestWait(		"1" ) );
			es.AddLast( TestDispose(	es, "2" ) );
			es.AddLast( TestWait(		"3" ) );
			es.Run();
			es.Dispose();
			SMLog.Debug( $"{nameof( es.Run )} : end" );


			es = new SMEvent();
			es.AddLast( "1", () =>
				es.AddLast( "2", TestDispose( es, "2" ) )
			);
			es.AddLast( "3", TestDispose( es, "3" ) );
			es.Run();
			es.Run();
			es.Dispose();
			SMLog.Debug( $"{nameof( es.Run )} : end" );


			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestErrorRun() => From( async () => {
			await UTask.DelayFrame( _asyncCanceler, 2 );
			SMLog.Warning( "Start" );


			var es = new SMEvent();
			es.AddLast( "1", TestWait(	"1" ) );
			es.AddLast( "2", TestError(	"2" ) );
			es.AddLast( "3", TestWait(	"3" ) );
			es.Run();
			es.Run();
			es.Dispose();
			SMLog.Debug( $"{nameof( es.Run )} : end" );


			es = new SMEvent();
			es.AddLast( "1", () => {
				es.AddLast( "2", TestError( "2" ) );
				es.Remove( "1" );
			} );
			es.AddLast( "3", TestError( "3" ) );
			es.Run();
			es.Run();
			es.Dispose();
			SMLog.Debug( $"{nameof( es.Run )} : end" );


			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );
	}
}