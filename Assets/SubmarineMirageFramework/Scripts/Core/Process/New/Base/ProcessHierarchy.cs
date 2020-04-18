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
	using UniRx.Async;
	using KoganeUnityLib;
	using MultiEvent;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using UnityObject = UnityEngine.Object;


	// TODO : コメント追加、整頓


	public class ProcessHierarchy : IDisposable {
		public ProcessBody.Type _type			{ get; private set; }
		public ProcessBody.LifeSpan _lifeSpan	{ get; private set; }
		public string _belongSceneName			{ get; private set; }

		public GameObject _owner	{ get; private set; }
		readonly List<IProcess> _processes = new List<IProcess>();
		ProcessHierarchy _parent;
		readonly List<ProcessHierarchy> _children = new List<ProcessHierarchy>();
		ProcessHierarchy _top;

		readonly MultiDisposable _disposables = new MultiDisposable();


		public ProcessHierarchy( GameObject owner, IEnumerable<IProcess> processes, ProcessHierarchy parent ) {
			_owner = owner;

			SetParent( parent );
			SetChildren();
			SetBrothers( processes );

			_disposables.AddLast( () => _parent?._children.Remove( this ) );
			_disposables.AddLast( _processes );
			_disposables.AddLast( _children );

			SetTop();

			Log.Debug( $"作成 : {this}" );
		}

		~ProcessHierarchy() => Dispose();

		public void Dispose() => _disposables.Dispose();


		void SetParent( ProcessHierarchy parent ) {
			if ( _owner == null )	{ return; }

			_parent?._children.Remove( this );
			_parent = parent;
			_parent?._children.Add( this );
		}

		void SetChildren() {
			if ( _owner == null )	{ return; }

			var currents = new List<Transform> { _owner.transform };
			while ( !currents.IsEmpty() ) {
				var children = new List<Transform>();
				currents.ForEach( t => {
					foreach ( Transform child in t ) {
						var ps = child.GetComponents<MonoBehaviourProcess>();
						if ( !ps.IsEmpty() ) {
							new ProcessHierarchy( child.gameObject, ps, this );
						} else {
							children.Add( child );
						}
					}
				} );
				currents = children;
			}
		}

		void SetBrothers( IEnumerable<IProcess> processes ) {
			_processes.Clear();
			_processes.Add( processes );
			_processes.ForEach( p => p._hierarchy = this );
			if ( _owner != null ) {
				_processes.ForEach( p => ( (MonoBehaviourProcess)p ).Constructor() );
			}
		}

		void SetTop() {
			if ( _top != null )	{ return; }
			for ( _top = this; _top._parent != null; _top = _top._parent );

			SetAllHierarchiesData();

			if ( _top._owner != null && _lifeSpan == ProcessBody.LifeSpan.Forever ) {
				UnityObject.DontDestroyOnLoad( _top._owner );
			}
//			CoreProcessManager.s_instance.Register( _top ).Forget();
//			_disposables.AddLast( "Unregister", () => CoreProcessManager.s_instance.Unregister( _top ) );
		}

		void SetAllHierarchiesData() {
			var allHierarchy = GetHierarchiesInChildren( _top );
			var allProcesses = allHierarchy.SelectMany( h => h._processes );

			_type = (
				allProcesses.Any( p => p._type == ProcessBody.Type.FirstWork )	? ProcessBody.Type.FirstWork :
				allProcesses.Any( p => p._type == ProcessBody.Type.Work )		? ProcessBody.Type.Work
																				: ProcessBody.Type.DontWork
			);
			_lifeSpan = allProcesses.Any( p => p._lifeSpan == ProcessBody.LifeSpan.Forever ) ?
				ProcessBody.LifeSpan.Forever : ProcessBody.LifeSpan.InScene;
			_belongSceneName = (
				_lifeSpan == ProcessBody.LifeSpan.Forever	? ProcessBody.FOREVER_SCENE_NAME :
				_top._owner != null							? _top._owner.scene.name
															: SceneManager.s_instance._currentSceneName
			);

			allHierarchy.ForEach( h => {
				h._top = _top;
				h._type = _type;
				h._lifeSpan = _lifeSpan;
				h._belongSceneName = _belongSceneName;
			} );
		}


		List<ProcessHierarchy> GetHierarchiesInChildren( ProcessHierarchy parent ) {
			var result = new List<ProcessHierarchy>();
			var currents = new List<ProcessHierarchy> { parent };
			while ( !currents.IsEmpty() ) {
				var children = new List<ProcessHierarchy>();
				currents.ForEach( h => {
					result.Add( h );
					children.Add( h._children );
				} );
				currents = children;
			}
			return result;
		}


		public void ChangeParent( Transform parent, bool isWorldPositionStays ) {
			if ( _owner == null )	{ return; }

			_owner.transform.SetParent( parent, isWorldPositionStays );
//			if ( _top == this )	{ _disposables.Remove( "Unregister" ); }

			var p = _owner.GetComponentInParentUntilOneHierarchy<MonoBehaviourProcess>( true )
				?._hierarchy;
			SetParent( p );

			if ( _parent == null ) {
				_top = null;
				SetTop();

			} else {
				GetHierarchiesInChildren( this ).ForEach( h => {
					h._top = _parent._top;
					h._type = _parent._type;
					h._lifeSpan = _parent._lifeSpan;
					h._belongSceneName = _parent._belongSceneName;
				} );
			}
		}


		public T AddProcess<T>() where T : MonoBehaviourProcess {
			if ( _owner == null )	{ return null; }
			var p = _owner.AddComponent<T>();
			_processes.Add( p );
			p._hierarchy = this;
			p.Constructor();
			return p;
		}

		public T GetProcess<T>() where T : MonoBehaviourProcess {
			if ( _owner == null )	{ return null; }
			return (T)_processes.FirstOrDefault( p => p is T );
		}


		public ProcessHierarchy InstantiateHierarchy( GameObject instance ) {
			var ps = instance.GetComponents<MonoBehaviourProcess>();
			var parent = instance.GetComponentInParentUntilOneHierarchy<MonoBehaviourProcess>( true )
				?._hierarchy;
			return new ProcessHierarchy( instance.gameObject, ps, parent );
		}

		public GameObject InstantiateProcess( GameObject original, Transform parent,
												bool isWorldPositionStays )
		{
			var go = UnityObject.Instantiate( original, parent, isWorldPositionStays );
			InstantiateHierarchy( go );
			return go;
		}
		public GameObject InstantiateProcess( GameObject original, Transform parent ) {
			var go = UnityObject.Instantiate( original, parent );
			InstantiateHierarchy( go );
			return go;
		}
		public GameObject InstantiateProcess( GameObject original, Vector3 position, Quaternion rotation,
												Transform parent )
		{
			var go = UnityObject.Instantiate( original, position, rotation, parent );
			InstantiateHierarchy( go );
			return go;
		}
		public GameObject InstantiateProcess( GameObject original, Vector3 position, Quaternion rotation ) {
			var go = UnityObject.Instantiate( original, position, rotation );
			InstantiateHierarchy( go );
			return go;
		}
		public GameObject InstantiateProcess( GameObject original ) {
			var go = UnityObject.Instantiate( original );
			InstantiateHierarchy( go );
			return go;
		}


		public async UniTask Destroy() {
// TODO : Destroy中のオブジェクトを、管理クラスで監視する
			SetParent( null );
			await RunStateEvent( ProcessBody.RanState.Finalizing );
			Dispose();
		}


		public async UniTask RunStateEvent( ProcessBody.RanState state ) {
			using ( var events = new MultiAsyncEvent() ) {
				switch ( _type ) {
					case ProcessBody.Type.FirstWork:
						events.AddLast( async _ => {
							foreach ( var p in _processes ) {
								try										{ await p.RunStateEvent( state ); }
								catch ( OperationCanceledException )	{}
							}
						} );
						events.AddLast( async _ => {
							foreach ( var p in _children ) {
								await p.RunStateEvent( state );
							}
						} );
						break;

					case ProcessBody.Type.Work:
						events.AddLast( async _ => await UniTask.WhenAll(
							_processes.Select( async p => {
								try										{ await p.RunStateEvent( state ); }
								catch ( OperationCanceledException )	{}
							} )
							.Concat(
								_children.Select( p => p.RunStateEvent( state ) )
							)
						) );
						break;
				}
				if ( state == ProcessBody.RanState.Finalizing )	{ events.Reverse(); }
				using ( var canceler = new CancellationTokenSource() ) {
					await events.Run( canceler.Token );
				}
			}
		}


		public async UniTask ChangeActive( bool isActive, bool isChangeOwner ) {
			using ( var events = new MultiAsyncEvent() ) {
				if ( _owner != null && isChangeOwner ) {
					events.AddLast( async _ => {
						_owner.SetActive( isActive );
						await UniTaskUtility.DontWait();
					} );
				}
				switch ( _type ) {
					case ProcessBody.Type.FirstWork:
						events.AddLast( async _ => {
							foreach ( var p in _processes ) {
								try										{ await p.ChangeActive( isActive ); }
								catch ( OperationCanceledException )	{}
							}
						} );
						events.AddLast( async _ => {
							foreach ( var p in _children ) {
								await p.ChangeActive( isActive, false );
							}
						} );
						break;

					case ProcessBody.Type.Work:
						events.AddLast( async _ => await UniTask.WhenAll(
							_processes.Select( async p => {
								try										{ await p.ChangeActive( isActive ); }
								catch ( OperationCanceledException )	{}
							} )
							.Concat(
								_children.Select( p => p.ChangeActive( isActive, false ) )
							)
						) );
						break;
				}
				if ( !isActive )	{ events.Reverse(); }
				using ( var canceler = new CancellationTokenSource() ) {
					await events.Run( canceler.Token );
				}
			}
		}

		public async UniTask RunActiveEvent() {
			using ( var events = new MultiAsyncEvent() ) {
				switch ( _type ) {
					case ProcessBody.Type.FirstWork:
						events.AddLast( async _ => {
							foreach ( var p in _processes ) {
								try										{ await p.RunActiveEvent(); }
								catch ( OperationCanceledException )	{}
							}
						} );
						events.AddLast( async _ => {
							foreach ( var p in _children ) {
								await p.RunActiveEvent();
							}
						} );
						break;

					case ProcessBody.Type.Work:
						events.AddLast( async _ => await UniTask.WhenAll(
							_processes.Select( async p => {
								try										{ await p.RunActiveEvent(); }
								catch ( OperationCanceledException )	{}
							} )
							.Concat(
								_children.Select( p => p.RunActiveEvent() )
							)
						) );
						break;
				}
				using ( var canceler = new CancellationTokenSource() ) {
					await events.Run( canceler.Token );
				}
			}
		}


		public override string ToString() {
			var result = $"{this.GetAboutName()}(\n"
				+ $"    _type : {_type}\n"
				+ $"    _lifeSpan : {_lifeSpan}\n"
				+ $"    _belongSceneName : {_belongSceneName}\n"
				+ $"    _owner : {_owner}\n";

			result += $"    _processes : \n";
			_processes.ForEach( ( p, i ) => result += $"        {i} : {p.GetAboutName()}\n" );

			new KeyValuePair<string, ProcessHierarchy>[] {
				new KeyValuePair<string, ProcessHierarchy>( "_top", _top ),
				new KeyValuePair<string, ProcessHierarchy>( "_parent", _parent ),
			}
			.ForEach( pair => {
				var s = (
					pair.Value == null			? string.Empty :
					pair.Value == this			? "this" :
					pair.Value._owner != null	? $"{pair.Value._owner}"
												: $"{pair.Value._processes.FirstOrDefault()}"
				);
				result += $"    {pair.Key} : {s}\n";
			} );

			result += $"    _children : \n";
			_children.ForEach( ( h, i ) => result += $"        {i} : {h._owner}\n" );

			result += ")";
			return result;
		}
	}
}