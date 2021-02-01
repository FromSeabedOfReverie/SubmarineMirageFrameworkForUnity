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



	public class EnableObjectSMGroup : SMGroupModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;


		public EnableObjectSMGroup( SMObject target ) : base( target ) {}


		public override async UniTask Run() {
			if ( !_owner._isOperable )	{ return; }
			if ( _owner._activeState != SMTaskActiveState.Enable && !_owner.IsTop( _target ) )	{ return; }
			if ( !SMObjectBody.IsActiveInParentHierarchy( _target ) )	{ return; }


			if ( _target._isGameObject )	{ _target._gameObject.SetActive( true ); }

			foreach ( var t in SMGroupManagerBody.SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new EnableSMObject( t ) );
			}

			if ( _owner.IsTop( _target ) )	{ _owner._activeState = SMTaskActiveState.Enable; }
		}
	}
}