//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupManagerModifyler
namespace SubmarineMirage.Task.Group.Manager.Modifyler {
	using Cysharp.Threading.Tasks;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SendReregisterGroupSMGroupManager : SMGroupManagerModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Linker;


		public SendReregisterGroupSMGroupManager( SMGroup target ) : base( target ) {}


		public override async UniTask Run() {
			if ( _owner._isFinalizing )	{ return; }

			if ( _target._isGameObject ) {
				_target._scene.MoveGroup( _target );
			}

			SMGroupManagerApplyer.Unlink( _owner, _target );
			_target._groups._modifyler.Register( new ReceiveReregisterGroupSMGroupManager( _target ) );
			_modifyler.Reregister( _target._groups, _target );

			await UTask.DontWait();
		}
	}
}