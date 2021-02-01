//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler.Base {
	using Task.Base;
	using Extension;


	// TODO : コメント追加、整頓


	public class SMGroupManagerModifyler
		: BaseSMTaskModifyler<SMGroupManagerBody, SMGroupManagerModifyler, SMGroupManagerModifyData>
	{
		protected override SMTaskCanceler _asyncCanceler => _owner._asyncCancelerOnDisable;


		public SMGroupManagerModifyler( SMGroupManagerBody owner ) : base( owner ) {}


		public void Reregister( SMGroupManagerBody newOwner, SMGroupBody group ) {
			if ( group == null )	{ return; }
			_data.RemoveAll(
				d => d._target == group,
				d => newOwner._modifyler.Register( d )
			);
		}

		public void Unregister( SMGroupBody remove ) {
			if ( remove == null )	{ return; }
			_data.RemoveAll(
				d => d._target == remove,
				d => d.Dispose()
			);
		}
	}
}