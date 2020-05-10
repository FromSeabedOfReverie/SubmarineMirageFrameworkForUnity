//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestProcess {
	using System;
	using System.Threading;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using KoganeUnityLib;
	using Extension;
	using Debug;
	using Test;



	// TODO : コメント追加、整頓



	public partial class TestProcessHierarchy : Test {
/*
		・単体変数テスト
		_type、_lifeSpan、_sceneが、適切に設定されるか？
		_type、_lifeSpan、_sceneにより、適切に登録されるか？
		_ownerちゃんと設定？
*/
		void CreateTestVariable() {
#if false
			new Type[] { typeof( B1 ), typeof( B2 ), typeof( B3 ), typeof( B4 ), typeof( B5 ), typeof( B6 ) }
				.ForEach( t => t.Create() );
#else
			new Type[] { typeof( B1 ), typeof( B2 ), typeof( B3 ), typeof( B4 ), typeof( B5 ), typeof( B6 ) }
				.ForEach( t => t.Create() );
			new Type[] { typeof( M1 ), typeof( M2 ), typeof( M3 ), typeof( M4 ), typeof( M5 ), typeof( M6 ) }
				.ForEach( t => {
					var go = new GameObject( $"{t.Name}", t );
					go.SetActive( false );
				} );
#endif
		}

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestVariable() => From( TestVariableSub() );
		IEnumerator TestVariableSub() {
			Log.Debug( "TestVariableSub" );
			while ( true )	{ yield return null; }
		}


/*
		・複数変数テスト
		_type、_lifeSpan、_sceneが、適切に設定されるか？
		_type、_lifeSpan、_sceneにより、適切に登録されるか？
		_ownerちゃんと設定？
		_processes、複数もちゃんと設定？
*/
		void CreateTestMultiVariable() => TestProcessUtility.CreateMonoBehaviourProcess( @"
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

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestMultiVariable() => From( TestMultiVariableSub() );
		IEnumerator TestMultiVariableSub() {
			Log.Debug( "TestMultiVariableSub" );
			while ( true )	{ yield return null; }
		}

/*
		・階層テスト
		_top、_parent、_children、親子兄弟関係、が正しく設定されるか？
		SetParent、SetChildren、SetBrothers、それぞれ1回だけ呼ばれる？
		SetTop、SetAllData、親子階層含めて、1回だけ呼ばれる？
*/
		void CreateTestHierarchy() => TestProcessUtility.CreateMonoBehaviourProcess(
#if false
		@"
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
		"
#else
		@"
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
		"
#endif
		);

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestHierarchy() => From( TestHierarchySub() );
		IEnumerator TestHierarchySub() {
			Log.Debug( "TestHierarchySub" );
			while ( true )	{ yield return null; }
		}
	}
}