//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestUtility {
	using System;
	using System.Diagnostics;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Base;
	using Utility;
	using Debug;



	public partial class TestSMAsyncCanceler : SMStandardBase {
		readonly SMAsyncCanceler _asyncCanceler = new SMAsyncCanceler();
		readonly Stopwatch _stopwatch = new Stopwatch();
		int _index	{ get; set; }



		[OneTimeSetUp]
		protected void Awake() {
			SMLog.s_isEnable = true;
			_disposables.AddLast( () => {
				_asyncCanceler.Dispose();
			} );
		}

		[OneTimeTearDown]
		protected void OnDestroy() => Dispose();



		IEnumerator From( Func<UniTask> task ) => UTask.ToCoroutine( async () => {
			try {
				await task.Invoke();
			} catch ( OperationCanceledException ) {
			} catch ( Exception e ) {
				SMLog.Error( e );
				throw;
			}
		} );



		void StartMeasure() => _stopwatch.Restart();

		float StopMeasure() {
			_stopwatch.Stop();
			return _stopwatch.ElapsedMilliseconds / 1000f;
		}



		async UniTask TestCancel( SMAsyncCanceler canceler, SMAsyncCanceler runCanceler, string name ) {
			for ( var i = 0; i < 2; i++ ) {
				UTask.Void( async () => {
					await UTask.Delay( _asyncCanceler, 500 );
					SMLog.Debug( $"停止{_index}" );
					canceler.Cancel();
				} );
				try {
					SMLog.Debug( $"{name}トークン、待機開始{i}" );
					StartMeasure();
					await UTask.Delay( runCanceler, 2000 );
					SMLog.Debug( $"{name}トークン、待機終了{i} : {StopMeasure()}" );
				} catch ( OperationCanceledException ) {}
				_index++;
			}
		}

		async UniTask TestDelete2( SMAsyncCanceler delete1, SMAsyncCanceler delete2, string name1, string name2
		) {
			delete1.Dispose();
			SMLog.Debug( $"{name1}のみ削除 : {name2} : {delete2}" );
			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 500 );
				SMLog.Debug( "停止" );
				delete2.Cancel();
			} );
			try {
				SMLog.Debug( "待機開始" );
				await UTask.Delay( delete2, 2000 );
				SMLog.Debug( "待機終了" );
			} catch ( OperationCanceledException ) {}
			delete2.Dispose();
		}



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCreate() => From( async () => {
			var canceler = new SMAsyncCanceler();
			SMLog.Debug( $"Create :\n{canceler}" );
			canceler.Dispose();
			SMLog.Debug( $"End :\n{canceler}" );

			await UTask.DontWait();
		} );

		
		
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestSingle() => From( async () => {
			var canceler = new SMAsyncCanceler();
			canceler._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( "停止イベント" ) );
			SMLog.Debug( $"Create :\n{canceler}" );

			SMLog.Debug( "・停止テスト" );
			for ( var i = 0; i < 2; i++ ) {
				UTask.Void( async () => {
					await UTask.Delay( _asyncCanceler, 500 );
					SMLog.Debug( "停止" );
					canceler.Cancel();
				} );
				try {
					StartMeasure();
					SMLog.Debug( $"待機開始" );
					await UTask.Delay( canceler, 1000 );
					SMLog.Debug( "待機終了" );
				} catch ( OperationCanceledException ) {}
				SMLog.Debug( StopMeasure() );
				SMLog.Debug( $"End :\n{canceler}" );
			}

			SMLog.Debug( "・停止テスト（再作成なし）" );
			for ( var i = 0; i < 2; i++ ) {
				UTask.Void( async () => {
					await UTask.Delay( _asyncCanceler, 500 );
					SMLog.Debug( "停止" );
					canceler.Cancel( false );
				} );
				try {
					StartMeasure();
					SMLog.Debug( $"待機開始" );
					await UTask.Delay( canceler, 1000 );
					SMLog.Debug( "待機終了" );
				} catch ( OperationCanceledException ) {}
				SMLog.Debug( StopMeasure() );
				SMLog.Debug( $"End :\n{canceler}" );
			}
			for ( var i = 0; i < 2; i++ ) {
				canceler.Cancel( false );
				SMLog.Debug( $"Cancel( false ) :\n{canceler}" );
			}
			await UTask.Delay( _asyncCanceler, 500 );

			SMLog.Debug( "・再作成テスト" );
			canceler.Recreate();
			SMLog.Debug( $"Recreate :\n{canceler}" );
			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 500 );
				SMLog.Debug( "停止" );
				canceler.Cancel( false );
			} );
			try {
				StartMeasure();
				SMLog.Debug( $"待機開始" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( "待機終了" );
			} catch ( OperationCanceledException ) {}
			SMLog.Debug( StopMeasure() );
			SMLog.Debug( $"End :\n{canceler}" );
			for ( var i = 0; i < 2; i++ ) {
				canceler.Recreate();
				SMLog.Debug( $"Recreate :\n{canceler}" );
			}

			SMLog.Debug( "・削除テスト" );
			canceler.Dispose();
			canceler = new SMAsyncCanceler();
			canceler._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( "停止イベント" ) );
			canceler.Cancel( false );
			canceler.Dispose();
			SMLog.Debug( $"End :\n{canceler}" );
		} );


		
