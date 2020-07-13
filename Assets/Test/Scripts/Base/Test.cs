//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System.Threading;
	using Cysharp.Threading.Tasks;
	using Main;
	using MultiEvent;
	using UTask;


	// TODO : コメント追加、整頓


	public abstract class Test : BaseTest {
		protected readonly MultiAsyncEvent _createEvent = new MultiAsyncEvent();
		protected readonly MultiAsyncEvent _initializeEvent = new MultiAsyncEvent();
		protected readonly MultiSubject _finalizeEvent = new MultiSubject();
		protected readonly MultiDisposable _disposables	= new MultiDisposable();


		protected override async UniTask AwakeSub() {
			_disposables.AddLast( () => _finalizeEvent.Run() );
			SetAsyncCancelerDisposable();
			_disposables.AddLast(
				_createEvent,
				_initializeEvent,
				_finalizeEvent
			);

			Create();
			await _createEvent.Run( _asyncCancel );

			UTask.Void( _asyncCancel, async cancel => {
				await UTask.WaitWhile( cancel, () => !SubmarineMirage.s_instance._isInitialized );
				await _initializeEvent.Run( cancel );
				_isInitialized = true;
			} );
		}

		void SetAsyncCancelerDisposable() {
			_disposables.AddFirst( "_asyncCanceler", () => {
				_asyncCanceler.Cancel();
				_asyncCanceler.Dispose();
			} );
		}

		public override void Dispose() => _disposables.Dispose();


		protected void StopAsync() {
			_disposables.Remove( "_asyncCanceler" );
			_asyncCanceler = new CancellationTokenSource();
			SetAsyncCancelerDisposable();
		}
	}
}