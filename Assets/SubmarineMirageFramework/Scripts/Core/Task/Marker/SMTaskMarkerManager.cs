//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Marker {
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Task;
	using Base;
	using Service;
	using Extension;
	using Debug;



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

			_disposables.AddFirst( () => {
				SMTaskManager.DISPOSE_TASK_TYPES.ForEach( type => {
					GetAlls( type, true )
						.Reverse()
						.ForEach( task => task.Dispose() );
				} );
				_markers.Clear();
			} );
		}



		public async UniTask InitializeAll() {
			var manager = SMServiceLocator.Resolve<SMTaskManager>();

			SMTaskManager.CREATE_TASK_TYPES
				.SelectMany( type => GetAlls( type, true ) )
				.ForEach( task => manager.CreateTask( task ).Forget() );
			await manager._modifyler.WaitRunning();

			SMTaskManager.RUN_TASK_TYPES
				.SelectMany( type => GetAlls( type, true ) )
				.ForEach( task => manager.SelfInitializeTask( task ).Forget() );
			await manager._modifyler.WaitRunning();

			SMTaskManager.RUN_TASK_TYPES
				.SelectMany( type => GetAlls( type, true ) )
				.ForEach( task => manager.InitializeTask( task ).Forget() );
			await manager._modifyler.WaitRunning();

			SMTaskManager.RUN_TASK_TYPES
				.SelectMany( type => GetAlls( type, true ) )
				.ForEach( task => manager.InitialEnableTask( task ).Forget() );
			await manager._modifyler.WaitRunning();
		}

		public async UniTask FinalizeAll() {
			var manager = SMServiceLocator.Resolve<SMTaskManager>();

			SMTaskManager.DISPOSE_RUN_TASK_TYPES
				.SelectMany( type => GetAlls( type, true ) )
				.Reverse()
				.ForEach( task => manager.FinalDisableTask( task ).Forget() );
			await manager._modifyler.WaitRunning();

			SMTaskManager.DISPOSE_RUN_TASK_TYPES
				.SelectMany( type => GetAlls( type, true ) )
				.Reverse()
				.ForEach( task => manager.FinalizeTask( task ).Forget() );
			await manager._modifyler.WaitRunning();

			Dispose();
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
			if ( first == null )	{ yield break; }
			var last = GetLast( type, isRaw )._next;

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