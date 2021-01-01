//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Object.Modifyler {
	using Task.Modifyler;



	// TODO : コメント追加、整頓



	public class SMObjectModifyler
		: BaseSMTaskModifyler<SMObject, SMObjectModifyler, SMObjectModifyData>
	{
		protected override SMTaskCanceler _asyncCanceler => _owner._asyncCanceler;


		public SMObjectModifyler( SMObject owner ) : base( owner ) {}
	}
}