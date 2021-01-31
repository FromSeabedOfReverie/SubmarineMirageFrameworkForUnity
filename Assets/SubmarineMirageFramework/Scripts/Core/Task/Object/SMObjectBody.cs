//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Base {
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using KoganeUnityLib;
	using Task.Modifyler.Base;
	using Task.Group;
	using Task.Group.Manager.Modifyler;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMObjectBody : BaseSMTaskModifylerOwner<SMObjectModifyler> {
		public SMObject _object	{ get; private set; }
		[SMShowLine] public GameObject _gameObject	{ get; private set; }
		[SMShowLine] public SMGroup _group	{ get; set; }
		[SMShowLine] public SMBehaviourBody _behaviour	{ get; set; }

		[SMShowLine] public SMObjectBody _previous	{ get; set; }
		[SMShowLine] public SMObjectBody _next		{ get; set; }
		[SMShowLine] public SMObjectBody _parent	{ get; set; }
		[SMShowLine] public SMObjectBody _child		{ get; set; }

		[SMHide] public bool _isGameObject	=> _gameObject != null;
		public bool _isDisabling	{ get; set; }

		public readonly SMTaskCanceler _asyncCanceler = new SMTaskCanceler();



		public SMObjectBody( SMObject smObject, GameObject gameObject, IEnumerable<ISMBehaviour> behaviours,
								SMObject parent
		) {
			_modifyler = new SMObjectModifyler( this );
			_object = smObject;
			_gameObject = gameObject;

			SetupBehaviours( behaviours );
			SetupParent( parent );
			SetupChildren();
			SetupTop();

			_disposables.AddLast( () => {
				_isFinalizing = true;
				_ranState = SMTaskRunState.Finalize;

				_asyncCanceler.Dispose();
			} );
		}



		void SetupBehaviours( IEnumerable<ISMBehaviour> behaviours ) {
			var bodies = behaviours.Select( b => b._body );
			_behaviour = bodies.First();
			SMBehaviourBody last = null;
			bodies.ForEach( b => {
				if ( last != null ) {
					last._next = b;
					b._previous = last;
				}
				last = b;

				b._object = this;
				b._asyncCancelerOnDisable.SetParent( _asyncCanceler );
				b._asyncCancelerOnDispose.SetParent( _asyncCanceler );
			} );
		}

		void SetupParent( SMObject parent ) {
			if ( !_isGameObject )	{ return; }

			if ( parent != null )	{ parent._body.LinkChild( this ); }
		}

		void SetupChildren() {
			if ( !_isGameObject )	{ return; }

			var currents = new Queue<Transform>();
			currents.Enqueue( _gameObject.transform );
			while ( !currents.IsEmpty() ) {
				foreach ( Transform child in currents.Dequeue() ) {
					var bs = child.GetComponents<SMMonoBehaviour>();
					if ( !bs.IsEmpty() ) {
						new SMObject( child.gameObject, bs, _object );
					} else {
						currents.Enqueue( child );
					}
				}
			}
		}

		void SetupTop() {
			if ( _parent != null )	{ return; }

			_group = new SMGroup( _object );
		}



		public void DisposeAll() {
			var os = GetAllChildren().ToArray();

			var bs = os
				.SelectMany( o => o._behaviour.GetBehaviours() )
				.Reverse()
				.ToArray();
			SMGroupManagerApplyer.DISPOSE_TASK_TYPES
				.SelectMany( t =>
					bs.Where( b => b._type == t )
				)
				.ForEach( b => b.Dispose() );

			os.ForEach( o => o.Dispose() );
		}



		public void LinkChild( SMObjectBody add ) {
			add._parent = this;
			var last = GetLastChild();

			if ( last != null ) {
				add._previous = last;
				last._next = add;
			} else {
				_child = add;
			}
		}


		public void Unlink() {
			if ( _parent?._child == this )	{ _parent._child = _next; }
			_parent = null;

			if ( _previous != null )	{ _previous._next = _next; }
			if ( _next != null )		{ _next._previous = _previous; }
			_previous = null;
			_next = null;
		}



		public bool IsActiveInHierarchy() {
			if ( !_isGameObject )	{ return true; }

			return _gameObject.activeInHierarchy;
		}

		public bool IsActiveInParentHierarchy() {
			if ( !_isGameObject )	{ return true; }

			var parent = _gameObject.transform.parent;
			if ( parent == null )	{ return true; }

			return parent.gameObject.activeInHierarchy;
		}



		public void FixedUpdate( SMTaskType type ) {
			if ( !_isFinalizing )	{ return; }
			if ( !_isActive )		{ return; }
			if ( _ranState < SMTaskRunState.InitialEnable )	{ return; }

			_behaviour.GetBehaviours()
				.Where( b => b._type == type )
				.ForEach( b => b.FixedUpdate() );

			if ( type == SMTaskType.Work && _ranState == SMTaskRunState.InitialEnable ) {
				_ranState = SMTaskRunState.FixedUpdate;
			}
		}


		public void Update( SMTaskType type ) {
			if ( !_isFinalizing )	{ return; }
			if ( !_isActive )		{ return; }
			if ( _ranState < SMTaskRunState.FixedUpdate )	{ return; }

			_behaviour.GetBehaviours()
				.Where( b => b._type == type )
				.ForEach( b => b.Update() );

			if ( type == SMTaskType.Work && _ranState == SMTaskRunState.FixedUpdate ) {
				_ranState = SMTaskRunState.Update;
			}
		}


		public void LateUpdate( SMTaskType type ) {
			if ( !_isFinalizing )	{ return; }
			if ( !_isActive )		{ return; }
			if ( _ranState < SMTaskRunState.Update )	{ return; }
			
			_behaviour.GetBehaviours()
				.Where( b => b._type == type )
				.ForEach( b => b.LateUpdate() );

			if ( type == SMTaskType.Work && _ranState == SMTaskRunState.Update ) {
				_ranState = SMTaskRunState.LateUpdate;
			}
		}



		public SMObjectBody GetFirst() {
			var first = _parent?._child;
			if ( first != null )	{ return first; }

			SMObjectBody current = null;
			for ( current = this; current._previous != null; current = current._previous )	{}
			return current;
		}
		public SMObjectBody GetLast() {
			SMObjectBody current = null;
			for ( current = this; current._next != null; current = current._next )	{}
			return current;
		}
		public SMObjectBody GetLastChild()
			=> _child?.GetLast();

		public SMObjectBody GetTop() {
			SMObjectBody top;
			for ( top = this; top._parent != null; top = top._parent ) {}
			return top;
		}



		public IEnumerable<SMObjectBody> GetBrothers() {
			for ( var current = GetFirst(); current != null; current = current._next )	{
				yield return current;
			}
		}
		public IEnumerable<SMObjectBody> GetAllParents() {
			for ( var current = this; current != null; current = current._parent ) {
				yield return current;
			}
		}
		public IEnumerable<SMObjectBody> GetChildren()
			=> _child?.GetBrothers() ?? Enumerable.Empty<SMObjectBody>();

		public IEnumerable<SMObjectBody> GetAllChildren() {
			var currents = new Queue<SMObjectBody>();
			currents.Enqueue( this );
			while ( !currents.IsEmpty() ) {
				var o = currents.Dequeue();
				yield return o;
				o.GetChildren().ForEach( c => currents.Enqueue( c ) );
			}
		}



		public override void SetToString() {
			base.SetToString();


			_toStringer.SetValue( nameof( _gameObject ), i => _gameObject != null ? _gameObject.name : "null" );
			_toStringer.SetValue( nameof( _behaviour ), i =>
				_toStringer.DefaultValue( _behaviour.GetBehaviours(), i, true ) );
			_toStringer.SetValue( nameof( _parent ), i => _toStringer.DefaultValue( _parent, i, true ) );
			_toStringer.SetValue( nameof( _previous ), i => _toStringer.DefaultValue( _previous, i, true ) );
			_toStringer.SetValue( nameof( _next ), i => _toStringer.DefaultValue( _next, i, true ) );
			_toStringer.SetValue( nameof( _child ), i => _toStringer.DefaultValue( GetChildren(), i, true ) );
			_toStringer.SetValue( nameof( _asyncCanceler ), i => $"_isCancel : {_asyncCanceler._isCancel}" );


			_toStringer.SetLineValue( nameof( _gameObject ), () => _gameObject != null ? _gameObject.name : "null" );
			_toStringer.SetLineValue( nameof( _behaviour ), () => string.Join( ",",
				_behaviour.GetBehaviours().Select( b => b.GetAboutName() )
			) );
			_toStringer.SetLineValue( nameof( _isDispose ), () =>
				_behaviour.GetBehaviours().All( b => b._isDispose ) ? nameof( Dispose ) : "" );
			_toStringer.SetLineValue( nameof( _group ), () => $"△{_group._topObject._id}" );
			_toStringer.SetLineValue( nameof( _parent ), () => $"←{_parent?._id}" );
			_toStringer.SetLineValue( nameof( _previous ), () => $"↑{_previous?._id}" );
			_toStringer.SetLineValue( nameof( _next ), () => $"↓{_next?._id}" );
			_toStringer.SetLineValue( nameof( _child ), () => "→" + string.Join( ",",
				GetChildren().Select( o => o._id )
			) );
			_toStringer.AddLine( nameof( _behaviour._ranState ), () =>
				_behaviour.GetBehaviours().Max( b => b._ranState ).ToString() );
			_toStringer.AddLine( nameof( _behaviour._isActive ), () =>
				_behaviour.GetBehaviours().Any( b => b._isActive ) ? "◯" : "×" );
		}
	}
}