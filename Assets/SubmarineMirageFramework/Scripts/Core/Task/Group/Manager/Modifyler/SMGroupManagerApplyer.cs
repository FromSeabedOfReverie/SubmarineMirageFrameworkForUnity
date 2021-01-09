//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupManagerModifyler
namespace SubmarineMirage.Task.Group.Manager.Modifyler {
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Group.Modifyler;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public static class SMGroupManagerApplyer {
		public static readonly SMTaskRunAllType[] SEQUENTIAL_RUN_TYPES = new SMTaskRunAllType[] {
			SMTaskRunAllType.Sequential, SMTaskRunAllType.Parallel,
		};
		public static readonly SMTaskRunAllType[] REVERSE_SEQUENTIAL_RUN_TYPES = new SMTaskRunAllType[] {
			SMTaskRunAllType.Parallel, SMTaskRunAllType.ReverseSequential,
		};
		public static readonly SMTaskRunAllType[] ALL_RUN_TYPES = new SMTaskRunAllType[] {
			SMTaskRunAllType.Sequential, SMTaskRunAllType.Parallel, SMTaskRunAllType.DontRun,
		};
		public static readonly SMTaskType[] UPDATE_TASK_TYPES = new SMTaskType[] {
			SMTaskType.FirstWork, SMTaskType.Work,
		};
		public static readonly SMTaskType[] DISPOSE_TASK_TYPES = new SMTaskType[] {
			SMTaskType.Work, SMTaskType.FirstWork, SMTaskType.DontWork,
		};


		public static SMTaskType ToTaskType( SMTaskRunAllType type ) {
			switch ( type ) {
				case SMTaskRunAllType.Sequential:
				case SMTaskRunAllType.ReverseSequential:	return SMTaskType.FirstWork;
				case SMTaskRunAllType.Parallel:				return SMTaskType.Work;
				default:									return SMTaskType.DontWork;
			}
		}



		public static void DisposeAll( SMGroupManager manager ) {
			manager.GetAllGroups()
				.Reverse()
				.ForEach( g => SMGroupApplyer.DisposeAll( g ) );
			manager.Dispose();
		}



		public static void Link( SMGroupManager manager, SMGroup add ) {
#if TestGroupManagerModifyler
			SMLog.Debug( $"{nameof( Link )} : start" );
			SMLog.Debug( manager );	// 追加前を表示
#endif
			if ( manager._group == null ) {
				manager._group = add;
			} else {
				var last = manager._group.GetLast();
#if TestGroupManagerModifyler
				SMLog.Debug( string.Join( "\n",
					$"{nameof( last )} : {last?.ToLineString()}",
					$"{nameof( add )} : {add?.ToLineString()}"
				) );
#endif
				add._previous = last;
				last._next = add;
#if TestGroupManagerModifyler
				SMLog.Debug( string.Join( "\n",
					$"{nameof( last )} : {last?.ToLineString()}",
					$"{nameof( add )} : {add?.ToLineString()}"
				) );
#endif
			}
#if TestGroupManagerModifyler
			SMLog.Debug( manager );	// 追加後を表示
			SMLog.Debug( $"{nameof( Link )} : end" );
#endif
		}


		public static void Unlink( SMGroupManager manager, SMGroup remove ) {
			if ( manager._group == remove )	{ manager._group = remove._next; }
			if ( remove._previous != null )		{ remove._previous._next = remove._next; }
			if ( remove._next != null )			{ remove._next._previous = remove._previous; }
			remove._previous = null;
			remove._next = null;
		}



		public static async UniTask RegisterRunEventToOwner( SMGroupManager manager, SMGroup add ) {
			if (	manager._ranState >= SMTaskRunState.FinalDisable &&
					add._ranState < SMTaskRunState.FinalDisable
			) {
				foreach ( var t in REVERSE_SEQUENTIAL_RUN_TYPES ) {
					add._modifyler.Register( new FinalDisableSMGroup( t ) );
				}
			}

			if (	manager._ranState >= SMTaskRunState.Finalize &&
					add._ranState < SMTaskRunState.Finalize
			) {
				foreach ( var t in REVERSE_SEQUENTIAL_RUN_TYPES ) {
					add._modifyler.Register( new FinalizeSMGroup( t ) );
				}
			}

			if ( manager._isFinalizing )	{ return; }


			if (	manager._ranState >= SMTaskRunState.Create &&
					add._ranState < SMTaskRunState.Create
			) {
				// 非GameObjectの場合、生成直後だと、継承先コンストラクタ前に実行されてしまう為、1フレーム待機
				if ( !add._isGameObject ) {
// TODO : 無くす
					await UTask.NextFrame( add._asyncCanceler );
				}
				foreach ( var t in ALL_RUN_TYPES ) {
					add._modifyler.Register( new CreateSMGroup( t ) );
				}
			}

			if (	manager._ranState >= SMTaskRunState.SelfInitialize &&
					add._ranState < SMTaskRunState.SelfInitialize
			) {
				foreach ( var t in SEQUENTIAL_RUN_TYPES ) {
					add._modifyler.Register( new SelfInitializeSMGroup( t ) );
				}
			}

			if (	manager._ranState >= SMTaskRunState.Initialize &&
					add._ranState < SMTaskRunState.Initialize
			) {
				foreach ( var t in SEQUENTIAL_RUN_TYPES ) {
					add._modifyler.Register( new InitializeSMGroup( t ) );
				}
			}

			if (	manager._ranState >= SMTaskRunState.InitialEnable &&
					add._ranState < SMTaskRunState.InitialEnable
			) {
				foreach ( var t in SEQUENTIAL_RUN_TYPES ) {
					add._modifyler.Register( new InitialEnableSMGroup( t ) );
				}
			}


// TODO : 念の為、活動状態変更も設定する
		}



		public static void FixedUpdate( SMGroupManager manager ) {
			if ( !manager._isFinalizing )	{ return; }
			if ( !manager._isActive )		{ return; }
			if ( manager._ranState < SMTaskRunState.InitialEnable )	{ return; }

			var gs = manager.GetAllGroups().ToArray();
			UPDATE_TASK_TYPES.ForEach( t =>
				gs.ForEach( g => SMGroupApplyer.FixedUpdate( g, t ) )
			);

			if ( manager._ranState == SMTaskRunState.InitialEnable ) {
				manager._ranState = SMTaskRunState.FixedUpdate;
			}
		}


		public static void Update( SMGroupManager manager ) {
			if ( !manager._isFinalizing )	{ return; }
			if ( !manager._isActive )		{ return; }
			if ( manager._ranState < SMTaskRunState.FixedUpdate )	{ return; }

			var gs = manager.GetAllGroups().ToArray();
			UPDATE_TASK_TYPES.ForEach( t =>
				gs.ForEach( g => SMGroupApplyer.Update( g, t ) )
			);

			if ( manager._ranState == SMTaskRunState.FixedUpdate )	{ manager._ranState = SMTaskRunState.Update; }
		}


		public static void LateUpdate( SMGroupManager manager ) {
			if ( !manager._isFinalizing )	{ return; }
			if ( !manager._isActive )		{ return; }
			if ( manager._ranState < SMTaskRunState.Update )	{ return; }

			var gs = manager.GetAllGroups().ToArray();
			UPDATE_TASK_TYPES.ForEach( t =>
				gs.ForEach( g => SMGroupApplyer.LateUpdate( g, t ) )
			);

			if ( manager._ranState == SMTaskRunState.Update )	{ manager._ranState = SMTaskRunState.LateUpdate; }
		}
	}
}