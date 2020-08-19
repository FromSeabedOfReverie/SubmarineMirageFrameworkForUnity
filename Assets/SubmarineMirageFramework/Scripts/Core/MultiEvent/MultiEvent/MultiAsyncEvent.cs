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
	using Debug;


	// TODO : コメント追加、整頓


	public class MultiAsyncEvent : BaseMultiEvent< Func<UTaskCanceler, UniTask> > {
		UTaskCanceler _canceler;
		public bool _isRunning	{ get; private set; }


		public override void Dispose() {
			_canceler?.Dispose();	// _disposables.Add()だと、最初期実行登録ができない
			base.Dispose();
		}

		public override void OnRemove( Func<UTaskCanceler, UniTask> function ) {}


		public async UniTask Run( UTaskCanceler canceler ) {
			CheckDisposeError();

			if ( _isRunning ) {
				Log.Warning( $"既に実行中の為、未実行 : {this}" );
				return;
			}

			try {
				_isRunning = true;
				using ( _canceler = canceler.CreateChild() ) {
					var temp = _events.Copy();
					foreach ( var pair in temp ) {
						await pair.Value.Invoke( _canceler );
					}
				}
			} finally {
				_isRunning = false;
			}
		}


		public override string ToString() => base.ToString().InsertLast( "\n",
			$"    {nameof( _isRunning )} : {_isRunning}\n"
		);
	}
}