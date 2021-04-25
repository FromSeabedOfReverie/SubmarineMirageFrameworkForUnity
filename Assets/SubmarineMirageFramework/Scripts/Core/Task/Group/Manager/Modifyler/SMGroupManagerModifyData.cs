//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using System.Collections.Generic;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMGroupManagerModifyData : SMTaskModifyData {
		[SMShowLine] public new SMGroupBody _target	{ get; private set; }


		public SMGroupManagerModifyData( SMGroupBody target )
			=> _target = target;


		protected override IEnumerable<SMTask> GetAllLowers() {
			var t = ( SMGroupManagerBody )base._target;
			return t.GetAllGroups();
		}
	}
}