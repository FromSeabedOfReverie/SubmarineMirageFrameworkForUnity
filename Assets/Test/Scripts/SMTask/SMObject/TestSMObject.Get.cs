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
	using UnityEngine.TestTools;
	using KoganeUnityLib;
	using Scene;
	using Extension;
	using Debug;
	using Test;



	// TODO : コメント追加、整頓



	public partial class TestSMObject : Test {
/*
		・SMObject取得テスト
		GetAllParents、GetAllChildren、動作確認
*/
		void CreateTestGetObjects() => TestSMTaskUtility.CreateSMMonoBehaviour( @"
			M1, M1, M2,
				null,
					M2,
					M1,
						M3,
							null,
								M4,
									M5,
							null,
								M4, M4,
				null,
					M1,
						M6,
							M1, M1
				M1,
		" );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestGetObjects() => From( TestGetObjectsSub() );
		IEnumerator TestGetObjectsSub() {
			Log.Debug( $"{nameof( TestGetObjectsSub )}" );

			var top = SceneManager.s_instance.GetBehaviour<M1>()._object;
			var center = SceneManager.s_instance.GetBehaviour<M3>()._object;
			var bottom = SceneManager.s_instance.GetBehaviour<M5>()._object;
			TestSMTaskUtility.LogSMObject( $"{nameof( top )}",		top );
			TestSMTaskUtility.LogSMObject( $"{nameof( center )}",	center );
			TestSMTaskUtility.LogSMObject( $"{nameof( bottom )}",	bottom );

			TestSMTaskUtility.LogSMObjects( $"{nameof( top.GetAllChildren )}",		top.GetAllChildren() );
			TestSMTaskUtility.LogSMObjects( $"{nameof( center.GetAllChildren )}",	center.GetAllChildren() );
			TestSMTaskUtility.LogSMObjects( $"{nameof( bottom.GetAllChildren )}",	bottom.GetAllChildren() );

			TestSMTaskUtility.LogSMObjects( $"{nameof( top.GetAllParents )}",		top.GetAllParents() );
			TestSMTaskUtility.LogSMObjects( $"{nameof( center.GetAllParents )}",	center.GetAllParents() );
			TestSMTaskUtility.LogSMObjects( $"{nameof( bottom.GetAllParents )}",	bottom.GetAllParents() );

			while ( true )	{ yield return null; }
		}


/*
		・取得テスト
		GetBehaviour<T>、GetBehaviour(type)、GetBehaviours<T>、GetBehaviours(type)、動作確認
*/
		void CreateTestGetBehaviour()
			=> CreateTestGetObjects();

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestGetBehaviour() => From( TestGetBehaviourSub() );
		IEnumerator TestGetBehaviourSub() {
			Log.Debug( $"{nameof( TestGetBehaviourSub )}" );

			var top = SceneManager.s_instance.GetBehaviour<M1>()._object;
			TestSMTaskUtility.LogSMObject( $"{nameof( top )}", top );

			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}<{nameof( M1 )}>",
				top.GetBehaviour<M1>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}({nameof( M1 )})",
				top.GetBehaviour( typeof( M1 ) ) );

			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}<{nameof( M2 )}>",
				top.GetBehaviour<M2>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}({nameof( M2 )})",
				top.GetBehaviour( typeof( M2 ) ) );

			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}<{nameof( M3 )}>",
				top.GetBehaviour<M3>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}({nameof( M3 )})",
				top.GetBehaviour( typeof( M3 ) ) );


			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}<{nameof( M1 )}>",
				top.GetBehaviours<M1>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}({nameof( M1 )})",
				top.GetBehaviours( typeof( M1 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}<{nameof( M2 )}>",
				top.GetBehaviours<M2>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}({nameof( M2 )})",
				top.GetBehaviours( typeof( M2 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}<{nameof( M3 )}>",
				top.GetBehaviours<M3>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}({nameof( M3 )})",
				top.GetBehaviours( typeof( M3 ) ) );

			while ( true )	{ yield return null; }
		}


