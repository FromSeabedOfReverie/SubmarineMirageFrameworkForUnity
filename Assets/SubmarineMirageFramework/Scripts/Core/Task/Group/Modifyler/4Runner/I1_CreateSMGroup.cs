//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupModifyler
namespace SubmarineMirage.Task.Group.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using Debug;



	// TODO : コメント追加、整頓



	public class CreateSMGroup : SMGroupModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;
		[SMShowLine] SMTaskType _taskType	{ get; set; }


		public CreateSMGroup( SMTaskType taskType ) : base( null ) {
			_taskType = taskType;
		}

		protected override void Cancel() {}


		public override async UniTask Run() {
		}
	}
}