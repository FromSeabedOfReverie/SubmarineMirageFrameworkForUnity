//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process.New {
	using System;
	using UnityEngine;
	using UniRx.Async;
	using Extension;
	using Utility;


	// TODO : コメント追加、整頓


	public class ChangeParentHierarchyModifyData : HierarchyModifyData {
		Transform _parent;
		bool _isWorldPositionStays;


		public ChangeParentHierarchyModifyData( ProcessHierarchy hierarchy, Transform parent,
												bool isWorldPositionStays
		) : base( hierarchy )
		{
			_parent = parent;
			_isWorldPositionStays = isWorldPositionStays;
			if ( hierarchy._owner == null ) {
				throw new NotSupportedException( $"{nameof(BaseProcess)}._hierarchyの、親変更不可 :\n{hierarchy}" );
			}
		}


		protected override async UniTask Run() {
			_hierarchy._owner.transform.SetParent( _parent, _isWorldPositionStays );

			if ( _hierarchy._isTop ) {
				_owner.Gets( _hierarchy._type )
					.Remove( _hierarchy );
			}
			_hierarchy.SetParent(
				_hierarchy._owner.GetComponentInParentUntilOneHierarchy<MonoBehaviourProcess>( true )
					?._hierarchy
			);
			_hierarchy.SetTop();

			await UniTaskUtility.DontWait();
		}
	}
}