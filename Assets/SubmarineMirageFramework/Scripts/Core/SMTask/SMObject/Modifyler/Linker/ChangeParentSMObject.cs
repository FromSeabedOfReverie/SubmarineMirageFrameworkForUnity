//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using Extension;
	using Debug;



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

			UnLinkObject( _object );
			if ( !_object._isTop )	{ SetAllObjectData( _object._top ); }

			var parent = _object._owner.GetComponentInParentUntilOneHierarchy<SMMonoBehaviour>( true )
				?._object;
			if ( parent != null )	{ AddChildObject( parent, _object ); }
			else					{ RegisterObject(); }

			var top = _object._top;
			SetTopObject( _object );

			top._modifyler.ReRegister( _object );

			if ( _object._parent != null && _object._owner.activeSelf ) {
				var isParentActive = _object._parent._owner.activeInHierarchy;
				await new ChangeActiveSMObject( _object, isParentActive, false ).Run();
			}
		}


		public override string ToString() => base.ToString().InsertLast( ", ",
			string.Join( ", ",
				_parent?.name,
				_isWorldPositionStays
			)
			+ ", "
		);
	}
}