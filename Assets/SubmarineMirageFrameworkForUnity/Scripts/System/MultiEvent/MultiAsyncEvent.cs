//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.MultiEvent {
	using System;
	using System.Threading;
	using System.Collections.Generic;
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


		protected override void OnRemove( Func<CancellationToken, UniTask> function ) {}


		public async UniTask Invoke( CancellationToken cancel ) {
			CheckDisposeError();

			using ( var linkedCanceler = _canceler.Token.Add( cancel ) ) {
				var i = 0;
				KeyValuePair< string, Func<CancellationToken, UniTask> > pair;
// TODO : Remove、Add、Insert等で、配列数変わるので、対応する
				while ( i < _events.Count ) {
					pair = _events[i];
					try {
						_isInvoking.Value = true;
						await pair.Value.Invoke( linkedCanceler.Token );
					} finally {
						_isInvoking.Value = false;
					}
					i++;
				}
			}
		}
	}
}