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
	using Extension;
	using Utility;
	using Debug;
	using TestBase;

	public class TestSMMultiDisposable : SMStandardTest {
		readonly SMMultiDisposable _events = new SMMultiDisposable();


		protected override void Create() {
			Application.targetFrameRate = 10;
			_disposables.AddLast( _events );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestCompositeDisposable() => From( async () => {
			var d = new CompositeDisposable();
			d.Add( Disposable.Create( () => SMLog.Debug( 1 ) ) );
			d.Add( Disposable.Create( () => SMLog.Debug( 2 ) ) );

			SMLog.Debug( "・解放 1" );
			d.Dispose();
			SMLog.Debug( "・解放 2" );
			d.Dispose();

			await UTask.DontWait();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestModifyler() => From( async () => {
			TestSMMultiEventSMUtility.SetModifyler( _events, a => Disposable.Create( a ), startName => {
				var ifName = $"{nameof( _events.InsertFirst )}";
				SMLog.Debug( $"・{ifName}" );
				_events.InsertFirst( startName, $"{ifName} 10", () => SMLog.Debug( $"{ifName} 10" ) );
				SMLog.Debug( _events );
				_events.InsertFirst( startName, () => SMLog.Debug( $"{ifName} 20" ) );
				SMLog.Debug( _events );
				_events.InsertFirst( startName, $"{ifName} 30 & 40",
					Disposable.Create( () => SMLog.Debug( $"{ifName} 30" ) ),
					Disposable.Create( () => SMLog.Debug( $"{ifName} 40" ) )
				);
				SMLog.Debug( _events );
				_events.InsertFirst( startName,
					Disposable.Create( () => SMLog.Debug( $"{ifName} 50" ) ),
					Disposable.Create( () => SMLog.Debug( $"{ifName} 60" ) )
				);
				SMLog.Debug( _events );
				_events.InsertFirst( startName, $"{ifName} 70 & 80", new [] {
					Disposable.Create( () => SMLog.Debug( $"{ifName} 70" ) ),
					Disposable.Create( () => SMLog.Debug( $"{ifName} 80" ) )
				} );
				SMLog.Debug( _events );
				_events.InsertFirst( startName, new [] {
					Disposable.Create( () => SMLog.Debug( $"{ifName} 90" ) ),
					Disposable.Create( () => SMLog.Debug( $"{ifName} 100" ) )
				} );
				SMLog.Debug( _events );

				var ilName = $"{nameof( _events.InsertLast )}";
				SMLog.Debug( $"・{ilName}" );
				_events.InsertLast( startName, $"{ilName} 10", () => SMLog.Debug( $"{ilName} 10" ) );
				SMLog.Debug( _events );
				_events.InsertLast( startName, () => SMLog.Debug( $"{ilName} 20" ) );
				SMLog.Debug( _events );
				_events.InsertLast( startName, $"{ilName} 30 & 40",
					Disposable.Create( () => SMLog.Debug( $"{ilName} 30" ) ),
					Disposable.Create( () => SMLog.Debug( $"{ilName} 40" ) )
				);
				SMLog.Debug( _events );
				_events.InsertLast( startName,
					Disposable.Create( () => SMLog.Debug( $"{ilName} 50" ) ),
					Disposable.Create( () => SMLog.Debug( $"{ilName} 60" ) )
				);
				SMLog.Debug( _events );
				_events.InsertLast( startName, $"{ilName} 70 & 80", new [] {
					Disposable.Create( () => SMLog.Debug( $"{ilName} 70" ) ),
					Disposable.Create( () => SMLog.Debug( $"{ilName} 80" ) )
				} );
				SMLog.Debug( _events );
				_events.InsertLast( startName, new [] {
					Disposable.Create( () => SMLog.Debug( $"{ilName} 90" ) ),
					Disposable.Create( () => SMLog.Debug( $"{ilName} 100" ) )
				} );
				SMLog.Debug( _events );

				var afName = $"{nameof( _events.AddFirst )}";
				SMLog.Debug( $"・{afName}" );
				_events.AddFirst( $"{afName} 10", () => SMLog.Debug( $"{afName} 10" ) );
				SMLog.Debug( _events );
				_events.AddFirst( () => SMLog.Debug( $"{afName} 20" ) );
				SMLog.Debug( _events );
				_events.AddFirst( $"{afName} 30 & 40",
					Disposable.Create( () => SMLog.Debug( $"{afName} 30" ) ),
					Disposable.Create( () => SMLog.Debug( $"{afName} 40" ) )
				);
				SMLog.Debug( _events );
				_events.AddFirst(
					Disposable.Create( () => SMLog.Debug( $"{afName} 50" ) ),
					Disposable.Create( () => SMLog.Debug( $"{afName} 60" ) )
				);
				SMLog.Debug( _events );
				_events.AddFirst( $"{afName} 70 & 80", new [] {
					Disposable.Create( () => SMLog.Debug( $"{afName} 70" ) ),
					Disposable.Create( () => SMLog.Debug( $"{afName} 80" ) )
				} );
				SMLog.Debug( _events );
				_events.AddFirst( new [] {
					Disposable.Create( () => SMLog.Debug( $"{afName} 90" ) ),
					Disposable.Create( () => SMLog.Debug( $"{afName} 100" ) )
				} );
				SMLog.Debug( _events );

				var alName = $"{nameof( _events.AddLast )}";
				SMLog.Debug( $"・{alName}" );
				_events.AddLast( $"{alName} 10", () => SMLog.Debug( $"{alName} 10" ) );
				SMLog.Debug( _events );
				_events.AddLast( () => SMLog.Debug( $"{alName} 20" ) );
				SMLog.Debug( _events );
				_events.AddLast( $"{alName} 30 & 40",
					Disposable.Create( () => SMLog.Debug( $"{alName} 30" ) ),
					Disposable.Create( () => SMLog.Debug( $"{alName} 40" ) )
				);
				SMLog.Debug( _events );
				_events.AddLast(
					Disposable.Create( () => SMLog.Debug( $"{alName} 50" ) ),
					Disposable.Create( () => SMLog.Debug( $"{alName} 60" ) )
				);
				SMLog.Debug( _events );
				_events.AddLast( $"{alName} 70 & 80", new [] {
					Disposable.Create( () => SMLog.Debug( $"{alName} 70" ) ),
					Disposable.Create( () => SMLog.Debug( $"{alName} 80" ) )
				} );
				SMLog.Debug( _events );
				_events.AddLast( new [] {
					Disposable.Create( () => SMLog.Debug( $"{alName} 90" ) ),
					Disposable.Create( () => SMLog.Debug( $"{alName} 100" ) )
				} );
				SMLog.Debug( _events );
			} );

			SMLog.Debug( "・解放 1" );
			_events.Dispose();
			SMLog.Debug( _events );

			SMLog.Debug( "・解放 2" );
			_events.Dispose();
			SMLog.Debug( _events );

			await UTask.DontWait();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestChangeWhileRunning() => From( async () => {
			TestSMMultiEventSMUtility.SetChangeWhileRunning( _events, a => Disposable.Create( a ), startName => {
				SMLog.Debug( _events );

				var ifName = $"{nameof( _events.InsertFirst )}";
				_events.InsertFirst( startName, $"{ifName} 10", () => SMLog.Debug( $"{ifName} 10" ) );
				_events.InsertFirst( startName, () => SMLog.Debug( $"{ifName} 20" ) );
				_events.InsertFirst( startName, $"{ifName} 30 & 40",
					Disposable.Create( () => SMLog.Debug( $"{ifName} 30" ) ),
					Disposable.Create( () => SMLog.Debug( $"{ifName} 40" ) )
				);
				_events.InsertFirst( startName,
					Disposable.Create( () => SMLog.Debug( $"{ifName} 50" ) ),
					Disposable.Create( () => SMLog.Debug( $"{ifName} 60" ) )
				);
				_events.InsertFirst( startName, $"{ifName} 70 & 80", new [] {
					Disposable.Create( () => SMLog.Debug( $"{ifName} 70" ) ),
					Disposable.Create( () => SMLog.Debug( $"{ifName} 80" ) )
				} );
				_events.InsertFirst( startName, new [] {
					Disposable.Create( () => SMLog.Debug( $"{ifName} 90" ) ),
					Disposable.Create( () => SMLog.Debug( $"{ifName} 100" ) )
				} );

				var ilName = $"{nameof( _events.InsertLast )}";
				_events.InsertLast( startName, $"{ilName} 10", () => SMLog.Debug( $"{ilName} 10" ) );
				_events.InsertLast( startName, () => SMLog.Debug( $"{ilName} 20" ) );
				_events.InsertLast( startName, $"{ilName} 30 & 40",
					Disposable.Create( () => SMLog.Debug( $"{ilName} 30" ) ),
					Disposable.Create( () => SMLog.Debug( $"{ilName} 40" ) )
				);
				_events.InsertLast( startName,
					Disposable.Create( () => SMLog.Debug( $"{ilName} 50" ) ),
					Disposable.Create( () => SMLog.Debug( $"{ilName} 60" ) )
				);
				_events.InsertLast( startName, $"{ilName} 70 & 80", new [] {
					Disposable.Create( () => SMLog.Debug( $"{ilName} 70" ) ),
					Disposable.Create( () => SMLog.Debug( $"{ilName} 80" ) )
				} );
				_events.InsertLast( startName, new [] {
					Disposable.Create( () => SMLog.Debug( $"{ilName} 90" ) ),
					Disposable.Create( () => SMLog.Debug( $"{ilName} 100" ) )
				} );

				var afName = $"{nameof( _events.AddFirst )}";
				_events.AddFirst( $"{afName} 10", () => SMLog.Debug( $"{afName} 10" ) );
				_events.AddFirst( () => SMLog.Debug( $"{afName} 20" ) );
				_events.AddFirst( $"{afName} 30 & 40",
					Disposable.Create( () => SMLog.Debug( $"{afName} 30" ) ),
					Disposable.Create( () => SMLog.Debug( $"{afName} 40" ) )
				);
				_events.AddFirst(
					Disposable.Create( () => SMLog.Debug( $"{afName} 50" ) ),
					Disposable.Create( () => SMLog.Debug( $"{afName} 60" ) )
				);
				_events.AddFirst( $"{afName} 70 & 80", new [] {
					Disposable.Create( () => SMLog.Debug( $"{afName} 70" ) ),
					Disposable.Create( () => SMLog.Debug( $"{afName} 80" ) )
				} );
				_events.AddFirst( new [] {
					Disposable.Create( () => SMLog.Debug( $"{afName} 90" ) ),
					Disposable.Create( () => SMLog.Debug( $"{afName} 100" ) )
				} );

				var alName = $"{nameof( _events.AddLast )}";
				_events.AddLast( $"{alName} 10", () => SMLog.Debug( $"{alName} 10" ) );
				_events.AddLast( () => SMLog.Debug( $"{alName} 20" ) );
				_events.AddLast( $"{alName} 30 & 40",
					Disposable.Create( () => SMLog.Debug( $"{alName} 30" ) ),
					Disposable.Create( () => SMLog.Debug( $"{alName} 40" ) )
				);
				_events.AddLast(
					Disposable.Create( () => SMLog.Debug( $"{alName} 50" ) ),
					Disposable.Create( () => SMLog.Debug( $"{alName} 60" ) )
				);
				_events.AddLast( $"{alName} 70 & 80", new [] {
					Disposable.Create( () => SMLog.Debug( $"{alName} 70" ) ),
					Disposable.Create( () => SMLog.Debug( $"{alName} 80" ) )
				} );
				_events.AddLast( new [] {
					Disposable.Create( () => SMLog.Debug( $"{alName} 90" ) ),
					Disposable.Create( () => SMLog.Debug( $"{alName} 100" ) )
				} );

				SMLog.Debug( _events );
			} );

			SMLog.Debug( "・解放 1" );
			_events.Dispose();
			SMLog.Debug( _events );

			SMLog.Debug( "・解放 2" );
			_events.Dispose();
			SMLog.Debug( _events );

			await UTask.DontWait();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( async () => {
			_disposables.AddLast( TestSMMultiEventSMUtility.SetKey( _events, a => Disposable.Create( a ) ) );

			await UTask.Never( _asyncCanceler );
		} );
	}
}