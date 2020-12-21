//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Group.Modifyler {
	using Task.Modifyler;



	// TODO : コメント追加、整頓



	public abstract class SMGroupModifyData
		: BaseSMTaskModifyData<SMGroupManager, SMGroupModifyler, SMGroup>
	{
		public SMGroupModifyData( SMGroup target ) : base( target ) {}
	}
}