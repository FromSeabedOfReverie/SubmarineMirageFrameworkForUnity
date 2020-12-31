//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupManager
namespace SubmarineMirage.Task.Group.Manager {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Scene;
	using Modifyler;
	using Task.Modifyler;
	using Behaviour;
	using Object;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMGroupManager : BaseSMTaskModifylerOwner<SMGroupManagerModifyler> {
		static readonly SMTaskType[] GET_ALL_TOPS_TASK_TYPES =
			new SMTaskType[] { SMTaskType.FirstWork, SMTaskType.Work, SMTaskType.DontWork };
		static readonly SMTaskType[] DISPOSE_TASK_TYPES =
			new SMTaskType[] { SMTaskType.Work, SMTaskType.FirstWork, SMTaskType.DontWork };

		public SMScene _owner	{ get; private set; }
		public readonly Dictionary<SMTaskType, SMGroup> _groups = new Dictionary<SMTaskType, SMGroup>();
		public bool _isEnter	{ get; private set; }

		[SMHide] public SMTaskCanceler _asyncCancelerOnDisable => _owner._activeAsyncCanceler;



		public SMGroupManager( SMScene owner ) {
			_modifyler = new SMGroupManagerModifyler( this );
			_owner = owner;
			EnumUtils.GetValues<SMTaskType>().ForEach( t => _groups[t] = null );

			_disposables.AddLast( () => {
				DisposeGroups();
			} );
		}

		void DisposeGroups() {
			DISPOSE_TASK_TYPES
				.SelectMany( t => GetAllGroups( t, true ) )
				.Where( g => g != null )
				.ToArray()
				.ForEach( g => g.Dispose() );
			_groups.Clear();
		}


		public IEnumerable<SMGroup> GetAllGroups( SMTaskType? type = null, bool isReverse = false ) {
			IEnumerable<SMGroup> result = null;
			if ( type.HasValue ) {
				result = _groups[type.Value]?.GetBrothers() ?? Enumerable.Empty<SMGroup>();
			} else {
				result = GET_ALL_TOPS_TASK_TYPES
					.Select( t => _groups.GetOrDefault( t ) )
					.Where( g => g != null )
					.SelectMany( g => g.GetBrothers() );
			}
			if ( isReverse ) {
#if TestGroupManager
				SMLog.Debug( $"Start Reverse :\n{string.Join( "\n", result.Select( g => g.ToLineString() ) )}" );
#endif
				result = result.Reverse();
#if TestGroupManager
				SMLog.Debug( $"End Reverse :\n{string.Join( "\n", result.Select( g => g.ToLineString() ) )}" );
				SMLog.Debug(
					$"Don't Reverse :\n{string.Join( "\n", GetAllGroups( type ).Select( g => g.ToLineString() ) )}" );
#endif
			}
			return result;
		}

		public IEnumerable<SMObject> GetAllTops( SMTaskType? type = null, bool isReverse = false )
			=> GetAllGroups( type, isReverse )
				.Select( g => g._topObject );



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



		public async UniTask RunAllStateEvents( SMTaskType type, SMTaskRunState state, bool isWait = true ) {
			RunStateSMGroupManager.RegisterAndRun( this, type, state );
			if ( isWait )	{ await _modifyler.WaitRunning(); }
		}

		public async UniTask ChangeAllActives( SMTaskType taskType, bool isActive, bool isWait = true ) {
			_modifyler.Register( new ChangeActiveSMGroupManager( taskType, isActive ) );
			if ( isWait )	{ await _modifyler.WaitRunning(); }
		}

		public async UniTask RunAllInitialActives( SMTaskType taskType, bool isWait = true ) {
			_modifyler.Register( new RunInitialActiveSMGroupManager( taskType ) );
			if ( isWait )	{ await _modifyler.WaitRunning(); }
		}



		public async UniTask Enter() {
			await Load();
//			SMLog.Debug( _modifyler );
//			await UTask.NextFrame( _asyncCancelerOnDisable );
//			await UTask.WaitWhile( _asyncCancelerOnDisable, () => !Input.GetKeyDown( KeyCode.Return ) );
			_isEnter = true;
			return;
			await RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.Create );
			await RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.SelfInitialize );
			await RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.Initialize );

			await RunAllStateEvents( SMTaskType.Work, SMTaskRunState.Create );
			await RunAllStateEvents( SMTaskType.Work, SMTaskRunState.SelfInitialize );
			await RunAllStateEvents( SMTaskType.Work, SMTaskRunState.Initialize );

			await RunAllInitialActives( SMTaskType.FirstWork );
			await RunAllInitialActives( SMTaskType.Work );
		}

		public async UniTask Exit() {
			await ChangeAllActives( SMTaskType.Work, false );
			await ChangeAllActives( SMTaskType.FirstWork, false );

			await RunAllStateEvents( SMTaskType.Work, SMTaskRunState.Finalize );
			await RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.Finalize );

			DisposeGroups();
			EnumUtils.GetValues<SMTaskType>().ForEach( t => _groups[t] = null );

			_isEnter = false;
		}


		async UniTask Load() {
			if ( _owner == _owner._fsm._foreverScene )	{ return; }
			SMTimeManager.s_instance.StartMeasure();

			var currents = new Queue<Transform>(
				_owner._scene.GetRootGameObjects().Select( go => go.transform )
			);
			while ( !currents.IsEmpty() ) {
				var current = currents.Dequeue();
				var bs = current.GetComponents<SMMonoBehaviour>();
				if ( !bs.IsEmpty() ) {
					new SMObject( current.gameObject, bs, null );
					await UTask.NextFrame( _asyncCancelerOnDisable );
				} else {
					foreach ( Transform child in current ) {
						currents.Enqueue( child );
					}
				}
			}

			SMLog.Debug( $"{nameof( Load )} {_owner.GetAboutName()} : {SMTimeManager.s_instance.StopMeasure()}秒" );
		}



		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _groups ), i => {
				var arrayI = StringSMUtility.IndentSpace( i + 1 );
				return "\n" + string.Join( ",\n", _groups.Select( pair =>
					$"{arrayI}{pair.Key} : {pair.Value.ToLineString()}"
				) );
			} );
		}
	}
}