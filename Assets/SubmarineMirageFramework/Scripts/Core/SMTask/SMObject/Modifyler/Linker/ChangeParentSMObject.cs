//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using Extension;


	// TODO : コメント追加、整頓


	public class ChangeParentSMObject : SMObjectModifyData {
		public override ModifyType _type => ModifyType.Linker;
		Transform _parent;
		bool _isWorldPositionStays;


		public ChangeParentSMObject( SMObject smObject, Transform parent, bool isWorldPositionStays )
			: base( smObject )
		{
			if ( _object._owner == null ) {
				throw new NotSupportedException( $"{nameof( SMMonoBehaviour )}で無い為、親変更不可 :\n{_object}" );
			}
			_parent = parent;
			_isWorldPositionStays = isWorldPositionStays;
		}

		public override void Cancel() {}


		public override async UniTask Run() {
			_object._owner.transform.SetParent( _parent, _isWorldPositionStays );

			var top = _object._top;
			UnLinkObject( _object );
			if ( top != _object )	{ SetAllObjectData( top ); }

			var parent = _object._owner.GetComponentInParentUntilOneHierarchy<SMMonoBehaviour>( true )
				?._object;
			if ( parent != null )	{ AddChildObject( parent, _object ); }
			else					{ RegisterObject(); }

			SetTopObject( _object );

			top._modifyler._data.RemoveAll(
				d => d._object == _object,
				d => _object._top._modifyler.Register( d )
			);

			if ( _object._parent != null && _object._owner.activeSelf ) {
				var isParentActive = _object._parent._owner.activeInHierarchy;
				await new ChangeActiveSMObject( _object, isParentActive, false ).Run();
			}
		}


		public override string ToString() => base.ToString().InsertLast( " ",
			string.Join( ", ",
				_parent?.name,
				_isWorldPositionStays
			)
			+ ", "
		);
	}
}