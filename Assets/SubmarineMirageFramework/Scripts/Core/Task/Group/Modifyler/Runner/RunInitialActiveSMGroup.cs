//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupModifyler
namespace SubmarineMirage.Task.Group.Modifyler {
	using Cysharp.Threading.Tasks;
	using Debug;



	// TODO : コメント追加、整頓



	public class RunInitialActiveSMGroup : SMGroupModifyData {
		[SMShowLine] SMTaskType _taskType	{ get; set; }


		public RunInitialActiveSMGroup( SMTaskType taskType ) : base( null ) {
			_type = SMTaskModifyType.Runner;
			_taskType = taskType;
		}

		protected override void Cancel() {}


		public override async UniTask Run() {
			var gs = _owner.GetAllGroups( _taskType );

			switch ( _taskType ) {
				case SMTaskType.FirstWork:
					foreach ( var g in gs ) {
						await g.RunInitialActive();
					}
					return;

				case SMTaskType.Work:
					await gs.Select( g => g.RunInitialActive() );
					return;
			}
		}
	}
}