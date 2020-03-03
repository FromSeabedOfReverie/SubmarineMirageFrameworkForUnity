//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process {
	using System;
	using UniRx.Async;

	public class MultiAsyncEvent : BaseMultiEvent< Func<UniTask> > {
		public async UniTask Invoke() {
			foreach ( var pair in _events ) {
				if ( pair.Value != null ) {
					await pair.Value.Invoke();
				}
			}
		}
	}
}