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
	using Extension;


	// TODO : コメント追加、整頓


	public class MultiAsyncEvent : BaseMultiEvent< Func<UTaskCanceler, UniTask> > {
		UTaskCanceler _canceler;


		public override void Dispose() {
			_canceler?.Dispose();
			base.Dispose();
		}

		public override void OnRemove( Func<UTaskCanceler, UniTask> function ) {}


		public async UniTask Run( UTaskCanceler canceler ) {
			CheckDisposeError();

			using ( _canceler = canceler.CreateChild() ) {
				var temp = _events.Copy();
				foreach ( var pair in temp ) {
					await pair.Value.Invoke( _canceler );
				}
			}
		}
	}
}