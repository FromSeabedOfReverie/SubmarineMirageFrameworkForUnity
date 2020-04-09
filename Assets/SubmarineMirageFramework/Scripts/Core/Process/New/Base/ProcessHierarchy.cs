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

		GameObject _owner;
		readonly List<IProcess> _processes = new List<IProcess>();
		ProcessHierarchy _parent;
		ProcessHierarchy _child;
		ProcessHierarchy _top;

		readonly MultiDisposable _disposables = new MultiDisposable();


		public ProcessHierarchy( GameObject owner, IEnumerable<IProcess> processes ) {
			_owner = owner;

			SetParent();
			SetBrothers( processes );

			_disposables.AddLast( () => {
				if ( _parent != null )	{ _parent._child = null; }
			} );
			_disposables.AddLast( _processes );
			_disposables.AddLast( _child );

			SetTopData();

			Log.Debug( $"作成 : {this}" );
		}

		~ProcessHierarchy() => Dispose();

		public void Dispose() => _disposables.Dispose();


		void SetParent() {
			if ( _owner == null )	{ return; }

			if ( _parent != null ) {
				_parent._child = null;
				_parent = null;
			}
			var parents = _owner.GetComponentsInParentUntilOneHierarchy<MonoBehaviourProcess>( true );
			if ( parents.IsEmpty() )	{ return; }
			var parent = parents.FirstOrDefault( p => p._hierarchy != null );
			if ( parent == null ) {
				parent = parents.FirstOrDefault();
				parent._hierarchy = new ProcessHierarchy( parent.gameObject, parents );
			}
			_parent = parent._hierarchy;
			_parent._child = this;
		}

		public void SetBrothers( IEnumerable<IProcess> processes ) {
			_processes.Clear();
			_processes.Add( processes );
			_processes.ForEach( p => p._hierarchy = this );
		}

		void SetTopData() {
			if ( _top != null )	{ return; }
			for ( _top = this; _top._parent != null; _top = _top._parent ) {
			}

			var allHierarchy = new List<ProcessHierarchy>();
			var allProcesses = new List<IProcess>();
			for ( var h = _top; h != null; h = h._child ) {
				allHierarchy.Add( h );
				allProcesses.Add( h._processes );
			}

			_type = (
				allProcesses.Any( p => p._type == ProcessBody.Type.FirstWork )	? ProcessBody.Type.FirstWork :
				allProcesses.Any( p => p._type == ProcessBody.Type.Work )		? ProcessBody.Type.Work
																				: ProcessBody.Type.DontWork
			);
			_lifeSpan = allProcesses.Any( p => p._lifeSpan == ProcessBody.LifeSpan.Forever ) ?
				ProcessBody.LifeSpan.Forever : ProcessBody.LifeSpan.InScene;
			_belongSceneName = (
				_lifeSpan == ProcessBody.LifeSpan.Forever	? ProcessBody.FOREVER_SCENE_NAME :
				_owner != null								? _owner.scene.name
															: SceneManager.s_instance._currentSceneName
			);
			if ( _owner != null && _lifeSpan == ProcessBody.LifeSpan.Forever ) {
// TODO : DontDestroyOnLoad後に、シーンのルートオブジェクトを取得する方法は無い為、
//			フラグ付与前にシーンから取得し、初期化関数を呼び、関数内でフラグを設定する
				UnityObject.DontDestroyOnLoad( _owner );
			}

			allHierarchy.ForEach( h => {
				h._top = _top;
				h._type = _type;
				h._lifeSpan = _lifeSpan;
				h._belongSceneName = _belongSceneName;
			} );

			if ( _top == this ) {
//				CoreProcessManager.s_instance.Register( this ).Forget();
//				_disposables.AddLast( "Unregister", () => CoreProcessManager.s_instance.Unregister( this ) );
			}
		}


		public void ChangeParent( Transform parent, bool isWorldPositionStays ) {
			if ( _owner == null )	{ return; }

			_owner.transform.SetParent( parent, isWorldPositionStays );
//			if ( _top == this )	{ _disposables.Remove( "Unregister" ); }
			SetParent();

			if ( _parent == null ) {
				_top = null;
				SetTopData();
			} else {
				for ( var h = this; h != null; h = h._child ) {
					h._top = h._parent._top;
					h._type = h._parent._type;
					h._lifeSpan = h._parent._lifeSpan;
					h._belongSceneName = h._parent._belongSceneName;
				}
			}
		}


