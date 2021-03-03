//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene.Modifyler.Base {
	using Task.Modifyler.Base;
	using Scene.Base;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMSceneManagerModifyler
		: SMTaskModifyler<SMSceneManagerBody, SMSceneManagerModifyler, SMSceneManagerModifyData>
	{
		protected override SMAsyncCanceler _asyncCanceler => _target._asyncCancelerOnDispose;


		public SMSceneManagerModifyler( SMSceneManagerBody owner ) : base( owner ) {}
	}
}