//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Base {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using KoganeUnityLib;
	using Task.Modifyler.Base;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMObjectBody : BaseSMTaskModifylerOwner<SMObjectModifyler> {
		public SMObject _object	{ get; private set; }
		public SMGroupBody _groupBody	{ get; set; }
		[SMShowLine] public SMBehaviourBody _behaviourBody	{ get; set; }

		[SMShowLine] public GameObject _gameObject	{ get; private set; }

		public SMObjectBody _previous	{ get; set; }
		public SMObjectBody _next		{ get; set; }
		public SMObjectBody _parent	{ get; set; }
		public SMObjectBody _child		{ get; set; }

		[SMShow] public bool _isDisabling	{ get; set; }

		public readonly SMAsyncCanceler _asyncCanceler = new SMAsyncCanceler();



#region ToString
		public override void SetToString() {
			base.SetToString();


			_toStringer.SetValue( nameof( _gameObject ), i => _gameObject.name );
			_toStringer.SetValue( nameof( _behaviourBody ), i =>
				_toStringer.DefaultValue( _behaviourBody.GetBrothers(), i, true ) );
			_toStringer.Add( nameof( _parent ), i => _toStringer.DefaultValue( _parent, i, true ) );
			_toStringer.Add( nameof( _previous ), i => _toStringer.DefaultValue( _previous, i, true ) );
			_toStringer.Add( nameof( _next ), i => _toStringer.DefaultValue( _next, i, true ) );
			_toStringer.Add( nameof( _child ), i => _toStringer.DefaultValue( GetChildren(), i, true ) );


			_toStringer.SetLineValue( nameof( _gameObject ), () => _gameObject.name );
			_toStringer.SetLineValue( nameof( _behaviourBody ), () => string.Join( ",",
				_behaviourBody.GetBrothers().Select( b => b.GetAboutName() )
			) );
			_toStringer.AddLine( nameof( _groupBody ), () => $"△{_groupBody._objectBody._id}" );
			_toStringer.AddLine( nameof( _parent ), () => $"←{_parent?._id}" );
			_toStringer.AddLine( nameof( _previous ), () => $"↑{_previous?._id}" );
			_toStringer.AddLine( nameof( _next ), () => $"↓{_next?._id}" );
			_toStringer.AddLine( nameof( _child ), () => "→" + string.Join( ",",
				GetChildren().Select( o => o._id )
			) );

			_toStringer.AddLine( nameof( _ranState ), () => _toStringer.DefaultLineValue( _ranState ) );
			_toStringer.AddLine( nameof( _isActive ), () => _isActive ? "◯" : "×" );
		}
#endregion



		public SMObjectBody( SMObject smObject, SMGroupBody groupBody, SMObjectBody parentBody,
								GameObject gameObject, IEnumerable<SMBehaviour> behaviours
		) {
			_modifyler = new SMObjectModifyler( this );
			_object = smObject;
			_groupBody = groupBody;
			_gameObject = gameObject;

			SetupParent( parentBody );
			SetupBehaviours( behaviours );
			SetupChildren();

			_disposables.AddLast( () => {
				_isFinalizing = true;
				_ranState = SMTaskRunState.Finalize;

				_asyncCanceler.Dispose();
			} );
		}

		public override void Dispose() => base.Dispose();

		public void DisposeAllChildren() {
			var os = GetAllChildren()
				.Reverse()
				.ToArray();
			SMGroupManagerBody.DISPOSE_TASK_TYPES.ForEach( t => {
				os.ForEach( o =>
					o._behaviourBody.DisposeBrothers( t )
				);
			} );
			os.ForEach( o => o._object.Dispose() );
		}



		void SetupParent( SMObjectBody parentBody ) {
			if ( parentBody == null )	{ return; }

			parentBody.LinkChild( this );
		}

		void SetupBehaviours( IEnumerable<SMBehaviour> behaviours ) {
			var bodies = behaviours
				.Select( b => {
					b.Constructor();
					return b._body;
				} )
				.ToArray();

			_behaviourBody = bodies.First();

			SMBehaviourBody last = null;
			bodies.ForEach( b => {
				if ( last != null ) {
					last._next = b;
					b._previous = last;
				}
				last = b;

				b.SetupObject( this );
			} );
		}

		void SetupChildren() {
			var currents = new Queue<Transform>();
			currents.Enqueue( _gameObject.transform );
			while ( !currents.IsEmpty() ) {
				foreach ( Transform child in currents.Dequeue() ) {
					var bs = child.GetComponents<SMBehaviour>();
					if ( !bs.IsEmpty() ) {
						new SMObject( _groupBody, this, child.gameObject, bs );
					} else {
						currents.Enqueue( child );
					}
				}
			}
		}



		public void SetGroupOfAllChildren( SMGroupBody groupBody )
			=> GetAllChildren()
				.ForEach( o => o._groupBody = groupBody );



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
			if ( _previous != null )		{ _previous._next = _next; }
			if ( _next != null )			{ _next._previous = _previous; }
			_parent = null;
			_previous = null;
			_next = null;
		}



		public void StopAsync() => _asyncCanceler.Cancel();



		public bool IsActiveInHierarchy()
			=> _gameObject.activeInHierarchy;

		public bool IsActiveInParentHierarchy() {
			var parent = _gameObject.transform.parent;
			if ( parent == null )	{ return true; }

			return parent.gameObject.activeInHierarchy;
		}



		public void FixedUpdate( SMTaskType type ) {
			if ( !_isFinalizing )	{ return; }
			if ( !_isActive )		{ return; }
			if ( _ranState < SMTaskRunState.InitialEnable )	{ return; }

			_behaviourBody.GetBrothers()
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

			_behaviourBody.GetBrothers()
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
			
			_behaviourBody.GetBrothers()
				.Where( b => b._type == type )
				.ForEach( b => b.LateUpdate() );

			if ( type == SMTaskType.Work && _ranState == SMTaskRunState.Update ) {
				_ranState = SMTaskRunState.LateUpdate;
			}
		}



		public void Destroy()
			=> _groupBody.DestroyObject( this );

		public void ChangeActive( bool isActive )
			=> _groupBody.ChangeActiveObject( this, isActive );

		public void ChangeParent( Transform parent, bool isWorldPositionStays )
			=> _groupBody.ChangeParentObject( this, parent, isWorldPositionStays );


		public T AddBehaviour<T>() where T : SMBehaviour
			=> _groupBody.AddBehaviour<T>( this );

		public SMBehaviour AddBehaviour( Type type )
			=> _groupBody.AddBehaviour( this, type );



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
			SMObjectBody current = null;
			for ( current = this; current._parent != null; current = current._parent ) {}
			return current;
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
	}
}