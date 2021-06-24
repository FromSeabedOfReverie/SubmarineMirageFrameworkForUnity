//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestTaskManager
namespace SubmarineMirage.Task {
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
	using Debug;



	public class SMTaskManager : SMStandardBase, ISMModifyTarget, ISMService {
		public static readonly SMTaskRunType[] CREATE_TASK_TYPES = new SMTaskRunType[] {
			SMTaskRunType.Dont, SMTaskRunType.Sequential, SMTaskRunType.Parallel,
		};
		public static readonly SMTaskRunType[] RUN_TASK_TYPES = new SMTaskRunType[] {
			SMTaskRunType.Sequential, SMTaskRunType.Parallel,
		};
		public static readonly SMTaskRunType[] DISPOSE_RUN_TASK_TYPES = RUN_TASK_TYPES.DeepCopy().Reverse();
		public static readonly SMTaskRunType[] DISPOSE_TASK_TYPES = CREATE_TASK_TYPES.DeepCopy().Reverse();

		SMTask _root { get; set; }
		SMTaskMarkerManager _taskMarkers { get; set; }
		public SMModifyler _modifyler { get; private set; }

		public readonly SMSubject _fixedUpdateEvent = new SMSubject();
		public readonly SMSubject _updateEvent = new SMSubject();
		public readonly SMSubject _lateUpdateEvent = new SMSubject();
		public readonly SMSubject _onGUIEvent = new SMSubject();



		public SMTaskManager() {
			_modifyler = new SMModifyler( this, typeof( SMTaskModifyData ) );
//			_modifyler._isDebug = true;

			_taskMarkers = new SMTaskMarkerManager( this.GetAboutName() );
			var tasks = new SMTask[] {
				GetFirst( SMTaskRunType.Dont, true ),
#if TestTaskManager
				new Test.SMTestTask( "テスト", SMTaskRunType.Dont, false ),
#endif
				GetLast( SMTaskRunType.Dont, true ),
				GetFirst( SMTaskRunType.Sequential, true ),
				GetLast( SMTaskRunType.Sequential, true ),
				GetFirst( SMTaskRunType.Parallel, true ),
				GetLast( SMTaskRunType.Parallel, true ),
			};
			_root = tasks.First();
			SMTask last = null;
			tasks.ForEach( current => {
				current._taskManager = this;
				if ( last != null )	{ last.Link( current ); }
				last = current;
			} );

			_fixedUpdateEvent.AddLast().Subscribe( _ => {
				_modifyler._isLock.Value = true;
				RUN_TASK_TYPES.ForEach( type => {
					GetAlls( type ).ForEach( task => task.FixedUpdate() );
				} );
				_modifyler._isLock.Value = false;
			} );
			_updateEvent.AddLast().Subscribe( _ => {
				_modifyler._isLock.Value = true;
				RUN_TASK_TYPES.ForEach( type => {
					GetAlls( type ).ForEach( task => task.Update() );
				} );
				_modifyler._isLock.Value = false;
			} );
			_lateUpdateEvent.AddLast().Subscribe( _ => {
				_modifyler._isLock.Value = true;
				RUN_TASK_TYPES.ForEach( type => {
					GetAlls( type ).ForEach( task => task.LateUpdate() );
				} );
				_modifyler._isLock.Value = false;
			} );

			_disposables.AddFirst( () => {
				_taskMarkers.Dispose();
				_root = null;
				_modifyler.Dispose();

				_fixedUpdateEvent.Dispose();
				_updateEvent.Dispose();
				_lateUpdateEvent.Dispose();
				_onGUIEvent.Dispose();
			} );
			_disposables.AddFirst(
				Observable.EveryFixedUpdate()	.Subscribe( _ => _fixedUpdateEvent.Run() ),
				Observable.EveryUpdate()		.Subscribe( _ => _updateEvent.Run() ),
				Observable.EveryLateUpdate()	.Subscribe( _ => _lateUpdateEvent.Run() )
			);
			if ( SMDebugManager.IS_DEVELOP ) {
				_disposables.AddFirst(
					UniRxSMExtension.EveryOnGUI().Subscribe( _ => _onGUIEvent.Run() )
				);
			}
		}



		public UniTask Initialize()
			=> _taskMarkers.InitializeAll();

		public async UniTask Finalize() {
			await _taskMarkers.FinalizeAll();
			Dispose();
		}



		public void Register( SMTaskModifyData modifyData )
			=> _modifyler.Register( modifyData );

		public UniTask RegisterAndWaitRunning( SMTaskModifyData modifyData )
			=> _modifyler.RegisterAndWaitRunning( modifyData );



		public void Register( SMTask add, bool isAdjustRun ) {
			Register( new RegisterSMTask( add ) );
			if ( isAdjustRun ) {
				Register( new AdjustRunSMTask( add ) );
			}
		}

		public void Unregister( SMTask sub )
			=> Register( new UnregisterSMTask( sub ) );



		public SMTask GetFirst( SMTaskRunType type, bool isRaw = false )
			=> _taskMarkers.GetFirst( type, isRaw );

		public SMTask GetLast( SMTaskRunType type, bool isRaw = false )
			=> _taskMarkers.GetLast( type, isRaw );



		public IEnumerable<SMTask> GetAlls( SMTaskRunType type )
			=> _taskMarkers.GetAlls( type, true );

		public IEnumerable<SMTask> GetAlls() {
			for ( var t = _root; t != null; t = t._next ) {
				yield return t;
			}
		}
	}
}