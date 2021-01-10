//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Group.Manager.Modifyler {
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using Task.Modifyler;
	using Group;
	using Group.Modifyler;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMGroupManagerModifyData
		: BaseSMTaskModifyData<SMGroupManager, SMGroupManagerModifyler, SMGroupModifyData, SMGroup>
	{
		[SMShowLine] public SMGroup _target	{ get; private set; }


		public SMGroupManagerModifyData( SMGroup target )
			=> _target = target;


		protected override UniTask RegisterAndRunLower( SMGroup lowerTarget, SMGroupModifyData data )
			=> lowerTarget._modifyler.RegisterAndRun( data );

		protected override IEnumerable<SMGroup> GetAllLowers() => _owner.GetAllGroups();

		protected override bool IsTargetLower( SMGroup lowerTarget, SMTaskType type ) => true;
	}
}