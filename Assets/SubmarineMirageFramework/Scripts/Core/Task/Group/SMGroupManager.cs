//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestTask
namespace SubmarineMirage.Task {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Base;
	using Scene;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMGroupManager : SMStandardBase {
		static readonly SMTaskType[] GET_ALL_TOPS_TASK_TYPES =
			new SMTaskType[] { SMTaskType.FirstWork, SMTaskType.Work, SMTaskType.DontWork };
		static readonly SMTaskType[] DISPOSE_TASK_TYPES =
			new SMTaskType[] { SMTaskType.Work, SMTaskType.FirstWork, SMTaskType.DontWork };

		public SMScene _owner	{ get; private set; }
		public readonly Dictionary<SMTaskType, SMGroup> _groups = new Dictionary<SMTaskType, SMGroup>();
		public bool _isEnter	{ get; private set; }

		public SMTaskCanceler _asyncCancelerOnDisable => _owner._activeAsyncCanceler;


		public SMGroupManager( SMScene owner ) {
			_owner = owner;

			EnumUtils.GetValues<SMTaskType>().ForEach( t => _groups[t] = null );
			_disposables.AddLast( () => DisposeGroups() );
		}

		void DisposeGroups() {
			DISPOSE_TASK_TYPES
				.SelectMany( t => GetAllGroups( t, true ) )
				.Where( g => g != null )
				.ToArray()
				.ForEach( g => g.Dispose() );
			_groups.Clear();
		}



		public void Register( SMGroup add ) {
#if TestTask
			SMLog.Debug( $"{nameof( Register )} : start\n{this}" );
			// 追加前の物一覧を表示
			SMLog.Debug( string.Join( "\n", _groups.Select( pair => $"{pair.Key} : {pair.Value?.ToLineString()}" ) ) );
#endif
			var g = _groups[add._type];
			if ( g == null ) {
				_groups[add._type] = add;
			} else {
				var last = g.GetLast();
#if TestTask
				SMLog.Debug( string.Join( "\n",
					$"{nameof( last )} : {last?.ToLineString()}",
					$"{nameof( add )} : {add?.ToLineString()}"
				) );
#endif
				add._previous = last;
				last._next = add;
#if TestTask
				SMLog.Debug( string.Join( "\n",
					$"{nameof( last )} : {last?.ToLineString()}",
					$"{nameof( add )} : {add?.ToLineString()}"
				) );
#endif
			}
#if TestTask
			// 追加後の物一覧を表示
			SMLog.Debug( string.Join( "\n", _groups.Select( pair => $"{pair.Key} : {pair.Value?.ToLineString()}" ) ) );
			SMLog.Debug( $"{nameof( Register )} : end\n{this}" );
#endif
		}

		public void Unregister( SMGroup remove, SMTaskType lastType ) {
			if ( _groups[lastType] == remove )	{ _groups[lastType] = remove._next; }
			if ( remove._previous != null )		{ remove._previous._next = remove._next; }
			if ( remove._next != null )			{ remove._next._previous = remove._previous; }
			remove._previous = null;
			remove._next = null;
		}

		public void Unregister( SMGroup remove ) => Unregister( remove, remove._type );



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
#if TestTask
				SMLog.Debug( $"Start Reverse :\n{string.Join( "\n", result.Select( g => g.ToLineString() ) )}" );
#endif
				result = result.Reverse();
#if TestTask
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



		public async UniTask RunAllStateEvents( SMTaskType type, SMTaskRunState state ) {
			var gs = GetAllGroups( type, type == SMTaskType.FirstWork && state == SMTaskRunState.Finalizing );
			switch ( type ) {
				case SMTaskType.FirstWork:	foreach ( var g in gs )	{ await g.RunStateEvent( state ); }	return;
				case SMTaskType.Work:		await gs.Select( g => g.RunStateEvent( state ) );			return;
			}
		}

		public async UniTask ChangeAllActives( SMTaskType type, bool isActive ) {
			var gs = GetAllGroups( type, type == SMTaskType.FirstWork && !isActive );
			switch ( type ) {
				case SMTaskType.FirstWork:	foreach ( var g in gs )	{ await g.ChangeActive( isActive ); }	return;
				case SMTaskType.Work:		await gs.Select( g => g.ChangeActive( isActive ) );				return;
			}
		}

		public async UniTask RunAllInitialActives( SMTaskType type ) {
			var gs = GetAllGroups( type );
			switch ( type ) {
				case SMTaskType.FirstWork:	foreach ( var g in gs )	{ await g.RunInitialActive(); }	return;
				case SMTaskType.Work:		await gs.Select( g => g.RunInitialActive() );			return;
			}
		}



		public async UniTask Enter() {
			await Load();
//			SMLog.Debug( _modifyler );
//			await UTask.NextFrame( _asyncCancelerOnDisable );
//			await UTask.WaitWhile( _asyncCancelerOnDisable, () => !Input.GetKeyDown( KeyCode.Return ) );
			_isEnter = true;
			return;
			await RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.Create );
			await RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.SelfInitializing );
			await RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.Initializing );

			await RunAllStateEvents( SMTaskType.Work, SMTaskRunState.Create );
			await RunAllStateEvents( SMTaskType.Work, SMTaskRunState.SelfInitializing );
			await RunAllStateEvents( SMTaskType.Work, SMTaskRunState.Initializing );

			await RunAllInitialActives( SMTaskType.FirstWork );
			await RunAllInitialActives( SMTaskType.Work );
		}

		public async UniTask Exit() {
			await ChangeAllActives( SMTaskType.Work, false );
			await ChangeAllActives( SMTaskType.FirstWork, false );

			await RunAllStateEvents( SMTaskType.Work, SMTaskRunState.Finalizing );
			await RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.Finalizing );

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
	}
}