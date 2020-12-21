//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestObjectModifyler
namespace SubmarineMirage.Task.Object.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using Behaviour;
	using Object;
	using Debug;


	// TODO : コメント追加、整頓


	public class ReceiveChangeParentSMObject : SMObjectModifyData {
		[SMShowLine] SMObject _parent	{ get; set; }


		public ReceiveChangeParentSMObject( SMObject target, SMObject parent ) : base( target ) {
			_type = SMTaskModifyType.Interrupter;
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
#if TestObjectModifyler
				SMLog.Debug( string.Join( "\n",
					$"{nameof( ChangeActiveSMObject )} : start",
					$"isActive : {_target._owner.activeSelf}",
					$"isParentActive : {_parent._owner.activeInHierarchy}",
					$"{nameof( _target )} : {_target}"
				) );
#endif

				var isParentActive = _parent._owner.activeInHierarchy;
				await new ChangeActiveSMObject( _target, isParentActive, false ).Run();

#if TestObjectModifyler
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