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
	using Cysharp.Threading.Tasks;
	using Modifyler;
	using Utility;
	using Debug;
	using TestBase;



	public class TestSMModifyler : SMUnitTest {
		protected override void Create() {
		}



		async UniTask RegisterTestData( SMModifyler modifyler, string name, SMModifyType type ) {
			name = $"{nameof( RegisterTestData )} {name}";
			await modifyler.Register(
				name,
				type,
				async () => {
					SMLog.Warning( $"実行 : start\n{name}" );
					await UTask.Delay( modifyler._asyncCanceler, 1000 );
					SMLog.Warning( $"実行 : end\n{name}" );
				}
			);
		}

		async UniTask RegisterCancelData( SMModifyler modifyler, string name, SMModifyType type ) {
			name = $"{nameof( RegisterCancelData )} {name}";
			await modifyler.Register(
				name,
				type,
				async () => {
					using ( var c = new SMAsyncCanceler() ) {
						UTask.Void( async () => {
							await UTask.Delay( c, 500 );
							SMLog.Warning( $"Run : 停止\n{name}" );
							c.Cancel();
						} );
						await UTask.Delay( c, 1000 );
					}
				}
			);
		}

		async UniTask RegisterErrorData( SMModifyler modifyler, string name, SMModifyType type ) {
			name = $"{nameof( RegisterErrorData )} {name}";
			await modifyler.Register(
				name,
				type,
				async () => {
					await UTask.Delay( modifyler._asyncCanceler, 500 );
					throw new Exception( $"試験失敗 : \n{name}" );
				}
			);
		}

		async UniTask RegisterDisposeData( SMModifyler modifyler, string name, SMModifyType type ) {
			name = $"{nameof( RegisterDisposeData )} {name}";
			await modifyler.Register(
				name,
				type,
				async () => {
					await UTask.Delay( modifyler._asyncCanceler, 500 );
					SMLog.Warning( $"Run : 解放\n{name}" );
					modifyler.Dispose();
				}
			);
		}



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCreate() => From( async () => {
			SMLog.Warning( "Start" );

			var m = new SMModifyler( "1", false );
			m.Dispose();
			m = new SMModifyler( "2", true );
			m.Dispose();

			SMLog.Warning( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDispose() => From( async () => {
			using ( var c = new SMAsyncCanceler() ) {
				await UTask.DelayFrame( c, 2 );
			}
			SMLog.Warning( "Start" );

			var m = new SMModifyler( "1" );
			m.Dispose();

			try {
				m._isLock = true;
			} catch ( Exception e )	{ SMLog.Error( e ); }
			try {
				m._isDebug = true;
			} catch ( Exception e ) { SMLog.Error( e ); }

			try {
				RegisterTestData( m, "", SMModifyType.Normal ).Forget();
			} catch ( Exception e ) { SMLog.Error( e ); }
			try {
				await m.WaitRunning();
			} catch ( Exception e ) { SMLog.Error( e ); }

			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestIs() => From( async () => {
			SMLog.Warning( "Start" );
			var m = new SMModifyler( "1", false );

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
			RegisterTestData( m, "1", SMModifyType.Normal ).Forget();
			RegisterTestData( m, "2", SMModifyType.Normal ).Forget();
			m._isLock = false;
			RegisterTestData( m, "3", SMModifyType.Normal ).Forget();
			RegisterTestData( m, "4", SMModifyType.Normal ).Forget();
			await m.WaitRunning();
			SMLog.Debug( "実行完了" );

			m._isDebug = true;
			RegisterTestData( m, "5", SMModifyType.Normal ).Forget();
			RegisterTestData( m, "6", SMModifyType.Normal ).Forget();
			await m.WaitRunning();
			m._isDebug = false;

			m.Dispose();
			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRegister() => From( async () => {
			SMLog.Warning( "Start" );
			var m = new SMModifyler( "1" );
			m._isLock = true;
			m._isDebug = true;

			RegisterTestData( m, "1", SMModifyType.Normal ).Forget();
			RegisterTestData( m, "2", SMModifyType.Single ).Forget();
			RegisterTestData( m, "3", SMModifyType.Single ).Forget();
			RegisterTestData( m, "4", SMModifyType.Parallel ).Forget();
			RegisterTestData( m, "5", SMModifyType.Parallel ).Forget();
			RegisterTestData( m, "6", SMModifyType.Normal ).Forget();
			RegisterTestData( m, "7", SMModifyType.Single ).Forget();
			RegisterTestData( m, "8", SMModifyType.Normal ).Forget();
			RegisterTestData( m, "9", SMModifyType.Parallel ).Forget();
			RegisterTestData( m, "10", SMModifyType.Priority ).Forget();
			RegisterTestData( m, "11", SMModifyType.Priority ).Forget();
			RegisterTestData( m, "12", SMModifyType.Interrupt ).Forget();
			RegisterTestData( m, "13", SMModifyType.Interrupt ).Forget();

			m._isLock = false;
			await m.WaitRunning();

			m.Dispose();
			SMLog.Warning( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRun() => From( async () => {
			SMLog.Warning( "Start" );
			var m = new SMModifyler( "1" );

			m._isLock = true;
			RegisterTestData( m, "1", SMModifyType.Parallel ).Forget();
			RegisterTestData( m, "2", SMModifyType.Normal ).Forget();
			RegisterTestData( m, "3", SMModifyType.Parallel ).Forget();
			RegisterTestData( m, "4", SMModifyType.Parallel ).Forget();
			RegisterTestData( m, "5", SMModifyType.Normal ).Forget();
			RegisterTestData( m, "6", SMModifyType.Parallel ).Forget();
			m._isLock = false;
			await m.WaitRunning();
			SMLog.Debug( $"{nameof( m.WaitRunning )} : end" );


			RegisterTestData( m, "7", SMModifyType.Normal ).Forget();
			UTask.Void( async () => {
				await RegisterTestData( m, "8", SMModifyType.Normal );
				SMLog.Debug( $"RegisterWait : end" );
			} );
			RegisterTestData( m, "9", SMModifyType.Normal ).Forget();
			await m.WaitRunning();
			SMLog.Debug( $"{nameof( m.WaitRunning )} : end" );


			RegisterTestData( m, "10", SMModifyType.Normal ).Forget();
			UTask.Void( async () => {
				await RegisterCancelData( m, "11", SMModifyType.Normal );
				SMLog.Debug( $"RegisterWait : end\n{m}" );
			} );
			RegisterTestData( m, "12", SMModifyType.Normal ).Forget();
			try {
				await m.WaitRunning();
			} catch ( OperationCanceledException ) { }
			SMLog.Debug( $"{nameof( m.WaitRunning )} : end" );

			RegisterTestData( m, "13", SMModifyType.Parallel ).Forget();
			UTask.Void( async () => {
				await RegisterCancelData( m, "14", SMModifyType.Parallel );
				SMLog.Debug( $"RegisterWait : end\n{m}" );
			} );
			RegisterTestData( m, "15", SMModifyType.Parallel ).Forget();
			RegisterTestData( m, "16", SMModifyType.Normal ).Forget();
			try {
				await m.WaitRunning();
			} catch ( OperationCanceledException ) { }
			SMLog.Debug( $"{nameof( m.WaitRunning )} : end" );


			RegisterTestData( m, "17", SMModifyType.Normal ).Forget();
			UTask.Void( async () => {
				await RegisterDisposeData( m, "18", SMModifyType.Normal );
				SMLog.Debug( $"RegisterWait : end" );
			} );
			RegisterTestData( m, "19", SMModifyType.Normal ).Forget();
			try {
				await m.WaitRunning();
			} catch ( OperationCanceledException ) {}
			SMLog.Debug( $"{nameof( m.WaitRunning )} : end" );

			m = new SMModifyler( "2" );
			RegisterTestData( m, "20", SMModifyType.Parallel ).Forget();
			UTask.Void( async () => {
				await RegisterDisposeData( m, "21", SMModifyType.Parallel );
				SMLog.Debug( $"RegisterWait : end" );
			} );
			RegisterTestData( m, "22", SMModifyType.Parallel ).Forget();
			RegisterTestData( m, "23", SMModifyType.Normal ).Forget();
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

			var m = new SMModifyler( "1" );
			RegisterTestData(	m, "1", SMModifyType.Normal ).Forget();
			RegisterErrorData(	m, "2", SMModifyType.Normal ).Forget();
			RegisterTestData(	m, "3", SMModifyType.Normal ).Forget();
			try {
				await m.WaitRunning();
			} catch ( Exception e )	{ SMLog.Error( e ); }
			m.Dispose();

			SMLog.Warning( "End" );
		} );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestErrorRun2() => From( async () => {
			SMLog.Warning( "Start" );

			var m = new SMModifyler( "1" );
			RegisterTestData(	m, "1", SMModifyType.Parallel ).Forget();
			RegisterErrorData(	m, "2", SMModifyType.Parallel ).Forget();
			RegisterTestData(	m, "3", SMModifyType.Parallel ).Forget();
			try {
				await m.WaitRunning();
			} catch ( Exception e ) { SMLog.Error( e ); }
			m.Dispose();

			SMLog.Warning( $"End" );
		} );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestErrorRun3() => From( async () => {
			SMLog.Warning( "Start" );

			var m = new SMModifyler( "1" );
			RegisterTestData(		m, "1", SMModifyType.Normal ).Forget();
			RegisterDisposeData(	m, "2", SMModifyType.Normal ).Forget();
			RegisterTestData(		m, "3", SMModifyType.Normal ).Forget();
			try {
				await m.WaitRunning();
			} catch ( Exception e ) { SMLog.Error( e ); }
			m.Dispose();

			SMLog.Warning( $"End" );
		} );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestErrorRun4() => From( async () => {
			SMLog.Warning( "Start" );

			var m  = new SMModifyler( "1" );
			RegisterTestData(		m, "1", SMModifyType.Parallel ).Forget();
			RegisterDisposeData(	m, "2", SMModifyType.Parallel ).Forget();
			RegisterTestData(		m, "3", SMModifyType.Parallel ).Forget();
			try {
				await m.WaitRunning();
			} catch ( Exception e ) { SMLog.Error( e ); }
			m.Dispose();

			SMLog.Warning( "End" );
		} );
	}
}