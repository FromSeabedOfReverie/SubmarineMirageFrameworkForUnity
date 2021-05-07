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



	public class EnableObjectSMGroup : SMGroupModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;


		public EnableObjectSMGroup( SMObjectBody smObject ) : base( smObject ) {}


		public override async UniTask Run() {
			if ( !_target._isOperable )	{ return; }
			if ( _target._activeState != SMTaskActiveState.Enable && !_target.IsTop( _object ) )	{ return; }
			if ( !_object.IsActiveInParentHierarchy() )	{ return; }


			_object._gameObject.SetActive( true );

			foreach ( var t in SMGroupManagerBody.SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new EnableSMObject( t ) );
			}

			if ( _target.IsTop( _object ) )	{ _target._activeState = SMTaskActiveState.Enable; }
		}
	}
}