/*
		・取得テスト
		GetBehaviourInParent<T>、GetBehaviourInParent(type)
		GetBehavioursInParent<T>、GetBehavioursInParent(type)
		GetBehaviourInChildren<T>、GetBehaviourInChildren(type)
		GetBehavioursInChildren<T>、GetBehavioursInChildren(type)
*/
		void CreateTestGetObjectBehaviour()
			=> CreateTestGetObjects();

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestGetObjectBehaviour() => From( TestGetObjectBehaviourSub() );
		IEnumerator TestGetObjectBehaviourSub() {
			Log.Debug( $"{nameof( TestGetObjectBehaviourSub )}" );

			var top = SceneManager.s_instance.GetBehaviour<M1>()._object;
			var center = SceneManager.s_instance.GetBehaviour<M3>()._object;
			var bottom = SceneManager.s_instance.GetBehaviour<M5>()._object;
			TestSMTaskUtility.LogSMObject( $"{nameof( top )}",		top );
			TestSMTaskUtility.LogSMObject( $"{nameof( center )}",	center );
			TestSMTaskUtility.LogSMObject( $"{nameof( bottom )}",	bottom );


			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviourInParent )}<{nameof( M1 )}>",
				top.GetBehaviourInParent<M1>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviourInParent )}({nameof( M1 )})",
				top.GetBehaviourInParent( typeof( M1 ) ) );

			TestSMTaskUtility.LogBehaviour( $"{nameof( center.GetBehaviourInParent )}<{nameof( M1 )}>",
				center.GetBehaviourInParent<M1>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( center.GetBehaviourInParent )}({nameof( M1 )})",
				center.GetBehaviourInParent( typeof( M1 ) ) );

			TestSMTaskUtility.LogBehaviour( $"{nameof( bottom.GetBehaviourInParent )}<{nameof( M1 )}>",
				bottom.GetBehaviourInParent<M1>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( bottom.GetBehaviourInParent )}({nameof( M1 )})",
				bottom.GetBehaviourInParent( typeof( M1 ) ) );


			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehavioursInParent )}<{nameof( M1 )}>",
				top.GetBehavioursInParent<M1>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehavioursInParent )}({nameof( M1 )})",
				top.GetBehavioursInParent( typeof( M1 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( center.GetBehavioursInParent )}<{nameof( M1 )}>",
				center.GetBehavioursInParent<M1>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( center.GetBehavioursInParent )}({nameof( M1 )})",
				center.GetBehavioursInParent( typeof( M1 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( bottom.GetBehavioursInParent )}<{nameof( M1 )}>",
				bottom.GetBehavioursInParent<M1>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( bottom.GetBehavioursInParent )}({nameof( M1 )})",
				bottom.GetBehavioursInParent( typeof( M1 ) ) );


			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviourInChildren )}<{nameof( M1 )}>",
				top.GetBehaviourInChildren<M1>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviourInChildren )}({nameof( M1 )})",
				top.GetBehaviourInChildren( typeof( M1 ) ) );

			TestSMTaskUtility.LogBehaviour( $"{nameof( center.GetBehaviourInChildren )}<{nameof( M1 )}>",
				center.GetBehaviourInChildren<M1>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( center.GetBehaviourInChildren )}({nameof( M1 )})",
				center.GetBehaviourInChildren( typeof( M1 ) ) );

			TestSMTaskUtility.LogBehaviour( $"{nameof( bottom.GetBehaviourInChildren )}<{nameof( M1 )}>",
				bottom.GetBehaviourInChildren<M1>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( bottom.GetBehaviourInChildren )}({nameof( M1 )})",
				bottom.GetBehaviourInChildren( typeof( M1 ) ) );


			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehavioursInChildren )}<{nameof( M1 )}>",
				top.GetBehavioursInChildren<M1>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehavioursInChildren )}({nameof( M1 )})",
				top.GetBehavioursInChildren( typeof( M1 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( center.GetBehavioursInChildren )}<{nameof( M1 )}>",
				center.GetBehavioursInChildren<M1>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( center.GetBehavioursInChildren )}({nameof( M1 )})",
				center.GetBehavioursInChildren( typeof( M1 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( bottom.GetBehavioursInChildren )}<{nameof( M1 )}>",
				bottom.GetBehavioursInChildren<M1>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( bottom.GetBehavioursInChildren )}({nameof( M1 )})",
				bottom.GetBehavioursInChildren( typeof( M1 ) ) );


			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviourInParent )}<{nameof( M4 )}>",
				top.GetBehaviourInParent<M4>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviourInParent )}({nameof( M4 )})",
				top.GetBehaviourInParent( typeof( M4 ) ) );

			TestSMTaskUtility.LogBehaviour( $"{nameof( center.GetBehaviourInParent )}<{nameof( M4 )}>",
				center.GetBehaviourInParent<M4>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( center.GetBehaviourInParent )}({nameof( M4 )})",
				center.GetBehaviourInParent( typeof( M4 ) ) );

			TestSMTaskUtility.LogBehaviour( $"{nameof( bottom.GetBehaviourInParent )}<{nameof( M4 )}>",
				bottom.GetBehaviourInParent<M4>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( bottom.GetBehaviourInParent )}({nameof( M4 )})",
				bottom.GetBehaviourInParent( typeof( M4 ) ) );


			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehavioursInParent )}<{nameof( M4 )}>",
				top.GetBehavioursInParent<M4>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehavioursInParent )}({nameof( M4 )})",
				top.GetBehavioursInParent( typeof( M4 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( center.GetBehavioursInParent )}<{nameof( M4 )}>",
				center.GetBehavioursInParent<M4>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( center.GetBehavioursInParent )}({nameof( M4 )})",
				center.GetBehavioursInParent( typeof( M4 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( bottom.GetBehavioursInParent )}<{nameof( M4 )}>",
				bottom.GetBehavioursInParent<M4>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( bottom.GetBehavioursInParent )}({nameof( M4 )})",
				bottom.GetBehavioursInParent( typeof( M4 ) ) );


			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviourInChildren )}<{nameof( M4 )}>",
				top.GetBehaviourInChildren<M4>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviourInChildren )}({nameof( M4 )})",
				top.GetBehaviourInChildren( typeof( M4 ) ) );

			TestSMTaskUtility.LogBehaviour( $"{nameof( center.GetBehaviourInChildren )}<{nameof( M4 )}>",
				center.GetBehaviourInChildren<M4>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( center.GetBehaviourInChildren )}({nameof( M4 )})",
				center.GetBehaviourInChildren( typeof( M4 ) ) );

			TestSMTaskUtility.LogBehaviour( $"{nameof( bottom.GetBehaviourInChildren )}<{nameof( M4 )}>",
				bottom.GetBehaviourInChildren<M4>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( bottom.GetBehaviourInChildren )}({nameof( M4 )})",
				bottom.GetBehaviourInChildren( typeof( M4 ) ) );


			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehavioursInChildren )}<{nameof( M4 )}>",
				top.GetBehavioursInChildren<M4>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehavioursInChildren )}({nameof( M4 )})",
				top.GetBehavioursInChildren( typeof( M4 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( center.GetBehavioursInChildren )}<{nameof( M4 )}>",
				center.GetBehavioursInChildren<M4>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( center.GetBehavioursInChildren )}({nameof( M4 )})",
				center.GetBehavioursInChildren( typeof( M4 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( bottom.GetBehavioursInChildren )}<{nameof( M4 )}>",
				bottom.GetBehavioursInChildren<M4>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( bottom.GetBehavioursInChildren )}({nameof( M4 )})",
				bottom.GetBehavioursInChildren( typeof( M4 ) ) );

			while ( true )	{ yield return null; }
		}


