//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask {
	using System;
	using System.Linq;
	using System.Threading;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using MultiEvent;
	using Scene;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMHierarchyManager : IDisposableExtension {
		public BaseScene _owner	{ get; private set; }
		public readonly Dictionary<SMTaskType, SMHierarchy> _hierarchies
			= new Dictionary<SMTaskType, SMHierarchy>();
		public bool _isEnter	{ get; private set; }

		public CancellationToken _activeAsyncCancel => _owner._activeAsyncCancel;

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public SMHierarchyManager( BaseScene owner ) {
			_owner = owner;

			EnumUtils.GetValues<SMTaskType>().ForEach( t => _hierarchies[t] = null );
			_disposables.AddLast( () => {
				_hierarchies
					.SelectMany( pair => pair.Value?.GetBrothers() )
					.ForEach( h => h?.Dispose() );
				_hierarchies.Clear();
			} );
		}

		public void Dispose() => _disposables.Dispose();

		~SMHierarchyManager() => Dispose();



		public void Add( SMHierarchy hierarchy ) {
			var last = GetLast( hierarchy._type );
			if ( last != null )	{ last.Add( hierarchy ); }
			else				{ _hierarchies[hierarchy._type] = hierarchy; }
		}

		public SMHierarchy GetLast( SMTaskType type ) {
			return _hierarchies.GetOrDefault( type )?.GetLast() ?? null;
		}

		public IEnumerable<SMHierarchy> Get( SMTaskType? type = null, bool isReverse = false ) {
			var result = (
				type.HasValue	? _hierarchies[type.Value]?.GetBrothers() ?? Enumerable.Empty<SMHierarchy>()
								: _hierarchies
									.Where( pair => pair.Value != null )
									.SelectMany( pair => pair.Value.GetBrothers() )
			);

			if ( isReverse ) {
/*
				var befor = string.Join(
					", ",
					(	type.HasValue	? _hierarchies[type.Value].GetBrothers()
										: _hierarchies
											.Where( pair => pair.Value != null )
											.SelectMany( pair => pair.Value.GetBrothers() )
					).Select( h => h._process.GetAboutName() )
				);
				Log.Debug( $"befor reverse : {befor}" );
*/
				result = result.Reverse();
/*
				Log.Debug( $"new : {string.Join( ", ", result.Select( h => h._process.GetAboutName() ) )}" );
				var after = string.Join(
					", ",
					(	type.HasValue	? _hierarchies[type.Value].GetBrothers()
										: _hierarchies
											.Where( pair => pair.Value != null )
											.SelectMany( pair => pair.Value.GetBrothers() )
					).Select( h => h._process.GetAboutName() )
				);
				Log.Debug( $"after reverse : {after}" );
*/
			}
			return result;
		}


		public T GetProcess<T>( SMTaskType? type = null ) where T : ISMBehavior {
			return Get( type )
				.Select( h => h.GetProcessInChildren<T>() )
				.FirstOrDefault( p => p != null );
		}
		public ISMBehavior GetProcess( Type systemType, SMTaskType? type = null ) {
			return Get( type )
				.Select( h => h.GetProcessInChildren( systemType ) )
				.FirstOrDefault( p => p != null );
		}

		public IEnumerable<T> GetProcesses<T>( SMTaskType? type = null ) where T : ISMBehavior {
			return Get( type )
				.SelectMany( h => h.GetProcessesInChildren<T>() );
		}
		public IEnumerable<ISMBehavior> GetProcesses( Type systemType, SMTaskType? type = null ) {
			return Get( type )
				.SelectMany( h => h.GetProcessesInChildren( systemType ) );
		}


		public void Register( SMHierarchy top )
			=> _modifyler.Register( new RegisterSMHierarchy( top ) );

		public void ReRegister( SMHierarchy top, SMTaskType lastType, BaseScene lastScene )
			=> _modifyler.Register( new ReRegisterSMHierarchy( top, lastType, lastScene ) );

		public void Unregister( SMHierarchy top )
			=> _modifyler.Register( new UnregisterSMHierarchy( top ) );


		public T AddProcess<T>( SMHierarchy hierarchy ) where T : SMMonoBehaviour {
			if ( hierarchy._owner == null ) {
				throw new NotSupportedException(
					$"{nameof(SMBehavior)}._hierarchyに、追加不可 :\n{hierarchy}" );
			}
			var p = hierarchy._owner.AddComponent<T>();
			_modifyler.Register( new AddSMHierarchy( hierarchy, p ) );
			return p;
		}
		public SMMonoBehaviour AddProcess( SMHierarchy hierarchy, Type type ) {
			if ( hierarchy._owner == null ) {
				throw new NotSupportedException(
					$"{nameof(SMBehavior)}._hierarchyに、追加不可 :\n{hierarchy}" );
			}
			var p = (SMMonoBehaviour)hierarchy._owner.AddComponent( type );
			_modifyler.Register( new AddSMHierarchy( hierarchy, p ) );
			return p;
		}


		public void Destroy( SMHierarchy top )
			=> _modifyler.Register( new DestroySMHierarchy( top ) );

		public void ChangeParent( SMHierarchy hierarchy, Transform parent, bool isWorldPositionStays )
			=> _modifyler.Register(
				new ChangeParentSMHierarchy( hierarchy, parent, isWorldPositionStays )
			);





		public async UniTask RunAllStateEvents( SMTaskType type, SMTaskRanState state ) {
			var hs = Get( type, type == SMTaskType.FirstWork && state == SMTaskRanState.Finalizing );
			switch ( type ) {
				case SMTaskType.FirstWork:
					foreach ( var h in hs ) {
						await h.RunStateEvent( state );
					}
					break;

				case SMTaskType.Work:
					await UniTask.WhenAll(
						hs.Select( h => h.RunStateEvent( state ) )
					);
					break;
			}
		}

		public async UniTask ChangeAllActives( SMTaskType type, bool isActive ) {
			var hs = Get( type, type == SMTaskType.FirstWork && !isActive );
			switch ( type ) {
				case SMTaskType.FirstWork:
					foreach ( var h in hs ) {
						await h.ChangeActive( isActive, true );
					}
					break;

				case SMTaskType.Work:
					await UniTask.WhenAll(
						hs.Select( h => h.ChangeActive( isActive, true ) )
					);
					break;
			}
		}

		public async UniTask RunAllActiveEvents( SMTaskType type ) {
			var hs = Get( type );
			switch ( type ) {
				case SMTaskType.FirstWork:
					foreach ( var h in hs ) {
						await h.RunActiveEvent();
					}
					break;

				case SMTaskType.Work:
					await UniTask.WhenAll(
						hs.Select( h => h.RunActiveEvent() )
					);
					break;
			}
		}


		public async UniTask Enter() {
			await Load();
//			Log.Debug( _modifyler );
//			await UniTaskUtility.Yield( _activeAsyncCancel );
//			await UniTaskUtility.WaitWhile( _activeAsyncCancel, () => !Input.GetKeyDown( KeyCode.Return ) );
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

			Get( SMTaskType.Work ).ForEach( h => h.Dispose() );
			Get( SMTaskType.FirstWork ).ForEach( h => h.Dispose() );
			Get( SMTaskType.DontWork ).ForEach( h => h.Dispose() );

			_hierarchies.ForEach( pair => _hierarchies[pair.Key] = null );

			_isEnter = false;
		}


		async UniTask Load() {
			if ( _owner == _owner._fsm._foreverScene )	{ return; }
			TimeManager.s_instance.StartMeasure();

			var currents = _owner._scene.GetRootGameObjects().Select( go => go.transform );
			while ( !currents.IsEmpty() ) {
				var children = Enumerable.Empty<Transform>();
				foreach ( var t in currents ) {
					var ps = t.GetComponents<SMMonoBehaviour>();
					if ( !ps.IsEmpty() ) {
						new SMHierarchy( t.gameObject, ps, null );
						await UniTaskUtility.Yield( _activeAsyncCancel );
					} else {
						foreach ( Transform child in t ) {
							children.Concat( child );
						}
					}
				}
				currents = children;
				await UniTaskUtility.Yield( _activeAsyncCancel );
			}

			Log.Debug( $"Load {_owner.GetAboutName()} : {TimeManager.s_instance.StopMeasure()}秒" );
		}
	}
}