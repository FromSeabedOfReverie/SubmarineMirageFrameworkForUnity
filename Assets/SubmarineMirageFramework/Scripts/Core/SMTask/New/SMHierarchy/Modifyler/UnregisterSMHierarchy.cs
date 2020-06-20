//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using UnityEngine;
	using UniRx.Async;
	using Utility;


	// TODO : コメント追加、整頓


	public class UnregisterSMHierarchy : SMHierarchyModifyData {
		public UnregisterSMHierarchy( SMHierarchy hierarchy ) : base( hierarchy ) {}


		protected override async UniTask Run() {
			_owner.Get( _hierarchy._type )
				.Remove( _hierarchy );
			_hierarchy.Dispose();
			if ( _hierarchy._owner != null )	{ Object.Destroy( _hierarchy._owner ); }

			await UniTaskUtility.DontWait();
		}
	}
}