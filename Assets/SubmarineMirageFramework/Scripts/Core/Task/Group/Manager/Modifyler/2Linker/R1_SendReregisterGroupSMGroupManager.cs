//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Utility;
	using Debug;



	public class SendReregisterGroupSMGroupManager : SMGroupManagerModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Linker;


		public SendReregisterGroupSMGroupManager( SMGroupBody group ) : base( group ) {}


		public override async UniTask Run() {
			if ( _target._isFinalizing )	{ return; }


			var newManager = _group._managerBody;

			newManager._scene.MoveGroup( _group );

			_target.Unlink( _group );

			newManager._modifyler.Register( new ReceiveReregisterGroupSMGroupManager( _group ) );
			_target.ReregisterModifyler( newManager, _group );

			await UTask.DontWait();
		}
	}
}