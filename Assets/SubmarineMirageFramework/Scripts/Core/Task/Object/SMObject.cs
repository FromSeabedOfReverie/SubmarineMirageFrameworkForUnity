//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestObject
#define TestObjectModifyler
namespace SubmarineMirage.Task.Object {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Modifyler;
	using Task.Modifyler;
	using Behaviour;
	using Group;
	using Group.Modifyler;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMObject : BaseSMTaskModifylerOwner<SMObjectModifyler> {
		[SMShowLine] public SMGroup _group	{ get; set; }
		[SMShowLine] public GameObject _gameObject	{ get; private set; }

		[SMShowLine] public ISMBehaviour _behaviour	{ get; set; }
		[SMShowLine] public SMObject _previous	{ get; set; }
		[SMShowLine] public SMObject _next	{ get; set; }
		[SMShowLine] public SMObject _parent	{ get; set; }
		[SMShowLine] public SMObject _child	{ get; set; }

		[SMHide] public bool _isGameObject	=> _gameObject != null;
		public bool _isDisabling	{ get; set; }

		public readonly SMTaskCanceler _asyncCanceler = new SMTaskCanceler();



		public SMObject( GameObject gameObject, IEnumerable<ISMBehaviour> behaviours, SMObject parent,
							bool isDebug = false
		) {
#if TestObject
			SMLog.Debug( $"{nameof( SMObject )}() : start\n{this}" );
#endif
			_modifyler = new SMObjectModifyler( this );
			_gameObject = gameObject;

			SetupBehaviours( behaviours );
			SetupParent( parent );
			SetupChildren();

			if ( !isDebug
#if !TestObjectModifyler
					|| true
#endif
			) {
				SetupTop();
			}

			_disposables.AddLast( () => {
				_isFinalizing = true;
				_ranState = SMTaskRunState.Finalize;

				_asyncCanceler.Dispose();
			} );
#if TestObject
			_disposables.AddLast( () => SMLog.Debug( $"{nameof( SMObject )}.{nameof( Dispose )} : {this}" ) );
			SMLog.Debug( $"{nameof( SMObject )}() : end\n{this}" );
#endif
		}

		public override void Dispose() => base.Dispose();



		void SetupBehaviours( IEnumerable<ISMBehaviour> behaviours ) {
#if TestObject
			SMLog.Debug( $"{nameof( SetupBehaviours )} : start\n{this}" );
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
				if ( _isGameObject )	{ ( (SMMonoBehaviour)b ).Constructor(); }
				b._asyncCancelerOnDisable.SetParent( _asyncCanceler );
				b._asyncCancelerOnDispose.SetParent( _asyncCanceler );
			} );
#if TestObject
//			SMLog.Debug( string.Join( "\n", behaviours.Select( b => b.ToLineString() ) ) );
			SMLog.Debug( $"{nameof( SetupBehaviours )} : end\n{this}" );
#endif
		}

		void SetupParent( SMObject parent ) {
			if ( !_isGameObject )	{ return; }
#if TestObject
			SMLog.Debug( $"{nameof( SetupParent )} : start\n{this}" );
#endif
			if ( parent != null )	{ SMObjectApplyer.LinkChild( parent, this ); }
#if TestObject
			SMLog.Debug( $"{nameof( SetupParent )} : end\n{this}" );
#endif
		}

		void SetupChildren() {
			if ( !_isGameObject )	{ return; }
#if TestObject
			SMLog.Debug( $"{nameof( SetupChildren )} : start\n{this}" );
#endif
			var currents = new Queue<Transform>();
			currents.Enqueue( _gameObject.transform );
			while ( !currents.IsEmpty() ) {
				foreach ( Transform child in currents.Dequeue() ) {
					var bs = child.GetComponents<SMMonoBehaviour>();
					if ( !bs.IsEmpty() ) {
						new SMObject( child.gameObject, bs, this );
					} else {
						currents.Enqueue( child );
					}
				}
			}
#if TestObject
			SMLog.Debug( $"{nameof( SetupChildren )} : end\n{this}" );
#endif
		}

		void SetupTop() {
			if ( _parent != null )	{ return; }

#if TestObject
			SMLog.Debug( $"{nameof( SetupTop )} : start\n{this}" );
#endif
			_group = new SMGroup( this );
#if TestObject
			SMLog.Debug( $"{nameof( SetupTop )} : end\n{this}" );
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
		public SMObject GetLastChild()
			=> _child?.GetLast();

		public SMObject GetTop() {
			SMObject top;
			for ( top = this; top._parent != null; top = top._parent ) {}
			return top;
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
		public IEnumerable<SMObject> GetChildren()
			=> _child?.GetBrothers() ?? Enumerable.Empty<SMObject>();

		public IEnumerable<SMObject> GetAllChildren() {
			var currents = new Queue<SMObject>();
			currents.Enqueue( this );
			while ( !currents.IsEmpty() ) {
				var o = currents.Dequeue();
				yield return o;
				o.GetChildren().ForEach( c => currents.Enqueue( c ) );
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



		public T AddBehaviour<T>() where T : SMMonoBehaviour
			=> (T)AddBehaviour( typeof( T ) );

		public SMMonoBehaviour AddBehaviour( Type type ) {
			var data = new AddBehaviourSMGroup( this, type );
			_group._modifyler.Register( data );
			return data._behaviour;
		}

		public void Destroy()
			=> _group._modifyler.Register( new DestroyObjectSMGroup( this ) );

		public void ChangeParent( Transform parent, bool isWorldPositionStays = true )
			=> _group._modifyler.Register(
				new SendChangeParentObjectSMGroup( this, parent, isWorldPositionStays ) );

		public void ChangeActive( bool isActive ) {
			if ( isActive )	{ _group._modifyler.Register( new EnableObjectSMGroup( this ) ); }
			else			{ _group._modifyler.Register( new DisableObjectSMGroup( this ) ); }
		}



		public override void SetToString() {
			base.SetToString();


			_toStringer.SetValue( nameof( _gameObject ), i => _gameObject != null ? _gameObject.name : "null" );
			_toStringer.SetValue( nameof( _behaviour ), i => "\n" + string.Join( ",\n",
				GetBehaviours().Select( b => b.ToLineString( i + 1 ) )
			) );
			_toStringer.SetValue( nameof( _parent ), i => _parent?.ToLineString() );
			_toStringer.SetValue( nameof( _previous ), i => _previous?.ToLineString() );
			_toStringer.SetValue( nameof( _next ), i => _next?.ToLineString() );
			_toStringer.SetValue( nameof( _child ), i => "\n" + string.Join( ",\n",
				 GetChildren().Select( o => o.ToLineString( i + 1 ) )
			) );
			_toStringer.SetValue( nameof( _asyncCanceler ), i => $"_isCancel : {_asyncCanceler._isCancel}" );


			_toStringer.SetLineValue( nameof( _gameObject ), () => _gameObject != null ? _gameObject.name : "null" );
			_toStringer.SetLineValue( nameof( _behaviour ), () => string.Join( ",",
				GetBehaviours().Select( b => b.GetAboutName() )
			) );
			_toStringer.SetLineValue( nameof( _isDispose ), () =>
				GetBehaviours().All( b => b._isDispose ) ? nameof( Dispose ) : "" );
			_toStringer.SetLineValue( nameof( _group ), () => $"△{_group._topObject._id}" );
			_toStringer.SetLineValue( nameof( _parent ), () => $"←{_parent?._id}" );
			_toStringer.SetLineValue( nameof( _previous ), () => $"↑{_previous?._id}" );
			_toStringer.SetLineValue( nameof( _next ), () => $"↓{_next?._id}" );
			_toStringer.SetLineValue( nameof( _child ), () => "→" + string.Join( ",",
				GetChildren().Select( o => o._id )
			) );
			_toStringer.AddLine( nameof( _behaviour._body._ranState ), () =>
				GetBehaviours().Max( b => b._body._ranState ).ToString() );
			_toStringer.AddLine( nameof( _behaviour._isActive ), () =>
				GetBehaviours().Any( b => b._isActive ) ? "◯" : "×" );
		}
	}
}