//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestAsync {
	using System;
	using System.Threading;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Debug;
	using Test;


	// 非同期停止の試験を行う


	public class TestAsynchronousCancel : Test {
		int _id;
		TestLoader _loader = new TestLoader();
		CancellationTokenSource _cancel = new CancellationTokenSource();


		protected override void Create() {
			Application.targetFrameRate = 30;
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() {
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Return ) ).Subscribe( _ => {
					Play().Forget( e => {
						if ( !( e is OperationCanceledException ) )	{ throw e; }
						_loader.ShowCancelLog();
					} );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Stop().Forget( e => {
						if ( !( e is OperationCanceledException ) )	{ throw e; }
						_loader.ShowCancelLog();
					} );
				} )
			);

			while ( true )	{ yield return null; }
		}


		async UniTask Play() {
			_cancel.Cancel();
			_cancel = new CancellationTokenSource();
			_id = _cancel.GetHashCode();

			Log.Debug( $"Play Start : {_id}" );
			await _loader.Load( _cancel );
			await UniTask.Delay( TimeSpan.FromSeconds( 10 ), false, PlayerLoopTiming.Update, _cancel.Token );
			Log.Debug( $"Play End : {_id}" );
		}


		async UniTask Stop() {
			_cancel.Cancel();
			_cancel = new CancellationTokenSource();
			_id = _cancel.GetHashCode();

			Log.Debug( $"Stop Start : {_id}" );
			await UniTask.Delay( TimeSpan.FromSeconds( 10 ), false, PlayerLoopTiming.Update, _cancel.Token );
			_loader.Unload();
			Log.Debug( $"Stop End : {_id}" );
		}



		public class TestLoader {
			int _id;
			public string _data;
			CancellationTokenSource _cancel = new CancellationTokenSource();


			public async UniTask Load( CancellationTokenSource cancel = null ) {
				_cancel.Cancel();
				_cancel = cancel != null ? cancel : new CancellationTokenSource();
				_id = _cancel.GetHashCode();
				Log.Debug( $"Load Start : {_id}" );

				try {
					if ( !_cancel.IsCancellationRequested ) {
						await LoadSub();
					}
				} catch ( OperationCanceledException ) {
					Log.Debug( "例外を捻り潰した" );
				}

				if ( _data.IsNullOrEmpty() ) {
					Log.Debug( $"読み込み失敗 : Cancel : {cancel.IsCancellationRequested}" );
					throw new OperationCanceledException( _cancel.Token );
				}
				Log.Debug( $"Load End : {_id}" );
			}


			async UniTask LoadSub() {
				Log.Debug( $"LoadSub Start : {_id}" );
				await UniTask.Delay( TimeSpan.FromSeconds( 10 ), false, PlayerLoopTiming.Update, _cancel.Token );
	//			throw new NullReferenceException();
				_data = "Data";
				Log.Debug( $"LoadSub End : {_id}" );
			}


			public void Unload() {
				Log.Debug( $"Unload : {_id}" );
				_cancel.Cancel();
				_data = null;
			}

			public void ShowCancelLog() {
				// そのフレームのDelay後に発行される
				Log.Debug( $"Cancel : {_id}" );
			}
		}
	}
}