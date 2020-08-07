//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestSMTask {
	using System;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using KoganeUnityLib;
	using SMTask;
	using Extension;
	using Debug;
	using Test;



	// TODO : コメント追加、整頓



	public partial class TestSMObject : Test {
/*
		・単体作成テスト
		_type、_lifeSpan、_sceneが、適切に設定されるか？
		_type、_lifeSpan、_sceneにより、適切に登録されるか？
		_ownerちゃんと設定？
		SMMonoBehaviour、コンストラクタ呼ぶ？
		リンク解放される？
*/
		void CreateTestCreate() {
			new Type[] { typeof( B1 ), typeof( B2 ), typeof( B3 ), typeof( B4 ), typeof( B5 ), typeof( B6 ) }
				.ForEach( t => t.Create() );
			new Type[] { typeof( M1 ), typeof( M2 ), typeof( M3 ), typeof( M4 ), typeof( M5 ), typeof( M6 ) }
				.ForEach( t => {
					var go = new GameObject( $"{t.Name}" );
					go.SetActive( false );
					var mb = (SMMonoBehaviour)go.AddComponent( t );
				} );
		}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCreate() => From( TestCreateSub() );
		IEnumerator TestCreateSub() {
			Log.Debug( $"{nameof( TestCreateSub )}" );
			while ( true )	{ yield return null; }
		}


/*
		・複数動作の作成テスト
		_type、_lifeSpan、_sceneが、適切に設定されるか？
		_type、_lifeSpan、_sceneにより、適切に登録されるか？
		_ownerちゃんと設定？
		_behaviour、複数もちゃんと設定？
		リンク設定される？
		リンク解放される？
*/
		void CreateTestCreateBehaviours() => TestSMBehaviourUtility.CreateBehaviours( @"
			M1, M1, M1,
			M2, M2, M2,
			M3, M3, M3,
			M4, M4, M4,
			M5, M5, M5,
			M6, M6, M6,

			M1,
			M1, M2,
			M1, M2, M3,
			M1, M2, M3, M4,
			M1, M2, M3, M4, M5,
			M1, M2, M3, M4, M5, M6,

			M1, M4,
			M1, M5,
			M1, M6,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCreateBehaviours() => From( TestCreateBehavioursSub() );
		IEnumerator TestCreateBehavioursSub() {
			Log.Debug( $"{nameof( TestCreateBehavioursSub )}" );
			while ( true )	{ yield return null; }
		}


/*
		・複数物の作成テスト
		_top、_parent、_child、親子兄弟関係、が正しく設定されるか？
		SetupParent、SetupChildren、SetupBehaviours、それぞれ1回だけ呼ばれる？
		SetupTop、親子階層含めて、1回だけ呼ばれる？
		リンク設定される？
		リンク解放される？
*/
		void CreateTestCreateObjects1() => TestSMBehaviourUtility.CreateBehaviours( @"
			M1,
				null,
					M2,
					M1,
						M3,
							null,
							null,
								M4,
				null,
					M1,
				M1,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCreateObjects1() => From( TestCreateObjects1Sub() );
		IEnumerator TestCreateObjects1Sub() {
			Log.Debug( $"{nameof( TestCreateObjects1Sub )}" );
			while ( true )	{ yield return null; }
		}


		void CreateTestCreateObjects2() => TestSMBehaviourUtility.CreateBehaviours( @"
			M1,
				M1,
					M1,
			M2,
				null,
					null,
						M2,
							null,
								null,
									M2,
										null,
											null,
			null,
				M3,

			M1, M1, M1,
				null,
					M1, M1, M1,
						null
							M1, M1, M1,
			M1,
			M1,
				M2,
			M1,
				M2,
					M3,
			M1,
				M2,
					M3,
						M4,
			M1,
				M2,
					M3,
						M4,
							M5,
			M1,
				M2,
					M3,
						M4,
							M5,
								M6,
			M1, M1,
				M1, M2,
			M1, M1,
				M1, M3,
			M1, M1,
				M1, M4,
			M1, M1,
				M1, M5,
			M1, M1,
				M1, M6,
			M1,
				null,
					M2,
					M1,
						M3,
							null,
							null,
								M4,
				null,
					M1,
				M1,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCreateObjects2() => From( TestCreateObjects2Sub() );
		IEnumerator TestCreateObjects2Sub() {
			Log.Debug( $"{nameof( TestCreateObjects2Sub )}" );
			while ( true )	{ yield return null; }
		}
	}
}