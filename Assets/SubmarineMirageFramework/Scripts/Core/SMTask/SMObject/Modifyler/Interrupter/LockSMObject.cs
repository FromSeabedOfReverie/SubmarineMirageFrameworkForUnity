//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using UTask;
	using Debug;


	// TODO : コメント追加、整頓


	public class LockSMObject : SMObjectModifyData {
		public override ModifyType _type => ModifyType.Interrupter;
		public readonly UTaskCanceler _canceler = new UTaskCanceler();
		public bool _isRunning	{ get; private set; }
		public bool _isCancel	{ get; private set; }


		public LockSMObject( SMObject smObject ) : base( smObject ) {
			if ( !_object._isTop ) {
				throw new NotSupportedException(
					$"最上階の{nameof( SMObject )}で無い為、登録解除不可 :\n{_object}" );
			}
		}

		public override void Cancel() {
			_canceler.Cancel();
			_isCancel = true;
		}

		public override async UniTask Run() {
			try {
				_isRunning = true;
				await UTask.Never( _canceler );
			} finally {
				_isRunning = false;
			}
		}
	}
}