//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Test {
	using System;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using Cysharp.Threading.Tasks;



	public class TestSMAsyncEvent : SMUnitTest {
		readonly SMAsyncCanceler _asyncCanceler = new SMAsyncCanceler();



		protected override void Create() {
			UniTaskScheduler.UnobservedExceptionWriteLogType = LogType.Error;

			_disposables.AddFirst( () => {
				_asyncCanceler.Dispose();
			} );
		}



		async UniTask TestWait( SMAsyncCanceler canceler, string name ) {
			SMLog.Warning( $"実行 : start\n{name}" );
			await UTask.Delay( canceler, 1000 );
			SMLog.Warning( $"実行 : end\n{name}" );
		}

		async UniTask TestCancel( SMAsyncCanceler canceler, string name ) {
			UTask.Void( async () => {
				await UTask.Delay( canceler, 500 );
				SMLog.Warning( $"停止 : \n{name}" );
				canceler.Cancel();
			} );
			await UTask.Delay( canceler, 1000 );
		}

		async UniTask TestError( SMAsyncCanceler canceler, string name ) {
			await UTask.Delay( canceler, 500 );
			throw new Exception( $"失敗 : \n{name}" );
		}

		async UniTask TestDispose( SMAsyncEvent asyncEvent, SMAsyncCanceler canceler, string name ) {
			await UTask.Delay( canceler, 500 );
			SMLog.Warning( $"破棄 : \n{name}" );
			asyncEvent.Dispose();
		}



		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestDispose() => From( async () => {
			await UTask.DelayFrame( _asyncCanceler, 2 );
			SMLog.Warning( "Start" );

			var ae = new SMAsyncEvent();
			ae.Dispose();

			try {
				ae._isDebug = true;
			} catch ( Exception e ) { SMLog.Error( e ); }

			ae.Remove( "hoge" );
			ae.Reverse();
			ae.InsertFirst( "hoge", c => UTask.DontWait() );
			ae.InsertLast( "hoge", c => UTask.DontWait() );
			ae.AddFirst( c => UTask.DontWait() );
			ae.AddLast( c => UTask.DontWait() );

			try {
				await ae.Run( _asyncCanceler );
			} catch ( Exception e ) { SMLog.Error( e ); }

			ae = new SMAsyncEvent();
			ae.AddLast( c => TestWait( c, "1" ) );
			ae.AddLast( c => TestWait( c, "2" ) );
			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 500 );
				SMLog.Debug( "削除" );
				ae.Dispose();
			} );
			try {
				await ae.Run( _asyncCanceler );
			} catch ( Exception e ) { SMLog.Error( e ); }

			SMLog.Warning( "End" );
		} );



		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestCancel() => From( async () => {
			SMLog.Warning( "Start" );

			var ae = new SMAsyncEvent();
			ae.AddLast( c => TestWait( c, "1" ) );
			ae.AddLast( c => TestWait( c, "2" ) );
			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 500 );
				SMLog.Debug( "停止" );
				_asyncCanceler.Cancel();
			} );
			try {
				await ae.Run( _asyncCanceler );
			} catch ( OperationCanceledException ) { SMLog.Debug( "Cancel Error" ); }
			SMLog.Debug( "Run End" );

			_asyncCanceler.Cancel( false );
			try {
				await ae.Run( _asyncCanceler );
			} catch ( OperationCanceledException ) { SMLog.Debug( "Cancel Error" ); }
			SMLog.Debug( "Run End" );

			ae.Dispose();

			SMLog.Warning( "End" );
		} );



		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestIs() => From( async () => {
			SMLog.Warning( "Start" );

			var ae = new SMAsyncEvent();
			ae._isDebug = false;
			ae._isDebug = false;
			ae._isDebug = true;
			ae._isDebug = true;
			ae._isDebug = false;
			ae._isDebug = false;

			ae._isDebug = true;
			ae.AddLast( c => TestWait( c, "1" ) );
			ae.AddLast( c => TestWait( c, "2" ) );
			await ae.Run( _asyncCanceler );

			ae.Dispose();
			SMLog.Warning( "End" );
		} );



		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestRegister() => From( async () => {
			SMLog.Warning( "Start" );

			var ae = new SMAsyncEvent();

			ae.AddLast( "1", c => TestWait( c, "1" ) );
			SMLog.Debug( ae );

			ae.InsertFirst( "1", "2", c => TestWait( c, "2" ) );
			SMLog.Debug( ae );
			ae.InsertFirst( "1", c => TestWait( c, "3" ) );
			SMLog.Debug( ae );

			ae.InsertLast( "1", "4", c => TestWait( c, "4" ) );
			SMLog.Debug( ae );
			ae.InsertLast( "1", c => TestWait( c, "5" ) );
			SMLog.Debug( ae );

			ae.AddFirst( "6", c => TestWait( c, "6" ) );
			SMLog.Debug( ae );
			ae.AddFirst( c => TestWait( c, "7" ) );
			SMLog.Debug( ae );

			ae.AddLast( "8", c => TestWait( c, "8" ) );
			SMLog.Debug( ae );
			ae.AddLast( c => TestWait( c, "9" ) );
			SMLog.Debug( ae );

			ae.AddLast( "1", c => TestWait( c, "1" ) );
			SMLog.Debug( ae );

			ae.Reverse();
			SMLog.Debug( ae );
			ae.Remove( "1" );
			SMLog.Debug( ae );

			ae._isDebug = true;
			await ae.Run( _asyncCanceler );

			ae.Dispose();

			ae = new SMAsyncEvent();
			ae.InsertFirst( "hoge", "10", c => TestWait( c, "10" ) );
			ae.InsertLast( "hoge", "11", c => TestWait( c, "11" ) );
			ae.Dispose();

			SMLog.Warning( "End" );
		} );



		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestRun() => From( async () => {
			SMLog.Warning( "Start" );


			var ae = new SMAsyncEvent();
			ae.AddLast( c => TestWait( c, "1" ) );
			ae.AddLast( c => TestWait( c, "2" ) );
			await ae.Run( _asyncCanceler );
			await ae.Run( _asyncCanceler );
			SMLog.Debug( $"{nameof( ae.Run )} : end" );
			ae.Dispose();


			ae = new SMAsyncEvent();
			ae.AddLast( "1", async c => {
				ae.AddLast( "2", cc => TestWait( cc, "2" ) );
				SMLog.Debug( ae );
				ae.AddLast( "3", cc => TestWait( cc, "3" ) );
				SMLog.Debug( ae );

				ae.Remove( "2" );
				SMLog.Debug( ae );
				ae.Remove( "1" );
				SMLog.Debug( ae );

				await UTask.DontWait();
			} );
			await ae.Run( _asyncCanceler );
			await ae.Run( _asyncCanceler );
			SMLog.Debug( $"{nameof( ae.Run )} : end" );
			ae.Dispose();


			ae = new SMAsyncEvent();
			ae.AddLast( c => TestWait(		c, "1" ) );
			ae.AddLast( c => TestCancel(	c, "2" ) );
			ae.AddLast( c => TestWait(		c, "3" ) );
			try {
				await ae.Run( _asyncCanceler );
			} catch ( OperationCanceledException ) { SMLog.Debug( "Cancel Error" ); }
			try {
				await ae.Run( _asyncCanceler );
			} catch ( OperationCanceledException ) { SMLog.Debug( "Cancel Error" ); }
			SMLog.Debug( $"{nameof( ae.Run )} : end" );
			ae.Dispose();


			SMLog.Warning( "End" );
		} );



		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestDisposeRun() => From( async () => {
			SMLog.Warning( "Start" );


			var ae = new SMAsyncEvent();
			ae.AddLast( c => TestWait(		c, "1" ) );
			ae.AddLast( c => TestDispose(	ae, c, "2" ) );
			ae.AddLast( c => TestWait(		c, "3" ) );
			try {
				await ae.Run( _asyncCanceler );
			} catch ( OperationCanceledException )	{ SMLog.Debug( "Cancel Error" ); }
			ae.Dispose();
			SMLog.Debug( $"{nameof( ae.Run )} : end" );


			ae = new SMAsyncEvent();
			ae.AddLast( "1", async c => {
				ae.AddLast( "2", cc => TestDispose( ae, c, "2" ) );
				await UTask.DontWait();
			} );
			ae.AddLast( "3", c => TestDispose( ae, c, "3" ) );
			try {
				await ae.Run( _asyncCanceler );
			} catch ( OperationCanceledException )	{ SMLog.Debug( "Cancel Error" ); }
			try {
				await ae.Run( _asyncCanceler );
			} catch ( Exception e )	{ SMLog.Error( e ); }
			ae.Dispose();
			SMLog.Debug( $"{nameof( ae.Run )} : end" );


			SMLog.Warning( "End" );
		} );



		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestErrorRun() => From( async () => {
			SMLog.Warning( "Start" );


			var ae = new SMAsyncEvent();
			ae.AddLast( "1", c => TestWait(		c, "1" ) );
			ae.AddLast( "2", c => TestError(	c, "2" ) );
			ae.AddLast( "3", c => TestWait(		c, "3" ) );
			try {
				await ae.Run( _asyncCanceler );
			} catch ( Exception e ) { SMLog.Error( $"{e}\n{ae}" ); }
			try {
				await ae.Run( _asyncCanceler );
			} catch ( Exception e ) { SMLog.Error( $"{e}\n{ae}" ); }
			ae.Dispose();
			SMLog.Debug( $"{nameof( ae.Run )} : end" );


			ae = new SMAsyncEvent();
			ae.AddLast( "1", async c => {
				ae.AddLast( "2", cc => TestError( c, "2" ) );
				ae.Remove( "1" );
				await UTask.DontWait();
			} );
			ae.AddLast( "3", c => TestError( c, "3" ) );
			try {
				await ae.Run( _asyncCanceler );
			} catch ( Exception e )	{ SMLog.Error( e ); }
			try {
				await ae.Run( _asyncCanceler );
			} catch ( Exception e )	{ SMLog.Error( e ); }
			ae.Dispose();
			SMLog.Debug( $"{nameof( ae.Run )} : end" );


			SMLog.Warning( "End" );
		} );
	}
}