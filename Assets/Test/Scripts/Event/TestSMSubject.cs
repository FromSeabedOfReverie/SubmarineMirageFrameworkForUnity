//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#if false
namespace SubmarineMirage.TestEvent {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using KoganeUnityLib;
	using Event;
	using Utility;
	using Debug;
	using TestBase;


	public class TestSMSubject : SMUnitTest {
		readonly SMMultiSubject _events = new SMMultiSubject();


		protected override void Create() {
			Application.targetFrameRate = 10;
			_disposables.AddLast( _events );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestSubject() => From( async () => {
			var ss = new List< Subject<Unit> >();
			2.Times( i => {
				var s = new Subject<Unit>();
				s.Subscribe( _ => SMLog.Debug( $"{i} a" ) );
				s.Subscribe( _ => SMLog.Debug( $"{i} b" ) );
				ss.Add( s );
			} );

			var d = Observable.EveryUpdate().Subscribe( _ => {
				SMLog.Debug( "・EveryUpdate" );
				ss.ForEach( s => s.OnNext( Unit.Default ) );
			} );
			await UTask.Delay( _asyncCanceler, 500 );

			SMLog.Debug( "・OnCompleted" );
			ss.ForEach( s => s.OnCompleted() );
			await UTask.Delay( _asyncCanceler, 500 );

			SMLog.Debug( "・解放" );
			d.Dispose();
			ss.ForEach( s => s.Dispose() );
			await UTask.Delay( _asyncCanceler, 500 );
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestModifyler() => From( async () => {
			TestSMEventSMUtility.SetModifyler(
				_events,
				a => {
					var s = new Subject<Unit>();
					s.Subscribe( _ => a() );
					return s;
				},
				startName => {
					var ifName = $"{nameof( _events.InsertFirst )}";
					SMLog.Debug( $"・{ifName}" );
					_events.InsertFirst( startName, $"{ifName} 10" ).Subscribe( _ => SMLog.Debug( $"{ifName} 10" ) );
					SMLog.Debug( _events );
					_events.InsertFirst( startName ).Subscribe( _ => SMLog.Debug( $"{ifName} 20" ) );
					SMLog.Debug( _events );

					var ilName = $"{nameof( _events.InsertLast )}";
					SMLog.Debug( $"・{ilName}" );
					_events.InsertLast( startName, $"{ilName} 10" ).Subscribe( _ => SMLog.Debug( $"{ilName} 10" ) );
					SMLog.Debug( _events );
					_events.InsertLast( startName ).Subscribe( _ => SMLog.Debug( $"{ilName} 20" ) );
					SMLog.Debug( _events );

					var afName = $"{nameof( _events.AddFirst )}";
					SMLog.Debug( $"・{afName}" );
					_events.AddFirst( $"{afName} 10" ).Subscribe( _ => SMLog.Debug( $"{afName} 10" ) );
					SMLog.Debug( _events );
					_events.AddFirst().Subscribe( _ => SMLog.Debug( $"{afName} 20" ) );
					SMLog.Debug( _events );

					var alName = $"{nameof( _events.AddLast )}";
					SMLog.Debug( $"・{alName}" );
					_events.AddLast( $"{alName} 10" ).Subscribe( _ => SMLog.Debug( $"{alName} 10" ) );
					SMLog.Debug( _events );
					_events.AddLast().Subscribe( _ => SMLog.Debug( $"{alName} 20" ) );
					SMLog.Debug( _events );
				}
			);

			SMLog.Debug( "・実行" );
			_events.Run();
			SMLog.Debug( _events );

			SMLog.Debug( "・解放" );
			_events.Dispose();
			SMLog.Debug( _events );

			await UTask.DontWait();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestChangeWhileRunning() => From( async () => {
			TestSMEventSMUtility.SetChangeWhileRunning(
				_events,
				a => {
					var s = new Subject<Unit>();
					s.Subscribe( _ => a() );
					return s;
				},
				startName => {
					SMLog.Debug( _events );

					var ifName = $"{nameof( _events.InsertFirst )}";
					_events.InsertFirst( startName, $"{ifName} 10" ).Subscribe( _ => SMLog.Debug( $"{ifName} 10" ) );
					_events.InsertFirst( startName ).Subscribe( _ => SMLog.Debug( $"{ifName} 20" ) );

					var ilName = $"{nameof( _events.InsertLast )}";
					_events.InsertLast( startName, $"{ilName} 10" ).Subscribe( _ => SMLog.Debug( $"{ilName} 10" ) );
					_events.InsertLast( startName ).Subscribe( _ => SMLog.Debug( $"{ilName} 20" ) );

					var afName = $"{nameof( _events.AddFirst )}";
					_events.AddFirst( $"{afName} 10" ).Subscribe( _ => SMLog.Debug( $"{afName} 10" ) );
					_events.AddFirst().Subscribe( _ => SMLog.Debug( $"{afName} 20" ) );

					var alName = $"{nameof( _events.AddLast )}";
					_events.AddLast( $"{alName} 10" ).Subscribe( _ => SMLog.Debug( $"{alName} 10" ) );
					_events.AddLast().Subscribe( _ => SMLog.Debug( $"{alName} 20" ) );

					SMLog.Debug( _events );
				}
			);

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
				TestSMEventSMUtility.SetKey( _events, a => {
					var s = new Subject<Unit>();
					s.Subscribe( _ => a() );
					return s;
				} ),
				Observable.EveryUpdate().Subscribe( _ => _events.Run() )
			);

			await UTask.Never( _asyncCanceler );
		} );
	}
}
#endif