//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestUTask {
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


	public partial class TestUniTask : SMStandardTest {
		int _id;
		TestLoader _loader = new TestLoader();
		CancellationTokenSource _canceler = new CancellationTokenSource();



		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestCancel() => From( TestCancelSub() );
		IEnumerator TestCancelSub() {
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
			_canceler.Cancel();
			_canceler = new CancellationTokenSource();
			_id = _canceler.GetHashCode();

			Log.Debug( $"{nameof( Play )} Start : {_id}" );
			await _loader.Load( _canceler );
			await UniTask.Delay( 10000, cancellationToken: _canceler.Token );
			Log.Debug( $"{nameof( Play )} End : {_id}" );
		}


		async UniTask Stop() {
			_canceler.Cancel();
			_canceler = new CancellationTokenSource();
			_id = _canceler.GetHashCode();

			Log.Debug( $"{nameof( Stop )} Start : {_id}" );
			await UniTask.Delay( 10000, cancellationToken: _canceler.Token );
			_loader.Unload();
			Log.Debug( $"{nameof( Stop )} End : {_id}" );
		}



		public class TestLoader {
			int _id;
			public string _data;
			CancellationTokenSource _canceler = new CancellationTokenSource();


			public async UniTask Load( CancellationTokenSource canceler = null ) {
				_canceler.Cancel();
				_canceler = canceler != null ? canceler : new CancellationTokenSource();
				_id = _canceler.GetHashCode();
				Log.Debug( $"{nameof( Load )} Start : {_id}" );

				try {
					if ( !_canceler.IsCancellationRequested ) {
						await LoadSub();
					}
				} catch ( OperationCanceledException ) {
					Log.Debug( "例外を捻り潰した" );
				}

				if ( _data.IsNullOrEmpty() ) {
					Log.Debug( $"読み込み失敗 : Cancel : {canceler.IsCancellationRequested}" );
					throw new OperationCanceledException( _canceler.Token );
				}
				Log.Debug( $"{nameof( Load )} End : {_id}" );
			}


			async UniTask LoadSub() {
				Log.Debug( $"{nameof( LoadSub )} Start : {_id}" );
				await UniTask.Delay( 10000, cancellationToken: _canceler.Token );
	//			throw new NullReferenceException();
				_data = "Data";
				Log.Debug( $"{nameof( LoadSub )} End : {_id}" );
			}


			public void Unload() {
				Log.Debug( $"{nameof( Unload )} : {_id}" );
				_canceler.Cancel();
				_data = null;
			}

			public void ShowCancelLog() {
				// そのフレームのDelay後に発行される
				Log.Debug( $"Cancel : {_id}" );
			}
		}
	}
}