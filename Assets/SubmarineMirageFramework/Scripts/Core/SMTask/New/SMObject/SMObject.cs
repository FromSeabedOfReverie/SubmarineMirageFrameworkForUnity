//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestSMObject
namespace SubmarineMirage.SMTask {
	using System;
	using System.Linq;
	using System.Threading;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx.Async;
	using KoganeUnityLib;
	using MultiEvent;
	using Modifyler;
	using Scene;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMObject : IDisposableExtension {
		public SMTaskType _type			{ get; private set; }
		public SMTaskLifeSpan _lifeSpan	{ get; private set; }
		public BaseScene _scene			{ get; private set; }
		public SMObjectManager _objects => _scene?._objects;

		public GameObject _owner		{ get; private set; }
		public SMObjectModifyler _modifyler	{ get; private set; }

		public ISMBehavior _behavior	{ get; private set; }
		public SMObject _previous		{ get; private set; }
		public SMObject _next			{ get; private set; }
		public SMObject _parent			{ get; private set; }
		public SMObject _child			{ get; private set; }
		public SMObject _top			{ get; private set; }

		public bool _isTop => _top == this;

		readonly CancellationTokenSource _asyncCanceler = new CancellationTokenSource();
		public CancellationToken _asyncCancel => _asyncCanceler.Token;

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public SMObject( GameObject owner, IEnumerable<ISMBehavior> behaviours, SMObject parent ) {
#if TestSMObject
			Log.Debug( $"start : {behaviours.FirstOrDefault().GetAboutName()}.{this}" );
#endif
			_owner = owner;

			_modifyler = new SMObjectModifyler( this );
			_disposables.AddLast( _modifyler );

			SetBehaviours( behaviours );
			SetParent( parent );
			SetChildren();
			if ( _parent == null )	{ SetTop(); }

			_disposables.AddLast( () => GetBehaviours().ForEach( b => b.Dispose() ) );
			_disposables.AddLast( () => GetChildren().ForEach( o => o.Dispose() ) );
			_disposables.AddLast( () => {
				_asyncCanceler.Cancel();
				_asyncCanceler.Dispose();
			} );
			_disposables.AddLast( () => UnLink() );
			
#if TestSMObject
			Log.Debug( $"end : {_behavior.GetAboutName()}.{this}" );
#endif
		}

		~SMObject() => Dispose();

		public void Dispose() => _disposables.Dispose();

		public void UnLink() {
			if ( _parent?._child == this )	{ _parent._child = _next; }
			_parent = null;

			if ( _previous != null )	{ _previous._next = _next; }
			if ( _next != null )		{ _next._previous = _previous; }
			_previous = null;
			_next = null;
		}



		void SetBehaviours( IEnumerable<ISMBehavior> behaviours ) {
#if TestSMObject
			Log.Debug( $"start {nameof( SetBehaviours )} : {this}" );
#endif
			_behavior = behaviours.First();
			ISMBehavior last = null;
			behaviours.ForEach( b => {
				if ( last != null ) {
					last._next = b;
					b._previous = last;
				}
				last = b;

				b._object = this;
				if ( _owner != null )	{ ( (SMMonoBehaviour)b ).Constructor(); }
			} );
#if TestSMObject
			Log.Debug( $"end {nameof( SetBehaviours )} : {this}" );
#endif
		}

		public void SetParent( SMObject parent ) {
			if ( _owner == null )	{ return; }
#if TestSMObject
			Log.Debug( $"start {nameof( SetParent )} : {this}" );
#endif
			if ( parent != null )	{ parent.AddChild( this ); }
			else					{ Add( this ); }
#if TestSMObject
			Log.Debug( $"end {nameof( SetParent )} : {this}" );
#endif
		}

		void SetChildren() {
			GetAllChildren();

			if ( _owner == null )	{ return; }
#if TestSMObject
			Log.Debug( $"start {nameof( SetChildren )} : {this}" );
#endif
			var currents = Enumerable.Empty<Transform>()
				.Concat( _owner.transform );
			while ( !currents.IsEmpty() ) {
				var children = Enumerable.Empty<Transform>();
				currents.ForEach( t => {
					foreach ( Transform child in t ) {
						var bs = child.GetComponents<SMMonoBehaviour>();
						if ( !bs.IsEmpty() ) {
							new SMObject( child.gameObject, bs, this );
						} else {
							children.Concat( child );
						}
					}
				} );
				currents = children;
			}
#if TestSMObject
			Log.Debug( $"end {nameof( SetChildren )} : {this}" );
#endif
		}

		public void SetTop() {
#if TestSMObject
			Log.Debug( $"start {nameof( SetTop )} : {this}" );
#endif
			for ( _top = this; _top._parent != null; _top = _top._parent )	{}
			SetAllData();
#if TestSMObject
			Log.Debug( $"end {nameof( SetTop )} : {this}" );
#endif
		}

		public void SetAllData() {
#if TestSMObject
			Log.Debug( $"start {nameof( SetAllData )} : {this}" );
#endif
			var lastType = _top._type;
			var lastScene = _top._scene;
			var allObjects = _top.GetAllChildren();
			var allBehaviours = allObjects.SelectMany( o => o.GetBehaviours() );
#if TestSMObject
			Log.Debug(
				$"{nameof( allBehaviours )} : \n"
					+ $"{string.Join( ", ", allBehaviours.Select( b => b.GetAboutName() ) )}"
			);
#endif
			_top._type = (
				allBehaviours.Any( b => b._type == SMTaskType.FirstWork )	? SMTaskType.FirstWork :
				allBehaviours.Any( b => b._type == SMTaskType.Work )		? SMTaskType.Work
																			: SMTaskType.DontWork
			);
			_top._lifeSpan = allBehaviours.Any( b => b._lifeSpan == SMTaskLifeSpan.Forever ) ?
				SMTaskLifeSpan.Forever : SMTaskLifeSpan.InScene;
			_top._scene = (
				// SceneManager作成時の場合、循環参照になる為、設定出来ない
				!SceneManager.s_isCreated					? null :
				_top._lifeSpan == SMTaskLifeSpan.Forever	? SceneManager.s_instance._fsm._foreverScene :
				_top._owner != null						? SceneManager.s_instance._fsm.Get( _top._owner.scene ) :
				SceneManager.s_instance._fsm._scene != null
															? SceneManager.s_instance._fsm._scene
															: SceneManager.s_instance._fsm._startScene
			);

			allObjects.ForEach( o => {
				o._top = _top;
				o._type = _top._type;
				o._lifeSpan = _top._lifeSpan;
				o._scene = _top._scene;
			} );

			if ( lastScene == null ) {
#if TestSMObject
				Log.Debug( $"Register : {_top}" );
#endif
				// SceneManager作成時の場合、循環参照になる為、設定出来ない
				if ( _objects != null ) { _top.Register(); }
			} else if ( _top._type != lastType || _top._scene != lastScene ) {
#if TestSMObject
				Log.Debug( $"ReRegister : {_top}" );
#endif
				_top.ReRegister( lastType, lastScene );
			} else {
#if TestSMObject
				Log.Debug( $"DontRegister : {_top}" );
#endif
			}
#if TestSMObject
			Log.Debug( $"end {nameof( SetAllData )} : {this}" );
#endif
		}


		public SMObject GetFirst() {
			var first = _parent?._child;
			if ( first != null )	{ return first; }

			SMObject current = null;
			for ( current = this; current._previous != null; current = current._previous )	{}
			return current;
		}
		public SMObject GetLast() {
			SMObject current = null;
			for ( current = this; current._next != null; current = current._next )	{}
			return current;
		}
		public SMObject GetLastChild() {
			SMObject current = null;
			for ( current = _child; current?._next != null; current = current._next )	{}
			return current;
		}
		public IEnumerable<SMObject> GetBrothers() {
			for ( var current = GetFirst(); current != null; current = current._next )	{
				yield return current;
			}
		}
		public IEnumerable<SMObject> GetAllParents() {
			for ( var current = this; current != null; current = current._parent ) {
				yield return current;
			}
		}
		public IEnumerable<SMObject> GetChildren() {
			for ( var current = _child; current != null; current = current._next ) {
				yield return current;
			}
		}
		public IEnumerable<SMObject> GetAllChildren() {
			var currents = Enumerable.Empty<SMObject>()
				.Concat( this );
			while ( !currents.IsEmpty() ) {
				var children = Enumerable.Empty<SMObject>();
				foreach ( var o in currents ) {
					yield return  o;
					children.Concat( o.GetChildren() );
				}
				currents = children;
			}
		}


		public T GetBehaviour<T>() where T : ISMBehavior
			=> (T)GetBehaviours()
				.FirstOrDefault( b => b is T );

		public ISMBehavior GetBehaviour( Type type )
			=> GetBehaviours()
				.FirstOrDefault( b => b.GetType() == type );

		public ISMBehavior GetBehaviourAtLast() {
			ISMBehavior current = null;
			for ( current = _behavior; current._next != null; current = current._next )	{}
			return current;
		}

		public IEnumerable<ISMBehavior> GetBehaviours() {
			for ( var current = _behavior; current != null; current = current._next ) {
				yield return current;
			}
		}
		public IEnumerable<T> GetBehaviours<T>() where T : ISMBehavior
			=> GetBehaviours()
				.Where( b => b is T )
				.Select( b => (T)b );

		public IEnumerable<ISMBehavior> GetBehaviours( Type type )
			=> GetBehaviours()
				.Where( b => b.GetType() == type );

		public T GetBehaviourInParent<T>() where T : ISMBehavior
			=> GetAllParents()
				.Select( o => o.GetBehaviour<T>() )
				.FirstOrDefault( b => b != null );

		public ISMBehavior GetBehaviourInParent( Type type )
			=> GetAllParents()
				.Select( o => o.GetBehaviour( type ) )
				.FirstOrDefault( b => b != null );

		public IEnumerable<T> GetBehavioursInParent<T>() where T : ISMBehavior
			=> GetAllParents()
				.SelectMany( o => o.GetBehaviours<T>() );

		public IEnumerable<ISMBehavior> GetBehavioursInParent( Type type )
			=> GetAllParents()
				.SelectMany( o => o.GetBehaviours( type ) );

		public T GetBehaviourInChildren<T>() where T : ISMBehavior
			=> GetAllChildren()
				.Select( o => o.GetBehaviour<T>() )
				.FirstOrDefault( b => b != null );

		public ISMBehavior GetBehaviourInChildren( Type type )
			=> GetAllChildren()
				.Select( o => o.GetBehaviour( type ) )
				.FirstOrDefault( b => b != null );

		public IEnumerable<T> GetBehavioursInChildren<T>() where T : ISMBehavior
			=> GetAllChildren()
				.SelectMany( o => o.GetBehaviours<T>() );

		public IEnumerable<ISMBehavior> GetBehavioursInChildren( Type type )
			=> GetAllChildren()
				.SelectMany( o => o.GetBehaviours( type ) );




		public void Add( SMObject smObject ) {
			smObject.UnLink();

			var last = GetLast();
			smObject._parent = last._parent;
			smObject._previous = last;
			last._next = smObject;
		}
		void AddChild( SMObject smObject ) {
			smObject.UnLink();

			smObject._parent = this;
			var last = GetLastChild();
			if ( last != null ) {
				smObject._previous = last;
				last._next = smObject;
			} else {
				_child = smObject;
			}
		}

		public T AddBehaviour<T>() where T : SMMonoBehaviour {
			var data = new AddSMObject( this, typeof( T ) );
			_top._modifyler.Register( data );
			return (T)data._behavior;
		}

		public SMMonoBehaviour AddBehaviour( Type type ) {
			var data = new AddSMObject( this, type );
			_top._modifyler.Register( data );
			return data._behavior;
		}



		void Register()
			=> _top._modifyler.Register( new RegisterSMObject( this ) );

		void ReRegister( SMTaskType lastType, BaseScene lastScene )
			=> _top._modifyler.Register( new ReRegisterSMObject( this, lastType, lastScene ) );

		void Unregister()
			=> _top._modifyler.Register( new UnregisterSMObject( this ) );

		public void Destroy()
			=> _top._modifyler.Register( new DestroySMObject( this ) );

		public void ChangeParent( Transform parent, bool isWorldPositionStays )
			=> _top._modifyler.Register( new ChangeParentSMObject( this, parent, isWorldPositionStays ) );



		public override string ToString() {
			var result = $"{this.GetAboutName()}(\n"
				+ $"    {nameof( _type )} : {_type}\n"
				+ $"    {nameof( _lifeSpan )} : {_lifeSpan}\n"
				+ $"    {nameof( _scene )} : {_scene}\n"
				+ $"    {nameof( _owner )} : {_owner}\n";

			result += $"    {nameof( GetBehaviours )} : \n";
			GetBehaviours().ForEach( ( b, i ) =>
				result += $"        {i} : {b.GetAboutName()}( "
					+ $"{b._body._ranState}, {b._body._activeState}, {b._body._nextActiveState} )\n"
			);

			new KeyValuePair<string, SMObject>[] {
				new KeyValuePair<string, SMObject>( $"{nameof( _top )}", _top ),
				new KeyValuePair<string, SMObject>( $"{nameof( _parent )}", _parent ),
			}
			.ForEach( pair => {
				var s = (
					pair.Value == null			? string.Empty :
					pair.Value == this			? "this" :
					pair.Value._owner != null	? $"{pair.Value._owner}"
												: $"{pair.Value._behavior}"
				);
				result += $"    {pair.Key} : {s}\n";
			} );

			result += $"    {nameof( GetChildren )} : \n";
			GetChildren().ForEach( ( o, i ) => result += $"        {i} : {o._owner}\n" );

			result += ")";
			return result;
		}
	}
}