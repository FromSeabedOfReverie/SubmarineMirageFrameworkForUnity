//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process.New {
	using UnityEngine;
	using KoganeUnityLib;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class TestGameObjectUtility {

		public TestGameObjectUtility() {
			var top = new GameObject( "TestHoge top" );
			top.AddComponent<TestHoge>();
//			top.SetActive( false );
			var parent = top.transform;
			GameObject topChild = null;
			var id = 0;

			3.Times( brotherIndex => {
				3.Times( hierarchyIndex => {
					var go = new GameObject();
					go.name = $"TestHoge {id++}";
					go.SetParent( parent );
					parent = go.transform;

					if ( hierarchyIndex == 2 )	{ go.SetActive( false ); }
					if ( hierarchyIndex <= 0 && brotherIndex == 0 )	{ go.AddComponent<TestHoge>(); }
					if ( hierarchyIndex <= 1 && brotherIndex == 1 )	{ go.AddComponent<TestHoge>(); }
					if ( hierarchyIndex <= 2 && brotherIndex == 2 )	{ go.AddComponent<TestHoge>(); }
					if ( hierarchyIndex == 2 && brotherIndex == 0 )	{ topChild = go; }
				} );
				parent = top.transform;
			} );

			var ts = topChild.GetComponentsInParentUntilOneHierarchy<TestHoge>( false );
			ts.ForEach( testHoge => Log.Debug( testHoge.gameObject.name ) );
		}
	}


	public class TestHoge : MonoBehaviour {
		static int s_count;
		public int _id	{ get; private set; }
		void Awake() {
			_id = s_count++;
		}
	}
}