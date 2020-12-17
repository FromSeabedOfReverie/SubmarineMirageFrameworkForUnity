//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestCSharp {
	using System;
	using System.Threading;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Base;
	using Debug;
	using TestBase;


	// 非同期停止の試験を行う


	public partial class TestSMUniTask : SMStandardTest {
		readonly TestSMLoader _loader = new TestSMLoader();
		CancellationTokenSource _canceler	{ get; set; } = new CancellationTokenSource();
		int _hashCode	{ get; set; }



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
			_hashCode = _canceler.GetHashCode();

			SMLog.Debug( $"{nameof( Play )} Start : {_hashCode}" );
			await _loader.Load( _canceler );
			await UniTask.Delay( 10000, cancellationToken: _canceler.Token );
			SMLog.Debug( $"{nameof( Play )} End : {_hashCode}" );
		}


		async UniTask Stop() {
			_canceler.Cancel();
			_canceler = new CancellationTokenSource();
			_hashCode = _canceler.GetHashCode();

			SMLog.Debug( $"{nameof( Stop )} Start : {_hashCode}" );
			await UniTask.Delay( 10000, cancellationToken: _canceler.Token );
			_loader.Unload();
			SMLog.Debug( $"{nameof( Stop )} End : {_hashCode}" );
		}



		public class TestSMLoader : SMLightBase {
			int _hashCode	{ get; set; }
			public string _data	{ get; set; }
			CancellationTokenSource _canceler	{ get; set; } = new CancellationTokenSource();


			public override void Dispose()	{}


			public async UniTask Load( CancellationTokenSource canceler = null ) {
				_canceler.Cancel();
				_canceler = canceler ?? new CancellationTokenSource();
				_hashCode = _canceler.GetHashCode();
				SMLog.Debug( $"{nameof( Load )} Start : {_hashCode}" );

				try {
					if ( !_canceler.IsCancellationRequested ) {
						await LoadSub();
					}
				} catch ( OperationCanceledException ) {
					SMLog.Debug( "例外を捻り潰した" );
				}

				if ( _data.IsNullOrEmpty() ) {
					SMLog.Debug( $"読み込み失敗 : Cancel : {canceler.IsCancellationRequested}" );
					throw new OperationCanceledException( _canceler.Token );
				}
				SMLog.Debug( $"{nameof( Load )} End : {_hashCode}" );
			}


			async UniTask LoadSub() {
				SMLog.Debug( $"{nameof( LoadSub )} Start : {_hashCode}" );
				await UniTask.Delay( 10000, cancellationToken: _canceler.Token );
	//			throw new NullReferenceException();
				_data = "Data";
				SMLog.Debug( $"{nameof( LoadSub )} End : {_hashCode}" );
			}


			public void Unload() {
				SMLog.Debug( $"{nameof( Unload )} : {_hashCode}" );
				_canceler.Cancel();
				_data = null;
			}

			public void ShowCancelLog() {
				// そのフレームのDelay後に発行される
				SMLog.Debug( $"Cancel : {_hashCode}" );
			}
		}
	}
}