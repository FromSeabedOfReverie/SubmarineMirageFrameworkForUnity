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



	public class ChangeLinkSMTask : SMTaskModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;
		[SMShowLine] SMTask _previous;
		[SMShowLine] SMTask _task;


		public ChangeLinkSMTask( SMTask previous, SMTask task ) {
			_previous = previous;
			_task = task;
		}


		public override async UniTask Run() {
			_task.Unlink();
			_previous.Link( _task );

			_group.RegisterRunEventToOwner();

			await UTask.DontWait();
		}
	}
}