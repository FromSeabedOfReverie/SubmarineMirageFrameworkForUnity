//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestModifyler {
	using System;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using Modifyler;
	using Utility;
	using Debug;
	using TestBase;



	public class TestSMModifyler : SMUnitTest {
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCreate() => From( async () => {
			SMLog.Warning( "Start" );

			var m = new SMModifyler( this, typeof( SMTestModifyData ), false );
			m.Dispose();
			m = new SMModifyler( this, typeof( SMTestModifyData ), true );
			m.Dispose();

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDisposed() => From( async () => {
			SMLog.Warning( "Start" );

			var m = new SMModifyler( this, typeof( SMTestModifyData ) );
			m.Dispose();

			try {
				m._isLock = true;
			} catch ( Exception e )	{ SMLog.Error( e ); }
			try {
				m._isDebug = true;
			} catch ( Exception e ) { SMLog.Error( e ); }

			try {
				m.Register( new SMTestModifyData( "", SMModifyType.Normal ) );
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				m.Unregister( d => d is SMTestModifyData );
			} catch ( Exception e ) { SMLog.Error( e ); }

			try {
				await m.WaitRunning();
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				await m.RegisterAndWaitRunning( new SMTestModifyData( "", SMModifyType.Normal ) );
			} catch ( Exception e ) { SMLog.Error( e ); }

			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestIs() => From( async () => {
			SMLog.Warning( "Start" );
			var m = new SMModifyler( this, typeof( SMTestModifyData ), false );

			m._isLock = false;
			m._isLock = false;
			m._isLock = true;
			m._isLock = true;
			m._isLock = false;
			m._isLock = false;

			m._isDebug = false;
			m._isDebug = false;
			m._isDebug = true;
			m._isDebug = true;
			m._isDebug = false;
			m._isDebug = false;

			m._isLock = true;
			m.Register( new SMTestModifyData( "1", SMModifyType.Normal ) );
			m.Register( new SMTestModifyData( "2", SMModifyType.Normal ) );
			m._isLock = false;
			m.Register( new SMTestModifyData( "3", SMModifyType.Normal ) );
			m.Register( new SMTestModifyData( "4", SMModifyType.Normal ) );
			await m.WaitRunning();
			SMLog.Debug( "実行完了" );

			m._isDebug = true;
			m.Register( new SMTestModifyData( "1", SMModifyType.Normal ) );
			m.Register( new SMTestModifyData( "2", SMModifyType.Normal ) );
			await m.WaitRunning();
			m._isDebug = false;

			m.Dispose();
			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRegister() => From( async () => {
			SMLog.Warning( "Start" );
			var m = new SMModifyler( this, typeof( SMTestModifyData ) );
			m._isLock = true;
			m._isDebug = true;

			m.Register( new SMTestModifyData( $"{nameof( SMModifyType.Normal )}1",		SMModifyType.Normal ) );
			m.Register( new SMTestModifyData( $"{nameof( SMModifyType.Single )}1",		SMModifyType.Single ) );
			m.Register( new SMTestModifyData( $"{nameof( SMModifyType.Single )}2",		SMModifyType.Single ) );
			m.Register( new SMTestModifyData( $"{nameof( SMModifyType.Parallel )}1",	SMModifyType.Parallel ) );
			m.Register( new SMTestModifyData( $"{nameof( SMModifyType.Parallel )}2",	SMModifyType.Parallel ) );
			m.Register( new SMTestModifyData( $"{nameof( SMModifyType.Normal )}2",		SMModifyType.Normal ) );
			m.Register( new SMTestModifyData( $"{nameof( SMModifyType.Single )}3",		SMModifyType.Single ) );
			m.Register( new SMTestModifyData( $"{nameof( SMModifyType.Normal )}3",		SMModifyType.Normal ) );
			m.Register( new SMTestModifyData( $"{nameof( SMModifyType.Parallel )}3",	SMModifyType.Parallel ) );
			m.Register( new SMTestModifyData( $"{nameof( SMModifyType.First )}1",		SMModifyType.First ) );
			m.Register( new SMTestModifyData( $"{nameof( SMModifyType.First )}2",		SMModifyType.First ) );
			m.Register( new SMTestModifyData( $"{nameof( SMModifyType.Interrupt )}1",	SMModifyType.Interrupt ) );
			m.Register( new SMTestModifyData( $"{nameof( SMModifyType.Interrupt )}2",	SMModifyType.Interrupt ) );
			m.Unregister( d => d._type == SMModifyType.Normal );

			m._isLock = false;
			await m.WaitRunning();

			try {
				m.Register( new Event.Modifyler.RemoveSMEvent( "hoge" ) );
			} catch ( Exception e ) { SMLog.Error( e ); }

			m.Dispose();
			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRun() => From( async () => {
			SMLog.Warning( "Start" );
			var m = new SMModifyler( this, typeof( SMTestModifyData ) );

			m._isLock = true;
			m.Register( new SMTestModifyData( "1",	SMModifyType.Parallel ) );
			m.Register( new SMTestModifyData( "1",	SMModifyType.Normal ) );
			m.Register( new SMTestModifyData( "2",	SMModifyType.Parallel ) );
			m.Register( new SMTestModifyData( "3",	SMModifyType.Parallel ) );
			m.Register( new SMTestModifyData( "2",	SMModifyType.Normal ) );
			m.Register( new SMTestModifyData( "4",	SMModifyType.Parallel ) );
			m._isLock = false;
			await m.WaitRunning();
			SMLog.Debug( $"{nameof( m.WaitRunning )} : end" );

			m.Register( new SMTestModifyData( "3", SMModifyType.Normal ) );
			UTask.Void( async () => {
				await m.RegisterAndWaitRunning(
					new SMTestModifyData( "3.5", SMModifyType.Normal ) );
				SMLog.Debug( $"{nameof( m.RegisterAndWaitRunning )} : end" );
			} );
			m.Register( new SMTestModifyData( "4", SMModifyType.Normal ) );
			await m.WaitRunning();
			SMLog.Debug( $"{nameof( m.WaitRunning )} : end" );

			m.Register( new SMTestModifyData( "4.5", SMModifyType.Normal ) );
			UTask.Void( async () => {
				await m.RegisterAndWaitRunning(
					new SMTestDisposeModifyData( "5", SMModifyType.Normal ) );
				SMLog.Debug( $"{nameof( m.RegisterAndWaitRunning )} : end" );
			} );
			m.Register( new SMTestModifyData( "6", SMModifyType.Normal ) );
			try {
				await m.WaitRunning();
			} catch ( OperationCanceledException ) {}
			SMLog.Debug( $"{nameof( m.WaitRunning )} : end" );

			m = new SMModifyler( this, typeof( SMTestModifyData ) );
			m.Register( new SMTestModifyData( "4.5", SMModifyType.Parallel ) );
			UTask.Void( async () => {
				await m.RegisterAndWaitRunning(
					new SMTestDisposeModifyData( "5", SMModifyType.Parallel ) );
				SMLog.Debug( $"{nameof( m.RegisterAndWaitRunning )} : end" );
			} );
			m.Register( new SMTestModifyData( "6",	SMModifyType.Parallel ) );
			m.Register( new SMTestModifyData( "7",	SMModifyType.Normal ) );
			try {
				await m.WaitRunning();
			} catch ( OperationCanceledException ) {}
			SMLog.Debug( $"{nameof( m.WaitRunning )} : end" );

			m.Dispose();
			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestErrorRun1() => From( async () => {
			SMLog.Warning( "Start" );

			var m = new SMModifyler( this, typeof( SMTestModifyData ) );
			try {
				m.Register( new SMTestModifyData( "1",		SMModifyType.Normal ) );
				m.Register( new SMTestErrorModifyData( "2",	SMModifyType.Normal ) );
				m.Register( new SMTestModifyData( "3",		SMModifyType.Normal ) );
				await m.WaitRunning();
			} catch ( Exception e )	{ SMLog.Error( e ); }
			m.Dispose();

			SMLog.Warning( "End" );
		} );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestErrorRun2() => From( async () => {
			SMLog.Warning( "Start" );

			var m = new SMModifyler( this, typeof( SMTestModifyData ) );
			m.Register( new SMTestModifyData( "1",		SMModifyType.Parallel ) );
			m.Register( new SMTestErrorModifyData( "2",	SMModifyType.Parallel ) );
			m.Register( new SMTestModifyData( "3",		SMModifyType.Parallel ) );
			try {
				await m.WaitRunning();
			} catch ( Exception e ) { SMLog.Error( e ); }
			m.Dispose();

			SMLog.Warning( $"End" );
		} );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestErrorRun3() => From( async () => {
			SMLog.Warning( "Start" );

			var m = new SMModifyler( this, typeof( SMTestModifyData ) );
			m.Register( new SMTestModifyData( "4",			SMModifyType.Normal ) );
			m.Register( new SMTestDisposeModifyData( "5",	SMModifyType.Normal ) );
			m.Register( new SMTestModifyData( "6",			SMModifyType.Normal ) );
			try {
				await m.WaitRunning();
			} catch ( Exception e ) { SMLog.Error( e ); }
			m.Dispose();

			SMLog.Warning( $"End" );
		} );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestErrorRun4() => From( async () => {
			SMLog.Warning( "Start" );
			var m  = new SMModifyler( this, typeof( SMTestModifyData ) );
			m.Register( new SMTestModifyData( "4",			SMModifyType.Parallel ) );
			m.Register( new SMTestDisposeModifyData( "5",	SMModifyType.Parallel ) );
			m.Register( new SMTestModifyData( "6",			SMModifyType.Parallel ) );
			try {
				await m.WaitRunning();
			} catch ( Exception e ) { SMLog.Error( e ); }
			m.Dispose();

			SMLog.Warning( "End" );
		} );
	}
}