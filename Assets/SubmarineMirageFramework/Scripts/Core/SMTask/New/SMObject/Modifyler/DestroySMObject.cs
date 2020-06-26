//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using UnityEngine;
	using UniRx.Async;


	// TODO : コメント追加、整頓


	public class DestroySMObject : SMObjectModifyData {
		public DestroySMObject( SMObject smObject ) : base( smObject ) {}


		protected override async UniTask Run() {
			if ( _object._objects._objects[_object._type] == _object ) {
				var next = _object._next;
				_object.UnLink();
				_object._objects._objects[_object._type] = next;

			} else {
				var top = _object._top;
				_object.UnLink();
				top.SetAllData();
			}

			await RunObject();
		}


		public async UniTask RunObject() {
			await _object.ChangeActive( false, true );
			await _object.RunStateEvent( SMTaskRanState.Finalizing );
			_object.Dispose();
			if ( _object._owner != null )	{ Object.Destroy( _object._owner ); }
		}
	}
}