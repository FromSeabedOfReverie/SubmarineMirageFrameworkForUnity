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



	public abstract class SMGroupModifyData
		: SMTaskModifyData<SMGroupBody, SMGroupModifyler, SMObjectModifyData, SMObjectBody>
	{
		[SMShowLine] public SMObjectBody _target	{ get; private set; }


		public SMGroupModifyData( SMObjectBody target )
			=> _target = target;

		public override void Set( SMGroupBody owner ) {
			base.Set( owner );
			if ( _target == null )	{ _target = base._target._objectBody; }
			if ( _target == null ) {
				throw new ObjectDisposedException( $"{nameof( _target )}", $"既に削除済\n{_target}" );
			}
		}


		protected override UniTask RegisterAndRunLower( SMObjectBody lowerTarget, SMObjectModifyData data )
			=> lowerTarget._modifyler.RegisterAndRun( data );

		protected override IEnumerable<SMObjectBody> GetAllLowers() => _target.GetAllChildren();

		protected override bool IsTargetLower( SMObjectBody lowerTarget, SMTaskType type ) => true;
	}
}