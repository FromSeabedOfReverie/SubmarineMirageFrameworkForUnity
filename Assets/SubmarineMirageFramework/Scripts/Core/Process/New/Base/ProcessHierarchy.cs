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
	using Type = ProcessBody.Type;
	using LifeSpan = ProcessBody.LifeSpan;
	using RanState = ProcessBody.RanState;


	// TODO : コメント追加、整頓


	public class ProcessHierarchy : IDisposable {
		public Type _type			{ get; private set; }
		public LifeSpan _lifeSpan	{ get; private set; }
		public BaseScene _scene		{ get; private set; }
		public ProcessHierarchyManager _hierarchies => _scene?._hierarchies;

		public GameObject _owner		{ get; private set; }
		public ProcessHierarchy _top	{ get; private set; }
		public ProcessHierarchy _parent	{ get; private set; }
		public readonly List<ProcessHierarchy> _children = new List<ProcessHierarchy>();
		public readonly List<IProcess> _processes = new List<IProcess>();
		public bool _isTop => _top == this;

		readonly MultiDisposable _disposables = new MultiDisposable();


		public ProcessHierarchy( GameObject owner, IEnumerable<IProcess> processes, ProcessHierarchy parent ) {
			Log.Debug( $"作成開始 : {processes.FirstOrDefault().GetAboutName()}.{this}" );
			_owner = owner;

			SetBrothers( processes );
			SetParent( parent );
			SetChildren();
			if ( _parent == null )	{ SetTop(); }

			_disposables.AddLast( () => _processes.ForEach( p => p.Dispose() ) );
			_disposables.AddLast( () => _children.ForEach( p => p.Dispose() ) );

			Log.Debug( $"作成 : {_processes.FirstOrDefault().GetAboutName()}.{this}" );
		}

		~ProcessHierarchy() => Dispose();

		public void Dispose() => _disposables.Dispose();


		void SetBrothers( IEnumerable<IProcess> processes ) {
			Log.Debug( $"start SetBrothers : {this}" );
			_processes.Clear();
			_processes.Add( processes );
			_processes.ForEach( p => p._hierarchy = this );
			if ( _owner != null ) {
				_processes
					.Select( p => (MonoBehaviourProcess)p )
					.ForEach( p => p.Constructor() );
			}
			Log.Debug( $"end SetBrothers : {this}" );
		}

		public void SetParent( ProcessHierarchy parent ) {
			if ( _owner == null )	{ return; }
			Log.Debug( $"start SetParent : {this}" );

			_parent?._children.Remove( this );
			_parent = parent;
			_parent?._children.Add( this );
			Log.Debug( $"end SetParent : {this}" );
		}

		void SetChildren() {
			if ( _owner == null )	{ return; }
			Log.Debug( $"start SetChildren : {this}" );

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
			Log.Debug( $"end SetChildren : {this}" );
		}

		public void SetTop() {
			Log.Debug( $"start SetTop : {this}" );
			for ( _top = this; _top._parent != null; _top = _top._parent )	{}
			SetAllData();
			Log.Debug( $"end SetTop : {this}" );
		}

		public void SetAllData() {
			Log.Debug( $"start SetAllData : {this}" );
			var lastType = _top._type;
			var lastScene = _top._scene;
			var allHierarchy = _top.GetHierarchiesInChildren();
			var allProcesses = allHierarchy.SelectMany( h => h._processes );
			Log.Debug(
				$"allProcesses : \n{string.Join( ", ", allProcesses.Select( p => p.GetAboutName() ) )}"
			);

			_top._type = (
				allProcesses.Any( p => p._type == Type.FirstWork )	? Type.FirstWork :
				allProcesses.Any( p => p._type == Type.Work )		? Type.Work
																	: Type.DontWork
			);
			_top._lifeSpan = allProcesses.Any( p => p._lifeSpan == LifeSpan.Forever ) ?
				LifeSpan.Forever : LifeSpan.InScene;
			_top._scene = (
				// SceneManager作成時の場合、循環参照になる為、設定出来ない
				!SceneManager.s_isCreated			? null :
				_top._lifeSpan == LifeSpan.Forever	? SceneManager.s_instance._fsm._foreverScene :
				_top._owner != null					? SceneManager.s_instance._fsm.Get( _top._owner.scene ) :
				SceneManager.s_instance._fsm._scene != null
													? SceneManager.s_instance._fsm._scene
													: SceneManager.s_instance._fsm._startScene
			);

			allHierarchy.ForEach( h => {
				h._top = _top;
				h._type = _top._type;
				h._lifeSpan = _top._lifeSpan;
				h._scene = _top._scene;
			} );

			if ( lastScene == null ) {
//				Log.Debug( $"Register : {_top}" );
				// SceneManager作成時の場合、循環参照になる為、設定出来ない
				_hierarchies?.Register( _top );
			} else if ( _top._type != lastType || _top._scene != lastScene ) {
//				Log.Debug( $"ReRegister : {_top}" );
				_hierarchies.ReRegister( _top, lastType, lastScene );
			} else {
//				Log.Debug( $"DontRegister : {_top}" );
			}
			Log.Debug( $"end SetAllData : {this}" );
		}




		public T GetProcess<T>() where T : IProcess {
			return (T)_processes.FirstOrDefault( p => p is T );
		}

		public List<ProcessHierarchy> GetHierarchiesInChildren() {
			var result = new List<ProcessHierarchy>();
			var currents = new List<ProcessHierarchy> { this };
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

		public T Add<T>() where T : MonoBehaviourProcess => _hierarchies.Add<T>( this );

		public void Destroy() => _hierarchies.Destroy( this );

		public void ChangeParent( Transform parent, bool isWorldPositionStays )
			=> _hierarchies.ChangeParent( this, parent, isWorldPositionStays );




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