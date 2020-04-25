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
	using UniRx.Async;
	using Main.New;
	using MultiEvent;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class Test : BaseTest {
		protected readonly MultiAsyncEvent _initializeEvent = new MultiAsyncEvent();
		protected readonly MultiSubject _finalizeEvent = new MultiSubject();
		protected readonly MultiDisposable _disposables	= new MultiDisposable();


		protected override async UniTask AwakeSub() {
			try {
				await Task.Delay( 2, _asyncCancel );
				SetAsyncCancelerDisposable();
				_disposables.AddLast( _initializeEvent, _finalizeEvent );
				await UniTaskUtility.WaitWhile( _asyncCancel, () => !SubmarineMirage.s_isInitialized );
				Create();
				await _initializeEvent.Run( _asyncCancel );
				_isInitialized = true;

			} catch ( Exception e ) {
				Log.Error( e );
			}
		}

		void SetAsyncCancelerDisposable() {
			_disposables.AddFirst( "_asyncCanceler", () => {
				_asyncCanceler.Cancel();
				_asyncCanceler.Dispose();
			} );
		}

		protected override void OnDestroy() {
			try {
				_finalizeEvent.Run();
				base.OnDestroy();
			} catch ( Exception e ) {
				Log.Error( e );
			}
		}

		public override void Dispose() => _disposables.Dispose();

		~Test() => Dispose();


		protected void StopAsync() {
			_disposables.Remove( "_asyncCanceler" );
			_asyncCanceler = new CancellationTokenSource();
			SetAsyncCancelerDisposable();
		}
	}
}