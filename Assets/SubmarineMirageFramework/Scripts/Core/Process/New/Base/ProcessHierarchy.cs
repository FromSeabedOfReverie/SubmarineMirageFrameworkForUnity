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
	using Type = ProcessBody.Type;
	using LifeSpan = ProcessBody.LifeSpan;
	using RanState = ProcessBody.RanState;
	using ActiveState = ProcessBody.ActiveState;


	// TODO : コメント追加、整頓


	public class ProcessHierarchy : IDisposable {
		public Type _type			{ get; private set; }
		public LifeSpan _lifeSpan	{ get; private set; }
		public BaseScene _scene		{ get; private set; }

		public GameObject _owner	{ get; private set; }
		public readonly List<IProcess> _processes = new List<IProcess>();
		public ProcessHierarchy _parent;
		public readonly List<ProcessHierarchy> _children = new List<ProcessHierarchy>();
		public ProcessHierarchy _top;

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


		public void SetParent( ProcessHierarchy parent ) {
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

		public void SetTop() {
			if ( _top != null )	{ return; }
			for ( _top = this; _top._parent != null; _top = _top._parent );
			SetAllHierarchiesData( _top );
		}

		void SetAllHierarchiesData( ProcessHierarchy parent ) {
			var allHierarchy = GetHierarchiesInChildren( parent );
			var allProcesses = allHierarchy.SelectMany( h => h._processes );

			_type = (
				allProcesses.Any( p => p._type == Type.FirstWork )	? Type.FirstWork :
				allProcesses.Any( p => p._type == Type.Work )		? Type.Work
																	: Type.DontWork
			);
			_lifeSpan = allProcesses.Any( p => p._lifeSpan == LifeSpan.Forever ) ?
				LifeSpan.Forever : LifeSpan.InScene;
			_scene = (
				_lifeSpan == LifeSpan.Forever	? SceneManager.s_instance._fsm._foreverScene :
				_top._owner != null				? SceneManager.s_instance._fsm.Get( _top._owner.scene )
												: SceneManager.s_instance._fsm._scene
			);

			allHierarchy.ForEach( h => {
				h._top = _top;
				h._type = _type;
				h._lifeSpan = _lifeSpan;
				h._scene = _scene;
			} );

// TODO : 登録解除、再登録システムを作成する
			_top._scene._hierarchies.Register( _top );
			_disposables.AddLast( "Unregister", () => _top._scene._hierarchies.Unregister( _top ) );
		}


		public List<ProcessHierarchy> GetHierarchiesInChildren( ProcessHierarchy parent ) {
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


		public async UniTask RunStateEvent( RanState state ) {
			using ( var events = new MultiAsyncEvent() ) {
				switch ( _type ) {
					case Type.FirstWork:
						events.AddLast( async _ => {
							foreach ( var p in _processes ) {
								try										{ await p.RunStateEvent( state ); }
								catch ( OperationCanceledException )	{}
							}
						} );
						events.AddLast( async _ => {
							foreach ( var h in _children ) {
								await h.RunStateEvent( state );
							}
						} );
						break;

					case Type.Work:
						events.AddLast( async _ => await UniTask.WhenAll(
							_processes.Select( async p => {
								try										{ await p.RunStateEvent( state ); }
								catch ( OperationCanceledException )	{}
							} )
							.Concat(
								_children.Select( h => h.RunStateEvent( state ) )
							)
						) );
						break;
				}
				if ( state == RanState.Finalizing )	{ events.Reverse(); }
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
					case Type.FirstWork:
						events.AddLast( async _ => {
							foreach ( var p in _processes ) {
								try										{ await p.ChangeActive( isActive ); }
								catch ( OperationCanceledException )	{}
							}
						} );
						events.AddLast( async _ => {
							foreach ( var h in _children ) {
								await h.ChangeActive( isActive, false );
							}
						} );
						break;

					case Type.Work:
						events.AddLast( async _ => await UniTask.WhenAll(
							_processes.Select( async p => {
								try										{ await p.ChangeActive( isActive ); }
								catch ( OperationCanceledException )	{}
							} )
							.Concat(
								_children.Select( h => h.ChangeActive( isActive, false ) )
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
					case Type.FirstWork:
						events.AddLast( async _ => {
							foreach ( var p in _processes ) {
								try										{ await p.RunActiveEvent(); }
								catch ( OperationCanceledException )	{}
							}
						} );
						events.AddLast( async _ => {
							foreach ( var h in _children ) {
								await h.RunActiveEvent();
							}
						} );
						break;

					case Type.Work:
						events.AddLast( async _ => await UniTask.WhenAll(
							_processes.Select( async p => {
								try										{ await p.RunActiveEvent(); }
								catch ( OperationCanceledException )	{}
							} )
							.Concat(
								_children.Select( h => h.RunActiveEvent() )
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
				+ $"    _scene : {_scene}\n"
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