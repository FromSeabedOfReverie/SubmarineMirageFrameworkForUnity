//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMObject
namespace SubmarineMirage.SMTask {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using MultiEvent;
	using UTask;
	using Modifyler;
	using Scene;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMObject : IDisposableExtension {
		public SMTaskType _type;
		public SMTaskLifeSpan _lifeSpan;
		public BaseScene _scene;
		public SMObjectManager _objects => _scene?._objects;

		public GameObject _owner		{ get; private set; }
		public SMObjectModifyler _modifyler	{ get; private set; }

		public ISMBehaviour _behaviour	{ get; private set; }
		public SMObject _previous;
		public SMObject _next;
		public SMObject _parent;
		public SMObject _child;
		public SMObject _top;

		public bool _isTop => _top == this;

		public readonly UTaskCanceler _asyncCanceler = new UTaskCanceler();

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public SMObject( GameObject owner, IEnumerable<ISMBehaviour> behaviours, SMObject parent ) {
#if TestSMObject
			Log.Debug( $"{behaviours.FirstOrDefault().GetAboutName()}.{this} : start" );
#endif
			_owner = owner;

			_modifyler = new SMObjectModifyler( this );
			_disposables.AddLast( _modifyler );

			SetupBehaviours( behaviours );
			SetupParent( parent );
			SetupChildren();
			SetupTop();

			_disposables.AddLast( () => GetBehaviours().ForEach( b => b.Dispose() ) );
			_disposables.AddLast( () => GetChildren().ForEach( o => o.Dispose() ) );
			_disposables.AddLast( _asyncCanceler );
			_disposables.AddLast( () => SMObjectModifyData.UnLinkObject( this ) );
			
#if TestSMObject
			Log.Debug( $"{_behaviour.GetAboutName()}.{this} : end" );
#endif
		}

		~SMObject() => Dispose();

		public void Dispose() => _disposables.Dispose();



		void SetupBehaviours( IEnumerable<ISMBehaviour> behaviours ) {
#if TestSMObject
			Log.Debug( $"{nameof( SetupBehaviours )} : {this} : start" );
#endif
			_behaviour = behaviours.First();
			ISMBehaviour last = null;
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
			Log.Debug( $"{nameof( SetupBehaviours )} : {this} : end" );
#endif
		}

		void SetupParent( SMObject parent ) {
			if ( _owner == null )	{ return; }
#if TestSMObject
			Log.Debug( $"{nameof( SetupParent )} : {this} : start" );
#endif
			if ( parent != null )	{ SMObjectModifyData.AddChildObject( parent, this ); }
#if TestSMObject
			Log.Debug( $"{nameof( SetupParent )} : {this} : end" );
#endif
		}

		void SetupChildren() {
			if ( _owner == null )	{ return; }
#if TestSMObject
			Log.Debug( $"{nameof( SetupChildren )} : {this} : start" );
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
			Log.Debug( $"{nameof( SetupChildren )} : {this} : end" );
#endif
		}

		void SetupTop() {
			if ( _parent == null )	{ SMObjectModifyData.SetTopObject( this ); }
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


		public T GetBehaviour<T>() where T : ISMBehaviour
			=> (T)GetBehaviours()
				.FirstOrDefault( b => b is T );

		public ISMBehaviour GetBehaviour( Type type )
			=> GetBehaviours()
				.FirstOrDefault( b => b.GetType() == type );

		public ISMBehaviour GetBehaviourAtLast() {
			ISMBehaviour current = null;
			for ( current = _behaviour; current._next != null; current = current._next )	{}
			return current;
		}

		public IEnumerable<ISMBehaviour> GetBehaviours() {
			for ( var current = _behaviour; current != null; current = current._next ) {
				yield return current;
			}
		}
		public IEnumerable<T> GetBehaviours<T>() where T : ISMBehaviour
			=> GetBehaviours()
				.Where( b => b is T )
				.Select( b => (T)b );

		public IEnumerable<ISMBehaviour> GetBehaviours( Type type )
			=> GetBehaviours()
				.Where( b => b.GetType() == type );

		public T GetBehaviourInParent<T>() where T : ISMBehaviour
			=> GetAllParents()
				.Select( o => o.GetBehaviour<T>() )
				.FirstOrDefault( b => b != null );

		public ISMBehaviour GetBehaviourInParent( Type type )
			=> GetAllParents()
				.Select( o => o.GetBehaviour( type ) )
				.FirstOrDefault( b => b != null );

		public IEnumerable<T> GetBehavioursInParent<T>() where T : ISMBehaviour
			=> GetAllParents()
				.SelectMany( o => o.GetBehaviours<T>() );

		public IEnumerable<ISMBehaviour> GetBehavioursInParent( Type type )
			=> GetAllParents()
				.SelectMany( o => o.GetBehaviours( type ) );

		public T GetBehaviourInChildren<T>() where T : ISMBehaviour
			=> GetAllChildren()
				.Select( o => o.GetBehaviour<T>() )
				.FirstOrDefault( b => b != null );

		public ISMBehaviour GetBehaviourInChildren( Type type )
			=> GetAllChildren()
				.Select( o => o.GetBehaviour( type ) )
				.FirstOrDefault( b => b != null );

		public IEnumerable<T> GetBehavioursInChildren<T>() where T : ISMBehaviour
			=> GetAllChildren()
				.SelectMany( o => o.GetBehaviours<T>() );

		public IEnumerable<ISMBehaviour> GetBehavioursInChildren( Type type )
			=> GetAllChildren()
				.SelectMany( o => o.GetBehaviours( type ) );



		public T AddBehaviour<T>() where T : SMMonoBehaviour {
			var data = new AddBehaviourSMObject( this, typeof( T ) );
			_top._modifyler.Register( data );
			return (T)data._behaviour;
		}

		public SMMonoBehaviour AddBehaviour( Type type ) {
			var data = new AddBehaviourSMObject( this, type );
			_top._modifyler.Register( data );
			return data._behaviour;
		}

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
												: $"{pair.Value._behaviour}"
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