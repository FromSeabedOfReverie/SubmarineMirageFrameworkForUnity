//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using Task.Base;
	using Task.Modifyler.Base;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SendReregisterGroupSMGroupManager : SMGroupManagerModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Linker;


		public SendReregisterGroupSMGroupManager( SMGroupBody target ) : base( target ) {}


		public override async UniTask Run() {
			if ( _owner._isFinalizing )	{ return; }

			if ( _target._isGameObject ) {
				_target._managerBody._scene.MoveGroup( _target );
			}

			_owner.Unlink( _target );
			_target._managerBody._modifyler.Register( new ReceiveReregisterGroupSMGroupManager( _target ) );
			_modifyler.Reregister( _target._managerBody, _target );

			await UTask.DontWait();
		}
	}
}