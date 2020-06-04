//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process.New {
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
	using Type = ProcessBody.Type;
	using RanState = ProcessBody.RanState;


	// TODO : コメント追加、整頓


	public class ProcessHierarchyManager : IDisposableExtension {
		public BaseScene _owner	{ get; private set; }
		public readonly Dictionary<Type, ProcessHierarchy> _hierarchies
			= new Dictionary<Type, ProcessHierarchy>();
		public HierarchyModifyler _modifyler	{ get; private set; }
		public bool _isEnter	{ get; private set; }

		public CancellationToken _activeAsyncCancel => _owner._activeAsyncCancel;

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public ProcessHierarchyManager( BaseScene owner ) {
			_owner = owner;

			EnumUtils.GetValues<Type>().ForEach( t => _hierarchies[t] = null );
			_disposables.AddLast( () => {
				_hierarchies
					.SelectMany( pair => pair.Value?.GetBrothers() )
					.ForEach( h => h?.Dispose() );
				_hierarchies.Clear();
			} );

			_modifyler = new HierarchyModifyler( this );
			_disposables.AddLast( _modifyler );
		}

		public void Dispose() => _disposables.Dispose();

		~ProcessHierarchyManager() => Dispose();





		public IEnumerable<ProcessHierarchy> Get( Type? type = null, bool isReverse = false ) {
			var result = (
				type.HasValue	? _hierarchies[type.Value]?.GetBrothers() ?? Enumerable.Empty<ProcessHierarchy>()
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


		public T GetProcess<T>( Type? type = null ) where T : IProcess {
			return Get( type )
				.Select( h => h.GetProcessInChildren<T>() )
				.FirstOrDefault( p => p != null );
		}
		public IProcess GetProcess( System.Type systemType, Type? type = null ) {
			return Get( type )
				.Select( h => h.GetProcessInChildren( systemType ) )
				.FirstOrDefault( p => p != null );
		}

		public IEnumerable<T> GetProcesses<T>( Type? type = null ) where T : IProcess {
			return Get( type )
				.SelectMany( h => h.GetProcessesInChildren<T>() );
		}
		public IEnumerable<IProcess> GetProcesses( System.Type systemType, Type? type = null ) {
			return Get( type )
				.SelectMany( h => h.GetProcessesInChildren( systemType ) );
		}


		public void Register( ProcessHierarchy top )
			=> _modifyler.Register( new RegisterHierarchyModifyData( top ) );

		public void ReRegister( ProcessHierarchy top, Type lastType, BaseScene lastScene )
			=> _modifyler.Register( new ReRegisterHierarchyModifyData( top, lastType, lastScene ) );

		public void Unregister( ProcessHierarchy top )
			=> _modifyler.Register( new UnregisterHierarchyModifyData( top ) );


		public T AddProcess<T>( ProcessHierarchy hierarchy ) where T : MonoBehaviourProcess {
			if ( hierarchy._owner == null ) {
				throw new NotSupportedException(
					$"{nameof(BaseProcess)}._hierarchyに、追加不可 :\n{hierarchy}" );
			}
			var p = hierarchy._owner.AddComponent<T>();
			_modifyler.Register( new AddHierarchyModifyData( hierarchy, p ) );
			return p;
		}
		public MonoBehaviourProcess AddProcess( ProcessHierarchy hierarchy, System.Type type ) {
			if ( hierarchy._owner == null ) {
				throw new NotSupportedException(
					$"{nameof(BaseProcess)}._hierarchyに、追加不可 :\n{hierarchy}" );
			}
			var p = (MonoBehaviourProcess)hierarchy._owner.AddComponent( type );
			_modifyler.Register( new AddHierarchyModifyData( hierarchy, p ) );
			return p;
		}


		public void Destroy( ProcessHierarchy top )
			=> _modifyler.Register( new DestroyHierarchyModifyData( top ) );

		public void ChangeParent( ProcessHierarchy hierarchy, Transform parent, bool isWorldPositionStays )
			=> _modifyler.Register(
				new ChangeParentHierarchyModifyData( hierarchy, parent, isWorldPositionStays )
			);





		public async UniTask RunAllStateEvents( Type type, RanState state ) {
			_modifyler._isLock.Value = true;
			var hs = Get( type, type == Type.FirstWork && state == RanState.Finalizing );
			switch ( type ) {
				case Type.FirstWork:
					foreach ( var h in hs ) {
						await h.RunStateEvent( state );
					}
					break;

				case Type.Work:
					await UniTask.WhenAll(
						hs.Select( h => h.RunStateEvent( state ) )
					);
					break;
			}
			_modifyler._isLock.Value = false;
		}

		public async UniTask ChangeAllActives( Type type, bool isActive ) {
			_modifyler._isLock.Value = true;
			var hs = Get( type, type == Type.FirstWork && !isActive );
			switch ( type ) {
				case Type.FirstWork:
					foreach ( var h in hs ) {
						await h.ChangeActive( isActive, true );
					}
					break;

				case Type.Work:
					await UniTask.WhenAll(
						hs.Select( h => h.ChangeActive( isActive, true ) )
					);
					break;
			}
			_modifyler._isLock.Value = false;
		}

		public async UniTask RunAllActiveEvents( Type type ) {
			_modifyler._isLock.Value = true;
			var hs = Get( type );
			switch ( type ) {
				case Type.FirstWork:
					foreach ( var h in hs ) {
						await h.RunActiveEvent();
					}
					break;

				case Type.Work:
					await UniTask.WhenAll(
						hs.Select( h => h.RunActiveEvent() )
					);
					break;
			}
			_modifyler._isLock.Value = false;
		}


		public async UniTask Enter() {
			await Load();
//			Log.Debug( $"end Load : {_owner.GetAboutName()} {_modifyler._isLock.Value}" );
//			Log.Debug( _modifyler );
//			await UniTaskUtility.Yield( _activeAsyncCancel );
//			await UniTaskUtility.WaitWhile( _activeAsyncCancel, () => !Input.GetKeyDown( KeyCode.Return ) );
			_isEnter = true;
			return;
			await RunAllStateEvents( Type.FirstWork, RanState.Creating );
			await RunAllStateEvents( Type.FirstWork, RanState.Loading );
			await RunAllStateEvents( Type.FirstWork, RanState.Initializing );

			await RunAllStateEvents( Type.Work, RanState.Creating );
			await RunAllStateEvents( Type.Work, RanState.Loading );
			await RunAllStateEvents( Type.Work, RanState.Initializing );

			await RunAllActiveEvents( Type.FirstWork );
			await RunAllActiveEvents( Type.Work );
		}

		public async UniTask Exit() {
			await ChangeAllActives( Type.Work, false );
			await ChangeAllActives( Type.FirstWork, false );

			await RunAllStateEvents( Type.Work, RanState.Finalizing );
			await RunAllStateEvents( Type.FirstWork, RanState.Finalizing );

			Get( Type.Work ).ForEach( h => h.Dispose() );
			Get( Type.FirstWork ).ForEach( h => h.Dispose() );
			Get( Type.DontWork ).ForEach( h => h.Dispose() );

			_hierarchies.ForEach( pair => _hierarchies[pair.Key] = null );

			_isEnter = false;
		}


		async UniTask Load() {
			if ( _owner == _owner._fsm._foreverScene )	{ return; }
			TimeManager.s_instance.StartMeasure();

			var currents = _owner._scene.GetRootGameObjects().Select( go => go.transform );
			while ( !currents.IsEmpty() ) {
				var children = Enumerable.Empty<Transform>();
				currents.ForEach( t => {
					var ps = t.GetComponents<MonoBehaviourProcess>();
					if ( !ps.IsEmpty() ) {
						new ProcessHierarchy( t.gameObject, ps, null );
					} else {
						foreach ( Transform child in t ) {
							children.Concat( child );
						}
					}
				} );
				currents = children;
				await UniTaskUtility.Yield( _activeAsyncCancel );
			}

			Log.Debug( $"Load {_owner.GetAboutName()} : {TimeManager.s_instance.StopMeasure()}秒" );
		}
	}
}