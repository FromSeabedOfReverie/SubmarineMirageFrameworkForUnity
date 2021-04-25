//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using System;
	using System.Collections.Generic;
	using SubmarineMirage.Modifyler;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMGroupModifyData : SMTaskModifyData {
		[SMShowLine] public new SMObjectBody _target	{ get; private set; }


		public SMGroupModifyData( SMObjectBody target )
			=> _target = target;

		public override void Set( ISMModifyTarget target, SMModifyler modifyler ) {
			base.Set( target, modifyler );

			if ( _target == null ) {
				var t = ( SMGroupBody )base._target;
				_target = t._objectBody;
			}
			if ( _target == null ) {
				throw new ObjectDisposedException( $"{nameof( _target )}", $"既に削除済\n{_target}" );
			}
		}


		protected override IEnumerable<SMTask> GetAllLowers()
			=> _target.GetAllChildren();
	}
}