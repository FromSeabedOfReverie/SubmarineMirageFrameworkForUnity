//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Debug;


	public class FinalizeSMTask : SMTaskModifyData {
		[SMShowLine] public override SMModifyType _type => GetType( _task );
		[SMShowLine] SMTask _task;


		public FinalizeSMTask( SMTask task ) {
			_task = task;
		}


		public override async UniTask Run() {
			if ( _task._ranState != SMTaskRunState.FinalDisable )	{ return; }


			await _task._finalizeEvent.Run( _task._asyncCancelerOnDispose );
			_task._ranState = SMTaskRunState.Finalize;

			_task.Dispose();
		}
	}
}