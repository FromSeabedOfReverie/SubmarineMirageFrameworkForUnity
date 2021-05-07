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
	using Base;
	using Scene;
	using Extension;
	using Utility;
	using Debug;



	public class SMObject : SMStandardBase {
		[SMShowLine] public SMObjectBody _body	{ get; private set; }

		public SMAsyncCanceler _asyncCanceler => _body._asyncCanceler;



		public SMObject( SMGroupBody groupBody, SMObjectBody parentBody, GameObject gameObject,
							IEnumerable<SMBehaviour> behaviours
		) {
			_body = new SMObjectBody( this, groupBody, parentBody, gameObject, behaviours );

			_disposables.AddLast( () => {
				_body.Dispose();
			} );
		}

		public override void Dispose() => base.Dispose();



		public static SMObject Instantiate( GameObject original, SMScene scene, Vector3? position = null,
											Quaternion? rotation = null
		) {
			var go = (
				position.HasValue	? original.Instantiate( position.Value, rotation.Value )
									: original.Instantiate()
			);
			var bs = go.GetComponents<SMBehaviour>();
			var group = new SMGroup( scene._groupManagerBody, go, bs );
			return group._body._objectBody._object;
		}



		public void Destroy()
			=> _body.Destroy();

		public void ChangeActive( bool isActive )
			=> _body.ChangeActive( isActive );

		public void ChangeParent( Transform parent, bool isWorldPositionStays = true )
			=> _body.ChangeParent( parent, isWorldPositionStays );


		public T AddBehaviour<T>() where T : SMBehaviour
			=> _body.AddBehaviour<T>();

		public SMBehaviour AddBehaviour( Type type )
			=> _body.AddBehaviour( type );



		public T GetBehaviour<T>() where T : SMBehaviour
			=> _body._behaviourBody._behaviour.GetBehaviour<T>();

		public SMBehaviour GetBehaviour( Type type )
			=> _body._behaviourBody._behaviour.GetBehaviour( type );

		public IEnumerable<T> GetBehaviours<T>() where T : SMBehaviour
			=> _body._behaviourBody._behaviour.GetBehaviours<T>();

		public IEnumerable<SMBehaviour> GetBehaviours( Type type )
			=> _body._behaviourBody._behaviour.GetBehaviours( type );


		public T GetBehaviourInParent<T>() where T : SMBehaviour
			=> (T)GetBehaviourInParent( typeof( T ) );

		public SMBehaviour GetBehaviourInParent( Type type )
			=> GetBehavioursInParent( type ).FirstOrDefault();

		public IEnumerable<T> GetBehavioursInParent<T>() where T : SMBehaviour
			=> GetBehavioursInParent( typeof( T ) )
				.Select( b => (T)b );

		public IEnumerable<SMBehaviour> GetBehavioursInParent( Type type )
			=> _body.GetAllParents()
				.SelectMany( o => o._object.GetBehaviours( type ) );


		public T GetBehaviourInChildren<T>() where T : SMBehaviour
			=> (T)GetBehaviourInChildren( typeof( T ) );

		public SMBehaviour GetBehaviourInChildren( Type type )
			=> GetBehavioursInChildren( type ).FirstOrDefault();

		public IEnumerable<T> GetBehavioursInChildren<T>() where T : SMBehaviour
			=> GetBehavioursInChildren( typeof( T ) )
				.Select( b => (T)b );

		public IEnumerable<SMBehaviour> GetBehavioursInChildren( Type type )
			=> _body.GetAllChildren()
				.SelectMany( o => o._object.GetBehaviours( type ) );
	}
}