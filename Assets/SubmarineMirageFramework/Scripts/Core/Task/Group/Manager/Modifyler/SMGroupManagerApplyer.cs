//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupManagerModifyler
namespace SubmarineMirage.Task.Group.Manager.Modifyler {
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
		public static readonly SMTaskType[] DISPOSE_TASK_TYPES = new SMTaskType[] {
			SMTaskType.Work, SMTaskType.FirstWork, SMTaskType.DontWork,
		};



		public static void Link( SMGroupManager owner, SMGroup add ) {
#if TestGroupManagerModifyler
			SMLog.Debug( $"{nameof( Link )} : start" );
			SMLog.Debug( owner );	// 追加前を表示
#endif
			if ( owner._topGroup == null ) {
				owner._topGroup = add;
			} else {
				var last = owner._topGroup.GetLast();
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
			SMLog.Debug( owner );	// 追加後を表示
			SMLog.Debug( $"{nameof( Link )} : end" );
#endif
		}


		public static void Unlink( SMGroupManager owner, SMGroup remove ) {
			if ( owner._topGroup == remove )	{ owner._topGroup = remove._next; }
			if ( remove._previous != null )		{ remove._previous._next = remove._next; }
			if ( remove._next != null )			{ remove._next._previous = remove._previous; }
			remove._previous = null;
			remove._next = null;
		}
	}
}