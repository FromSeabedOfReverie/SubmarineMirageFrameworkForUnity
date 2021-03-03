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
		[SMShowLine] public override SMModifyType _type => SMModifyType.Linker;


		public SendReregisterGroupSMGroupManager( SMGroupBody target ) : base( target ) {}


		public override async UniTask Run() {
			if ( _target._isFinalizing )	{ return; }


			var newManager = _target._managerBody;

			newManager._scene.MoveGroup( _target );

			_target.Unlink( _target );

			newManager._modifyler.Register( new ReceiveReregisterGroupSMGroupManager( _target ) );
			_modifyler.Reregister( newManager, _target );

			await UTask.DontWait();
		}
	}
}