//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupManagerModifyler
namespace SubmarineMirage.Task.Group.Manager.Modifyler {
	using Cysharp.Threading.Tasks;
	using Group.Modifyler;
	using Scene;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class RegisterGroupSMGroupManager : SMGroupManagerModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Linker;


		public RegisterGroupSMGroupManager( SMGroup target ) : base( target ) {}

		protected override void Cancel() {
			SMGroupApplyer.DisposeAll( _target );
			_target._gameObject.Destroy();
		}


		public override async UniTask Run() {
			if ( _target._isGameObject && _target._lifeSpan == SMTaskLifeSpan.Forever ) {
				SMSceneManager.s_instance.MoveGroup( _target, _owner._scene );
			}
			SMGroupManagerApplyer.Link( _owner, _target );

			await SMGroupManagerApplyer.RegisterRunEventToOwner( _owner, _target );
		}
	}
}