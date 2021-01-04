//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Group.Modifyler {
	using Cysharp.Threading.Tasks;
	using Object;
	using Object.Modifyler;
	using Group.Manager.Modifyler;
	using Debug;



	// TODO : コメント追加、整頓



	public class ChangeParentActiveObjectSMGroup : SMGroupModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.FirstRunner;


		public ChangeParentActiveObjectSMGroup( SMObject target ) : base( target ) {}


		public override async UniTask Run() {
			if ( !_owner._isFinalizing )	{ return; }


			if ( SMObjectApplyer.IsActiveInHierarchy( _target ) ) {
				await Enable();
			} else {
				await Disable();
			}
		}


		async UniTask Enable() {
			foreach ( var t in SMGroupManagerApplyer.SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new EnableSMObject( t ) );
			}

			if ( _owner.IsTop( _target ) )	{ _owner._activeState = SMTaskActiveState.Enable; }
		}


		async UniTask Disable() {
			if ( _owner.IsTop( _target ) )	{ _owner._activeState = SMTaskActiveState.Disable; }

			foreach ( var t in SMGroupManagerApplyer.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new DisableSMObject( t ) );
			}
		}
	}
}