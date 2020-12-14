//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Main;
	using Base;
	using UTask;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class SMRawTest : SMBaseTest, ISMRawBase {
		[Hide] public CompositeDisposable _disposables	{ get; private set; } = new CompositeDisposable();
		public bool _isDispose => _disposables.IsDisposed;
		[Hide] protected Func<UTaskCanceler, UniTask> _createEvent;
		[Hide] protected Func<UTaskCanceler, UniTask> _initializeEvent;
		[Hide] protected readonly Subject<Unit> _finalizeEvent = new Subject<Unit>();


		protected override async UniTask AwakeSub() {
			_disposables.Add( () => {
				_finalizeEvent.OnNext( Unit.Default );
				_asyncCanceler.Dispose();
				_createEvent = null;
				_initializeEvent = null;
				_finalizeEvent.OnCompleted();
				_finalizeEvent.Dispose();
			} );

			Create();
			if ( _createEvent != null ) {
				await _createEvent.Invoke( _asyncCanceler );
			}

			UTask.Void( async () => {
				await UTask.WaitWhile( _asyncCanceler, () => !SubmarineMirage.s_instance._isInitialized );
				if ( _initializeEvent != null ) {
					await _initializeEvent.Invoke( _asyncCanceler );
				}
				_isInitialized = true;
			} );
		}

		public override void Dispose() => _disposables.Dispose();


		protected void StopAsync() => _asyncCanceler.Cancel();
	}
}