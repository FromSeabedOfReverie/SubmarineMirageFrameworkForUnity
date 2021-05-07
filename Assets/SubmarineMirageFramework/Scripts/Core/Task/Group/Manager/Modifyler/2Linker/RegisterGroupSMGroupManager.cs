//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Extension;
	using Utility;
	using Debug;



	public class RegisterGroupSMGroupManager : SMGroupManagerModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Linker;


		public RegisterGroupSMGroupManager( SMGroupBody group ) : base( group ) {}

		protected override void Cancel() {
			_group.DisposeAllObjects();
			_group._gameObject.Destroy();
		}


		public override async UniTask Run() {
			_target.Link( _group );
			_group.RegisterRunEventToOwner();

			await UTask.DontWait();
		}
	}
}