/*
		・SMMonoBehaviour内の取得テスト
*/
		void CreateTestGetInSMMonoBehaviour()
			=> CreateTestGetObjects();

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestGetInSMMonoBehaviour() => From( TestGetInSMMonoBehaviourSub() );
		IEnumerator TestGetInSMMonoBehaviourSub() {
			Log.Debug( $"{nameof( TestGetInSMMonoBehaviourSub )}" );

			var top = SceneManager.s_instance.GetBehaviour<M1>();
			var center = SceneManager.s_instance.GetBehaviour<M3>();
			TestSMTaskUtility.LogSMObject( $"{nameof( top )}",		top._object );
			TestSMTaskUtility.LogSMObject( $"{nameof( center )}",	center._object );


			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}<{nameof( M1 )}>",
				top.GetBehaviour<M1>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}({nameof( M1 )})",
				top.GetBehaviour( typeof( M1 ) ) );

			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}<{nameof( M2 )}>",
				top.GetBehaviour<M2>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}({nameof( M2 )})",
				top.GetBehaviour( typeof( M2 ) ) );

			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}<{nameof( M3 )}>",
				top.GetBehaviour<M3>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}({nameof( M3 )})",
				top.GetBehaviour( typeof( M3 ) ) );


			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}<{nameof( M1 )}>",
				top.GetBehaviours<M1>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}({nameof( M1 )})",
				top.GetBehaviours( typeof( M1 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}<{nameof( M2 )}>",
				top.GetBehaviours<M2>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}({nameof( M2 )})",
				top.GetBehaviours( typeof( M2 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}<{nameof( M3 )}>",
				top.GetBehaviours<M3>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}({nameof( M3 )})",
				top.GetBehaviours( typeof( M3 ) ) );


			TestSMTaskUtility.LogBehaviour( $"{nameof( center.GetBehaviourInParent )}<{nameof( M1 )}>",
				center.GetBehaviourInParent<M1>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( center.GetBehaviourInParent )}({nameof( M1 )})",
				center.GetBehaviourInParent( typeof( M1 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( center.GetBehavioursInParent )}<{nameof( M1 )}>",
				center.GetBehavioursInParent<M1>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( center.GetBehavioursInParent )}({nameof( M1 )})",
				center.GetBehavioursInParent( typeof( M1 ) ) );


			TestSMTaskUtility.LogBehaviour( $"{nameof( center.GetBehaviourInChildren )}<{nameof( M1 )}>",
				center.GetBehaviourInChildren<M1>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( center.GetBehaviourInChildren )}({nameof( M1 )})",
				center.GetBehaviourInChildren( typeof( M1 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( center.GetBehavioursInChildren )}<{nameof( M1 )}>",
				center.GetBehavioursInChildren<M1>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( center.GetBehavioursInChildren )}({nameof( M1 )})",
				center.GetBehavioursInChildren( typeof( M1 ) ) );


			TestSMTaskUtility.LogBehaviour( $"{nameof( center.GetBehaviourInParent )}<{nameof( M4 )}>",
				center.GetBehaviourInParent<M4>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( center.GetBehaviourInParent )}({nameof( M4 )})",
				center.GetBehaviourInParent( typeof( M4 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( center.GetBehavioursInParent )}<{nameof( M4 )}>",
				center.GetBehavioursInParent<M4>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( center.GetBehavioursInParent )}({nameof( M4 )})",
				center.GetBehavioursInParent( typeof( M4 ) ) );


			TestSMTaskUtility.LogBehaviour( $"{nameof( center.GetBehaviourInChildren )}<{nameof( M4 )}>",
				center.GetBehaviourInChildren<M4>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( center.GetBehaviourInChildren )}({nameof( M4 )})",
				center.GetBehaviourInChildren( typeof( M4 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( center.GetBehavioursInChildren )}<{nameof( M4 )}>",
				center.GetBehavioursInChildren<M4>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( center.GetBehavioursInChildren )}({nameof( M4 )})",
				center.GetBehavioursInChildren( typeof( M4 ) ) );

			while ( true )	{ yield return null; }
		}


