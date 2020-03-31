//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process.New {
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
	using UnityObject = UnityEngine.Object;


	// TODO : コメント追加、整頓


	public class ProcessHierarchy : IDisposable {
		GameObject _owner;
		readonly List<IProcess> _processes = new List<IProcess>();
		ProcessHierarchy _parent;
		ProcessHierarchy _child;

		public ProcessBody.Type _type	{ get; private set; }
		public string _belongSceneName	{ get; private set; }

		readonly MultiDisposable _disposables = new MultiDisposable();


		public ProcessHierarchy( GameObject owner ) {
			_owner = owner;

			SetParent();
			SetBrothers();

// TODO : 始祖の親から、末端の子まで全判定し、_type、_belongSceneNameを決定する
			var parent = _parent;
			while ( parent._parent != null ) {
				parent = parent._parent;
			}
			if ( parent != null ) {

			}
			_type = _processes.Any( p => p._type == ProcessBody.Type.FirstWork ) ?
				ProcessBody.Type.FirstWork : ProcessBody.Type.Work;
			var lifeSpan = _processes.Any( p => p._lifeSpan == ProcessBody.LifeSpan.Forever ) ?
				ProcessBody.LifeSpan.Forever : ProcessBody.LifeSpan.InScene;
			_belongSceneName = (
				lifeSpan == ProcessBody.LifeSpan.Forever	? ProcessBody.FOREVER_SCENE_NAME :
				_owner != null								? _owner.scene.name
															: SceneManager.s_instance._currentSceneName
			);
			if ( _owner != null && lifeSpan == ProcessBody.LifeSpan.Forever ) {
				UnityObject.DontDestroyOnLoad( _owner );
			}

			_disposables.AddLast( () => {
				if ( _parent != null )	{ _parent._child = null; }
			} );
			_disposables.AddLast( _processes );
			_disposables.AddLast( _child );

			if ( _parent == null ) {
//				CoreProcessManager.s_instance.Register( this ).Forget();
//				_disposables.AddLast( () => CoreProcessManager.s_instance.Unregister( this ) );
			}
		}

		~ProcessHierarchy() => Dispose();

		public void Dispose() => _disposables.Dispose();


		void SetParent() {
			if ( _owner == null )	{ return; }

			if ( _parent != null ) {
				_parent._child = null;
				_parent = null;
			}
			var parent = _owner.GetComponentInParentUntilOneHierarchy<MonoBehaviourProcess>( true );
			if ( parent == null )	{ return; }

			if ( parent._processHierarchy == null ) {
				parent._processHierarchy = new ProcessHierarchy( parent.gameObject );
			}
			_parent = parent._processHierarchy;
			_parent._child = this;
		}

		void SetBrothers() {
			if ( _owner == null )	{ return; }

			_processes.Clear();
			_owner.GetComponents<MonoBehaviourProcess>().ForEach( p => {
				if ( p._processHierarchy == null )	{ p._processHierarchy = this; }
				_processes.Add( p );
			} );
		}

		public void ChangeParent( Transform parent, bool isWorldPositionStays ) {
			_owner.transform.SetParent( parent, isWorldPositionStays );
			SetParent();
		}


		public async UniTask RunStateEvent( ProcessBody.RanState state ) {
			using ( var events = new MultiAsyncEvent() ) {
				events.AddLast( async _ => {
					foreach ( var p in _processes ) {
						await p.RunStateEvent( state );
					}
				} );
				events.AddLast( async _ => {
					if ( _child != null )	{ await _child.RunStateEvent( state ); }
				} );
				if ( state == ProcessBody.RanState.Finalizing )	{ events.Reverse(); }

				using ( var canceler = new CancellationTokenSource() ) {
					await events.Run( canceler.Token );
				}
			}
		}


		public async UniTask ChangeActive( bool isActive, bool isChangeOwner ) {
			using ( var events = new MultiAsyncEvent() ) {
				events.AddLast( async _ => {
					if ( _owner != null && isChangeOwner )	{ _owner.SetActive( isActive ); }
					await UniTaskUtility.DontWait();
				} );
				events.AddLast( async _ => {
					foreach ( var p in _processes ) {
						await p.ChangeActive( isActive );
					}
				} );
				events.AddLast( async _ => {
					if ( _child != null )	{ await _child.ChangeActive( isActive, false ); }
					await UniTaskUtility.DontWait();
				} );
				if ( !isActive )	{ events.Reverse(); }

				using ( var canceler = new CancellationTokenSource() ) {
					await events.Run( canceler.Token );
				}
			}
		}
	}
}