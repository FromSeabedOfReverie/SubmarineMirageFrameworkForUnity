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
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class ReceiveReregisterGroupSMGroupManager : SMGroupManagerModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.FirstLinker;


		public ReceiveReregisterGroupSMGroupManager( SMGroupBody target ) : base( target ) {}

		protected override void Cancel() {
			_target.DisposeAllObjects();
			_target._gameObject.Destroy();
		}


		public override async UniTask Run() {
			_target.Link( _target );
			_target.RegisterRunEventToOwner();

			await UTask.DontWait();
		}
	}
}