//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestUtility {
	using System.Linq;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using UTask;
	using SMTask;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using Test;
	using TestSMTask;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;



	// TODO : コメント追加、整頓



	public class TestGameObjectUtility : Test {
		protected override void Create() {
			Application.targetFrameRate = 30;

			_createEvent.AddLast( async canceler => {
				Log.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestGetComponentsInParentUntilOneHierarchy ):
														CreateTestGetComponentsInParentUntilOneHierarchy();	break;
				}
				Log.Debug( $"end {nameof( Create )}{_testName}" );

				await UTask.DontWait();
			} );
		}



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetRootGameObjects() => From( async () => {
			await UnitySceneManager.LoadSceneAsync(
				"TestChange1", UnityEngine.SceneManagement.LoadSceneMode.Additive
				).ToUniTask( _asyncCanceler );
			UnitySceneManager.SetActiveScene( UnitySceneManager.GetSceneByName( "TestChange1" ) );
			new GameObject( "New" );
			var gos = UnitySceneManager.GetActiveScene().GetRootGameObjects();
			gos.ForEach( go => Log.Debug( go ) );

			await UTask.Never( _asyncCanceler );
		} );



		public void CreateTestGetComponentsInParentUntilOneHierarchy() => TestSMBehaviourUtility.CreateBehaviours(
			@"
				M1, M1,
					M1, M1,
						false,
							,
					M1, M1,
						false, M1, M1,
							,
					,
						false,
							M1, M1,
			"
		);

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetComponentsInParentUntilOneHierarchy() => From( async () => {
			var top = SceneManager.s_instance.GetBehaviour<M1>().transform;


			Log.Debug( "・0番目の最子テスト" );
			var child = top.GetChild( 0 ).GetChild( 0 ).GetChild( 0 );

			var bs = child.GetComponentsInParentUntilOneHierarchy<M1>( true );
			Log.Debug( string.Join( "\n",
				$"{nameof( GameObjectExtension.GetComponentsInParentUntilOneHierarchy )} :",
				string.Join( "\n", bs.Select( b => b.ToLineString() ) )
			) );

			bs = child.GetComponentsInParentUntilOneHierarchy<M1>( false );
			Log.Debug( string.Join( "\n",
				$"{nameof( GameObjectExtension.GetComponentsInParentUntilOneHierarchy )} :",
				string.Join( "\n", bs.Select( b => b.ToLineString() ) )
			) );

			var be = child.GetComponentInParentUntilOneHierarchy<M1>( true );
			Log.Debug(
				$"{nameof( GameObjectExtension.GetComponentsInParentUntilOneHierarchy )} : {be?.ToLineString()}" );

			be = child.GetComponentInParentUntilOneHierarchy<M1>( false );
			Log.Debug(
				$"{nameof( GameObjectExtension.GetComponentsInParentUntilOneHierarchy )} : {be?.ToLineString()}" );


			Log.Debug( "・2番目の最子テスト" );
			child = top.GetChild( 2 ).GetChild( 0 ).GetChild( 0 );
			bs = child.GetComponentsInParentUntilOneHierarchy<M1>( true );
			Log.Debug( string.Join( "\n",
				$"{nameof( GameObjectExtension.GetComponentsInParentUntilOneHierarchy )} :",
				string.Join( "\n", bs.Select( b => b.ToLineString() ) )
			) );

			bs = child.GetComponentsInParentUntilOneHierarchy<M1>( false );
			Log.Debug( string.Join( "\n",
				$"{nameof( GameObjectExtension.GetComponentsInParentUntilOneHierarchy )} :",
				string.Join( "\n", bs.Select( b => b.ToLineString() ) )
			) );

			be = child.GetComponentInParentUntilOneHierarchy<M1>( true );
			Log.Debug(
				$"{nameof( GameObjectExtension.GetComponentsInParentUntilOneHierarchy )} : {be?.ToLineString()}" );

			be = child.GetComponentInParentUntilOneHierarchy<M1>( false );
			Log.Debug(
				$"{nameof( GameObjectExtension.GetComponentsInParentUntilOneHierarchy )} : {be?.ToLineString()}" );


			await UTask.Never( _asyncCanceler );
		} );
	}
}