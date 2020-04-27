//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process.New {
	using UniRx.Async;
	using RanState = ProcessBody.RanState;


	// TODO : コメント追加、整頓


	public class DestroyHierarchyModifyData : HierarchyModifyData {
		public DestroyHierarchyModifyData( ProcessHierarchy hierarchy ) {
			_hierarchy = hierarchy;
		}


		protected override async UniTask Run() {
			if ( _hierarchy == _hierarchy._top ) {
				_owner.Gets( _hierarchy._type )
					.Remove( _hierarchy );
			} else {
// TODO : ChangeParentに変更する
				_hierarchy.SetParent( null );
			}
			await RunHierarchy();
		}


		public async UniTask RunHierarchy() {
			await _hierarchy.RunStateEvent( RanState.Finalizing );
			_hierarchy.Dispose();
		}
	}
}