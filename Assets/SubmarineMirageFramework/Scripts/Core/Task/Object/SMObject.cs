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
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMObject : SMStandardBase {
		[SMShowLine] public SMObjectBody _body	{ get; private set; }

		public SMAsyncCanceler _asyncCanceler => _body._asyncCanceler;



		public SMObject( SMGroupBody groupBody, SMObjectBody parentBody, GameObject gameObject,
							IEnumerable<ISMBehaviour> behaviours
		) {
			_body = new SMObjectBody( this, groupBody, parentBody, gameObject, behaviours );

			_disposables.AddLast( () => {
				_body.Dispose();
			} );
		}

		public override void Dispose() => base.Dispose();



		public static SMObject Instantiate( GameObject original, Vector3? position = null,
											Quaternion? rotation = null
		) {
			var go = (
				position.HasValue	? original.Instantiate( position.Value, rotation.Value )
									: original.Instantiate()
			);
			var bs = go.GetComponents<SMMonoBehaviour>();
			var group = new SMGroup( go, bs );
			return group._body._objectBody._object;
		}



		public void Destroy()
			=> _body.Destroy();

		public void ChangeActive( bool isActive )
			=> _body.ChangeActive( isActive );

		public void ChangeParent( Transform parent, bool isWorldPositionStays = true )
			=> _body.ChangeParent( parent, isWorldPositionStays );


		public T AddBehaviour<T>() where T : SMMonoBehaviour
			=> _body.AddBehaviour<T>();

		public SMMonoBehaviour AddBehaviour( Type type )
			=> _body.AddBehaviour( type );



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