//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Test {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;



	public class TestSMSubject : SMUnitTest {
		readonly SMAsyncCanceler _asyncCanceler = new SMAsyncCanceler();



		protected override void Create() {
			UniTaskScheduler.UnobservedExceptionWriteLogType = LogType.Error;

			_disposables.AddFirst( () => {
				_asyncCanceler.Dispose();
			} );
		}



		Action<Unit> TestWait( string name )
			=> _ => SMLog.Warning( $"実行 : \n{name}" );

		Action<Unit> TestError( string name )
			=> _ => throw new Exception( $"失敗 : \n{name}" );

		Action<Unit> TestDispose( SMSubject @event, string name )
			=> _ => {
				SMLog.Warning( $"破棄 : \n{name}" );
				@event.Dispose();
			};



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestSubject() => From( async () => {
			List< Subject<Unit> > CreateSubjects() {
				var result = new List<Subject<Unit>>();
				2.Times( i => {
					var s = new Subject<Unit>();
					2.Times( ii => s.Subscribe(
						_ => SMLog.Debug( $"{i}.{ii}" ),
						e => SMLog.Error( e ),
						() => SMLog.Debug( $"{i}.{ii} : Complete" )
					) );
					result.Add( s );
				} );
				return result;
			}


			await UTask.DelayFrame( _asyncCanceler, 2 );
			SMLog.Warning( "Start" );


			var ss = CreateSubjects();
			SMLog.Debug( "Create" );
			var d = Observable.EveryUpdate().Subscribe( _ => {
				SMLog.Debug( "・EveryUpdate" );
				ss.ForEach( s => s.OnNext( Unit.Default ) );
			} );
			await UTask.DelayFrame( _asyncCanceler, 3 );

			SMLog.Debug( "・OnCompleted" );
			ss.ForEach( s => s.OnCompleted() );
			await UTask.DelayFrame( _asyncCanceler, 3 );

			SMLog.Debug( "・破棄" );
			ss.ForEach( s => s.Dispose() );
			ss.Clear();
			await UTask.DelayFrame( _asyncCanceler, 3 );


			ss = CreateSubjects();
			SMLog.Debug( "Create" );
			await UTask.DelayFrame( _asyncCanceler, 3 );

			SMLog.Debug( "・破棄" );
			ss.ForEach( s => s.Dispose() );
			SMLog.Debug( "・OnCompleted" );
			try {
				ss.ForEach( s => s.OnCompleted() );
			} catch ( Exception e )	{ SMLog.Error( e ); }
			ss.Clear();
			await UTask.DelayFrame( _asyncCanceler, 3 );


			d.Dispose();
			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDispose() => From( async () => {
			await UTask.DelayFrame( _asyncCanceler, 2 );
			SMLog.Warning( "Start" );

			var se = new SMSubject();
			se.Dispose();

			se.Remove( "hoge" );
			se.Reverse();
			se.InsertFirst( "hoge" ).Subscribe( _ => {} );
			se.InsertLast( "hoge" ).Subscribe( _ => {} );
			se.AddFirst().Subscribe( _ => {} );
			se.AddLast().Subscribe( _ => {} );
			se.Run();

			se = new SMSubject();
			se.AddLast().Subscribe( TestWait( "1" ) );
			se.AddLast().Subscribe( TestDispose( se, "2" ) );
			se.AddLast().Subscribe( TestWait( "3" ) );
			se.Run();

			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRegister() => From( async () => {
			SMLog.Warning( "Start" );

			var se = new SMSubject();

			se.AddLast( "1" ).Subscribe( TestWait( "1" ) );
			SMLog.Debug( se );

			se.InsertFirst( "1", "2" ).Subscribe( TestWait( "2" ) );
			SMLog.Debug( se );
			se.InsertFirst( "1" ).Subscribe( TestWait( "3" ) );
			SMLog.Debug( se );

			se.InsertLast( "1", "4" ).Subscribe( TestWait( "4" ) );
			SMLog.Debug( se );
			se.InsertLast( "1" ).Subscribe( TestWait( "5" ) );
			SMLog.Debug( se );

			se.AddFirst( "6" ).Subscribe( TestWait( "6" ) );
			SMLog.Debug( se );
			se.AddFirst().Subscribe( TestWait( "7" ) );
			SMLog.Debug( se );

			se.AddLast( "8" ).Subscribe( TestWait( "8" ) );
			SMLog.Debug( se );
			se.AddLast().Subscribe( TestWait( "9" ) );
			SMLog.Debug( se );

			se.AddLast( "1" ).Subscribe( TestWait( "1" ) );
			SMLog.Debug( se );

			se.Reverse();
			SMLog.Debug( se );
			se.Remove( "1" );
			SMLog.Debug( se );

			se._isDebug = true;
			se.Run();
			await UTask.WaitWhile( _asyncCanceler, () => se._isRunning );

			se.Dispose();

			se = new SMSubject();
			try {
				se.InsertFirst( "hoge", "10" ).Subscribe( TestWait( "10" ) );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				se.InsertLast( "hoge", "11" ).Subscribe( TestWait( "11" ) );
			} catch ( Exception e ) { SMLog.Error( e ); }
			se.Dispose();

			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRun() => From( async () => {
			SMLog.Warning( "Start" );


			var se = new SMSubject();
			se.AddLast().Subscribe( TestWait( "1" ) );
			se.AddLast().Subscribe( TestWait( "2" ) );
			se.Run();
			se.Run();
			SMLog.Debug( $"{nameof( se.Run )} : end" );
			se.Dispose();


			se = new SMSubject();
			se.AddLast( "1" ).Subscribe( _ => {
				se.AddLast( "2" ).Subscribe( TestWait( "2" ) );
				SMLog.Debug( se );
				se.AddLast( "3" ).Subscribe( TestWait( "3" ) );
				SMLog.Debug( se );

				se.Remove( "2" );
				SMLog.Debug( se );
				se.Remove( "1" );
				SMLog.Debug( se );
			} );
			se.AddLast( "1.5" ).Subscribe( TestWait( "1.5" ) );
			se.Run();
			se.Run();
			SMLog.Debug( $"{nameof( se.Run )} : end" );
			se.Dispose();


			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDisposeRun() => From( async () => {
			await UTask.DelayFrame( _asyncCanceler, 2 );
			SMLog.Warning( "Start" );


			var se = new SMSubject();
			se.AddLast().Subscribe( TestWait(		"1" ) );
			se.AddLast().Subscribe( TestDispose(	se, "2" ) );
			se.AddLast().Subscribe( TestWait(		"3" ) );
			se.Run();
			se.Dispose();
			SMLog.Debug( $"{nameof( se.Run )} : end" );


			se = new SMSubject();
			se.AddLast( "1" ).Subscribe( _ =>
				se.AddLast( "2" ).Subscribe( TestDispose( se, "2" ) )
			);
			se.AddLast( "3" ).Subscribe( TestDispose( se, "3" ) );
			se.Run();
			se.Run();
			se.Dispose();
			SMLog.Debug( $"{nameof( se.Run )} : end" );


			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestErrorRun() => From( async () => {
			await UTask.DelayFrame( _asyncCanceler, 2 );
			SMLog.Warning( "Start" );


			var se = new SMSubject();
			se.AddLast( "1" ).Subscribe( TestWait(	"1" ) );
			se.AddLast( "2" ).Subscribe( TestError(	"2" ) );
			se.AddLast( "3" ).Subscribe( TestWait(	"3" ) );
			se.Run();
			se.Run();
			se.Dispose();
			SMLog.Debug( $"{nameof( se.Run )} : end" );


			se = new SMSubject();
			se.AddLast( "1" ).Subscribe( _ => {
				se.AddLast( "2" ).Subscribe( TestError( "2" ) );
				se.Remove( "1" );
			} );
			se.AddLast( "3" ).Subscribe( TestError( "3" ) );
			se.Run();
			se.Run();
			se.Dispose();
			SMLog.Debug( $"{nameof( se.Run )} : end" );


			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );
	}
}