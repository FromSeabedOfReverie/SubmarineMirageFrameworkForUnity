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



	// TODO : コメント追加、整頓



	public class DisableObjectSMGroup : SMGroupModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;


		public DisableObjectSMGroup( SMObjectBody target ) : base( target ) {}


		public override async UniTask Run() {
			if ( !_target._isOperable )	{ return; }
			if ( _target._activeState == SMTaskActiveState.Disable )	{ return; }
			if ( !_target.IsActiveInParentHierarchy() )	{ return; }


			if ( _target.IsTop( _target ) )	{ _target._activeState = SMTaskActiveState.Disable; }

			foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new DisableSMObject( t ) );
			}

			_target._gameObject.SetActive( false );
		}
	}
}