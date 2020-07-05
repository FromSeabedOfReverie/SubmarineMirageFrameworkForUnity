//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System;
	using System.Threading;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Main.New;
	using Utility;


	// TODO : コメント追加、整頓


	public abstract class RawTest : BaseTest {
		protected Func<CancellationToken, UniTask> _createEvent;
		protected Func<CancellationToken, UniTask> _initializeEvent;
		protected readonly Subject<Unit> _finalizeEvent = new Subject<Unit>();
		protected readonly CompositeDisposable _disposables = new CompositeDisposable();


		protected override async UniTask AwakeSub() {
			_disposables.Add( Disposable.Create( () => {
				_finalizeEvent.OnNext( Unit.Default );
			} ) );
			_disposables.Add( Disposable.Create( () => {
				_asyncCanceler.Cancel();
				_asyncCanceler.Dispose();
			} ) );
			_disposables.Add( Disposable.Create( () => {
				_createEvent = null;
				_initializeEvent = null;
				_finalizeEvent.OnCompleted();
				_finalizeEvent.Dispose();
			} ) );

			Create();
			if ( _createEvent != null ) {
				await _createEvent.Invoke( _asyncCancel );
			}

			UniTask.Void( async () => {
				await UniTaskUtility.WaitWhile( _asyncCancel, () => !SubmarineMirage.s_instance._isInitialized );
				if ( _initializeEvent != null ) {
					await _initializeEvent.Invoke( _asyncCancel );
				}
				_isInitialized = true;
			} );
		}

		public override void Dispose() => _disposables.Dispose();


		protected void StopAsync() {
			_asyncCanceler.Cancel();
			_asyncCanceler.Dispose();
			_asyncCanceler = new CancellationTokenSource();
		}
	}
}