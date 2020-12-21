//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Object.Modifyler {
	using KoganeUnityLib;
	using Task.Modifyler;
	using Object;
	using Group;
	using Extension;



	// TODO : コメント追加、整頓



	public class SMObjectModifyler
		: BaseSMTaskModifyler<SMGroup, SMObjectModifyler, SMObjectModifyData, SMObject>
	{
		protected override SMTaskCanceler _asyncCanceler => _owner._topObject._asyncCanceler;



		public SMObjectModifyler( SMGroup owner ) : base( owner ) {}



		public void Move( SMObjectModifyler remove ) {
			remove._data.ForEach( d => Register( d ) );
			remove._data.Clear();
			remove.Dispose();
		}

		public void Reregister( SMGroup newOwner ) => _data.RemoveAll(
			d => d._target._group == newOwner,
			d => newOwner._modifyler.Register( d )
		);
	}
}