//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler.Base {
	using KoganeUnityLib;
	using Task.Base;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMGroupModifyler
		: SMTaskModifyler<SMGroupBody, SMGroupModifyler, SMGroupModifyData>
	{
		protected override SMAsyncCanceler _asyncCanceler => _target._asyncCanceler;



		public SMGroupModifyler( SMGroupBody owner ) : base( owner ) {}



		public void Move( SMGroupModifyler remove ) {
			remove._data.ForEach( d => Register( d ) );
			remove._data.Clear();
			remove.Dispose();
		}

		public void Reregister( SMGroupBody newOwner ) => _data.RemoveAll(
			d => d._target._groupBody == newOwner,
			d => newOwner._modifyler.Register( d )
		);

		public void Unregister( SMObjectBody remove ) => _data.RemoveAll(
			d => d._target == remove,
			d => d.Dispose()
		);
	}
}