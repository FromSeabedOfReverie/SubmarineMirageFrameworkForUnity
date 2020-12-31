//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupModifyler
namespace SubmarineMirage.Task.Group.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using Behaviour;
	using Object;
	using Object.Modifyler;
	using Debug;


	// TODO : コメント追加、整頓


	public class ReceiveChangeParentObjectSMGroup : SMGroupModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.FirstLinker;
		[SMShowLine] SMObject _parent	{ get; set; }


		public ReceiveChangeParentObjectSMGroup( SMObject target, SMObject parent ) : base( target ) {
			_parent = parent;

			if ( !_target._isGameObject || !_parent._isGameObject ) {
				throw new NotSupportedException( $"{nameof( SMMonoBehaviour )}で無い為、追加不可 :\n{_target}" );
			}
		}

		protected override void Cancel() => _target.Dispose();


		public override async UniTask Run() {
			SMObjectApplyer.LinkChild( _parent, _target );
			_parent._group.SetAllData();

			if ( _target._owner.activeSelf ) {
#if TestGroupModifyler
				SMLog.Debug( string.Join( "\n",
					$"{nameof( ChangeActiveSMObject )} : start",
					$"isActive : {_target._owner.activeSelf}",
					$"isParentActive : {_parent._owner.activeInHierarchy}",
					$"{nameof( _target )} : {_target}"
				) );
#endif

				var isParentActive = _parent._owner.activeInHierarchy;
				await new ChangeActiveSMObject( _target, isParentActive, false ).Run();

#if TestGroupModifyler
				SMLog.Debug( string.Join( "\n",
					$"{nameof( ChangeActiveSMObject )} : end",
					$"isActive : {_target._owner.activeSelf}",
					$"isParentActive : {_parent._owner.activeInHierarchy}",
					$"{nameof( _target )} : {_target}"
				) );
#endif
			}
		}
	}
}