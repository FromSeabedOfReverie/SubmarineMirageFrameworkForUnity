//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Utility {
	using UnityEngine;
	using Process.New;
	using Extension;
	using UnityObject = UnityEngine.Object;


	// TODO : コメント追加、整頓


	public static class ProcessUtility {
		static ProcessHierarchy InstantiateHierarchy( GameObject instance ) {
			var ps = instance.GetComponents<MonoBehaviourProcess>();
			var parent = instance.GetComponentInParentUntilOneHierarchy<MonoBehaviourProcess>( true )
				?._hierarchy;
			return new ProcessHierarchy( instance.gameObject, ps, parent );
		}

		public static ProcessHierarchy Instantiate( GameObject original, Transform parent,
													bool isWorldPositionStays
		) {
			var go = UnityObject.Instantiate( original, parent, isWorldPositionStays );
			return InstantiateHierarchy( go );
		}

		public static ProcessHierarchy Instantiate( GameObject original, Transform parent ) {
			var go = UnityObject.Instantiate( original, parent );
			return InstantiateHierarchy( go );
		}

		public static ProcessHierarchy Instantiate( GameObject original, Vector3 position, Quaternion rotation,
													Transform parent
		) {
			var go = UnityObject.Instantiate( original, position, rotation, parent );
			return InstantiateHierarchy( go );
		}

		public static ProcessHierarchy Instantiate( GameObject original, Vector3 position, Quaternion rotation ) {
			var go = UnityObject.Instantiate( original, position, rotation );
			return InstantiateHierarchy( go );
		}

		public static ProcessHierarchy Instantiate( GameObject original ) {
			var go = UnityObject.Instantiate( original );
			return InstantiateHierarchy( go );
		}
	}
}