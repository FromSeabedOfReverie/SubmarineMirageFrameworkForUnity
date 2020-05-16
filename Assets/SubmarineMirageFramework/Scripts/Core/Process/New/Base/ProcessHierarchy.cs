//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestProcess
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

		int _isRunningCount;

		readonly CancellationTokenSource _asyncCanceler = new CancellationTokenSource();
		CancellationToken _asyncCancel => _asyncCanceler.Token;

		readonly MultiDisposable _disposables = new MultiDisposable();


		public ProcessHierarchy( GameObject owner, IEnumerable<IProcess> processes, ProcessHierarchy parent ) {
#if TestProcess
			Log.Debug( $"start : {processes.FirstOrDefault().GetAboutName()}.{this}" );
#endif
			_owner = owner;

			SetBrothers( processes );
			SetParent( parent );
			SetChildren();
			if ( _parent == null )	{ SetTop(); }

			_disposables.AddLast( () => _processes.ForEach( p => p.Dispose() ) );
			_disposables.AddLast( () => _children.ForEach( p => p.Dispose() ) );
			_disposables.AddLast( () => {
				_asyncCanceler.Cancel();
				_asyncCanceler.Dispose();
			} );
			
#if TestProcess
			Log.Debug( $"end : {_processes.FirstOrDefault().GetAboutName()}.{this}" );
#endif
		}

		~ProcessHierarchy() => Dispose();

		public void Dispose() => _disposables.Dispose();


		void SetBrothers( IEnumerable<IProcess> processes ) {
#if TestProcess
			Log.Debug( $"start SetBrothers : {this}" );
#endif
			_processes.Clear();
			_processes.Add( processes );
			_processes.ForEach( p => p._hierarchy = this );
			if ( _owner != null ) {
				_processes
					.Select( p => (MonoBehaviourProcess)p )
					.ForEach( p => p.Constructor() );
			}
#if TestProcess
			Log.Debug( $"end SetBrothers : {this}" );
#endif
		}

		public void SetParent( ProcessHierarchy parent ) {
			if ( _owner == null )	{ return; }
#if TestProcess
			Log.Debug( $"start SetParent : {this}" );
#endif
			_parent?._children.Remove( this );
			_parent = parent;
			_parent?._children.Add( this );
#if TestProcess
			Log.Debug( $"end SetParent : {this}" );
#endif
		}

		void SetChildren() {
			if ( _owner == null )	{ return; }
#if TestProcess
			Log.Debug( $"start SetChildren : {this}" );
#endif
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
#if TestProcess
			Log.Debug( $"end SetChildren : {this}" );
#endif
		}

		public void SetTop() {
#if TestProcess
			Log.Debug( $"start SetTop : {this}" );
#endif
			for ( _top = this; _top._parent != null; _top = _top._parent )	{}
			SetAllData();
#if TestProcess
			Log.Debug( $"end SetTop : {this}" );
#endif
		}

		public void SetAllData() {
#if TestProcess
			Log.Debug( $"start SetAllData : {this}" );
#endif
			var lastType = _top._type;
			var lastScene = _top._scene;
			var allHierarchy = _top.GetHierarchiesInChildren();
			var allProcesses = allHierarchy.SelectMany( h => h._processes );
#if TestProcess
			Log.Debug(
				$"allProcesses : \n{string.Join( ", ", allProcesses.Select( p => p.GetAboutName() ) )}"
			);
#endif
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
#if TestProcess
				Log.Debug( $"Register : {_top}" );
#endif
				// SceneManager作成時の場合、循環参照になる為、設定出来ない
				_hierarchies?.Register( _top );
			} else if ( _top._type != lastType || _top._scene != lastScene ) {
#if TestProcess
				Log.Debug( $"ReRegister : {_top}" );
#endif
				_hierarchies.ReRegister( _top, lastType, lastScene );
			} else {
#if TestProcess
				Log.Debug( $"DontRegister : {_top}" );
#endif
			}
#if TestProcess
			Log.Debug( $"end SetAllData : {this}" );
