//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler.Base {
	using Task.Base;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMObjectModifyler
		: BaseSMTaskModifyler<SMObjectBody, SMObjectModifyler, SMObjectModifyData>
	{
		protected override SMAsyncCanceler _asyncCanceler => _owner._asyncCanceler;


		public SMObjectModifyler( SMObjectBody owner ) : base( owner ) {}
	}
}