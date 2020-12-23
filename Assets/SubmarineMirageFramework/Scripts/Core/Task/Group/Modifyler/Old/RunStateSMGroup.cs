//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupModifyler
namespace SubmarineMirage.Task.Group.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using Debug;



	// TODO : コメント追加、整頓



	public class RunStateSMGroup : SMGroupModifyData {
		[SMShowLine] SMTaskType _taskType	{ get; set; }
		[SMShowLine] SMTaskRunState _state	{ get; set; }


		public RunStateSMGroup( SMTaskType taskType, SMTaskRunState state ) : base( null ) {
			_type = SMTaskModifyType.Runner;
			_taskType = taskType;
			_state = state;

			switch ( _state ) {
				case SMTaskRunState.FixedUpdate:
				case SMTaskRunState.Update:
				case SMTaskRunState.LateUpdate:
					throw new ArgumentOutOfRangeException(
						$"{_state}",
						$"負荷軽減の為、静的関数 {nameof( RegisterAndRun )} 以外で、実行不可"
					);
				case SMTaskRunState.None:
				case SMTaskRunState.SelfInitialize:
				case SMTaskRunState.Initialize:
				case SMTaskRunState.Finalize:
					throw new ArgumentOutOfRangeException(
						$"{_state}", $"実行状態に、実行後の型を指定した為、実行不可" );
			}
		}

		protected override void Cancel() {}


		public override async UniTask Run() {
			var gs = _owner.GetAllGroups(
				_taskType, _taskType == SMTaskType.FirstWork && _state == SMTaskRunState.Finalize );

			switch ( _taskType ) {
				case SMTaskType.FirstWork:
					foreach ( var g in gs ) {
						await g.RunStateEvent( _state );
					}
					return;

				case SMTaskType.Work:
					await gs.Select( g => g.RunStateEvent( _state ) );
					return;
			}
		}


		public static void RegisterAndRun( SMGroupManager owner, SMTaskType taskType, SMTaskRunState state ) {
			switch ( state ) {
				// 駄目元で、即時実行
				case SMTaskRunState.FixedUpdate:
				case SMTaskRunState.Update:
				case SMTaskRunState.LateUpdate:
					var gs = owner.GetAllGroups( taskType );
					foreach ( var g in gs ) {
						g.RunStateEvent( state, false ).Forget();
					}
					return;

// TODO : キューに貯まっている他のタスクを無視して、即実行したいけど、逐次実行でも問題ない？
//				case SMTaskRunState.Finalizing:
//					return;

				default:
					owner._modifyler.Register( new RunStateSMGroup( taskType, state ) );
					return;
			}
		}
	}
}