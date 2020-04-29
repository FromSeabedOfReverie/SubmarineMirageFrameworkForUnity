//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process.New {
	using UnityEngine;
	using UniRx.Async;
	using RanState = ProcessBody.RanState;


	// TODO : コメント追加、整頓


	public class DestroyHierarchyModifyData : HierarchyModifyData {
		public DestroyHierarchyModifyData( ProcessHierarchy hierarchy ) : base( hierarchy ) {}


		protected override async UniTask Run() {
			if ( _hierarchy._isTop ) {
				_owner.Gets( _hierarchy._type )
					.Remove( _hierarchy );
			} else {
				var lastTop = _hierarchy._top;
				_hierarchy.SetParent( null );
				lastTop.SetAllData();
			}

			await RunHierarchy();
		}


		public async UniTask RunHierarchy() {
			await _hierarchy.ChangeActive( false, true );
			await _hierarchy.RunStateEvent( RanState.Finalizing );
			_hierarchy.Dispose();
			if ( _hierarchy._owner != null )	{ Object.Destroy( _hierarchy._owner ); }
		}
	}
}