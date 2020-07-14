//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.MultiEvent {
	using System;
	using Cysharp.Threading.Tasks;
	using UTask;


	// TODO : コメント追加、整頓


	public class MultiAsyncEvent : BaseMultiEvent< Func<UTaskCanceler, UniTask> > {
		UTaskCanceler _canceler;


		public MultiAsyncEvent() {
			_disposables.AddFirst( () => _canceler?.Dispose() );
		}

		public override void OnRemove( Func<UTaskCanceler, UniTask> function ) {}


		public async UniTask Run( UTaskCanceler canceler ) {
			CheckDisposeError();

			_canceler = canceler;
			_isLock = true;
			try {
				foreach ( var pair in _events ) {
					await pair.Value.Invoke( _canceler );
				}
			} finally {
				_isLock = false;
				_canceler = null;
			}
		}
	}
}