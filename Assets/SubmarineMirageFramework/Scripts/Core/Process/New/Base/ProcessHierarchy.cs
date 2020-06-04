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
	using ActiveState = ProcessBody.ActiveState;


	// TODO : コメント追加、整頓


	public class ProcessHierarchy : IDisposableExtension {
		public Type _type			{ get; private set; }
		public LifeSpan _lifeSpan	{ get; private set; }
		public BaseScene _scene		{ get; private set; }
		public ProcessHierarchyManager _hierarchies => _scene?._hierarchies;

		public GameObject _owner	{ get; private set; }
		public IProcess _process	{ get; private set; }
		public ProcessHierarchy _previous	{ get; private set; }
		public ProcessHierarchy _next		{ get; private set; }
		public ProcessHierarchy _parent		{ get; private set; }
		public ProcessHierarchy _child		{ get; private set; }
		public ProcessHierarchy _top		{ get; private set; }

		public bool _isTop => _top == this;
		int _isRunningCount;

		readonly CancellationTokenSource _asyncCanceler = new CancellationTokenSource();
		CancellationToken _asyncCancel => _asyncCanceler.Token;

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public ProcessHierarchy( GameObject owner, IEnumerable<IProcess> processes, ProcessHierarchy parent ) {
#if TestProcess
			Log.Debug( $"start : {processes.FirstOrDefault().GetAboutName()}.{this}" );
#endif
			_owner = owner;

			SetProcesses( processes );
			SetParent( parent );
			SetChildren();
			if ( _parent == null )	{ SetTop(); }

			_disposables.AddLast( () => GetProcesses().ForEach( p => p.Dispose() ) );
			_disposables.AddLast( () => GetChildren().ForEach( p => p.Dispose() ) );
			_disposables.AddLast( () => {
				_asyncCanceler.Cancel();
				_asyncCanceler.Dispose();
			} );
			_disposables.AddLast( () => UnLink() );
			
#if TestProcess
			Log.Debug( $"end : {_process.GetAboutName()}.{this}" );
#endif
		}

		~ProcessHierarchy() => Dispose();

		public void Dispose() => _disposables.Dispose();

		void UnLink() {
			if ( _parent?._child == this )	{ _parent._child = _next; }
			_parent = null;

			if ( _previous != null )	{ _previous._next = _next; }
			if ( _next != null )		{ _next._previous = _previous; }
			_previous = null;
			_next = null;
		}



		void SetProcesses( IEnumerable<IProcess> processes ) {
#if TestProcess
			Log.Debug( $"start {nameof( SetProcesses )} : {this}" );
#endif
			_process = processes.First();
			IProcess last = null;
			processes.ForEach( p => {
				if ( last != null ) {
					last._next = p;
					p._previous = last;
				}
				last = p;

				p._hierarchy = this;
				if ( _owner != null )	{ ( (MonoBehaviourProcess)p ).Constructor(); }
			} );
#if TestProcess
			Log.Debug( $"end {nameof( SetProcesses )} : {this}" );
#endif
		}

		public void SetParent( ProcessHierarchy parent ) {
			if ( _owner == null )	{ return; }
#if TestProcess
			Log.Debug( $"start {nameof( SetParent )} : {this}" );
#endif
			if ( parent != null )	{ parent.AddChild( this ); }
			else					{ Add( this ); }
#if TestProcess
			Log.Debug( $"end {nameof( SetParent )} : {this}" );
#endif
		}

		void SetChildren() {
			GetAllChildren();

			if ( _owner == null )	{ return; }
#if TestProcess
			Log.Debug( $"start {nameof( SetChildren )} : {this}" );
#endif
			var currents = Enumerable.Empty<Transform>()
				.Concat( _owner.transform );
			while ( !currents.IsEmpty() ) {
				var children = Enumerable.Empty<Transform>();
				currents.ForEach( t => {
					foreach ( Transform child in t ) {
						var ps = child.GetComponents<MonoBehaviourProcess>();
						if ( !ps.IsEmpty() ) {
							new ProcessHierarchy( child.gameObject, ps, this );
						} else {
							children.Concat( child );
						}
					}
				} );
				currents = children;
			}
#if TestProcess
			Log.Debug( $"end {nameof( SetChildren )} : {this}" );
#endif
		}

		public void SetTop() {
#if TestProcess
			Log.Debug( $"start {nameof( SetTop )} : {this}" );
#endif
			for ( _top = this; _top._parent != null; _top = _top._parent )	{}
			SetAllData();
#if TestProcess
			Log.Debug( $"end {nameof( SetTop )} : {this}" );
#endif
		}

		public void SetAllData() {
#if TestProcess
			Log.Debug( $"start {nameof( SetAllData )} : {this}" );
#endif
			var lastType = _top._type;
			var lastScene = _top._scene;
			var allHierarchy = _top.GetAllChildren();
			var allProcesses = allHierarchy.SelectMany( h => h.GetProcesses() );
#if TestProcess
			Log.Debug(
				$"{nameof( allProcesses )} : \n"
				+ $"{string.Join( ", ", allProcesses.Select( p => p.GetAboutName() ) )}"
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
			Log.Debug( $"end {nameof( SetAllData )} : {this}" );
#endif
		}


		public ProcessHierarchy GetFirst() {
			var first = _parent?._child;
			if ( first != null )	{ return first; }

			ProcessHierarchy current = null;
			for ( current = this; current._previous != null; current = current._previous )	{}
			return current;
		}
		public ProcessHierarchy GetLast() {
			ProcessHierarchy current = null;
			for ( current = this; current._next != null; current = current._next )	{}
			return current;
		}
		public ProcessHierarchy GetLastChild() {
			ProcessHierarchy current = null;
			for ( current = _child; current?._next != null; current = current._next )	{}
			return current;
		}
		public IEnumerable<ProcessHierarchy> GetBrothers() {
			for ( var current = GetFirst(); current != null; current = current._next )	{
				yield return current;
			}
		}
		public IEnumerable<ProcessHierarchy> GetAllParents() {
			for ( var current = this; current != null; current = current._parent ) {
				yield return current;
			}
		}
		public IEnumerable<ProcessHierarchy> GetChildren() {
			for ( var current = _child; current != null; current = current._next ) {
				yield return current;
			}
		}
		public IEnumerable<ProcessHierarchy> GetAllChildren() {
			var currents = Enumerable.Empty<ProcessHierarchy>()
				.Concat( this );
			while ( !currents.IsEmpty() ) {
				var children = Enumerable.Empty<ProcessHierarchy>();
				foreach ( var h in currents ) {
					yield return  h;
					children.Concat( h.GetChildren() );
				}
				currents = children;
			}
		}


		public T GetProcess<T>() where T : IProcess
			=> (T)GetProcesses().FirstOrDefault( p => p is T );

		public IProcess GetProcess( System.Type type )
			=> GetProcesses().FirstOrDefault( p => p.GetType() == type );

		public IProcess GetProcessAtLast() {
			IProcess current = null;
			for ( current = _process; current._next != null; current = current._next )	{}
			return current;
		}
		public IEnumerable<IProcess> GetProcesses() {
			for ( var current = _process; current != null; current = current._next ) {
				yield return current;
			}
		}
		public IEnumerable<T> GetProcesses<T>() where T : IProcess {
			return GetProcesses()
				.Where( p => p is T )
				.Select( p => (T)p );
		}
		public IEnumerable<IProcess> GetProcesses( System.Type type ) {
			return GetProcesses()
				.Where( p => p.GetType() == type );
		}

		public T GetProcessInParent<T>() where T : IProcess {
			return GetAllParents()
				.Select( h => h.GetProcess<T>() )
				.FirstOrDefault( p => p != null );
		}
		public IProcess GetProcessInParent( System.Type type ) {
			return GetAllParents()
				.Select( h => h.GetProcess( type ) )
				.FirstOrDefault( p => p != null );
		}

		public IEnumerable<T> GetProcessesInParent<T>() where T : IProcess {
			return GetAllParents()
				.SelectMany( h => h.GetProcesses<T>() );
		}
		public IEnumerable<IProcess> GetProcessesInParent( System.Type type ) {
			return GetAllParents()
				.SelectMany( h => h.GetProcesses( type ) );
		}

		public T GetProcessInChildren<T>() where T : IProcess {
			return GetAllChildren()
				.Select( h => h.GetProcess<T>() )
				.FirstOrDefault( p => p != null );
		}
		public IProcess GetProcessInChildren( System.Type type ) {
			return GetAllChildren()
				.Select( h => h.GetProcess( type ) )
				.FirstOrDefault( p => p != null );
		}

		public IEnumerable<T> GetProcessesInChildren<T>() where T : IProcess {
			return GetAllChildren()
				.SelectMany( h => h.GetProcesses<T>() );
		}
		public IEnumerable<IProcess> GetProcessesInChildren( System.Type type ) {
			return GetAllChildren()
				.SelectMany( h => h.GetProcesses( type ) );
		}



		void Add( ProcessHierarchy hierarchy ) {
			hierarchy.UnLink();

			var last = GetLast();
			hierarchy._parent = last._parent;
			hierarchy._previous = last;
			last._next = hierarchy;
		}
		void AddChild( ProcessHierarchy hierarchy ) {
			hierarchy.UnLink();

			hierarchy._parent = this;
			var last = GetLastChild();
			if ( last != null ) {
				hierarchy._previous = last;
				last._next = hierarchy;
			} else {
				_child = hierarchy;
			}
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
				switch ( state ) {
					case RanState.Creating:
						break;
					case RanState.Loading:
					case RanState.Initializing:
						await UniTaskUtility.WaitWhile( _asyncCancel, () => _isRunningCount > 0 );
						break;
					case RanState.FixedUpdate:
					case RanState.Update:
					case RanState.LateUpdate:
						return;
					case RanState.Finalizing:
						break;
				}
			}

			using ( var events = new MultiAsyncEvent() ) {
				switch ( _type ) {
					case Type.FirstWork:
						foreach ( var p in GetProcesses() ) {
							events.AddLast( async _ => {
								try										{ await p.RunStateEvent( state ); }
								catch ( OperationCanceledException )	{}
							} );
						}
						events.AddLast( async _ => {
							foreach ( var h in GetChildren() ) {
								await h.RunStateEvent( state );
							}
						} );
						break;

					case Type.Work:
						events.AddLast( async _ => await UniTask.WhenAll(
							GetProcesses().Select( async p => {
								try										{ await p.RunStateEvent( state ); }
								catch ( OperationCanceledException )	{}
							} )
							.Concat(
								GetChildren().Select( h => h.RunStateEvent( state ) )
							)
						) );
						break;

					case Type.DontWork:
						if ( state != RanState.Creating )	{ break; }
						events.AddLast( async _ => await UniTask.WhenAll(
							GetProcesses().Select( async p => {
								try										{ await p.RunStateEvent( state ); }
								catch ( OperationCanceledException )	{}
							} )
							.Concat(
								GetChildren().Select( h => h.RunStateEvent( state ) )
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


		bool IsCanChangeActive( bool isChangeOwner ) {
			if ( _parent?._owner != null && !_parent._owner.activeInHierarchy )	{ return false; }
			if ( !isChangeOwner && _owner != null && !_owner.activeSelf )		{ return false; }
			return true;
		}


		public async UniTask ChangeActive( bool isActive, bool isChangeOwner ) {
			// 活動状態を、高速で沢山切り替えた場合、全部、待機実行される（間を端折らない為、遅い）
			// しかし現状、各Processの活動状態が、それぞれ違う値になる為、暫定実装
			await UniTaskUtility.WaitWhile( _asyncCancel, () => _isRunningCount > 0 );

			using ( var events = new MultiAsyncEvent() ) {
				var isCanChangeActive = false;
				if ( isChangeOwner && _owner != null && _type != Type.DontWork ) {
					events.AddLast( async _ => {
// TODO : Disable時でも、Activeにしてしまうが、Managerの方で呼ばないはず、確認する
						_owner.SetActive( isActive );
						await UniTaskUtility.DontWait();
						Log.Debug( $"{_owner.GetAboutName()}.SetActive : {isActive}" );
					} );
				}
				if ( isActive ) {
					events.AddLast( async _ => {
						isCanChangeActive = IsCanChangeActive( isChangeOwner );
						await UniTaskUtility.DontWait();
					} );
				}

				switch ( _type ) {
					case Type.FirstWork:
						foreach ( var p in GetProcesses() ) {
							events.AddLast( async _ => {
								if ( !isCanChangeActive )	{ return; }
								try										{ await p.ChangeActive( isActive ); }
								catch ( OperationCanceledException )	{}
							} );
						}
						events.AddLast( async _ => {
							if ( !isCanChangeActive )	{ return; }
							foreach ( var h in GetChildren() ) {
								await h.ChangeActive( isActive, false );
							}
						} );
						break;

					case Type.Work:
						events.AddLast( async _ => {
							if ( !isCanChangeActive )	{ return; }
							await UniTask.WhenAll(
								GetProcesses().Select( async p => {
									try										{ await p.ChangeActive( isActive ); }
									catch ( OperationCanceledException )	{}
								} )
								.Concat(
									GetChildren()
										.Select( h => h.ChangeActive( isActive, false ) )
								)
							);
						} );
						break;
				}

				if ( !isActive ) {
					events.AddLast( async _ => {
						isCanChangeActive = IsCanChangeActive( isChangeOwner );
						await UniTaskUtility.DontWait();
					} );
					events.Reverse();
				}
				try {
					_isRunningCount++;
					await events.Run( _asyncCancel );
				} finally {
					_isRunningCount--;
				}
			}
		}


// TODO : 親がdisableでも、enableしそう
		public async UniTask RunActiveEvent() {
			await UniTaskUtility.WaitWhile( _asyncCancel, () => _isRunningCount > 0 );

			using ( var events = new MultiAsyncEvent() ) {
				switch ( _type ) {
					case Type.FirstWork:
						foreach ( var p in GetProcesses() ) {
							events.AddLast( async _ => {
								try										{ await p.RunActiveEvent(); }
								catch ( OperationCanceledException )	{}
							} );
						}
						events.AddLast( async _ => {
							foreach ( var h in GetChildren() ) {
								await h.RunActiveEvent();
							}
						} );
						break;

					case Type.Work:
						events.AddLast( async _ => await UniTask.WhenAll(
							GetProcesses().Select( async p => {
								try										{ await p.RunActiveEvent(); }
								catch ( OperationCanceledException )	{}
							} )
							.Concat(
								GetChildren().Select( h => h.RunActiveEvent() )
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

			result += $"    {nameof( GetProcesses )} : \n";
			GetProcesses().ForEach( ( p, i ) =>
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
												: $"{pair.Value._process}"
				);
				result += $"    {pair.Key} : {s}\n";
			} );

			result += $"    {nameof( GetChildren )} : \n";
			GetChildren().ForEach( ( h, i ) => result += $"        {i} : {h._owner}\n" );

			result += ")";
			return result;
		}
	}
}