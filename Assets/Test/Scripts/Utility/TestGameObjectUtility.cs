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
	using UnityEngine.SceneManagement;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using Test;
	using TestTask;



	// TODO : コメント追加、整頓



	public class TestGameObjectUtility : SMStandardTest {
		protected override void Create() {
			Application.targetFrameRate = 30;

			_createEvent.AddLast( async canceler => {
				SMLog.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestGetComponentsInParentUntilOneHierarchy ):
														CreateTestGetComponentsInParentUntilOneHierarchy();	break;
				}
				SMLog.Debug( $"end {nameof( Create )}{_testName}" );

				await UTask.DontWait();
			} );
		}



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetRootGameObjects() => From( async () => {
			await SceneManager.LoadSceneAsync( "TestChange1", LoadSceneMode.Additive )
				.ToUniTask( _asyncCanceler );
			SceneManager.SetActiveScene( SceneManager.GetSceneByName( "TestChange1" ) );
			new GameObject( "New" );
			var gos = SceneManager.GetActiveScene().GetRootGameObjects();
			gos.ForEach( go => SMLog.Debug( go ) );

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
			var top = SMSceneManager.s_instance.GetBehaviour<M1>().transform;


			SMLog.Debug( "・0番目の最子テスト" );
			var child = top.GetChild( 0 ).GetChild( 0 ).GetChild( 0 );

			var bs = child.GetComponentsInParentUntilOneHierarchy<M1>( true );
			SMLog.Debug( string.Join( "\n",
				$"{nameof( GameObjectSMExtension.GetComponentsInParentUntilOneHierarchy )} :",
				string.Join( "\n", bs.Select( b => b.ToLineString() ) )
			) );

			bs = child.GetComponentsInParentUntilOneHierarchy<M1>( false );
			SMLog.Debug( string.Join( "\n",
				$"{nameof( GameObjectSMExtension.GetComponentsInParentUntilOneHierarchy )} :",
				string.Join( "\n", bs.Select( b => b.ToLineString() ) )
			) );

			var be = child.GetComponentInParentUntilOneHierarchy<M1>( true );
			SMLog.Debug(
				$"{nameof( GameObjectSMExtension.GetComponentsInParentUntilOneHierarchy )} : {be?.ToLineString()}" );

			be = child.GetComponentInParentUntilOneHierarchy<M1>( false );
			SMLog.Debug(
				$"{nameof( GameObjectSMExtension.GetComponentsInParentUntilOneHierarchy )} : {be?.ToLineString()}" );


			SMLog.Debug( "・2番目の最子テスト" );
			child = top.GetChild( 2 ).GetChild( 0 ).GetChild( 0 );
			bs = child.GetComponentsInParentUntilOneHierarchy<M1>( true );
			SMLog.Debug( string.Join( "\n",
				$"{nameof( GameObjectSMExtension.GetComponentsInParentUntilOneHierarchy )} :",
				string.Join( "\n", bs.Select( b => b.ToLineString() ) )
			) );

			bs = child.GetComponentsInParentUntilOneHierarchy<M1>( false );
			SMLog.Debug( string.Join( "\n",
				$"{nameof( GameObjectSMExtension.GetComponentsInParentUntilOneHierarchy )} :",
				string.Join( "\n", bs.Select( b => b.ToLineString() ) )
			) );

			be = child.GetComponentInParentUntilOneHierarchy<M1>( true );
			SMLog.Debug(
				$"{nameof( GameObjectSMExtension.GetComponentsInParentUntilOneHierarchy )} : {be?.ToLineString()}" );

			be = child.GetComponentInParentUntilOneHierarchy<M1>( false );
			SMLog.Debug(
				$"{nameof( GameObjectSMExtension.GetComponentsInParentUntilOneHierarchy )} : {be?.ToLineString()}" );


			await UTask.Never( _asyncCanceler );
		} );
	}
}