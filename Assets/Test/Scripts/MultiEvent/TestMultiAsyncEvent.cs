//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestMultiEvent {
	using System;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using MultiEvent;
	using Task;
	using Utility;
	using Debug;
	using Test;


	public class TestMultiAsyncEvent : SMStandardTest {
		SMMultiAsyncEvent _events = new SMMultiAsyncEvent();


		protected override void Create() {
			Application.targetFrameRate = 10;
			_disposables.AddLast( _events );
		}


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDispose() => From( async () => {
			SMLog.Debug( $"{nameof( TestDispose )}" );
			var canceler = new SMTaskCanceler();
			


			SMLog.Debug( "・即削除テスト" );
			var asyncEvent = new SMMultiAsyncEvent();
			SMLog.Debug( asyncEvent );
			asyncEvent.Dispose();
			SMLog.Debug( asyncEvent );


			SMLog.Debug( "・停止テスト" );
			asyncEvent = new SMMultiAsyncEvent();
			2.Times( i => asyncEvent.AddLast( async c => {
				SMLog.Debug( $"{nameof( SMMultiAsyncEvent )} : 待機開始 {i}" );
				await UTask.Delay( c, 1000 );
				SMLog.Debug( $"{nameof( SMMultiAsyncEvent )} : 待機終了 {i}" );
			} ) );
			UTask.Void( async () => {
				await UTask.Delay( canceler, 500 );
				SMLog.Debug( "停止" );
				canceler.Cancel();
			} );
			try {
				await asyncEvent.Run( canceler );
			} catch ( OperationCanceledException ) {}
			asyncEvent.Dispose();


			SMLog.Debug( "・実行中削除テスト" );
			asyncEvent = new SMMultiAsyncEvent();
			2.Times( i => asyncEvent.AddLast( async c => {
				SMLog.Debug( $"{nameof( SMMultiAsyncEvent )} : 待機開始 {i}" );
				await UTask.Delay( c, 1000 );
				SMLog.Debug( $"{nameof( SMMultiAsyncEvent )} : 待機終了 {i}" );
			} ) );
			UTask.Void( async () => {
				SMLog.Debug( "待機開始" );
				await UTask.Delay( canceler, 2000 );
				SMLog.Debug( "待機終了" );
			} );
			UTask.Void( async () => {
				await UTask.Delay( canceler, 500 );
				SMLog.Debug( "削除" );
				asyncEvent.Dispose();
			} );
			try {
				await asyncEvent.Run( canceler );
			} catch ( OperationCanceledException ) {}
			await UTask.Delay( canceler, 2000 );


			canceler.Dispose();
		} );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestModifyler() => From( async () => {
			TestMultiEventUtility.SetModifyler(
				_events,
				a => new Func<SMTaskCanceler, UniTask>( async canceler => {
					a();
					SMLog.Debug( "start" );
					await UTask.Delay( canceler, 500 );
					SMLog.Debug( "end" );
				} )
			);

			SMLog.Debug( "・実行" );
			await _events.Run( _asyncCanceler );
			SMLog.Debug( _events );
		} );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestChangeWhileRunning() => From( async () => {
			TestMultiEventUtility.SetChangeWhileRunning(
				_events,
				a => new Func<SMTaskCanceler, UniTask>( async canceler => {
					a();
					SMLog.Debug( "start" );
					await UTask.Delay( canceler, 500 );
					SMLog.Debug( "end" );
				} )
			);

			SMLog.Debug( "・実行 1" );
			await _events.Run( _asyncCanceler );
			SMLog.Debug( _events );

			SMLog.Debug( "・実行 2" );
			await _events.Run( _asyncCanceler );
			SMLog.Debug( _events );
		} );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( async () => {
			var canceler = new SMTaskCanceler();
			_disposables.AddLast( canceler );

			_disposables.AddLast(
				TestMultiEventUtility.SetKey(
					_events,
					a => new Func<SMTaskCanceler, UniTask>( async c => {
						a();
						SMLog.Debug( "start" );
						await UTask.Delay( c, 1000 );
						SMLog.Debug( "end" );
					} )
				),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Space ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {nameof( _events.Run )}" );
					UTask.Void( async () => {
						SMLog.Debug( _events );
						await _events.Run( canceler );
						SMLog.Debug( _events );
					} );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Return ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {nameof( canceler.Cancel )}" );
					SMLog.Debug( _events );
					canceler.Cancel();
					SMLog.Debug( _events );
				} )
			);

			await UTask.Never( _asyncCanceler );
		} );
	}
}