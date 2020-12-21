//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Object.Modifyler {
	using System;
	using Task.Modifyler;
	using Object;
	using Group;


	// TODO : コメント追加、整頓


	public abstract class SMObjectModifyData : BaseSMTaskModifyData<SMGroup, SMObjectModifyler, SMObject> {
		public SMObjectModifyData( SMObject target ) : base( target ) {
			if ( _target != null && _target._isDispose ) {
				throw new ObjectDisposedException( $"{nameof( _target )}", $"既に解放、削除済\n{_target}" );
			}
		}
	}
}