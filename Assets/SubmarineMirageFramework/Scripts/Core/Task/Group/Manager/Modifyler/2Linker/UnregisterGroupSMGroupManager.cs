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


	public class UnregisterGroupSMGroupManager : SMGroupManagerModifyData {
		[SMShowLine] public override SMTaskModifyType _type => SMTaskModifyType.Linker;


		public UnregisterGroupSMGroupManager( SMGroupBody target ) : base( target ) {}


		public override async UniTask Run() {
			_modifyler.Unregister( _target );
			_owner.Unlink( _target );

			await UTask.DontWait();
		}
	}
}