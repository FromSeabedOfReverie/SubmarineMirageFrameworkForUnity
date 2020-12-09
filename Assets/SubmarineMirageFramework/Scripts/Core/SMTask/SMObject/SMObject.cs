//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTask
#define TestSMTaskModifyler
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
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public class SMObject : IDisposableExtension {
		static uint s_idCount;
		public uint _id					{ get; private set; }

// TODO : SMObjectGroup内に移植
//		public SMTaskType _type;
//		public SMTaskLifeSpan _lifeSpan;
//		public BaseScene _scene;
//		public SMObjectManager _objects => _scene?._objects;
//		public SMObjectModifyler _modifyler	{ get; private set; }
//		public SMObject _top;
//		public bool _isTop =>	_group.IsTop( this );

		public SMObjectGroup _group;
		public GameObject _owner		{ get; private set; }

		public ISMBehaviour _behaviour;
		public SMObject _previous;
		public SMObject _next;
		public SMObject _parent;
		public SMObject _child;

		public bool _isGameObject =>	_owner != null;
		public bool _isDispose =>		_disposables._isDispose;

		public readonly UTaskCanceler _asyncCanceler = new UTaskCanceler();

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public SMObject( GameObject owner, IEnumerable<ISMBehaviour> behaviours, SMObject parent,
							bool isDebug = false
		) {
			_id = ++s_idCount;
#if TestSMTask
			Log.Debug( $"{nameof( SMObject )}() : start\n{this}" );
#endif
			_owner = owner;

			SetupBehaviours( behaviours );
			SetupParent( parent );
			SetupChildren();

			if ( !isDebug
#if !TestSMTaskModifyler
					|| true
#endif
			) {
				SetupTop();
			}

			_disposables.AddLast( () => {
				GetChildren().Reverse().ToArray().ForEach( o => o.Dispose() );
				GetBehaviours().Reverse().ToArray().ForEach( b => b.Dispose() );

				_asyncCanceler.Dispose();	// 子が再生成される為、後に解放

				if ( _group.IsTop( this ) ) {
					_group.Dispose();
				} else {
					_group._modifyler.Unregister( this );
				}
				SMObjectModifyData.UnLinkObject( this );
				if ( _isGameObject )	{ UnityObject.Destroy( _owner ); }
			} );
#if TestSMTask
			_disposables.AddLast( () => Log.Debug( $"{nameof( SMObject )}.{nameof( Dispose )} : {this}" ) );
			Log.Debug( $"{nameof( SMObject )}() : end\n{this}" );
#endif
		}

		~SMObject() => Dispose();

		public void Dispose() => _disposables.Dispose();



		void SetupBehaviours( IEnumerable<ISMBehaviour> behaviours ) {
#if TestSMTask
			Log.Debug( $"{nameof( SetupBehaviours )} : start\n{this}" );
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
#if TestSMTask
//			Log.Debug( string.Join( "\n", behaviours.Select( b => b.ToLineString() ) ) );
			Log.Debug( $"{nameof( SetupBehaviours )} : end\n{this}" );
#endif
		}

		void SetupParent( SMObject parent ) {
			if ( !_isGameObject )	{ return; }
#if TestSMTask
			Log.Debug( $"{nameof( SetupParent )} : start\n{this}" );
#endif
			if ( parent != null )	{ SMObjectModifyData.AddChildObject( parent, this ); }
#if TestSMTask
			Log.Debug( $"{nameof( SetupParent )} : end\n{this}" );
#endif
		}

		void SetupChildren() {
			if ( !_isGameObject )	{ return; }
#if TestSMTask
			Log.Debug( $"{nameof( SetupChildren )} : start\n{this}" );
#endif
			var currents = new Queue<Transform>();
			currents.Enqueue( _owner.transform );
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
#if TestSMTask
			Log.Debug( $"{nameof( SetupChildren )} : end\n{this}" );
#endif
		}

		void SetupTop() {
			if ( _parent != null )	{ return; }

#if TestSMTask
			Log.Debug( $"{nameof( SetupTop )} : start\n{this}" );
#endif
			_group = new SMObjectGroup( this );
#if TestSMTask
			Log.Debug( $"{nameof( SetupTop )} : end\n{this}" );
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
			var data = new AddBehaviourSMObject( this, type );
			_group._modifyler.Register( data );
			return data._behaviour;
		}

		public void Destroy()
			=> _group._modifyler.Register( new DestroySMObject( this ) );

		public void ChangeParent( Transform parent, bool isWorldPositionStays = true )
			=> _group._modifyler.Register(
				new ReserveChangeParentSMObject( this, parent, isWorldPositionStays ) );

		public void ChangeActive( bool isActive )
			=> _group._modifyler.Register( new ChangeActiveSMObject( this, isActive, true ) );



		public override string ToString() => string.Join( "\n",
			$"{nameof( SMObject )}(",
			$"    {nameof( _id )} : {_id}",
			$"    {nameof( _group )} : {_group}",
			$"    {nameof( _owner )} : {( _owner != null ? _owner.name : "null" )}",

			$"    {nameof( GetBehaviours )} : ",
			string.Join( "\n", GetBehaviours().Select( b => $"        {b.ToLineString()}" ) ),
			$"    {nameof( _parent )} : {_parent?.ToLineString()}",
			$"    {nameof( _previous )} : {_previous?.ToLineString()}",
			$"    {nameof( _next )} : {_next?.ToLineString()}",
			$"    {nameof( GetChildren )} : ",
			string.Join( "\n", GetChildren().Select( o => $"        {o.ToLineString()}" ) ),

			"",
			$"    {nameof( _asyncCanceler )}._isCancel : {_asyncCanceler._isCancel}",
			$"    {nameof( _isDispose )} : {_isDispose}",
			")"
		);

		public string ToLineString( bool isViewLink = true ) {
			var bs = GetBehaviours().ToArray();
			var isDispose = bs.All( b => b._isDispose );

			var result = string.Join( " ",
				_id,
				_owner != null ? _owner.name : "null",
				$"({string.Join( ", ", bs.Select( b => b.GetAboutName() ) )})",
				""
			);
			if ( isDispose ) {
				result += "Dispose ";
			} else {
				result += string.Join( " ",
					bs.Max( b => b._body._ranState ),
					bs.Any( b => b._isActive ) ? "◯" : "×",
					""
				);
			}
			if ( isViewLink ) {
				result += string.Join( " ",
					$"△{_group._topObject._id}",
					$"←{_parent?._id}",
					$"↑{_previous?._id}",
					$"↓{_next?._id}",
					$"→{string.Join( ",", GetChildren().Select( o => o._id ) )}"
				);
			}
			return result;
		}
	}
}