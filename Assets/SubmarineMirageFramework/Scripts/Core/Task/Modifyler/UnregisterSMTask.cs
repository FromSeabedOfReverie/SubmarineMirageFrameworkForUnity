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



	public class UnregisterSMTask : SMTaskModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Normal;



		public UnregisterSMTask( SMTask task ) : base( task ) {}



		public override async UniTask Run() {
			_task.Unlink();

			await UTask.DontWait();
		}
	}
}