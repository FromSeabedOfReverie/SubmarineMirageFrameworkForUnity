//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using UnityEngine;
	using UniRx.Async;
	using Extension;
	using Utility;


	// TODO : コメント追加、整頓


	public class ChangeParentSMHierarchy : SMHierarchyModifyData {
		Transform _parent;
		bool _isWorldPositionStays;


		public ChangeParentSMHierarchy( SMHierarchy hierarchy, Transform parent,
												bool isWorldPositionStays
		) : base( hierarchy )
		{
			_parent = parent;
			_isWorldPositionStays = isWorldPositionStays;
			if ( hierarchy._owner == null ) {
				throw new NotSupportedException( $"{nameof(SMBehavior)}._hierarchyの、親変更不可 :\n{hierarchy}" );
			}
		}


		protected override async UniTask Run() {
			_hierarchy._owner.transform.SetParent( _parent, _isWorldPositionStays );

			if ( _hierarchy._isTop ) {
				_owner.Get( _hierarchy._type )
					.Remove( _hierarchy );
			}
			_hierarchy.SetParent(
				_hierarchy._owner.GetComponentInParentUntilOneHierarchy<SMMonoBehaviour>( true )
					?._hierarchy
			);
			_hierarchy.SetTop();

			if ( _hierarchy._parent?._owner.activeInHierarchy ?? false ) {
				// TODO : 親の活動状態と、変更してきた子の活動状態が異なる場合、子の活動状態を、親に合わせる
			}

			await UniTaskUtility.DontWait();
		}
	}
}