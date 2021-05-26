//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Marker {
	using System.Linq;
	using System.Collections.Generic;
	using KoganeUnityLib;
	using Base;
	using Service;



	public class SMTaskMarkerManager : SMStandardBase {
		static readonly SMTaskMarkerType[] MARKER_TYPES = new SMTaskMarkerType[] {
			SMTaskMarkerType.First, SMTaskMarkerType.Last,
		};

		readonly Dictionary<SMTaskRunType, SMTask[]> _markers = new Dictionary<SMTaskRunType, SMTask[]>();



		public SMTaskMarkerManager( string name ) {
			SMTaskManager.CREATE_TASK_TYPES.ForEach( t => {
				_markers[t] = new SMTask[MARKER_TYPES.Length];
				MARKER_TYPES.ForEach( i => {
					_markers[t][( int )i] = new SMTaskMarker( name, t, i );
				} );
			} );

			Link();

			_disposables.AddLast( () => {
				SMTaskManager.DISPOSE_TASK_TYPES.ForEach( type => {
					GetAlls( type, true )
						.Reverse()
						.ToArray()
						.ForEach( task => task.Dispose() );
				} );
				_markers.Clear();
			} );
		}

		void Link() {
			var taskManager = SMServiceLocator.Resolve<SMTaskManager>();
			if ( taskManager == null )	{ return; }

			SMTaskManager.CREATE_TASK_TYPES.ForEach( t => {
				MARKER_TYPES.ForEach( i => {
					taskManager.Register( _markers[t][( int )i] );
				} );
			} );
		}



		public SMTask GetFirst( SMTaskRunType type, bool isRaw = false ) {
			if ( isRaw ) {
				return _markers[type][( int )SMTaskMarkerType.First];
			}

			var first = GetFirst( type, true )._next;
			var last = GetLast( type, true );

			if ( first == last )	{ return null; }
			return first;
		}

		public SMTask GetLast( SMTaskRunType type, bool isRaw = false ) {
			if ( isRaw ) {
				return _markers[type][( int )SMTaskMarkerType.Last];
			}

			var first = GetFirst( type, true );
			var last = GetLast( type, true )._previous;

			if ( first == last )	{ return null; }
			return last;
		}



		public IEnumerable<SMTask> GetAlls( SMTaskRunType type, bool isRaw = false ) {
			var first = GetFirst( type, isRaw );
			var last = GetLast( type, isRaw );
			for ( var t = first; t != last; t = t._next ) {
				yield return t;
			}
		}

		public IEnumerable<SMTask> GetAlls( bool isRaw = false ) {
			foreach ( var type in SMTaskManager.CREATE_TASK_TYPES ) {
				foreach ( var task in GetAlls( type, isRaw ) ) {
					yield return task;
				}
			}
		}
	}
}