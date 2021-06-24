//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using SubmarineMirage.Modifyler;
	using Debug;



	public abstract class SMTaskModifyData : SMModifyData {
		public new SMTaskManager _target	{ get; private set; }
		[SMShowLine] protected SMTask _task	{ get; private set; }



		public SMTaskModifyData( SMTask task ) {
			_task = task;
		}

		public override void Set( ISMModifyTarget target, SMModifyler modifyler ) {
			base.Set( target, modifyler );
			_target = base._target as SMTaskManager;
		}



		protected SMModifyType GetType( SMTask task ) => (
			task._type == SMTaskRunType.Parallel	? SMModifyType.Parallel
													: SMModifyType.Last
		);
	}
}