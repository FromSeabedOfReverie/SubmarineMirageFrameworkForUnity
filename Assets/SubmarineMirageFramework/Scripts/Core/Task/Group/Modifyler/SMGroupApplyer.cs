//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupModifyler
namespace SubmarineMirage.Task.Group.Modifyler {
	using Debug;



	// TODO : コメント追加、整頓



	public static class SMGroupApplyer {
		public static void Link( SMGroupManager owner, SMGroup add ) {
#if TestGroupModifyler
			SMLog.Debug( $"{nameof( Link )} : start" );
			SMLog.Debug( owner );	// 追加前を表示
#endif
			var first = owner._groups[add._type];
			if ( first == null ) {
				owner._groups[add._type] = add;
			} else {
				var last = first.GetLast();
#if TestGroupModifyler
				SMLog.Debug( string.Join( "\n",
					$"{nameof( last )} : {last?.ToLineString()}",
					$"{nameof( add )} : {add?.ToLineString()}"
				) );
#endif
				add._previous = last;
				last._next = add;
#if TestGroupModifyler
				SMLog.Debug( string.Join( "\n",
					$"{nameof( last )} : {last?.ToLineString()}",
					$"{nameof( add )} : {add?.ToLineString()}"
				) );
#endif
			}
#if TestGroupModifyler
			SMLog.Debug( owner );	// 追加後を表示
			SMLog.Debug( $"{nameof( Link )} : end" );
#endif
		}


		public static void Unlink( SMGroupManager owner, SMGroup remove, SMTaskType lastType ) {
			if ( owner._groups[lastType] == remove ) {
				owner._groups[lastType] = remove._next;
			}
			if ( remove._previous != null )		{ remove._previous._next = remove._next; }
			if ( remove._next != null )			{ remove._next._previous = remove._previous; }
			remove._previous = null;
			remove._next = null;
		}


		public static void Unlink( SMGroupManager owner, SMGroup remove )
			=> Unlink( owner, remove, remove._type );
	}
}