#endif
		}


		public List<ProcessHierarchy> GetHierarchiesInParent() {
			var result = new List<ProcessHierarchy>();
			for ( var current = this; current != null; current = current._parent ) {
				result.Add( current );
			}
			return result;
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


		public T GetProcess<T>() where T : IProcess
			=> (T)_processes.FirstOrDefault( p => p is T );

		public IProcess GetProcess( System.Type type )
			=> _processes.FirstOrDefault( p => p.GetType() == type );

		public List<T> GetProcesses<T>() where T : IProcess {
			return _processes
				.Where( p => p is T )
				.Select( p => (T)p )
				.ToList();
		}
		public List<IProcess> GetProcesses( System.Type type ) {
			return _processes
				.Where( p => p.GetType() == type )
				.ToList();
		}

		public T GetProcessInParent<T>() where T : IProcess {
			return GetHierarchiesInParent()
				.Select( h => h.GetProcess<T>() )
				.FirstOrDefault( p => p != null );
		}
		public IProcess GetProcessInParent( System.Type type ) {
			return GetHierarchiesInParent()
				.Select( h => h.GetProcess( type ) )
				.FirstOrDefault( p => p != null );
		}

		public List<T> GetProcessesInParent<T>() where T : IProcess {
			return GetHierarchiesInParent()
				.SelectMany( h => h.GetProcesses<T>() )
				.ToList();
		}
		public List<IProcess> GetProcessesInParent( System.Type type ) {
			return GetHierarchiesInParent()
				.SelectMany( h => h.GetProcesses( type ) )
				.ToList();
		}

		public T GetProcessInChildren<T>() where T : IProcess {
			return GetHierarchiesInChildren()
				.Select( h => h.GetProcess<T>() )
				.FirstOrDefault( p => p != null );
		}
		public IProcess GetProcessInChildren( System.Type type ) {
			return GetHierarchiesInChildren()
				.Select( h => h.GetProcess( type ) )
				.FirstOrDefault( p => p != null );
		}

		public List<T> GetProcessesInChildren<T>() where T : IProcess {
			return GetHierarchiesInChildren()
				.SelectMany( h => h.GetProcesses<T>() )
				.ToList();
		}
		public List<IProcess> GetProcessesInChildren( System.Type type ) {
			return GetHierarchiesInChildren()
				.SelectMany( h => h.GetProcesses( type ) )
				.ToList();
		}


		public T AddProcess<T>() where T : MonoBehaviourProcess
			=> _hierarchies.AddProcess<T>( this );

		public MonoBehaviourProcess AddProcess( System.Type type )
			=> _hierarchies.AddProcess( this, type );


		public void Destroy()
			=> _hierarchies.Destroy( this );

		public void ChangeParent( Transform parent, bool isWorldPositionStays )
			=> _hierarchies.ChangeParent( this, parent, isWorldPositionStays );




		public async UniTask RunStateEvent( RanState state ) {
			if ( _isRunningCount > 0 ) {
				var isForcedRunState = false;
				switch ( state ) {
					case RanState.Creating:
					case RanState.Finalizing:
						isForcedRunState = true;
						break;
					case RanState.FixedUpdate:
					case RanState.Update:
					case RanState.LateUpdate:
						return;
				}
				await UniTaskUtility.WaitWhile(
					_asyncCancel, () => _isRunningCount > 0 && !isForcedRunState );
			}

			using ( var events = new MultiAsyncEvent() ) {
				switch ( _type ) {
					case Type.FirstWork:
						foreach ( var p in _processes ) {
							events.AddLast( async _ => {
								try										{ await p.RunStateEvent( state ); }
								catch ( OperationCanceledException )	{}
							} );
						}
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

					case Type.DontWork:
						if ( state != RanState.Creating )	{ break; }
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
				try {
					_isRunningCount++;
					await events.Run( _asyncCancel );
				} finally {
					_isRunningCount--;
				}
			}
		}


		public async UniTask ChangeActive( bool isActive, bool isChangeOwner ) {
// TODO : 活動状態を、高速で沢山切り替えた場合、全部、待機実行される（間を端折らない為、遅い）
			await UniTaskUtility.WaitWhile( _asyncCancel, () => _isRunningCount > 0 );

			using ( var events = new MultiAsyncEvent() ) {
				if ( isChangeOwner && _owner != null && _type != Type.DontWork ) {
					events.AddLast( async _ => {
// TODO : Disable時でも、Activeにしてしまうが、Managerの方で呼ばないはず、確認する
						_owner.SetActive( isActive );
						await UniTaskUtility.DontWait();
						Log.Debug( $"{_owner.GetAboutName()}.SetActive : {isActive}" );
					} );
				}
				switch ( _type ) {
					case Type.FirstWork:
						foreach ( var p in _processes ) {
							events.AddLast( async _ => {
								try										{ await p.ChangeActive( isActive ); }
								catch ( OperationCanceledException )	{}
							} );
						}
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
				try {
					_isRunningCount++;
					await events.Run( _asyncCancel );
				} finally {
					_isRunningCount--;
				}
			}
		}

		public async UniTask RunActiveEvent() {
			await UniTaskUtility.WaitWhile( _asyncCancel, () => _isRunningCount > 0 );

			using ( var events = new MultiAsyncEvent() ) {
				switch ( _type ) {
					case Type.FirstWork:
						foreach ( var p in _processes ) {
							events.AddLast( async _ => {
								try										{ await p.RunActiveEvent(); }
								catch ( OperationCanceledException )	{}
							} );
						}
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
				try {
					_isRunningCount++;
					await events.Run( _asyncCancel );
				} finally {
					_isRunningCount--;
				}
			}
		}


		public override string ToString() {
			var result = $"{this.GetAboutName()}(\n"
				+ $"    {nameof( _type )} : {_type}\n"
				+ $"    {nameof( _lifeSpan )} : {_lifeSpan}\n"
				+ $"    {nameof( _scene )} : {_scene}\n"
				+ $"    {nameof( _isRunningCount )} : {_isRunningCount}\n"
				+ $"    {nameof( _owner )} : {_owner}\n";

			result += $"    {nameof( _processes )} : \n";
			_processes.ForEach( ( p, i ) =>
				result += $"        {i} : {p.GetAboutName()}( "
					+ $"{p._body._ranState}, {p._body._activeState}, {p._body._nextActiveState} )\n"
			);

			new KeyValuePair<string, ProcessHierarchy>[] {
				new KeyValuePair<string, ProcessHierarchy>( $"{nameof( _top )}", _top ),
				new KeyValuePair<string, ProcessHierarchy>( $"{nameof( _parent )}", _parent ),
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

			result += $"    {nameof( _children )} : \n";
			_children.ForEach( ( h, i ) => result += $"        {i} : {h._owner}\n" );

			result += ")";
			return result;
		}
	}
}