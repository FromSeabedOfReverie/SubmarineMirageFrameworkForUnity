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
	using UTask;
	using Debug;
	using Test;


	public class TestMultiAsyncEvent : Test {
		MultiAsyncEvent _events = new MultiAsyncEvent();


		protected override void Create() {
			Application.targetFrameRate = 10;
			_disposables.AddLast( _events );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestDispose() => From( async () => {
			var canceler = new UTaskCanceler();


			Log.Debug( "・即削除テスト" );
			var asyncEvent = new MultiAsyncEvent();
			asyncEvent.Dispose();


			Log.Debug( "・停止テスト" );
			asyncEvent = new MultiAsyncEvent();
			2.Times( i => asyncEvent.AddLast( async c => {
				Log.Debug( $"{nameof( MultiAsyncEvent )} : 待機開始 {i}" );
				await UTask.Delay( c, 1000 );
				Log.Debug( $"{nameof( MultiAsyncEvent )} : 待機終了 {i}" );
			} ) );
			UTask.Void( async () => {
				await UTask.Delay( canceler, 500 );
				Log.Debug( "停止" );
				canceler.Cancel();
			} );
			try {
				await asyncEvent.Run( canceler );
			} catch ( OperationCanceledException ) {}
			asyncEvent.Dispose();


			Log.Debug( "・実行中削除テスト" );
			asyncEvent = new MultiAsyncEvent();
			2.Times( i => asyncEvent.AddLast( async c => {
				Log.Debug( $"{nameof( MultiAsyncEvent )} : 待機開始 {i}" );
				await UTask.Delay( c, 1000 );
				Log.Debug( $"{nameof( MultiAsyncEvent )} : 待機終了 {i}" );
			} ) );
			UTask.Void( async () => {
				Log.Debug( "待機開始" );
				await UTask.Delay( canceler, 2000 );
				Log.Debug( "待機終了" );
			} );
			UTask.Void( async () => {
				await UTask.Delay( canceler, 500 );
				Log.Debug( "削除" );
				asyncEvent.Dispose();
			} );
			try {
				await asyncEvent.Run( canceler );
			} catch ( OperationCanceledException ) {}
			await UTask.Delay( canceler, 2000 );


			canceler.Dispose();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestModifyler() => From( async () => {
			TestMultiEventUtility.SetModifyler(
				_events,
				a => new Func<UTaskCanceler, UniTask>( async canceler => {
					a();
					Log.Debug( "start" );
					await UTask.Delay( canceler, 500 );
					Log.Debug( "end" );
				} )
			);

			Log.Debug( "・実行" );
			await _events.Run( _asyncCanceler );
			Log.Debug( _events );
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestChangeWhileRunning() => From( async () => {
			TestMultiEventUtility.SetChangeWhileRunning(
				_events,
				a => new Func<UTaskCanceler, UniTask>( async canceler => {
					a();
					Log.Debug( "start" );
					await UTask.Delay( canceler, 500 );
					Log.Debug( "end" );
				} )
			);

			Log.Debug( "・実行 1" );
			await _events.Run( _asyncCanceler );
			Log.Debug( _events );

			Log.Debug( "・実行 2" );
			await _events.Run( _asyncCanceler );
			Log.Debug( _events );
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( async () => {
			var canceler = new UTaskCanceler();
			_disposables.AddLast( canceler );

			_disposables.AddLast(
				TestMultiEventUtility.SetKey(
					_events,
					a => new Func<UTaskCanceler, UniTask>( async c => {
						a();
						Log.Debug( "start" );
						await UTask.Delay( c, 1000 );
						Log.Debug( "end" );
					} )
				),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Space ) ).Subscribe( _ => {
					Log.Warning( "key down Run" );
					UTask.Void( async () => {
						Log.Debug( _events );
						await _events.Run( canceler );
						Log.Debug( _events );
					} );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Return ) ).Subscribe( _ => {
					Log.Warning( "key down Cancel" );
					Log.Debug( _events );
					canceler.Cancel();
					Log.Debug( _events );
				} )
			);

			await UTask.Never( _asyncCanceler );
		} );
	}
}