//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Utility {
	using UnityEngine;
	using Task.Behaviour;
	using Task.Object;
	using Extension;
	using UnityObject = UnityEngine.Object;


	// TODO : コメント追加、整頓


	public static class SMObjectSMUtility {
		static SMObject InstantiateObject( GameObject instance ) {
			var bs = instance.GetComponents<SMMonoBehaviour>();
			var parent = instance.GetComponentInParentUntilOneHierarchy<SMMonoBehaviour>( true )
				?._object;
			return new SMObject( instance.gameObject, bs, parent );
		}

		public static SMObject Instantiate( GameObject original, Transform parent, bool isWorldPositionStays ) {
			var go = UnityObject.Instantiate( original, parent, isWorldPositionStays );
			return InstantiateObject( go );
		}

		public static SMObject Instantiate( GameObject original, Transform parent ) {
			var go = UnityObject.Instantiate( original, parent );
			return InstantiateObject( go );
		}

		public static SMObject Instantiate( GameObject original, Vector3 position, Quaternion rotation,
											Transform parent
		) {
			var go = UnityObject.Instantiate( original, position, rotation, parent );
			return InstantiateObject( go );
		}

		public static SMObject Instantiate( GameObject original, Vector3 position, Quaternion rotation ) {
			var go = UnityObject.Instantiate( original, position, rotation );
			return InstantiateObject( go );
		}

		public static SMObject Instantiate( GameObject original ) {
			var go = UnityObject.Instantiate( original );
			return InstantiateObject( go );
		}
	}
}