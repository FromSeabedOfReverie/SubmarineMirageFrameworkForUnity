//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.MultiEvent {
	using System;
	using System.Threading;
	using UniRx.Async;
	using Extension;


	// TODO : コメント追加、整頓


	public class MultiAsyncEvent : BaseMultiEvent< Func<CancellationToken, UniTask> > {
		readonly CancellationTokenSource _canceler = new CancellationTokenSource();


		public MultiAsyncEvent() {
			_disposables.AddFirst( () => {
				_canceler.Cancel();
				_canceler.Dispose();
			} );
		}

		public override void OnRemove( Func<CancellationToken, UniTask> function ) {}


		public async UniTask Run( CancellationToken cancel ) {
			CheckDisposeError();

			var linkedCanceler = _canceler.Token.Add( cancel );
			_isInvoking.Value = true;
			try {
				foreach ( var pair in _events ) {
					await pair.Value.Invoke( linkedCanceler.Token );
				}
			} finally {
				_isInvoking.Value = false;
				linkedCanceler.Dispose();
			}
		}
	}
}