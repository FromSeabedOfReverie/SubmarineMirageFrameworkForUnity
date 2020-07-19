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
	using MultiEvent;
	using UTask;
	using Debug;
	using Test;

	public class TestMultiDisposable : Test {
		MultiDisposable _events = new MultiDisposable();


		protected override void Create() {
			Application.targetFrameRate = 10;
			_disposables.AddLast( _events );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestCompositeDisposable() => From( async () => {
			var d = new CompositeDisposable();
			d.Add( Disposable.Create( () => Log.Debug( 1 ) ) );
			d.Add( Disposable.Create( () => Log.Debug( 2 ) ) );

			Log.Debug( "・解放 1" );
			d.Dispose();
			Log.Debug( "・解放 2" );
			d.Dispose();

			await UTask.DontWait();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestModifyler() => From( async () => {
			TestMultiEventUtility.SetModifyler( _events, a => Disposable.Create( a ), startName => {
				var ifName = $"{nameof( _events.InsertFirst )}";
				Log.Debug( $"・{ifName}" );
				_events.InsertFirst( startName, $"{ifName} 10", () => Log.Debug( $"{ifName} 10" ) );
				Log.Debug( _events );
				_events.InsertFirst( startName, () => Log.Debug( $"{ifName} 20" ) );
				Log.Debug( _events );
				_events.InsertFirst( startName, $"{ifName} 30 & 40",
					Disposable.Create( () => Log.Debug( $"{ifName} 30" ) ),
					Disposable.Create( () => Log.Debug( $"{ifName} 40" ) )
				);
				Log.Debug( _events );
				_events.InsertFirst( startName,
					Disposable.Create( () => Log.Debug( $"{ifName} 50" ) ),
					Disposable.Create( () => Log.Debug( $"{ifName} 60" ) )
				);
				Log.Debug( _events );
				_events.InsertFirst( startName, $"{ifName} 70 & 80", new List<IDisposable> {
					Disposable.Create( () => Log.Debug( $"{ifName} 70" ) ),
					Disposable.Create( () => Log.Debug( $"{ifName} 80" ) )
				} );
				Log.Debug( _events );
				_events.InsertFirst( startName, new List<IDisposable> {
					Disposable.Create( () => Log.Debug( $"{ifName} 90" ) ),
					Disposable.Create( () => Log.Debug( $"{ifName} 100" ) )
				} );
				Log.Debug( _events );

				var ilName = $"{nameof( _events.InsertLast )}";
				Log.Debug( $"・{ilName}" );
				_events.InsertLast( startName, $"{ilName} 10", () => Log.Debug( $"{ilName} 10" ) );
				Log.Debug( _events );
				_events.InsertLast( startName, () => Log.Debug( $"{ilName} 20" ) );
				Log.Debug( _events );
				_events.InsertLast( startName, $"{ilName} 30 & 40",
					Disposable.Create( () => Log.Debug( $"{ilName} 30" ) ),
					Disposable.Create( () => Log.Debug( $"{ilName} 40" ) )
				);
				Log.Debug( _events );
				_events.InsertLast( startName,
					Disposable.Create( () => Log.Debug( $"{ilName} 50" ) ),
					Disposable.Create( () => Log.Debug( $"{ilName} 60" ) )
				);
				Log.Debug( _events );
				_events.InsertLast( startName, $"{ilName} 70 & 80", new List<IDisposable> {
					Disposable.Create( () => Log.Debug( $"{ilName} 70" ) ),
					Disposable.Create( () => Log.Debug( $"{ilName} 80" ) )
				} );
				Log.Debug( _events );
				_events.InsertLast( startName, new List<IDisposable> {
					Disposable.Create( () => Log.Debug( $"{ilName} 90" ) ),
					Disposable.Create( () => Log.Debug( $"{ilName} 100" ) )
				} );
				Log.Debug( _events );

				var afName = $"{nameof( _events.AddFirst )}";
				Log.Debug( $"・{afName}" );
				_events.AddFirst( $"{afName} 10", () => Log.Debug( $"{afName} 10" ) );
				Log.Debug( _events );
				_events.AddFirst( () => Log.Debug( $"{afName} 20" ) );
				Log.Debug( _events );
				_events.AddFirst( $"{afName} 30 & 40",
					Disposable.Create( () => Log.Debug( $"{afName} 30" ) ),
					Disposable.Create( () => Log.Debug( $"{afName} 40" ) )
				);
				Log.Debug( _events );
				_events.AddFirst(
					Disposable.Create( () => Log.Debug( $"{afName} 50" ) ),
					Disposable.Create( () => Log.Debug( $"{afName} 60" ) )
				);
				Log.Debug( _events );
				_events.AddFirst( $"{afName} 70 & 80", new List<IDisposable> {
					Disposable.Create( () => Log.Debug( $"{afName} 70" ) ),
					Disposable.Create( () => Log.Debug( $"{afName} 80" ) )
				} );
				Log.Debug( _events );
				_events.AddFirst( new List<IDisposable> {
					Disposable.Create( () => Log.Debug( $"{afName} 90" ) ),
					Disposable.Create( () => Log.Debug( $"{afName} 100" ) )
				} );
				Log.Debug( _events );

				var alName = $"{nameof( _events.AddLast )}";
				Log.Debug( $"・{alName}" );
				_events.AddLast( $"{alName} 10", () => Log.Debug( $"{alName} 10" ) );
				Log.Debug( _events );
				_events.AddLast( () => Log.Debug( $"{alName} 20" ) );
				Log.Debug( _events );
				_events.AddLast( $"{alName} 30 & 40",
					Disposable.Create( () => Log.Debug( $"{alName} 30" ) ),
					Disposable.Create( () => Log.Debug( $"{alName} 40" ) )
				);
				Log.Debug( _events );
				_events.AddLast(
					Disposable.Create( () => Log.Debug( $"{alName} 50" ) ),
					Disposable.Create( () => Log.Debug( $"{alName} 60" ) )
				);
				Log.Debug( _events );
				_events.AddLast( $"{alName} 70 & 80", new List<IDisposable> {
					Disposable.Create( () => Log.Debug( $"{alName} 70" ) ),
					Disposable.Create( () => Log.Debug( $"{alName} 80" ) )
				} );
				Log.Debug( _events );
				_events.AddLast( new List<IDisposable> {
					Disposable.Create( () => Log.Debug( $"{alName} 90" ) ),
					Disposable.Create( () => Log.Debug( $"{alName} 100" ) )
				} );
				Log.Debug( _events );
			} );

			Log.Debug( "・解放 1" );
			_events.Dispose();
			Log.Debug( _events );

			Log.Debug( "・解放 2" );
			_events.Dispose();
			Log.Debug( _events );

			await UTask.DontWait();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestChangeWhileRunning() => From( async () => {
			TestMultiEventUtility.SetChangeWhileRunning( _events, a => Disposable.Create( a ), startName => {
				Log.Debug( _events );

				var ifName = $"{nameof( _events.InsertFirst )}";
				_events.InsertFirst( startName, $"{ifName} 10", () => Log.Debug( $"{ifName} 10" ) );
				_events.InsertFirst( startName, () => Log.Debug( $"{ifName} 20" ) );
				_events.InsertFirst( startName, $"{ifName} 30 & 40",
					Disposable.Create( () => Log.Debug( $"{ifName} 30" ) ),
					Disposable.Create( () => Log.Debug( $"{ifName} 40" ) )
				);
				_events.InsertFirst( startName,
					Disposable.Create( () => Log.Debug( $"{ifName} 50" ) ),
					Disposable.Create( () => Log.Debug( $"{ifName} 60" ) )
				);
				_events.InsertFirst( startName, $"{ifName} 70 & 80", new List<IDisposable> {
					Disposable.Create( () => Log.Debug( $"{ifName} 70" ) ),
					Disposable.Create( () => Log.Debug( $"{ifName} 80" ) )
				} );
				_events.InsertFirst( startName, new List<IDisposable> {
					Disposable.Create( () => Log.Debug( $"{ifName} 90" ) ),
					Disposable.Create( () => Log.Debug( $"{ifName} 100" ) )
				} );

				var ilName = $"{nameof( _events.InsertLast )}";
				_events.InsertLast( startName, $"{ilName} 10", () => Log.Debug( $"{ilName} 10" ) );
				_events.InsertLast( startName, () => Log.Debug( $"{ilName} 20" ) );
				_events.InsertLast( startName, $"{ilName} 30 & 40",
					Disposable.Create( () => Log.Debug( $"{ilName} 30" ) ),
					Disposable.Create( () => Log.Debug( $"{ilName} 40" ) )
				);
				_events.InsertLast( startName,
					Disposable.Create( () => Log.Debug( $"{ilName} 50" ) ),
					Disposable.Create( () => Log.Debug( $"{ilName} 60" ) )
				);
				_events.InsertLast( startName, $"{ilName} 70 & 80", new List<IDisposable> {
					Disposable.Create( () => Log.Debug( $"{ilName} 70" ) ),
					Disposable.Create( () => Log.Debug( $"{ilName} 80" ) )
				} );
				_events.InsertLast( startName, new List<IDisposable> {
					Disposable.Create( () => Log.Debug( $"{ilName} 90" ) ),
					Disposable.Create( () => Log.Debug( $"{ilName} 100" ) )
				} );

				var afName = $"{nameof( _events.AddFirst )}";
				_events.AddFirst( $"{afName} 10", () => Log.Debug( $"{afName} 10" ) );
				_events.AddFirst( () => Log.Debug( $"{afName} 20" ) );
				_events.AddFirst( $"{afName} 30 & 40",
					Disposable.Create( () => Log.Debug( $"{afName} 30" ) ),
					Disposable.Create( () => Log.Debug( $"{afName} 40" ) )
				);
				_events.AddFirst(
					Disposable.Create( () => Log.Debug( $"{afName} 50" ) ),
					Disposable.Create( () => Log.Debug( $"{afName} 60" ) )
				);
				_events.AddFirst( $"{afName} 70 & 80", new List<IDisposable> {
					Disposable.Create( () => Log.Debug( $"{afName} 70" ) ),
					Disposable.Create( () => Log.Debug( $"{afName} 80" ) )
				} );
				_events.AddFirst( new List<IDisposable> {
					Disposable.Create( () => Log.Debug( $"{afName} 90" ) ),
					Disposable.Create( () => Log.Debug( $"{afName} 100" ) )
				} );

				var alName = $"{nameof( _events.AddLast )}";
				_events.AddLast( $"{alName} 10", () => Log.Debug( $"{alName} 10" ) );
				_events.AddLast( () => Log.Debug( $"{alName} 20" ) );
				_events.AddLast( $"{alName} 30 & 40",
					Disposable.Create( () => Log.Debug( $"{alName} 30" ) ),
					Disposable.Create( () => Log.Debug( $"{alName} 40" ) )
				);
				_events.AddLast(
					Disposable.Create( () => Log.Debug( $"{alName} 50" ) ),
					Disposable.Create( () => Log.Debug( $"{alName} 60" ) )
				);
				_events.AddLast( $"{alName} 70 & 80", new List<IDisposable> {
					Disposable.Create( () => Log.Debug( $"{alName} 70" ) ),
					Disposable.Create( () => Log.Debug( $"{alName} 80" ) )
				} );
				_events.AddLast( new List<IDisposable> {
					Disposable.Create( () => Log.Debug( $"{alName} 90" ) ),
					Disposable.Create( () => Log.Debug( $"{alName} 100" ) )
				} );

				Log.Debug( _events );
			} );

			Log.Debug( "・解放 1" );
			_events.Dispose();
			Log.Debug( _events );

			Log.Debug( "・解放 2" );
			_events.Dispose();
			Log.Debug( _events );

			await UTask.DontWait();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( async () => {
			_disposables.AddLast( TestMultiEventUtility.SetKey( _events, a => Disposable.Create( a ) ) );

			await UTask.Never( _asyncCanceler );
		} );
	}
}