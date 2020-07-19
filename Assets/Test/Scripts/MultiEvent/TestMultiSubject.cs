//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestMultiEvent {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using KoganeUnityLib;
	using MultiEvent;
	using UTask;
	using Debug;
	using Test;


	public class TestMultiSubject : Test {
		MultiSubject _events = new MultiSubject();


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
				s.Subscribe( _ => Log.Debug( $"{i} a" ) );
				s.Subscribe( _ => Log.Debug( $"{i} b" ) );
				ss.Add( s );
			} );

			var d = Observable.EveryUpdate().Subscribe( _ => {
				Log.Debug( "・EveryUpdate" );
				ss.ForEach( s => s.OnNext( Unit.Default ) );
			} );
			await UTask.Delay( _asyncCanceler, 500 );

			Log.Debug( "・OnCompleted" );
			ss.ForEach( s => s.OnCompleted() );
			await UTask.Delay( _asyncCanceler, 500 );

			Log.Debug( "・解放" );
			d.Dispose();
			ss.ForEach( s => s.Dispose() );
			await UTask.Delay( _asyncCanceler, 500 );
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestModifyler() => From( async () => {
			TestMultiEventUtility.SetModifyler(
				_events,
				a => {
					var s = new Subject<Unit>();
					s.Subscribe( _ => a() );
					return s;
				},
				startName => {
					var ifName = $"{nameof( _events.InsertFirst )}";
					Log.Debug( $"・{ifName}" );
					_events.InsertFirst( startName, $"{ifName} 10" ).Subscribe( _ => Log.Debug( $"{ifName} 10" ) );
					Log.Debug( _events );
					_events.InsertFirst( startName ).Subscribe( _ => Log.Debug( $"{ifName} 20" ) );
					Log.Debug( _events );

					var ilName = $"{nameof( _events.InsertLast )}";
					Log.Debug( $"・{ilName}" );
					_events.InsertLast( startName, $"{ilName} 10" ).Subscribe( _ => Log.Debug( $"{ilName} 10" ) );
					Log.Debug( _events );
					_events.InsertLast( startName ).Subscribe( _ => Log.Debug( $"{ilName} 20" ) );
					Log.Debug( _events );

					var afName = $"{nameof( _events.AddFirst )}";
					Log.Debug( $"・{afName}" );
					_events.AddFirst( $"{afName} 10" ).Subscribe( _ => Log.Debug( $"{afName} 10" ) );
					Log.Debug( _events );
					_events.AddFirst().Subscribe( _ => Log.Debug( $"{afName} 20" ) );
					Log.Debug( _events );

					var alName = $"{nameof( _events.AddLast )}";
					Log.Debug( $"・{alName}" );
					_events.AddLast( $"{alName} 10" ).Subscribe( _ => Log.Debug( $"{alName} 10" ) );
					Log.Debug( _events );
					_events.AddLast().Subscribe( _ => Log.Debug( $"{alName} 20" ) );
					Log.Debug( _events );
				}
			);

			Log.Debug( "・実行" );
			_events.Run();
			Log.Debug( _events );

			Log.Debug( "・解放" );
			_events.Dispose();
			Log.Debug( _events );

			await UTask.DontWait();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestChangeWhileRunning() => From( async () => {
			TestMultiEventUtility.SetChangeWhileRunning(
				_events,
				a => {
					var s = new Subject<Unit>();
					s.Subscribe( _ => a() );
					return s;
				},
				startName => {
					Log.Debug( _events );

					var ifName = $"{nameof( _events.InsertFirst )}";
					_events.InsertFirst( startName, $"{ifName} 10" ).Subscribe( _ => Log.Debug( $"{ifName} 10" ) );
					_events.InsertFirst( startName ).Subscribe( _ => Log.Debug( $"{ifName} 20" ) );

					var ilName = $"{nameof( _events.InsertLast )}";
					_events.InsertLast( startName, $"{ilName} 10" ).Subscribe( _ => Log.Debug( $"{ilName} 10" ) );
					_events.InsertLast( startName ).Subscribe( _ => Log.Debug( $"{ilName} 20" ) );

					var afName = $"{nameof( _events.AddFirst )}";
					_events.AddFirst( $"{afName} 10" ).Subscribe( _ => Log.Debug( $"{afName} 10" ) );
					_events.AddFirst().Subscribe( _ => Log.Debug( $"{afName} 20" ) );

					var alName = $"{nameof( _events.AddLast )}";
					_events.AddLast( $"{alName} 10" ).Subscribe( _ => Log.Debug( $"{alName} 10" ) );
					_events.AddLast().Subscribe( _ => Log.Debug( $"{alName} 20" ) );

					Log.Debug( _events );
				}
			);

			Log.Debug( "・実行 1" );
			_events.Run();
			Log.Debug( _events );

			Log.Debug( "・実行 2" );
			_events.Run();
			Log.Debug( _events );

			await UTask.DontWait();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( async () => {
			_disposables.AddLast(
				TestMultiEventUtility.SetKey( _events, a => {
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