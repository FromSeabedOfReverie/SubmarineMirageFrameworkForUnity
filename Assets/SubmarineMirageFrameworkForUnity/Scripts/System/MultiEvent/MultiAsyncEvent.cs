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

	// TODO : コメント追加、整頓

	public class MultiAsyncEvent : BaseMultiEvent< Func<CancellationToken, UniTask> > {
		public async UniTask Invoke( CancellationToken cancel ) {
			foreach ( var pair in _events ) {
				if ( pair.Value != null ) {
					await pair.Value.Invoke( cancel );
				}
			}
		}
	}
}