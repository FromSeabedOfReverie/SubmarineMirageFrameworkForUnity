//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Group.Modifyler {
	using Task.Modifyler;
	using Extension;


	// TODO : コメント追加、整頓


	public class SMGroupModifyler
		: BaseSMTaskModifyler<SMGroupManager, SMGroupModifyler, SMGroupModifyData, SMGroup>
	{
		protected override SMTaskCanceler _asyncCanceler => _owner._asyncCancelerOnDisable;


		public SMGroupModifyler( SMGroupManager owner ) : base( owner ) {}


		public void Reregister( SMGroupManager newOwner, SMGroup group ) => _data.RemoveAll(
			d => d._target == group,
			d => newOwner._modifyler.Register( d )
		);
	}
}