//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Group.Modifyler {
	using System;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using Task.Modifyler;
	using Object;
	using Object.Modifyler;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMGroupModifyData
		: BaseSMTaskModifyData<SMGroup, SMGroupModifyler, SMObjectModifyData, SMObject>
	{
		[SMShowLine] public SMObject _target	{ get; private set; }


		public SMGroupModifyData( SMObject target )
			=> _target = target;

		public override void Set( SMGroup owner ) {
			base.Set( owner );
			if ( _target == null )	{ _target = _owner._topObject; }
			if ( _target == null ) {
				throw new ObjectDisposedException( $"{nameof( _target )}", $"既に削除済\n{_target}" );
			}
		}


		protected override UniTask RegisterAndRunLower( SMObject lowerTarget, SMObjectModifyData data )
			=> lowerTarget._modifyler.RegisterAndRun( data );

		protected override IEnumerable<SMObject> GetAllLowers() => _target.GetAllChildren();

		protected override bool IsTargetLower( SMObject lowerTarget, SMTaskType type ) => true;
	}
}