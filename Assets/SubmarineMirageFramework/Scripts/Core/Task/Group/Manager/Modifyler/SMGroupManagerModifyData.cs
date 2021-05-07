//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using System.Collections.Generic;
	using SubmarineMirage.Modifyler;
	using Debug;



	public abstract class SMGroupManagerModifyData : SMTaskModifyData {
		public new SMGroupManagerBody _target { get; private set; }
		[SMShowLine] public SMGroupBody _group	{ get; private set; }


		public SMGroupManagerModifyData( SMGroupBody group )
			=> _group = group;


		public override void Set( ISMModifyTarget target, SMModifyler modifyler ) {
			base.Set( target, modifyler );

			_target = ( SMGroupManagerBody )base._target;
		}


		protected override IEnumerable<SMTask> GetAllLowers()
			=> _target.GetAllGroups();
	}
}