// TODO : これらの関数を作成
		public void AddComponent() {
		}

		public void Instantiate() {
		}

		public void Destroy() {
		}


		public async UniTask RunStateEvent( ProcessBody.RanState state ) {
			using ( var events = new MultiAsyncEvent() ) {
				switch( _type ) {
					case ProcessBody.Type.FirstWork:
						events.AddLast( async _ => {
							foreach ( var p in _processes ) {
								try										{ await p.RunStateEvent( state ); }
								catch ( OperationCanceledException )	{}
							}
						} );
						if ( _child != null ) {
							events.AddLast( async _ => await _child.RunStateEvent( state ) );
						}
						break;

					case ProcessBody.Type.Work:
						events.AddLast( async _ => {
							var tasks = _processes.Select( p => {
								try										{ return p.RunStateEvent( state ); }
								catch ( OperationCanceledException )	{ return UniTaskUtility.DontWait(); }
							} ).ToList();
							if ( _child != null )	{ tasks.Add( _child.RunStateEvent( state ) ); }
							await UniTask.WhenAll( tasks );
						} );
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
				switch( _type ) {
					case ProcessBody.Type.FirstWork:
						events.AddLast( async _ => {
							foreach ( var p in _processes ) {
								try										{ await p.ChangeActive( isActive ); }
								catch ( OperationCanceledException )	{}
							}
						} );
						if ( _child != null ) {
							events.AddLast( async _ => await _child.ChangeActive( isActive, false ) );
						}
						break;

					case ProcessBody.Type.Work:
						events.AddLast( async _ => {
							var tasks = _processes.Select( p => {
								try										{ return p.ChangeActive( isActive ); }
								catch ( OperationCanceledException )	{ return UniTaskUtility.DontWait(); }
							} ).ToList();
							if ( _child != null )	{ tasks.Add( _child.ChangeActive( isActive, false ) ); }
							await UniTask.WhenAll( tasks );
						} );
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
				switch( _type ) {
					case ProcessBody.Type.FirstWork:
						events.AddLast( async _ => {
							foreach ( var p in _processes ) {
								try										{ await p.RunActiveEvent(); }
								catch ( OperationCanceledException )	{}
							}
						} );
						if ( _child != null ) {
							events.AddLast( async _ => await _child.RunActiveEvent() );
						}
						break;

					case ProcessBody.Type.Work:
						events.AddLast( async _ => {
							var tasks = _processes.Select( p => {
								try										{ return p.RunActiveEvent(); }
								catch ( OperationCanceledException )	{ return UniTaskUtility.DontWait(); }
							} ).ToList();
							if ( _child != null )	{ tasks.Add( _child.RunActiveEvent() ); }
							await UniTask.WhenAll( tasks );
						} );
						break;
				}
				using ( var canceler = new CancellationTokenSource() ) {
					await events.Run( canceler.Token );
				}
			}
		}


		public override string ToString() {
//			return this.ToDeepString();

			var result = $"{this.GetAboutName()}(\n"
				+ $"    _type : {_type}\n"
				+ $"    _lifeSpan : {_lifeSpan}\n"
				+ $"    _belongSceneName : {_belongSceneName}\n"
				+ $"    _owner : {_owner}\n";

			result += $"    _processes : \n";
			_processes.ForEach( (p, i) => result += $"        {i} : {p.GetAboutName()}\n" );

			new KeyValuePair<string, ProcessHierarchy>[] {
				new KeyValuePair<string, ProcessHierarchy>( "_parent", _parent ),
				new KeyValuePair<string, ProcessHierarchy>( "_child", _child ),
				new KeyValuePair<string, ProcessHierarchy>( "_top", _top ),
			}
			.ForEach( pair => {
				var s = (
					pair.Value == null			? string.Empty :
					pair.Value == this			? "this" :
					pair.Value._owner != null	? pair.Value._owner.ToString()
												: pair.Value._processes.FirstOrDefault().ToString()
				);
				result += $"    {pair.Key} : {s}\n";
			} );

			result += ")";
			return result;
		}
	}
}