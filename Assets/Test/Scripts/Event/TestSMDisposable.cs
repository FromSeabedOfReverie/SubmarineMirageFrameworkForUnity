//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestEvent {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using Event;
	using Extension;
	using Utility;
	using Debug;
	using TestBase;



	public class TestSMDisposable : SMUnitTest {
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

		Action TestDispose( SMDisposable @event, string name )
			=> () => {
				SMLog.Warning( $"解放 : \n{name}" );
				@event.Dispose();
			};



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCompositeDisposable() => From( async () => {
			SMLog.Warning( "Start" );

			var d = new CompositeDisposable();
			d.Add( Disposable.Create( () => SMLog.Debug( 1 ) ) );
			d.Add( Disposable.Create( () => SMLog.Debug( 2 ) ) );

			SMLog.Debug( "・解放 1" );
			d.Dispose();
			SMLog.Debug( "・解放 2" );
			d.Dispose();

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDispose() => From( async () => {
			await UTask.DelayFrame( _asyncCanceler, 2 );
			SMLog.Warning( "Start" );

			var de = new SMDisposable();
			de.Dispose();
			de.InsertFirst( "hoge", "1", Disposable.Create( () => {} ) );
			de.InsertLast( "hoge", "2", Disposable.Create( () => {} ) );
			de.AddFirst( "3", Disposable.Create( () => {} ) );
			de.AddLast( "4", Disposable.Create( () => {} ) );

			de = new SMDisposable();
			de.AddLast( TestWait( "1" ) );
			de.AddLast( TestDispose( de, "2" ) );
			de.AddLast( TestWait( "3" ) );
			de.Dispose();

			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRegister() => From( async () => {
			SMLog.Warning( "Start" );

			var de = new SMDisposable();

			de.AddLast( "1", TestWait( "1" ) );
			SMLog.Debug( de );

			de.InsertFirst( "1", "2", TestWait( "2" ) );
			SMLog.Debug( de );
			de.InsertFirst( "1", TestWait( "3" ) );
			SMLog.Debug( de );

			de.InsertLast( "1", "4", TestWait( "4" ) );
			SMLog.Debug( de );
			de.InsertLast( "1", TestWait( "5" ) );
			SMLog.Debug( de );

			de.AddFirst( "6", TestWait( "6" ) );
			SMLog.Debug( de );
			de.AddFirst( TestWait( "7" ) );
			SMLog.Debug( de );

			de.AddLast( "8", TestWait( "8" ) );
			SMLog.Debug( de );
			de.AddLast( TestWait( "9" ) );
			SMLog.Debug( de );

			de.AddLast( "1", TestWait( "1" ) );
			SMLog.Debug( de );

			de.Reverse();
			SMLog.Debug( de );
			de.Remove( "1" );
			SMLog.Debug( de );

			de._isDebug = true;
			de.Run();
			await UTask.WaitWhile( _asyncCanceler, () => de._isRunning );

			de.Dispose();

			de = new SMDisposable();
			de.InsertFirst( "hoge", "10", TestWait( "10" ) );
			de.InsertLast( "hoge", "11", TestWait( "11" ) );
			de.Dispose();

			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRun() => From( async () => {
			SMLog.Warning( "Start" );


			var de = new SMDisposable();
			de.AddLast( TestWait( "1" ) );
			de.AddLast( TestWait( "2" ) );
			de.Run();
			de.Run();
			SMLog.Debug( $"{nameof( de.Run )} : end" );
			de.Dispose();


			de = new SMDisposable();
			de.AddLast( "1", () => {
				de.AddLast( "2", TestWait( "2" ) );
				SMLog.Debug( de );
				de.AddLast( "3", TestWait( "3" ) );
				SMLog.Debug( de );

				de.Remove( "2" );
				SMLog.Debug( de );
				de.Remove( "1" );
				SMLog.Debug( de );
			} );
			de.AddLast( "1.5", TestWait( "1.5" ) );
			de.Run();
			de.Run();
			SMLog.Debug( $"{nameof( de.Run )} : end" );
			de.Dispose();


			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDisposeRun() => From( async () => {
			await UTask.DelayFrame( _asyncCanceler, 2 );
			SMLog.Warning( "Start" );


			var de = new SMDisposable();
			de.AddLast( TestWait(		"1" ) );
			de.AddLast( TestDispose(	de, "2" ) );
			de.AddLast( TestWait(		"3" ) );
			de.Run();
			de.Dispose();
			SMLog.Debug( $"{nameof( de.Run )} : end" );


			de = new SMDisposable();
			de.AddLast( "1", () =>
				de.AddLast( "2", TestDispose( de, "2" ) )
			);
			de.AddLast( "3", TestDispose( de, "3" ) );
			de.Run();
			de.Run();
			de.Dispose();
			SMLog.Debug( $"{nameof( de.Run )} : end" );


			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestErrorRun() => From( async () => {
			await UTask.DelayFrame( _asyncCanceler, 2 );
			SMLog.Warning( "Start" );


			var de = new SMDisposable();
			de.AddLast( "1", TestWait(	"1" ) );
			de.AddLast( "2", TestError(	"2" ) );
			de.AddLast( "3", TestWait(	"3" ) );
			de.Run();
			de.Run();
			de.Dispose();
			SMLog.Debug( $"{nameof( de.Run )} : end" );


			de = new SMDisposable();
			de.AddLast( "1", () => {
				de.AddLast( "2", TestError( "2" ) );
				de.Remove( "1" );
			} );
			de.AddLast( "3", TestError( "3" ) );
			de.Run();
			de.Run();
			de.Dispose();
			SMLog.Debug( $"{nameof( de.Run )} : end" );


			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );
	}
}