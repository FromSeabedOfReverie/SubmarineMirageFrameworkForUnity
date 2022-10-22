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
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;



	public class TestSMAsyncCanceler : SMUnitTest {
		readonly SMAsyncCanceler _asyncCanceler = new SMAsyncCanceler();



		protected override void Create() {
			_disposables.AddFirst( () => {
				_asyncCanceler.Dispose();
			} );
		}



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCreate() => From( async () => {
			var c = new SMAsyncCanceler();
			c.Dispose();

			SMLog.Debug( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDisposed() => From( async () => {
			var c1 = new SMAsyncCanceler();
			var c2 = new SMAsyncCanceler();
			c1.Dispose();

			try {
				c1._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( "cancel" ) );
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			try {
				c1.LinkLast( c2 );
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			try {
				c1.Unlink();
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			try {
				var c3 = c1.CreateChild();
				SMLog.Debug( c3 );
			} catch ( Exception e ) {
				SMLog.Error( e );
			}

			c1.Cancel();
			c1.Cancel( false );
			c1.Recreate();

			SMLog.Debug( string.Join( ",\n", c1.GetAlls() ) );

			try {
				_stopwatch.Start();
				SMLog.Debug( $"待機開始" );
				await UTask.Delay( c1, 1000 );
				SMLog.Debug( "待機終了" );
			} catch ( OperationCanceledException ) {}
			SMLog.Debug( _stopwatch.Stop() );

			SMLog.Debug( "End" );
			c2.Dispose();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestSingle() => From( async () => {
			var c = new SMAsyncCanceler();
			c._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( "cancel" ) );

			SMLog.Debug( "・停止テスト" );
			for ( var i = 0; i < 2; i++ ) {
				UTask.Void( async () => {
					await UTask.Delay( _asyncCanceler, 500 );
					SMLog.Debug( "停止" );
					c.Cancel();
				} );
				try {
					_stopwatch.Start();
					SMLog.Debug( $"待機開始" );
					await UTask.Delay( c, 1000 );
					SMLog.Debug( "待機終了" );
				} catch ( OperationCanceledException ) {}
				SMLog.Debug( _stopwatch.Stop() );
			}

			SMLog.Debug( "・停止テスト（再作成なし）" );
			for ( var i = 0; i < 2; i++ ) {
				UTask.Void( async () => {
					await UTask.Delay( _asyncCanceler, 500 );
					SMLog.Debug( "停止" );
					c.Cancel( false );
				} );
				try {
					_stopwatch.Start();
					SMLog.Debug( $"待機開始" );
					await UTask.Delay( c, 1000 );
					SMLog.Debug( "待機終了" );
				} catch ( OperationCanceledException ) {}
				SMLog.Debug( _stopwatch.Stop() );
			}
			for ( var i = 0; i < 2; i++ ) {
				c.Cancel( false );
			}
			await UTask.Delay( _asyncCanceler, 500 );

			SMLog.Debug( "・再作成テスト" );
			c.Recreate();
			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 500 );
				SMLog.Debug( "停止" );
				c.Cancel( false );
			} );
			try {
				_stopwatch.Start();
				SMLog.Debug( $"待機開始" );
				await UTask.Delay( c, 1000 );
				SMLog.Debug( "待機終了" );
			} catch ( OperationCanceledException ) {}
			SMLog.Debug( _stopwatch.Stop() );
			for ( var i = 0; i < 2; i++ ) {
				c.Recreate();
			}

			SMLog.Debug( "・削除テスト" );
			c.Dispose();
			c = new SMAsyncCanceler();
			c._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( "cancel" ) );
			c.Cancel( false );
			c.Dispose();

			SMLog.Debug( "End" );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCreateLink() => From( async () => {
			var c1 = new SMAsyncCanceler();
			var c2 = c1.CreateChild();
			var c3 = c1.CreateChild();
			c1._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( $"{nameof( c1 )}停止" ) );
			c2._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( $"{nameof( c2 )}停止" ) );
			c3._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( $"{nameof( c3 )}停止" ) );
			SMLog.Debug( string.Join( ",\n", c2.GetAlls() ) );
			c2.Dispose();
			SMLog.Debug( string.Join( ",\n", c1.GetAlls() ) );
			c3.Dispose();
			SMLog.Debug( string.Join( ",\n", c1.GetAlls() ) );
			c1.Dispose();

			c1 = new SMAsyncCanceler();
			c2 = new SMAsyncCanceler();
			c1.LinkLast( c2 );
			SMLog.Debug( string.Join( ",\n", c1.GetAlls() ) );
			c3 = new SMAsyncCanceler();
			c1.LinkLast( c3 );
			SMLog.Debug( string.Join( ",\n", c1.GetAlls() ) );
			c3.Unlink();
			SMLog.Debug( string.Join( ",\n", c1.GetAlls() ) );
			c2.LinkLast( c3 );
			SMLog.Debug( string.Join( ",\n", c2.GetAlls() ) );
			c1._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( $"{nameof( c1 )}停止" ) );
			c2._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( $"{nameof( c2 )}停止" ) );
			c3._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( $"{nameof( c3 )}停止" ) );
			c1.Dispose();
			SMLog.Debug( string.Join( ",\n", c3.GetAlls() ) );
			c2.Dispose();
			SMLog.Debug( string.Join( ",\n", c3.GetAlls() ) );
			c3.Dispose();

			SMLog.Debug( "End" );
			await UTask.DontWait();
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestMulti() => From( async () => {
			var c1 = new SMAsyncCanceler();
			var c2 = c1.CreateChild();
			var c3 = c2.CreateChild();
			c1._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( $"{nameof( c1 )}停止" ) );
			c2._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( $"{nameof( c2 )}停止" ) );
			c3._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( $"{nameof( c3 )}停止" ) );

			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 500 );
				SMLog.Debug( "停止" );
				c1.Cancel();
			} );
			try {
				_stopwatch.Start();
				SMLog.Debug( $"待機開始" );
				await UTask.Delay( c3, 1000 );
				SMLog.Debug( "待機終了" );
			} catch ( OperationCanceledException ) { }
			SMLog.Debug( _stopwatch.Stop() );

			c1.Cancel();
			c2.Cancel();
			c3.Cancel();

			c2.Cancel( false );
			c1.Cancel();
			c1.Cancel( false );
			c1.Cancel( false );
			c3.Cancel( false );
			c1.Cancel();
			c1.Cancel( false );

			c2.Recreate();
			c2.Cancel( false );
			c2.Cancel( false );

			SMLog.Debug( string.Join( ",\n", c3.GetAlls() ) );
			c1.Dispose();
			SMLog.Debug( string.Join( ",\n", c3.GetAlls() ) );
			c2.Dispose();
			SMLog.Debug( string.Join( ",\n", c3.GetAlls() ) );
			c3.Dispose();

			SMLog.Debug( "End" );
		} );
	}
}