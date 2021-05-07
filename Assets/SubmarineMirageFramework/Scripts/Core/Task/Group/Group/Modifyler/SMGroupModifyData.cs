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



	public abstract class SMGroupModifyData : SMTaskModifyData {
		public new SMGroupBody _target	{ get; private set; }
		[SMShowLine] public SMObjectBody _object { get; private set; }


		public SMGroupModifyData( SMObjectBody smObject )
			=> _object = smObject;

		public override void Set( ISMModifyTarget target, SMModifyler modifyler ) {
			base.Set( target, modifyler );


			_target = ( SMGroupBody )base._target;

			if ( _object == null )	{ _object = _target._objectBody; }
			if ( _object == null ) {
				throw new ObjectDisposedException( $"{nameof( _object )}", $"既に削除済\n{_object}" );
			}
		}


		protected override IEnumerable<SMTask> GetAllLowers()
			=> _object.GetAllChildren();
	}
}