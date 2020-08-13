//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestSMTask
namespace SubmarineMirage.SMTask {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using MultiEvent;
	using UTask;
	using Modifyler;
	using Scene;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMObjectManager : IDisposableExtension {
		static readonly SMTaskType[] GET_ALL_TOPS_TASK_TYPES =
			new SMTaskType[] { SMTaskType.FirstWork, SMTaskType.Work, SMTaskType.DontWork };
		static readonly SMTaskType[] DISPOSE_TASK_TYPES =
			new SMTaskType[] { SMTaskType.Work, SMTaskType.FirstWork, SMTaskType.DontWork };

		public BaseScene _owner	{ get; private set; }
		public readonly Dictionary<SMTaskType, SMObject> _objects = new Dictionary<SMTaskType, SMObject>();
		public bool _isEnter	{ get; private set; }

		public UTaskCanceler _activeAsyncCanceler => _owner._activeAsyncCanceler;

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public SMObjectManager( BaseScene owner ) {
			_owner = owner;

			EnumUtils.GetValues<SMTaskType>().ForEach( t => _objects[t] = null );
			_disposables.AddLast( () => {
				DISPOSE_TASK_TYPES
					.Select( t => _objects.GetOrDefault( t ) )
					.Where( o => o != null )
					.SelectMany( o => o.GetBrothers().Reverse() )
					.Where( o => o != null )
					.ToArray()
					.ForEach( o => o.Dispose() );
				_objects.Clear();
			} );
		}

		public void Dispose() => _disposables.Dispose();

		~SMObjectManager() => Dispose();



		public IEnumerable<SMObject> GetAllTops( SMTaskType? type = null, bool isReverse = false ) {
			IEnumerable<SMObject> result = null;
			if ( type.HasValue ) {
				result = _objects[type.Value]?.GetBrothers() ?? Enumerable.Empty<SMObject>();
			} else {
				result = GET_ALL_TOPS_TASK_TYPES
					.Select( t => _objects.GetOrDefault( t ) )
					.Where( o => o != null )
					.SelectMany( o => o.GetBrothers() );
			}
			if ( isReverse ) {
#if TestSMTask
				Log.Debug( $"Start Reverse :\n{string.Join( "\n", result.Select( o => o.ToLineString() ) )}" );
#endif
				result = result.Reverse();
#if TestSMTask
				Log.Debug( $"End Reverse :\n{string.Join( "\n", result.Select( o => o.ToLineString() ) )}" );
				Log.Debug(
					$"Don't Reverse :\n{string.Join( "\n", GetAllTops( type ).Select( o => o.ToLineString() ) )}" );
#endif
			}
			return result;
		}


		public T GetBehaviour<T>( SMTaskType? taskType = null ) where T : ISMBehaviour
			=> GetBehaviours<T>( taskType )
				.FirstOrDefault();

		public ISMBehaviour GetBehaviour( Type type, SMTaskType? taskType = null )
			=> GetBehaviours( type, taskType )
				.FirstOrDefault();

		public IEnumerable<T> GetBehaviours<T>( SMTaskType? taskType = null ) where T : ISMBehaviour
			=> GetBehaviours( typeof( T ), taskType )
				.Select( b => (T)b );

		public IEnumerable<ISMBehaviour> GetBehaviours( Type type, SMTaskType? taskType = null ) {
			var currents = new Queue<SMObject>( GetAllTops( taskType ) );
			while ( !currents.IsEmpty() ) {
				var o = currents.Dequeue();
				foreach ( var b in o.GetBehaviours( type ) ) {
					yield return b;
				}
				o.GetChildren().ForEach( c => currents.Enqueue( c ) );
			}
		}



		public async UniTask RunAllStateEvents( SMTaskType type, SMTaskRanState state ) {
			var os = GetAllTops( type, type == SMTaskType.FirstWork && state == SMTaskRanState.Finalizing );
			switch ( type ) {
				case SMTaskType.FirstWork:
					os.ForEach( o => RunStateSMObject.RunOrRegister( o, state ) );
					foreach ( var o in os ) {
						await o._modifyler.WaitRunning();
					}
					break;

				case SMTaskType.Work:
					os.ForEach( o => RunStateSMObject.RunOrRegister( o, state ) );
					await os.Select( o => o._modifyler.WaitRunning() );
					break;
			}
		}

		public async UniTask ChangeAllActives( SMTaskType type, bool isActive ) {
			var os = GetAllTops( type, type == SMTaskType.FirstWork && !isActive );
			switch ( type ) {
				case SMTaskType.FirstWork:
					os.ForEach( o => o._modifyler.Register( new ChangeActiveSMObject( o, isActive, true ) ) );
					foreach ( var o in os ) {
						await o._modifyler.WaitRunning();
					}
					break;

				case SMTaskType.Work:
					os.ForEach( o => o._modifyler.Register( new ChangeActiveSMObject( o, isActive, true ) ) );
					await os.Select( o => o._modifyler.WaitRunning() );
					break;
			}
		}

		public async UniTask RunAllActiveEvents( SMTaskType type ) {
			var os = GetAllTops( type );
			switch ( type ) {
				case SMTaskType.FirstWork:
					os.ForEach( o => o._modifyler.Register( new RunActiveSMObject( o ) ) );
					foreach ( var o in os ) {
						await o._modifyler.WaitRunning();
					}
					break;

				case SMTaskType.Work:
					os.ForEach( o => o._modifyler.Register( new RunActiveSMObject( o ) ) );
					await os.Select( o => o._modifyler.WaitRunning() );
					break;
			}
		}


		public async UniTask Enter() {
			await Load();
//			Log.Debug( _modifyler );
//			await UTask.NextFrame( _activeAsyncCanceler );
//			await UTask.WaitWhile( _activeAsyncCanceler, () => !Input.GetKeyDown( KeyCode.Return ) );
			_isEnter = true;
			return;
			await RunAllStateEvents( SMTaskType.FirstWork, SMTaskRanState.Creating );
			await RunAllStateEvents( SMTaskType.FirstWork, SMTaskRanState.Loading );
			await RunAllStateEvents( SMTaskType.FirstWork, SMTaskRanState.Initializing );

			await RunAllStateEvents( SMTaskType.Work, SMTaskRanState.Creating );
			await RunAllStateEvents( SMTaskType.Work, SMTaskRanState.Loading );
			await RunAllStateEvents( SMTaskType.Work, SMTaskRanState.Initializing );

			await RunAllActiveEvents( SMTaskType.FirstWork );
			await RunAllActiveEvents( SMTaskType.Work );
		}

		public async UniTask Exit() {
			await ChangeAllActives( SMTaskType.Work, false );
			await ChangeAllActives( SMTaskType.FirstWork, false );

			await RunAllStateEvents( SMTaskType.Work, SMTaskRanState.Finalizing );
			await RunAllStateEvents( SMTaskType.FirstWork, SMTaskRanState.Finalizing );

			GetAllTops( SMTaskType.Work ).ForEach( o => o.Dispose() );
			GetAllTops( SMTaskType.FirstWork ).ForEach( o => o.Dispose() );
			GetAllTops( SMTaskType.DontWork ).ForEach( o => o.Dispose() );

			_objects.ForEach( pair => _objects[pair.Key] = null );

			_isEnter = false;
		}


		async UniTask Load() {
			if ( _owner == _owner._fsm._foreverScene )	{ return; }
			TimeManager.s_instance.StartMeasure();

			var currents = new Queue<Transform>(
				_owner._scene.GetRootGameObjects().Select( go => go.transform )
			);
			while ( !currents.IsEmpty() ) {
				var current = currents.Dequeue();
				var bs = current.GetComponents<SMMonoBehaviour>();
				if ( !bs.IsEmpty() ) {
					new SMObject( current.gameObject, bs, null );
					await UTask.NextFrame( _activeAsyncCanceler );
				} else {
					foreach ( Transform child in current ) {
						currents.Enqueue( child );
					}
				}
			}

			Log.Debug( $"{nameof( Load )} {_owner.GetAboutName()} : {TimeManager.s_instance.StopMeasure()}秒" );
		}
	}
}