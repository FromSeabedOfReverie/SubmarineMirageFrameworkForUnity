//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler.Base {
	using System;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using Task.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMObjectModifyData
		: BaseSMTaskModifyData<SMObjectBody, SMObjectModifyler, SMBehaviourModifyData, SMBehaviourBody>
	{
		[SMShowLine] protected SMTaskRunAllType _runType	{ get; private set; }


		public SMObjectModifyData( SMTaskRunAllType runType )
			=> _runType = runType;

		public override void Set( SMObjectBody owner ) {
			base.Set( owner );
			if ( _owner == null ) {
				throw new ObjectDisposedException( $"{nameof( _owner )}", $"既に削除済\n{_owner}" );
			}
		}


		protected override UniTask RegisterAndRunLower( SMBehaviourBody lowerTarget, SMBehaviourModifyData data )
			=> lowerTarget._modifyler.RegisterAndRun( data );

		protected override IEnumerable<SMBehaviourBody> GetAllLowers()
			=> _owner._behaviourBody.GetBrothers();

		protected override bool IsTargetLower( SMBehaviourBody lowerTarget, SMTaskType type )
			=> lowerTarget._type == type;
	}
}