/*
		・SMBehaviourの取得テスト
*/
		void CreateTestGetSMBehaviour() {
			new Type[] { typeof( B1 ), typeof( B2 ), typeof( B3 ), typeof( B4 ), typeof( B5 ), typeof( B6 ) }
				.ForEach( t => t.Create() );
		}

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestGetSMBehaviour() => From( TestGetSMBehaviourSub() );
		IEnumerator TestGetSMBehaviourSub() {
			Log.Debug( $"{nameof( TestGetSMBehaviourSub )}" );

			var top = SceneManager.s_instance.GetBehaviour<B1>()._object;
			TestSMTaskUtility.LogSMObject( $"{nameof( top )}",	top );

			TestSMTaskUtility.LogSMObjects( $"{nameof( top.GetAllChildren )}",	top.GetAllChildren() );
			TestSMTaskUtility.LogSMObjects( $"{nameof( top.GetAllParents )}",	top.GetAllParents() );

			
			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}<{nameof( B1 )}>",
				top.GetBehaviour<B1>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}({nameof( B1 )})",
				top.GetBehaviour( typeof( B1 ) ) );

			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}<{nameof( B2 )}>",
				top.GetBehaviour<B2>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}({nameof( B2 )})",
				top.GetBehaviour( typeof( B2 ) ) );


			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}<{nameof( B1 )}>",
				top.GetBehaviours<B1>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}({nameof( B1 )})",
				top.GetBehaviours( typeof( B1 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}<{nameof( B2 )}>",
				top.GetBehaviours<B2>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}({nameof( B2 )})",
				top.GetBehaviours( typeof( B2 ) ) );


			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviourInParent )}<{nameof( B1 )}>",
				top.GetBehaviourInParent<B1>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviourInParent )}({nameof( B1 )})",
				top.GetBehaviourInParent( typeof( B1 ) ) );

			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviourInParent )}<{nameof( B2 )}>",
				top.GetBehaviourInParent<B2>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviourInParent )}({nameof( B2 )})",
				top.GetBehaviourInParent( typeof( B2 ) ) );


			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehavioursInParent )}<{nameof( B1 )}>",
				top.GetBehavioursInParent<B1>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehavioursInParent )}({nameof( B1 )})",
				top.GetBehavioursInParent( typeof( B1 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehavioursInParent )}<{nameof( B2 )}>",
				top.GetBehavioursInParent<B2>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehavioursInParent )}({nameof( B2 )})",
				top.GetBehavioursInParent( typeof( B2 ) ) );


			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviourInChildren )}<{nameof( B1 )}>",
				top.GetBehaviourInChildren<B1>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviourInChildren )}({nameof( B1 )})",
				top.GetBehaviourInChildren( typeof( B1 ) ) );

			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviourInChildren )}<{nameof( B2 )}>",
				top.GetBehaviourInChildren<B2>() );
			TestSMTaskUtility.LogBehaviour( $"{nameof( top.GetBehaviourInChildren )}({nameof( B2 )})",
				top.GetBehaviourInChildren( typeof( B2 ) ) );


			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehavioursInChildren )}<{nameof( B1 )}>",
				top.GetBehavioursInChildren<B1>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehavioursInChildren )}({nameof( B1 )})",
				top.GetBehavioursInChildren( typeof( B1 ) ) );

			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehavioursInChildren )}<{nameof( B2 )}>",
				top.GetBehavioursInChildren<B2>() );
			TestSMTaskUtility.LogBehaviours( $"{nameof( top.GetBehavioursInChildren )}({nameof( B2 )})",
				top.GetBehavioursInChildren( typeof( B2 ) ) );

			while ( true )	{ yield return null; }
		}
	}
}