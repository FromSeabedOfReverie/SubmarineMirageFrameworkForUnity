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
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class ReceiveReregisterGroupSMGroupManager : SMGroupManagerModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.FirstLinker;


		public ReceiveReregisterGroupSMGroupManager( SMGroup target ) : base( target ) {}

		protected override void Cancel() {
			SMGroupBody.DisposeAll( _target );
			_target._gameObject.Destroy();
		}


		public override async UniTask Run() {
			SMGroupManagerBody.Link( _owner, _target );
			SMGroupManagerBody.RegisterRunEventToOwner( _owner, _target );

			await UTask.DontWait();
		}
	}
}