//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Base;
	using Task.Base;
	using Task.Group.Modifyler;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMObject : SMStandardBase {
		[SMShowLine] public SMObjectBody _body	{ get; private set; }

		[SMHide] public bool _isGameObject	=> _body._isGameObject;
		public SMTaskCanceler _asyncCanceler => _body._asyncCanceler;



		public SMObject( GameObject gameObject, IEnumerable<ISMBehaviour> behaviours, SMObject parent ) {
			_body = new SMObjectBody( this, gameObject, behaviours, parent );

			_disposables.AddLast( () => {
				_body.Dispose();
			} );
		}

		public override void Dispose() => base.Dispose();



		public void Destroy()
			=> _body._groupBody._modifyler.Register( new DestroyObjectSMGroup( this ) );

		public void ChangeActive( bool isActive ) {
			if ( isActive )	{ _body._groupBody._modifyler.Register( new EnableObjectSMGroup( this ) ); }
			else			{ _body._groupBody._modifyler.Register( new DisableObjectSMGroup( this ) ); }
		}

		public void ChangeParent( Transform parent, bool isWorldPositionStays = true )
			=> _body._groupBody._modifyler.Register(
				new SendChangeParentObjectSMGroup( this, parent, isWorldPositionStays ) );


		public T AddBehaviour<T>() where T : SMMonoBehaviour
			=> (T)AddBehaviour( typeof( T ) );

		public SMMonoBehaviour AddBehaviour( Type type ) {
			var data = new AddBehaviourSMGroup( this, type );
			_body._groupBody._modifyler.Register( data );
			return data._behaviour;
		}



		public T GetBehaviour<T>() where T : ISMBehaviour
			=> _body._behaviourBody.GetBehaviour<T>();

		public ISMBehaviour GetBehaviour( Type type )
			=> _body._behaviourBody.GetBehaviour( type );

		public IEnumerable<T> GetBehaviours<T>() where T : ISMBehaviour
			=> _body._behaviourBody.GetBehaviours<T>();

		public IEnumerable<ISMBehaviour> GetBehaviours( Type type )
			=> _body._behaviourBody.GetBehaviours( type );


		public T GetBehaviourInParent<T>() where T : ISMBehaviour
			=> (T)GetBehaviourInParent( typeof( T ) );

		public ISMBehaviour GetBehaviourInParent( Type type )
			=> GetBehavioursInParent( type ).FirstOrDefault();

		public IEnumerable<T> GetBehavioursInParent<T>() where T : ISMBehaviour
			=> GetBehavioursInParent( typeof( T ) )
				.Select( b => (T)b );

		public IEnumerable<ISMBehaviour> GetBehavioursInParent( Type type )
			=> _body.GetAllParents()
				.SelectMany( o => o._object.GetBehaviours( type ) );


		public T GetBehaviourInChildren<T>() where T : ISMBehaviour
			=> (T)GetBehaviourInChildren( typeof( T ) );

		public ISMBehaviour GetBehaviourInChildren( Type type )
			=> GetBehavioursInChildren( type ).FirstOrDefault();

		public IEnumerable<T> GetBehavioursInChildren<T>() where T : ISMBehaviour
			=> GetBehavioursInChildren( typeof( T ) )
				.Select( b => (T)b );

		public IEnumerable<ISMBehaviour> GetBehavioursInChildren( Type type )
			=> _body.GetAllChildren()
				.SelectMany( o => o._object.GetBehaviours( type ) );
	}
}