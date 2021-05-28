//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;
	using Base;
	using Event;
	using SubmarineMirage.Modifyler;
	using Modifyler;
	using Marker;
	using Service;
	using Extension;
	using Utility;



	public class SMTaskManager : SMStandardBase, ISMModifyTarget, ISMService {
		public static readonly SMTaskRunType[] CREATE_TASK_TYPES = new SMTaskRunType[] {
			SMTaskRunType.Dont, SMTaskRunType.Sequential, SMTaskRunType.Parallel,
		};
		public static readonly SMTaskRunType[] UPDATE_TASK_TYPES = new SMTaskRunType[] {
			SMTaskRunType.Sequential, SMTaskRunType.Parallel,
		};
		public static readonly SMTaskRunType[] DISPOSE_UPDATE_TASK_TYPES = UPDATE_TASK_TYPES.Reverse();
		public static readonly SMTaskRunType[] DISPOSE_TASK_TYPES = CREATE_TASK_TYPES.Reverse();

		SMTask _root;
		SMTaskMarkerManager _taskMarkers;
		public SMModifyler _modifyler { get; private set; }
		public readonly ReactiveProperty<bool> _isUpdating = new ReactiveProperty<bool>();

		readonly SMSubject _fixedUpdateEvent = new SMSubject();
		readonly SMSubject _updateEvent = new SMSubject();
		readonly SMSubject _lateUpdateEvent = new SMSubject();
		public readonly SMSubject _onGUIEvent = new SMSubject();



		public SMTaskManager() {
			_taskMarkers = new SMTaskMarkerManager( this.GetAboutName() );
			var tasks = new SMTask[] {
				GetFirst( SMTaskRunType.Dont, true ),
				GetLast( SMTaskRunType.Dont, true ),
				GetFirst( SMTaskRunType.Sequential, true ),
				GetLast( SMTaskRunType.Sequential, true ),
				GetFirst( SMTaskRunType.Parallel, true ),
				GetLast( SMTaskRunType.Parallel, true ),
			};
			SMTask last = null;
			tasks.ForEach( current => {
				if ( last != null ) { last.Link( current ); }
				last = current;
			} );

			_root = GetFirst( SMTaskRunType.Dont, true );
			_modifyler = new SMModifyler( this, typeof( SMTaskModifyData ) );
/*
			_modifyler._isCanRunEvent = () => !_isUpdating.Value;
			_disposables.AddLast(
				_isUpdating
					.Where( b => !b )
					.Subscribe( _ => _modifyler.Run().Forget() )
			);
*/

			_fixedUpdateEvent.AddLast().Subscribe( _ => {
				_isUpdating.Value = true;
				UPDATE_TASK_TYPES.ForEach( t => {
					GetAlls( t ).ForEach( t => t.FixedUpdate() );
				} );
				_isUpdating.Value = false;
			} );

			_updateEvent.AddLast().Subscribe( _ => {
				_isUpdating.Value = true;
				UPDATE_TASK_TYPES.ForEach( t => {
					GetAlls( t ).ForEach( t => t.Update() );
				} );
				_isUpdating.Value = false;
			} );

			_lateUpdateEvent.AddLast().Subscribe( _ => {
				_isUpdating.Value = true;
				UPDATE_TASK_TYPES.ForEach( t => {
					GetAlls( t ).ForEach( t => t.LateUpdate() );
				} );
				_isUpdating.Value = false;
			} );

			_disposables.AddLast(
				Observable.EveryFixedUpdate()	.Subscribe( _ => _fixedUpdateEvent.Run() ),
				Observable.EveryUpdate()		.Subscribe( _ => _updateEvent.Run() ),
				Observable.EveryLateUpdate()	.Subscribe( _ => _lateUpdateEvent.Run() )
#if DEVELOP
				,
				UniRxSMExtension.EveryOnGUI()	.Subscribe( _ => _onGUIEvent.Run() )
#endif
			);

			_disposables.AddLast( () => {
				_modifyler.Dispose();
				_isUpdating.Dispose();
				_taskMarkers.Dispose();
				_root = null;

				_fixedUpdateEvent.Dispose();
				_updateEvent.Dispose();
				_lateUpdateEvent.Dispose();
				_onGUIEvent.Dispose();
			} );
		}



		public UniTask Initialize()
			=> _taskMarkers.InitializeAll();

		public async UniTask Finalize() {
			await _taskMarkers.FinalizeAll();
			Dispose();
		}



		public void Register( SMTask add, bool isAdjustRun ) {
			Register( new RegisterSMTask( add ) );
			if ( isAdjustRun ) {
				Register( new AdjustRunSMTask( add ) );
			}
		}

		public void Unregister( SMTask sub )
			=> Register( new UnregisterSMTask( sub ) );

		public void Register( SMTaskModifyData modifyData )
			=> _modifyler.Register( modifyData );



		public SMTask GetFirst( SMTaskRunType type, bool isRaw = false )
			=> _taskMarkers.GetFirst( type, isRaw );

		public SMTask GetLast( SMTaskRunType type, bool isRaw = false )
			=> _taskMarkers.GetLast( type, isRaw );



		IEnumerable<SMTask> GetAlls( SMTaskRunType type )
			=> _taskMarkers.GetAlls( type, true );

		IEnumerable<SMTask> GetAlls() {
			for ( var t = _root; t != null; t = t._next ) {
				yield return t;
			}
		}
	}
}