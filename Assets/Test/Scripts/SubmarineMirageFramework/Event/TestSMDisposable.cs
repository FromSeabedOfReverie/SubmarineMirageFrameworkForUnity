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
	using UniRx;



	public class TestSMDisposable : SMUnitTest {
		readonly SMAsyncCanceler _asyncCanceler = new SMAsyncCanceler();



		protected override void Create() {
			UniTaskScheduler.UnobservedExceptionWriteLogType = LogType.Error;

			_disposables.AddFirst( () => {
				_asyncCanceler.Dispose();
			} );
		}



		Action TestWait( string name )
			=> () => SMLog.Warning( $"実行 : \n{name}" );

		Action TestError( string name )
			=> () => throw new Exception( $"失敗 : \n{name}" );

		Action TestDispose( SMDisposable @event, string name )
			=> () => {
				SMLog.Warning( $"破棄 : \n{name}" );
				@event.Dispose();
			};



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCompositeDisposable() => From( async () => {
			SMLog.Warning( "Start" );

			var d = new CompositeDisposable();
			d.AddLast( () => SMLog.Debug( 1 ) );
			d.AddLast( () => SMLog.Debug( 2 ) );

			SMLog.Debug( "・破棄 1" );
			d.Dispose();
			SMLog.Debug( "・破棄 2" );
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
			de.InsertFirst( "hoge", "1", Disposable.Create( TestWait( "1" ) ) );
			de.InsertLast( "hoge", "2", Disposable.Create( TestWait( "2" ) ) );
			de.AddFirst( "3", Disposable.Create( TestWait( "3" ) ) );
			de.AddLast( "4", Disposable.Create( TestWait( "4" ) ) );
			SMLog.Debug( "Error : end" );


			de = new SMDisposable();
			de.AddLast( "5", () => {
				de.AddLast( "6", TestWait( "6" ) );
				SMLog.Debug( de );
			} );
			de.AddLast( "7", TestWait( "7" ) );
			de.Dispose();
			SMLog.Debug( "Dispose : end" );


			de = new SMDisposable();
			de.AddLast( "8", TestWait( "8" ) );
			de.AddLast( "9", TestDispose( de, "9" ) );
			de.AddLast( "10", TestWait( "10" ) );
			de.Dispose();
			SMLog.Debug( "Dispose : end" );

			de = new SMDisposable();
			de.AddLast( "11", () =>
				de.AddLast( "12", TestDispose( de, "12" ) )
			);
			de.AddLast( "13", TestDispose( de, "13" ) );
			de.Dispose();
			SMLog.Debug( "Dispose : end" );


			de = new SMDisposable();
			de.AddLast( "14", TestWait( "14" ) );
			de.AddLast( "15", TestError( "15" ) );
			de.AddLast( "16", TestWait( "16" ) );
			de.Dispose();
			SMLog.Debug( "Dispose : end" );

			de = new SMDisposable();
			de.AddLast( "17", () => {
				de.AddLast( "18", TestError( "18" ) );
			} );
			de.AddLast( "19", TestError( "19" ) );
			de.Dispose();
			SMLog.Debug( "Dispose : end" );


			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRegister() => From( async () => {
			await UTask.DelayFrame( _asyncCanceler, 2 );
			SMLog.Warning( "Start" );


			var de = new SMDisposable();

			de.AddLast( "1", TestWait( "1" ) );
			SMLog.Debug( de );

			de.InsertFirst( "1", "2",
				Disposable.Create( TestWait( "2.1" ) ),
				Disposable.Create( TestWait( "2.2" ) )
			);
			SMLog.Debug( de );
			de.InsertFirst( "1",
				Disposable.Create( TestWait( "3.1" ) ),
				Disposable.Create( TestWait( "3.2" ) )
			);
			SMLog.Debug( de );
			de.InsertFirst( "1", "4",
				TestWait( "4.1" ),
				TestWait( "4.2" )
			);
			SMLog.Debug( de );
			de.InsertFirst( "1",
				TestWait( "5.1" ),
				TestWait( "5.2" )
			);
			SMLog.Debug( de );

			de.InsertLast( "1", "6",
				Disposable.Create( TestWait( "6.1" ) ),
				Disposable.Create( TestWait( "6.2" ) )
			);
			SMLog.Debug( de );
			de.InsertLast( "1",
				Disposable.Create( TestWait( "7.1" ) ),
				Disposable.Create( TestWait( "7.2" ) )
			);
			SMLog.Debug( de );
			de.InsertLast( "1", "8",
				TestWait( "8.1" ),
				TestWait( "8.2" )
			);
			SMLog.Debug( de );
			de.InsertLast( "1",
				TestWait( "9.1" ),
				TestWait( "9.2" )
			);
			SMLog.Debug( de );

			de.AddFirst( "10",
				Disposable.Create( TestWait( "10.1" ) ),
				Disposable.Create( TestWait( "10.2" ) )
			);
			SMLog.Debug( de );
			de.AddFirst(
				Disposable.Create( TestWait( "11.1" ) ),
				Disposable.Create( TestWait( "11.2" ) )
			);
			SMLog.Debug( de );
			de.AddFirst( "12",
				TestWait( "12.1" ),
				TestWait( "12.2" )
			);
			SMLog.Debug( de );
			de.AddFirst(
				TestWait( "13.1" ),
				TestWait( "13.2" )
			);
			SMLog.Debug( de );

			de.AddLast( "14",
				Disposable.Create( TestWait( "14.1" ) ),
				Disposable.Create( TestWait( "14.2" ) )
			);
			SMLog.Debug( de );
			de.AddLast(
				Disposable.Create( TestWait( "15.1" ) ),
				Disposable.Create( TestWait( "15.2" ) )
			);
			SMLog.Debug( de );
			de.AddLast( "16",
				TestWait( "16.1" ),
				TestWait( "16.2" )
			);
			SMLog.Debug( de );
			de.AddLast(
				TestWait( "17.1" ),
				TestWait( "17.2" )
			);
			SMLog.Debug( de );

			de._isDebug = true;
			de.Dispose();


			de = new SMDisposable();
			de.AddLast( "1", TestWait( "1" ) );

			de.InsertFirst( "hoge", "2", Disposable.Create( TestWait( "2" ) ) );
			de.InsertLast( "hoge", "3", Disposable.Create( TestWait( "3" ) ) );

			try {
				de.InsertFirst( "1", "4", "hoge 4" );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				de.InsertLast( "1", "5", "hoge 5" );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				de.AddFirst( "6", "hoge 6" );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				de.AddLast( "7", "hoge 7" );
			} catch ( Exception e ) { SMLog.Error( e ); }

			de.Dispose();


			SMLog.Warning( "End" );
		} );
	}
}