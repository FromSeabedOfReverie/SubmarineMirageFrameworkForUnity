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



	public abstract class SMObjectModifyData : SMTaskModifyData {
		public new SMObjectBody _target { get; private set; }
		[SMShowLine] protected SMTaskRunAllType _runType	{ get; private set; }


		public SMObjectModifyData( SMTaskRunAllType runType )
			=> _runType = runType;

		public override void Set( ISMModifyTarget target, SMModifyler modifyler ) {
			base.Set( target, modifyler );

			_target = ( SMObjectBody )base._target;
			if ( _target == null ) {
				throw new ObjectDisposedException( $"{nameof( _target )}", $"既に削除済\n{_target}" );
			}
		}


		protected override IEnumerable<SMTask> GetAllLowers()
			=> _target._behaviourBody.GetBrothers();

		protected override bool IsTargetLower( SMTask lowerTask, SMTaskType type ) {
			var task = ( SMBehaviourBody )lowerTask;
			return task._type == type;
		}
	}
}