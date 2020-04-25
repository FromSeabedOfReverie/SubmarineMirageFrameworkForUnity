//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process.New {
	using UniRx.Async;
	using Utility;


	// TODO : コメント追加、整頓


	public class UnregisterHierarchyModifyData : HierarchyModifyData {
		public UnregisterHierarchyModifyData( ProcessHierarchy hierarchy ) {
			_hierarchy = hierarchy;
		}


		protected override async UniTask Run() {
			_hierarchy.Dispose();
			_owner.Gets( _hierarchy._type )
				.Remove( _hierarchy );

			await UniTaskUtility.DontWait();
		}
	}
}