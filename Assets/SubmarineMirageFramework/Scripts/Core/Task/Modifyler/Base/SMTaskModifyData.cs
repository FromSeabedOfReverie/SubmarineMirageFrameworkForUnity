//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using System;
	using UniRx;
	using SubmarineMirage.Modifyler;
	using Debug;



	public abstract class SMTaskModifyData : SMModifyData {
		public new SMTaskManager _target { get; private set; }
		IDisposable _notUpdateObservable;



		public override void Set( ISMModifyTarget target, SMModifyler modifyler ) {
			base.Set( target, modifyler );

			_target = base._target as SMTaskManager;
		}

		public override void Dispose() {
			base.Dispose();

			_notUpdateObservable?.Dispose();
			_notUpdateObservable = null;
		}



		protected SMModifyType GetType( SMTask task ) => (
			task._type == SMTaskRunType.Parallel	? SMModifyType.ParallellRunner
													: SMModifyType.Runner
		);

// TODO : リンク変更時に、下記関数を呼ぶ
		protected void RunWhenNotUpdate( Action runEvent ) {
			if ( !_target._isUpdating.Value ) {
				runEvent.Invoke();
				return;
			}

			_notUpdateObservable = _target._isUpdating
				.First( b => !b )
				.Subscribe( _ => {
					runEvent.Invoke();
					_notUpdateObservable?.Dispose();
					_notUpdateObservable = null;
				} );
		}
	}
}