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
	using Singleton.New;
	using MultiEvent;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using UnityObject = UnityEngine.Object;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
	using Type = ProcessBody.Type;
	using RanState = ProcessBody.RanState;


	// TODO : コメント追加、整頓


	public class ProcessHierarchyManager : IDisposableExtension {
		public BaseScene _owner	{ get; private set; }
		public readonly Dictionary< Type, List<ProcessHierarchy> > _hierarchies
			= new Dictionary< Type, List<ProcessHierarchy> >();
		HierarchyModifyler _modifyler;
		public bool _isEnter	{ get; private set; }

		public CancellationToken _activeAsyncCancel => _owner._activeAsyncCancel;

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public ProcessHierarchyManager( BaseScene owner ) {
			_owner = owner;

			EnumUtils.GetValues<Type>()
				.ForEach( t => _hierarchies[t] = new List<ProcessHierarchy>() );
			_disposables.AddLast( () => {
				_hierarchies
					.SelectMany( pair => pair.Value )
					.ForEach( h => h.Dispose() );
				_hierarchies.Clear();
			} );

			_modifyler = new HierarchyModifyler( this );
			_disposables.AddLast( _modifyler );
		}

		public void Dispose() => _disposables.Dispose();

		~ProcessHierarchyManager() => Dispose();


		public void Register( ProcessHierarchy top ) {
			_modifyler.Register( new RegisterHierarchyModifyData( top ) );
		}

		public void ReRegister( ProcessHierarchy top, ProcessHierarchyManager lastOwner ) {
			_modifyler.Register( new ReRegisterHierarchyModifyData( top, lastOwner ) );
		}

		public void Unregister( ProcessHierarchy top ) {
			_modifyler.Register( new UnregisterHierarchyModifyData( top ) );
		}

		public void Delete( ProcessHierarchy top ) {
			_modifyler.Register( new DeleteHierarchyModifyData( top ) );
		}


		public List<ProcessHierarchy> Gets( Type type, bool isReverse = false ) {
			var hs = _hierarchies[type];
			if ( isReverse ) {
				Log.Debug(
					$"befor reverse : " +
					$"{string.Join( ", ", _hierarchies[type].Select( h => h._processes.First().GetAboutName() ) )}"
				);
				hs = hs.ReverseByClone();
				Log.Debug(
					$"new : " +
					$"{string.Join( ", ", hs.Select( h => h._processes.First().GetAboutName() ) )}"
				);
				Log.Debug(
					$"after reverse : " +
					$"{string.Join( ", ", _hierarchies[type].Select( h => h._processes.First().GetAboutName() ) )}"
				);
			}
// TODO : 使用先で、Add、Removeされた場合、元もちゃんと変更されるか？
			return hs;
		}


		public async UniTask RunAllStateEvents( Type type, RanState state ) {
			var hs = Gets( type, type == Type.FirstWork && state == RanState.Finalizing );
			switch ( type ) {
				case Type.FirstWork:
					foreach ( var h in hs ) {
						await h.RunStateEvent( state );
					}
					return;

				case Type.Work:
					await UniTask.WhenAll(
						hs.Select( h => h.RunStateEvent( state ) )
					);
					return;
			}
		}

		public async UniTask ChangeAllActives( Type type, bool isActive ) {
			var hs = Gets( type, type == Type.FirstWork && !isActive );
			switch ( type ) {
				case Type.FirstWork:
					foreach ( var h in hs ) {
						await h.ChangeActive( isActive, true );
					}
					return;

				case Type.Work:
					await UniTask.WhenAll(
						hs.Select( h => h.ChangeActive( isActive, true ) )
					);
					return;
			}
		}

		public async UniTask RunAllActiveEvents( Type type ) {
			var hs = Gets( type );
			switch ( type ) {
				case Type.FirstWork:
					foreach ( var h in hs ) {
						await h.RunActiveEvent();
					}
					return;

				case Type.Work:
					await UniTask.WhenAll(
						hs.Select( h => h.RunActiveEvent() )
					);
					return;
			}
		}


		public async UniTask Enter() {
			await Load();
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

			Gets( Type.Work ).Clear();
			Gets( Type.FirstWork ).Clear();

			var hs = Gets( Type.DontWork );
			hs.ForEach( h => h.Dispose() );
			hs.Clear();

			_isEnter = false;
		}


		async UniTask Load() {
			var currents = _owner._scene.GetRootGameObjects().Select( go => go.transform ).ToList();
			while ( !currents.IsEmpty() ) {
				var children = new List<Transform>();
				currents.ForEach( t => {
					var ps = t.GetComponents<MonoBehaviourProcess>();
					if ( !ps.IsEmpty() ) {
						new ProcessHierarchy( t.gameObject, ps, null );
					} else {
						foreach ( Transform child in t ) {
							children.Add( child );
						}
					}
				} );
				currents = children;
			}

			// TODO : そのうち、オープンワールド用に、非同期読込に対応させる
			await UniTaskUtility.DontWait();
		}
	}
}