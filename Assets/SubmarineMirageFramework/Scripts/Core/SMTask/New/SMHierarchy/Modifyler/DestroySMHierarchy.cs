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


	public class DestroySMHierarchy : SMHierarchyModifyData {
		public DestroySMHierarchy( SMHierarchy hierarchy ) : base( hierarchy ) {}


		protected override async UniTask Run() {
			if ( _hierarchy._hierarchies._hierarchies[_hierarchy._type] == _hierarchy ) {
				var next = _hierarchy._next;
				_hierarchy.UnLink();
				_hierarchy._hierarchies._hierarchies[_hierarchy._type] = next;

			} else {
				var top = _hierarchy._top;
				_hierarchy.UnLink();
				top.SetAllData();
			}

			await RunHierarchy();
		}


		public async UniTask RunHierarchy() {
			await _hierarchy.ChangeActive( false, true );
			await _hierarchy.RunStateEvent( SMTaskRanState.Finalizing );
			_hierarchy.Dispose();
			if ( _hierarchy._owner != null )	{ Object.Destroy( _hierarchy._owner ); }
		}
	}
}