// TODO : テストここから
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCreateLink() => From( async () => {
			var c1 = new SMAsyncCanceler();
			var c2 = c1.CreateLinkCanceler();
			var c3 = c1.CreateLinkCanceler();
			c1._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( $"{nameof( c1 )}停止" ) );
			c2._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( $"{nameof( c2 )}停止" ) );
			c3._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( $"{nameof( c3 )}停止" ) );
			SMLog.Debug( string.Join( ",\n", c2.GetBrothers() ) );
			c3.Dispose();
			SMLog.Debug( string.Join( ",\n", c1.GetBrothers() ) );
			c2.Dispose();
			SMLog.Debug( string.Join( ",\n", c1.GetBrothers() ) );
			c1.Dispose();

			c1 = new SMAsyncCanceler();
			c2 = new SMAsyncCanceler();
			c1.Link( c2 );
			c3 = new SMAsyncCanceler();
			c1.Link( c3 );
			c3.Unlink();
			c2.Link( c3 );
			c1._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( $"{nameof( c1 )}停止" ) );
			c2._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( $"{nameof( c2 )}停止" ) );
			c3._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( $"{nameof( c3 )}停止" ) );
			SMLog.Debug( string.Join( ",\n", c2.GetBrothers() ) );
			c1.Dispose();
			c2.Dispose();
			c3.Dispose();
			SMLog.Debug( string.Join( ",\n", c3.GetBrothers() ) );

			SMLog.Debug( "End" );
			await UTask.DontWait();



			/*
			SMLog.Debug( "・親停止テスト" );
			_index = 0;
			await TestCancel( parent, parent, "親" );
			await TestCancel( parent, child, "子" );

			SMLog.Debug( "・子停止テスト" );
			_index = 0;
			await TestCancel( child, parent, "親" );
			await TestCancel( child, child, "子" );

			SMLog.Debug( "・親から削除テスト" );
			await TestDelete2( parent, child, "親", "子" );
			parent = null;
			child = null;

			SMLog.Debug( "・子から削除テスト" );
			( parent, child ) = createCanceler();
			await TestDelete2( child, parent, "子", "親" );
			parent = null;
			child = null;
			*/
		} );

		
		
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDisposed() => From( async () => {
			SMLog.Debug( $"{nameof( TestDisposed )}" );

			var c = new SMAsyncCanceler();
			c.Dispose();
			SMLog.Debug( string.Join( "\n",
				$"{nameof( c._isDispose )} : {c._isDispose}",
				$"{nameof( c._isCancel )} : {c._isCancel}"
			) );

			await UTask.DontWait();
		} );
	}
}