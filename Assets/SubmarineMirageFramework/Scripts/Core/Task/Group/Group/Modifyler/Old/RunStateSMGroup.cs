//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Group.Modifyler {
	using System;
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using Behaviour.Modifyler;
	using Object;
	using Group;
	using Debug;


	// TODO : コメント追加、整頓


	public class RunStateSMGroup : SMGroupModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;
		[SMShowLine] SMTaskRunState _state	{ get; set; }


		public RunStateSMGroup( SMObject target, SMTaskRunState state ) : base( target ) {
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
			switch ( _owner._type ) {
				case SMTaskType.FirstWork:
					if ( _state != SMTaskRunState.Finalize )	{ await SequentialRun( _target, _state ); }
					else										{ await ReverseRun( _target, _state ); }
					return;
				case SMTaskType.Work:
					await ParallelRun( _target, _state );
					return;
				case SMTaskType.DontWork:
					if ( _state != SMTaskRunState.Create )	{ return; }
					SyncRun( _target, SMTaskRunState.Create );
					return;
			}
		}

		async UniTask SequentialRun( SMObject smObject, SMTaskRunState state ) {
			foreach ( var b in smObject.GetBehaviours() )	{ await RunStateSMBehaviour.RegisterAndRun( b, state ); }
			foreach ( var o in smObject.GetChildren() )		{ await SequentialRun( o, state ); }
		}

		async UniTask ReverseRun( SMObject smObject, SMTaskRunState state ) {
			foreach ( var o in smObject.GetChildren().Reverse() )	{ await ReverseRun( o, state ); }
			foreach ( var b in smObject.GetBehaviours().Reverse() )
															{ await RunStateSMBehaviour.RegisterAndRun( b, state ); }
		}

		async UniTask ParallelRun( SMObject smObject, SMTaskRunState state ) {
			await Enumerable.Empty<UniTask>()
				.Concat( smObject.GetBehaviours().Select( b => RunStateSMBehaviour.RegisterAndRun( b, state ) ) )
				.Concat( smObject.GetChildren().Select( o => ParallelRun( o, state ) ) );
		}

		static void SyncRun( SMObject smObject, SMTaskRunState state ) {
			foreach ( var b in smObject.GetBehaviours() )
														{ RunStateSMBehaviour.RegisterAndRun( b, state ).Forget(); }
			foreach ( var o in smObject.GetChildren() )	{ SyncRun( o, state ); }
		}



		public static void RegisterAndRun( SMObject smObject, SMTaskRunState state ) {
			switch ( state ) {
				// 駄目元で、即時実行
				case SMTaskRunState.FixedUpdate:
				case SMTaskRunState.Update:
				case SMTaskRunState.LateUpdate:
					SyncRun( smObject, state );
					return;
// TODO : キューに貯まっている他のタスクを無視して、即実行したいけど、逐次実行でも問題ない？
//				case SMTaskRunState.Finalizing:
//					return;
				default:
					smObject._group._modifyler.Register( new RunStateSMGroup( smObject, state ) );
					return;
			}
		}

		public static void RegisterAndRun( SMGroup group, SMTaskRunState state )
			=> RegisterAndRun( group._topObject, state );
	}
}