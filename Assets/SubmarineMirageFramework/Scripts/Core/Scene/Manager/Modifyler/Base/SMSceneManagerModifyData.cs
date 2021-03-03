//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene.Modifyler.Base {
	using System;
	using System.Collections.Generic;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Task;
	using Task.Modifyler.Base;
	using Scene.Base;



	// TODO : コメント追加、整頓



	public abstract class SMSceneManagerModifyData
		: SMTaskModifyData<SMSceneManagerBody, SMSceneManagerModifyler, Unit, Unit>
	{
		protected override UniTask RegisterAndRunLower( Unit lowerTarget, Unit data )
			=> throw new InvalidOperationException(
				$"非対応 : {nameof( SMSceneManagerModifyData )}.{nameof( RegisterAndRunLower )}" );

		protected override IEnumerable<Unit> GetAllLowers()
			=> throw new InvalidOperationException(
				$"非対応 : {nameof( SMSceneManagerModifyData )}.{nameof( GetAllLowers )}" );

		protected override bool IsTargetLower( Unit lowerTarget, SMTaskType type )
			=> throw new InvalidOperationException(
				$"非対応 : {nameof( SMSceneManagerModifyData )}.{nameof( IsTargetLower )}" );
	}
}