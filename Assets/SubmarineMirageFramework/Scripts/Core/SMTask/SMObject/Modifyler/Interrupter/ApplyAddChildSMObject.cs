//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using Debug;


	// TODO : コメント追加、整頓


	public class ApplyAddChildSMObject : SMObjectModifyData {
		SMObject _parent;


		public ApplyAddChildSMObject( SMObject smObject, SMObject parent ) : base( smObject ) {
			_parent = parent;
			_type = ModifyType.Interrupter;

			if ( !_object._isGameObject || !_parent._isGameObject ) {
				throw new NotSupportedException( $"{nameof( SMMonoBehaviour )}で無い為、追加不可 :\n{_object}" );
			}
		}


		public override void Cancel() {
			_object.Dispose();
		}


		public override async UniTask Run() {
			AddChildObject( _parent, _object );
			_parent._group.SetAllData();

			if ( _object._owner.activeSelf ) {
#if TestSMTaskModifyler
				Log.Debug( string.Join( "\n",
					$"{nameof( ChangeActiveSMObject )} : start",
					$"isActive : {_object._owner.activeSelf}",
					$"isParentActive : {_parent._owner.activeInHierarchy}",
					$"{nameof( _object )} : {_object}"
				) );
#endif

				var isParentActive = _parent._owner.activeInHierarchy;
				await new ChangeActiveSMObject( _object, isParentActive, false ).Run();

#if TestSMTaskModifyler
				Log.Debug( string.Join( "\n",
					$"{nameof( ChangeActiveSMObject )} : end",
					$"isActive : {_object._owner.activeSelf}",
					$"isParentActive : {_parent._owner.activeInHierarchy}",
					$"{nameof( _object )} : {_object}"
				) );
#endif
			}
		}
	}
}