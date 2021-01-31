//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestBehaviourModifyler
namespace SubmarineMirage.Task.Modifyler.Base {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Task.Base;
	using Utility;



	// TODO : コメント追加、整頓



	public abstract class SMBehaviourModifyData
		: BaseSMTaskModifyData<SMBehaviourBody, SMBehaviourModifyler, Unit, Unit>
	{
		public override void Set( SMBehaviourBody owner ) {
			base.Set( owner );
			if ( _owner == null ) {
				throw new ObjectDisposedException( $"{nameof( _owner )}", $"既に削除済\n{_owner}" );
			}
		}


		protected override UniTask RegisterAndRunLower( Unit lowerTarget, Unit data ) => UTask.DontWait();

		protected override IEnumerable<Unit> GetAllLowers() => Enumerable.Empty<Unit>();

		protected override bool IsTargetLower( Unit lowerTarget, SMTaskType type ) => false;
	}
}