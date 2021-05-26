//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using System;
	using SubmarineMirage.Modifyler;
	using Debug;



	public abstract class SMTaskModifyData : SMModifyData {
		public new SMTaskManager _target { get; private set; }



		public override void Set( ISMModifyTarget target, SMModifyler modifyler ) {
			base.Set( target, modifyler );

			_target = base._target as SMTaskManager;

/*
			if ( _target == null ) {
				throw new ObjectDisposedException( $"{nameof( _target )}", $"既に削除済\n{_target}" );
			}
*/
		}

		protected SMModifyType GetType( SMTask task )
			=> (
				task._type == SMTaskRunType.Parallel	? SMModifyType.ParallellRunner
														: SMModifyType.Runner
			);
	}
}