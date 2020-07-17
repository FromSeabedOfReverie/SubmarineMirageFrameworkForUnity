//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestUtility {
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using UTask;
	using Extension;
	using Debug;
	using Test;


	// TODO : コメント追加、整頓


	public class TestGameObjectUtility : Test {
		protected override void Create() {}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestGetRootGameObjects() => From( async () => {
			await SceneManager.LoadSceneAsync( "TestChange1", LoadSceneMode.Additive )
				.ToUniTask( _asyncCanceler );
			SceneManager.SetActiveScene( SceneManager.GetSceneByName( "TestChange1" ) );
			new GameObject( "New" );
			var gos = SceneManager.GetActiveScene().GetRootGameObjects();
			gos.ForEach( go => Log.Debug( go ) );
			await UTask.WaitWhile( _asyncCanceler, () => true );
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestGetComponentsInParentUntilOneHierarchy() => From( async () => {
			var top = new GameObject( $"{nameof( TestMono )} top" );
			top.AddComponent<TestMono>();
//			top.SetActive( false );
			var parent = top.transform;
			GameObject topChild = null;
			var id = 0;

			3.Times( brotherIndex => {
				3.Times( hierarchyIndex => {
					var go = new GameObject();
					go.name = $"{nameof( TestMono )} {id++}";
					go.SetParent( parent );
					parent = go.transform;

					if ( hierarchyIndex == 2 )	{ go.SetActive( false ); }
					if ( hierarchyIndex <= 0 && brotherIndex == 0 )	{ go.AddComponent<TestMono>(); }
					if ( hierarchyIndex <= 1 && brotherIndex == 1 )	{ go.AddComponent<TestMono>(); }
					if ( hierarchyIndex <= 2 && brotherIndex == 2 )	{ go.AddComponent<TestMono>(); }
					if ( hierarchyIndex == 2 && brotherIndex == 0 )	{ topChild = go; }
				} );
				parent = top.transform;
			} );

			var ts = topChild.GetComponentsInParentUntilOneHierarchy<TestMono>( true );
			ts.ForEach( testHoge => Log.Debug( testHoge.gameObject.name ) );

			await UTask.WaitWhile( _asyncCanceler, () => true );
		} );


		public class TestMono : MonoBehaviour {
			static int s_count;
			public int _id	{ get; private set; }
			void Awake() {
				_id = s_count++;
			}
		}
	}
}