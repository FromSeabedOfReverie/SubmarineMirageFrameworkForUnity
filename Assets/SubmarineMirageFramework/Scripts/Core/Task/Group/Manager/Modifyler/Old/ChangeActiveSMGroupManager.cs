//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Group.Manager.Modifyler {
	using Cysharp.Threading.Tasks;
	using Debug;



	// TODO : コメント追加、整頓



	public class ChangeActiveSMGroupManager : SMGroupManagerModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;
		[SMShowLine] SMTaskType _taskType	{ get; set; }
		[SMShowLine] bool _isActive	{ get; set; }


		public ChangeActiveSMGroupManager( SMTaskType taskType, bool isActive ) : base( null ) {
			_taskType = taskType;
			_isActive = isActive;
		}

		protected override void Cancel() {}


		public override async UniTask Run() {
			var gs = _owner.GetAllGroups( _taskType, _taskType == SMTaskType.FirstWork && !_isActive );

			switch ( _taskType ) {
				case SMTaskType.FirstWork:
					foreach ( var g in gs ) {
						await g.ChangeActive( _isActive );
					}
					return;

				case SMTaskType.Work:
					await gs.Select( g => g.ChangeActive( _isActive ) );
					return;
			}
		}
	}
}