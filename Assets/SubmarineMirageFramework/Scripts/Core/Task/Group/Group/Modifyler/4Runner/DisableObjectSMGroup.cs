//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Debug;



	public class DisableObjectSMGroup : SMGroupModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;


		public DisableObjectSMGroup( SMObjectBody smObject ) : base( smObject ) {}


		public override async UniTask Run() {
			if ( !_target._isOperable )	{ return; }
			if ( _target._activeState == SMTaskActiveState.Disable )	{ return; }
			if ( !_object.IsActiveInParentHierarchy() )	{ return; }


			if ( _target.IsTop( _object ) )	{ _target._activeState = SMTaskActiveState.Disable; }

			foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new DisableSMObject( t ) );
			}

			_object._gameObject.SetActive( false );
		}
	}
}