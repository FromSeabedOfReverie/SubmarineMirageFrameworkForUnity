//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler.Base {
	using System;
	using System.Collections.Generic;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Task.Base;



	// TODO : コメント追加、整頓



	public abstract class SMBehaviourModifyData
		: SMTaskModifyData<SMBehaviourBody, SMBehaviourModifyler, Unit, Unit>
	{
		public override void Set( SMBehaviourBody owner ) {
			base.Set( owner );
			if ( _target == null ) {
				throw new ObjectDisposedException( $"{nameof( _target )}", $"既に削除済\n{_target}" );
			}
		}


		protected override UniTask RegisterAndRunLower( Unit lowerTarget, Unit data )
			=> throw new InvalidOperationException(
				$"非対応 : {nameof( SMBehaviourModifyData )}.{nameof( RegisterAndRunLower )}" );

		protected override IEnumerable<Unit> GetAllLowers()
			=> throw new InvalidOperationException(
				$"非対応 : {nameof( SMBehaviourModifyData )}.{nameof( GetAllLowers )}" );

		protected override bool IsTargetLower( Unit lowerTarget, SMTaskType type )
			=> throw new InvalidOperationException(
				$"非対応 : {nameof( SMBehaviourModifyData )}.{nameof( IsTargetLower )}" );
	}
}