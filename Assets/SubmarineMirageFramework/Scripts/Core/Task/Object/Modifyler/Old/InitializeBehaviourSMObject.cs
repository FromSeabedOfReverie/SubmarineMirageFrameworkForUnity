//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Object.Modifyler {
	using Cysharp.Threading.Tasks;
	using Behaviour;
	using Behaviour.Modifyler;
	using Object;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class InitializeBehaviourSMObject : SMObjectModifyData {
		[SMShowLine] SMMonoBehaviour _behaviour	{ get; set; }


		public InitializeBehaviourSMObject( SMObject target, SMMonoBehaviour behaviour ) : base( target ) {
			_type = SMTaskModifyType.Runner;
			_behaviour = behaviour;
		}

		protected override void Cancel() {}


		public override async UniTask Run() {
			switch ( _owner._type ) {
				case SMTaskType.DontWork:
// TODO : 作成直後に実行するとエラーになる為、待機しているが、できれば不要にしたい
					await UTask.NextFrame( _behaviour._asyncCancelerOnDisable );
					await RunStateSMBehaviour.RegisterAndRun( _behaviour, SMTaskRunState.Create );
					return;
				case SMTaskType.Work:
				case SMTaskType.FirstWork:
					if ( _owner._groups._isEnter ) {
// TODO : 作成直後に実行するとエラーになる為、待機しているが、できれば不要にしたい
						await UTask.NextFrame( _behaviour._asyncCancelerOnDisable );
						await RunStateSMBehaviour.RegisterAndRun( _behaviour, SMTaskRunState.Create );
						await RunStateSMBehaviour.RegisterAndRun( _behaviour, SMTaskRunState.SelfInitialize );
						await RunStateSMBehaviour.RegisterAndRun( _behaviour, SMTaskRunState.Initialize );
						await ChangeActiveSMBehaviour.RegisterAndRunInitial( _behaviour );
					}
					return;
			}
		}
	}
}