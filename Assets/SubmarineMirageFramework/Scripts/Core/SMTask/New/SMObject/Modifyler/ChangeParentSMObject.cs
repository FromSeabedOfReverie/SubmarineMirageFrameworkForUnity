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


	public class ChangeParentSMObject : SMObjectModifyData {
		Transform _parent;
		bool _isWorldPositionStays;


		public ChangeParentSMObject( SMObject smObject, Transform parent, bool isWorldPositionStays )
			: base( smObject )
		{
			_parent = parent;
			_isWorldPositionStays = isWorldPositionStays;
			if ( smObject._owner == null ) {
				throw new NotSupportedException(
					$"{nameof(SMBehavior)}.{nameof( _object )}の、親変更不可 :\n{smObject}" );
			}
		}


		protected override async UniTask Run() {
			_object._owner.transform.SetParent( _parent, _isWorldPositionStays );

			if ( _object._isTop ) {
				_owner.Get( _object._type )
					.Remove( _object );
			}
			_object.SetParent(
				_object._owner.GetComponentInParentUntilOneHierarchy<SMMonoBehaviour>( true )
					?._object
			);
			_object.SetTop();

			if ( _object._parent?._owner.activeInHierarchy ?? false ) {
				// TODO : 親の活動状態と、変更してきた子の活動状態が異なる場合、子の活動状態を、親に合わせる
			}

			await UniTaskUtility.DontWait();
		}
	}
}