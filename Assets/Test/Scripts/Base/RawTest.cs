//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using UniRx;
	using UniRx.Async;
	using Main.New;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class RawTest : BaseTest {
		protected readonly Subject<Unit> _initializeEvent = new Subject<Unit>();
		protected readonly Subject<Unit> _finalizeEvent = new Subject<Unit>();
		protected readonly CompositeDisposable _disposables = new CompositeDisposable();


		protected override async UniTask AwakeSub() {
			try {
				await Task.Delay( 2, _asyncCancel );
				_disposables.Add( Disposable.Create( () => {
					_asyncCanceler.Cancel();
					_asyncCanceler.Dispose();
					_initializeEvent.OnCompleted();
					_initializeEvent.Dispose();
					_finalizeEvent.OnCompleted();
					_finalizeEvent.Dispose();
				} ) );
				await UniTaskUtility.WaitWhile( _asyncCancel, () => !MainProcess.s_isInitialized );
				Create();
				_initializeEvent.OnNext( Unit.Default );
				_isInitialized = true;

			} catch ( Exception e ) {
				Log.Error( e );
			}
		}


		protected override void OnDestroy() {
			try {
				_finalizeEvent.OnNext( Unit.Default );
				base.OnDestroy();
			} catch ( Exception e ) {
				Log.Error( e );
			}
		}

		public override void Dispose() => _disposables.Dispose();

		~RawTest() => Dispose();


		protected void StopAsync() {
			_asyncCanceler.Cancel();
			_asyncCanceler.Dispose();
			_asyncCanceler = new CancellationTokenSource();
		}
	}
}