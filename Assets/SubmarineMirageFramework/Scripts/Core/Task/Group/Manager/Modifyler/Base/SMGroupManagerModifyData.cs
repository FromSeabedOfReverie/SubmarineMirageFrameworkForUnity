//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler.Base {
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using Task.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMGroupManagerModifyData
		: BaseSMTaskModifyData<SMGroupManagerBody, SMGroupManagerModifyler, SMGroupModifyData, SMGroupBody>
	{
		[SMShowLine] public SMGroupBody _target	{ get; private set; }


		public SMGroupManagerModifyData( SMGroupBody target )
			=> _target = target;


		protected override UniTask RegisterAndRunLower( SMGroupBody lowerTarget, SMGroupModifyData data )
			=> lowerTarget._modifyler.RegisterAndRun( data );

		protected override IEnumerable<SMGroupBody> GetAllLowers() => _owner.GetAllGroups();

		protected override bool IsTargetLower( SMGroupBody lowerTarget, SMTaskType type ) => true;
	}
}