//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestObjectModifyler
namespace SubmarineMirage.Task.Object.Modifyler {
	using Object;
	using Debug;



	// TODO : コメント追加、整頓



	public static class SMObjectApplyer {
		public static void LinkChild( SMObject parent, SMObject add ) {
#if TestObjectModifyler
			SMLog.Debug( $"{nameof( LinkChild )} : start" );
			SMLog.Debug( string.Join( "\n",
				$"{nameof( parent )} : {parent?.ToLineString()}",
				$"{nameof( add )} : {add?.ToLineString()}"
			) );
#endif
			add._parent = parent;
			var last = parent.GetLastChild();
#if TestObjectModifyler
			SMLog.Debug( $"{nameof( last )} : {last?.ToLineString()}" );
#endif
			if ( last != null ) {
				add._previous = last;
				last._next = add;
			} else {
				parent._child = add;
			}
#if TestObjectModifyler
			SMLog.Debug( string.Join( "\n",
				$"{nameof( parent )} : {parent?.ToLineString()}",
				$"{nameof( add )} : {add?.ToLineString()}",
				$"{nameof( last )} : {last?.ToLineString()}"
			) );
			SMLog.Debug( $"{nameof( LinkChild )} : end" );
#endif
		}


		public static void Unlink( SMObject smObject ) {
#if TestObjectModifyler
			SMLog.Debug( $"{nameof( Unlink )} : start" );
			var parent = smObject?._parent;
			var previous = smObject?._previous;
			var next = smObject?._next;
			SMLog.Debug( string.Join( "\n",
				$"{nameof( smObject )} : {smObject?.ToLineString()}",
				$"{nameof( parent )} : {parent?.ToLineString()}",
				$"{nameof( previous )} : {previous?.ToLineString()}",
				$"{nameof( next )} : {next?.ToLineString()}"
			) );
#endif
			if ( smObject._parent?._child == smObject ) {
				smObject._parent._child = smObject._next;
			}
			smObject._parent = null;

			if ( smObject._previous != null )	{ smObject._previous._next = smObject._next; }
			if ( smObject._next != null )		{ smObject._next._previous = smObject._previous; }
			smObject._previous = null;
			smObject._next = null;
#if TestObjectModifyler
			SMLog.Debug( string.Join( "\n",
				$"{nameof( smObject )} : {smObject?.ToLineString()}",
				$"{nameof( parent )} : {parent?.ToLineString()}",
				$"{nameof( previous )} : {previous?.ToLineString()}",
				$"{nameof( next )} : {next?.ToLineString()}"
			) );
			SMLog.Debug( $"{nameof( Unlink )} : end" );
#endif
		}
	}
}