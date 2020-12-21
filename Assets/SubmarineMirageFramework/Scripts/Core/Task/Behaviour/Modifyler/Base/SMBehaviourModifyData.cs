//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestBehaviourModifyler
namespace SubmarineMirage.Task.Behaviour.Modifyler {
	using System;
	using Task.Modifyler;


	// TODO : コメント追加、整頓


	public abstract class SMBehaviourModifyData
		: BaseSMTaskModifyData<SMBehaviourBody, SMBehaviourModifyler, SMBehaviourBody>
	{
		public SMBehaviourModifyData( SMBehaviourBody target ) : base( target ) {
			if ( _target == null || _target._isDispose ) {
				throw new ObjectDisposedException( $"{nameof( _target )}", $"既に解放、削除済\n{_target}" );
			}
		}
	}
}