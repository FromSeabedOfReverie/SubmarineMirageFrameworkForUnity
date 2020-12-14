//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using Cysharp.Threading.Tasks;
	using UTask;
	using Extension;


	// TODO : コメント追加、整頓


	public class InitializeBehaviourSMObject : SMObjectModifyData {
		SMMonoBehaviour _behaviour;


		public InitializeBehaviourSMObject( SMObject smObject, SMMonoBehaviour behaviour ) : base( smObject ) {
			_behaviour = behaviour;
			_type = ModifyType.Runner;
		}

		public override void Cancel() {}


		public override async UniTask Run() {
			switch ( _group._type ) {
				case SMTaskType.DontWork:
// TODO : 作成直後に実行するとエラーになる為、待機しているが、できれば不要にしたい
					await UTask.NextFrame( _behaviour._asyncCancelerOnDisable );
					await RunStateSMBehaviour.RegisterAndRun( _behaviour, SMTaskRunState.Create );
					return;
				case SMTaskType.Work:
				case SMTaskType.FirstWork:
					if ( _group._groups._isEnter ) {
// TODO : 作成直後に実行するとエラーになる為、待機しているが、できれば不要にしたい
						await UTask.NextFrame( _behaviour._asyncCancelerOnDisable );
						await RunStateSMBehaviour.RegisterAndRun( _behaviour, SMTaskRunState.Create );
						await RunStateSMBehaviour.RegisterAndRun( _behaviour, SMTaskRunState.SelfInitializing );
						await RunStateSMBehaviour.RegisterAndRun( _behaviour, SMTaskRunState.Initializing );
						await ChangeActiveSMBehaviour.RegisterAndRunInitial( _behaviour );
					}
					return;
			}
		}


		public override string ToString() => base.ToString().InsertLast( ", ",
			string.Join( ", ",
				_behaviour?.ToLineString()
			)
			+ ", "
		);
	}
}