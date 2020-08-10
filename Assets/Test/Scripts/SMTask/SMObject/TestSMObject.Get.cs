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
	using UTask;
	using SMTask;
	using Scene;
	using Extension;
	using Debug;
	using Test;



	// TODO : コメント追加、整頓



	public partial class TestSMObject : Test {
		void CreateTestObjects() => TestSMBehaviourUtility.CreateBehaviours( @"
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
							M1, M1,
				M1,
		" );


/*
		・SMObject取得テスト
		GetFirst、GetLast、GetLastChild、動作確認
*/
		void CreateTestGetFirstLastChild() => TestSMBehaviourUtility.CreateBehaviours( @"
			M4,
			M4,
			M4,
			M1,
				M1,
				M1,
				M1,
			M2,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetFirstLastChild() => From( async () => {
			Log.Debug( $"{nameof( TestGetFirstLastChild )}" );

			Log.Debug( "・最上階テスト" );
			var start = SceneManager.s_instance.GetBehaviour<M4>()._object._next;
			TestSMObjectUtility.LogObject( $"{nameof( SMObject.GetFirst )}",	start.GetFirst() );
			TestSMObjectUtility.LogObject( $"{nameof( SMObject.GetLast )}",		start.GetLast() );

			Log.Debug( "・子テスト" );
			var child = SceneManager.s_instance.GetBehaviour<M1>()._object._child._next;
			TestSMObjectUtility.LogObject( $"{nameof( SMObject.GetFirst )}",	child.GetFirst() );
			TestSMObjectUtility.LogObject( $"{nameof( SMObject.GetLast )}",		child.GetLast() );
			var parent = SceneManager.s_instance.GetBehaviour<M1>()._object;
			TestSMObjectUtility.LogObject( $"{nameof( SMObject.GetLastChild )}",	parent.GetLastChild() );

			Log.Debug( "・単体取得テスト" );
			start = SceneManager.s_instance.GetBehaviour<M2>()._object;
			TestSMObjectUtility.LogObject( $"{nameof( SMObject.GetFirst )}",		start.GetFirst() );
			TestSMObjectUtility.LogObject( $"{nameof( SMObject.GetLast )}",			start.GetLast() );

			Log.Debug( "・取得不可テスト" );
			TestSMObjectUtility.LogObject( $"{nameof( SMObject.GetLastChild )}",	start.GetLastChild() );

			await UTask.Never( _asyncCanceler );
		} );


/*
		・SMObject取得テスト
		GetBrothers、GetChildren、動作確認
*/
		void CreateTestGetBrothersChildren() => TestSMBehaviourUtility.CreateBehaviours( @"
			M4,
			M4,
			M4,
			M1,
				M1,
				M1,
				M1,
			M2,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetBrothersChildren() => From( async () => {
			Log.Debug( $"{nameof( TestGetBrothersChildren )}" );

			Log.Debug( "・最上階テスト" );
			var start = SceneManager.s_instance.GetBehaviour<M4>()._object._next;
			TestSMObjectUtility.LogObjects( $"{nameof( SMObject.GetBrothers )}",	start.GetBrothers() );

			Log.Debug( "・子テスト" );
			var child = SceneManager.s_instance.GetBehaviour<M1>()._object._child._next;
			TestSMObjectUtility.LogObjects( $"{nameof( SMObject.GetBrothers )}",	child.GetBrothers() );
			var parent = SceneManager.s_instance.GetBehaviour<M1>()._object;
			TestSMObjectUtility.LogObjects( $"{nameof( SMObject.GetChildren )}",	parent.GetChildren() );

			Log.Debug( "・単体取得テスト" );
			start = SceneManager.s_instance.GetBehaviour<M2>()._object;
			TestSMObjectUtility.LogObjects( $"{nameof( SMObject.GetBrothers )}",	start.GetBrothers() );

			Log.Debug( "・取得不可テスト" );
			TestSMObjectUtility.LogObjects( $"{nameof( SMObject.GetChildren )}",	start.GetChildren() );

			await UTask.Never( _asyncCanceler );
		} );


/*
		・SMObject取得テスト
		GetAllParents、GetAllChildren、動作確認
*/
		void CreateTestGetAllParentsChildren() => TestSMBehaviourUtility.CreateBehaviours( @"
			M4,
				M4,
					M4,
				M4,
					M4,
						M4,
					M4,
				null,
					M4,
				M4,
			M4,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetAllParentsChildren() => From( async () => {
			Log.Debug( $"{nameof( TestGetAllParentsChildren )}" );

			Log.Debug( "・複数取得テスト" );
			var start = SceneManager.s_instance.GetBehaviour<M4>()._object;
			SMObject lastChild = null;
			for ( lastChild = start._child._next; lastChild?._child != null; lastChild = lastChild._child ) {}
			TestSMObjectUtility.LogObjects( $"{nameof( SMObject.GetAllParents )}",	lastChild.GetAllParents() );
			TestSMObjectUtility.LogObjects( $"{nameof( SMObject.GetAllChildren )}",	start.GetAllChildren() );

			Log.Debug( "・単体取得テスト" );
			TestSMObjectUtility.LogObjects( $"{nameof( SMObject.GetAllParents )}",	start._next.GetAllParents() );
			TestSMObjectUtility.LogObjects( $"{nameof( SMObject.GetAllChildren )}",	start._next.GetAllChildren() );

			await UTask.Never( _asyncCanceler );
		} );


/*
		・取得テスト
		GetBehaviour<T>、GetBehaviour(type)、GetBehaviours<T>、GetBehaviours(type)、動作確認
*/
		void CreateTestGetBehaviour() => CreateTestObjects();

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetBehaviour() => From( TestGetBehaviourSub() );
		IEnumerator TestGetBehaviourSub() {
			Log.Debug( $"{nameof( TestGetBehaviourSub )}" );

			var top = SceneManager.s_instance.GetBehaviour<M1>()._object;
			TestSMObjectUtility.LogObject( $"{nameof( top )}", top );

			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}<{nameof( M1 )}>",
				top.GetBehaviour<M1>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}({nameof( M1 )})",
				top.GetBehaviour( typeof( M1 ) ) );

			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}<{nameof( M2 )}>",
				top.GetBehaviour<M2>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}({nameof( M2 )})",
				top.GetBehaviour( typeof( M2 ) ) );

			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}<{nameof( M3 )}>",
				top.GetBehaviour<M3>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}({nameof( M3 )})",
				top.GetBehaviour( typeof( M3 ) ) );


			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}<{nameof( M1 )}>",
				top.GetBehaviours<M1>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}({nameof( M1 )})",
				top.GetBehaviours( typeof( M1 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}<{nameof( M2 )}>",
				top.GetBehaviours<M2>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}({nameof( M2 )})",
				top.GetBehaviours( typeof( M2 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}<{nameof( M3 )}>",
				top.GetBehaviours<M3>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}({nameof( M3 )})",
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
		void CreateTestGetObjectBehaviour() => CreateTestObjects();

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetObjectBehaviour() => From( TestGetObjectBehaviourSub() );
		IEnumerator TestGetObjectBehaviourSub() {
			Log.Debug( $"{nameof( TestGetObjectBehaviourSub )}" );

			var top = SceneManager.s_instance.GetBehaviour<M1>()._object;
			var center = SceneManager.s_instance.GetBehaviour<M3>()._object;
			var bottom = SceneManager.s_instance.GetBehaviour<M5>()._object;
			TestSMObjectUtility.LogObject( $"{nameof( top )}",		top );
			TestSMObjectUtility.LogObject( $"{nameof( center )}",	center );
			TestSMObjectUtility.LogObject( $"{nameof( bottom )}",	bottom );


			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviourInParent )}<{nameof( M1 )}>",
				top.GetBehaviourInParent<M1>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviourInParent )}({nameof( M1 )})",
				top.GetBehaviourInParent( typeof( M1 ) ) );

			TestSMBehaviourUtility.LogBehaviour( $"{nameof( center.GetBehaviourInParent )}<{nameof( M1 )}>",
				center.GetBehaviourInParent<M1>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( center.GetBehaviourInParent )}({nameof( M1 )})",
				center.GetBehaviourInParent( typeof( M1 ) ) );

			TestSMBehaviourUtility.LogBehaviour( $"{nameof( bottom.GetBehaviourInParent )}<{nameof( M1 )}>",
				bottom.GetBehaviourInParent<M1>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( bottom.GetBehaviourInParent )}({nameof( M1 )})",
				bottom.GetBehaviourInParent( typeof( M1 ) ) );


			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehavioursInParent )}<{nameof( M1 )}>",
				top.GetBehavioursInParent<M1>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehavioursInParent )}({nameof( M1 )})",
				top.GetBehavioursInParent( typeof( M1 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( center.GetBehavioursInParent )}<{nameof( M1 )}>",
				center.GetBehavioursInParent<M1>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( center.GetBehavioursInParent )}({nameof( M1 )})",
				center.GetBehavioursInParent( typeof( M1 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( bottom.GetBehavioursInParent )}<{nameof( M1 )}>",
				bottom.GetBehavioursInParent<M1>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( bottom.GetBehavioursInParent )}({nameof( M1 )})",
				bottom.GetBehavioursInParent( typeof( M1 ) ) );


			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviourInChildren )}<{nameof( M1 )}>",
				top.GetBehaviourInChildren<M1>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviourInChildren )}({nameof( M1 )})",
				top.GetBehaviourInChildren( typeof( M1 ) ) );

			TestSMBehaviourUtility.LogBehaviour( $"{nameof( center.GetBehaviourInChildren )}<{nameof( M1 )}>",
				center.GetBehaviourInChildren<M1>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( center.GetBehaviourInChildren )}({nameof( M1 )})",
				center.GetBehaviourInChildren( typeof( M1 ) ) );

			TestSMBehaviourUtility.LogBehaviour( $"{nameof( bottom.GetBehaviourInChildren )}<{nameof( M1 )}>",
				bottom.GetBehaviourInChildren<M1>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( bottom.GetBehaviourInChildren )}({nameof( M1 )})",
				bottom.GetBehaviourInChildren( typeof( M1 ) ) );


			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehavioursInChildren )}<{nameof( M1 )}>",
				top.GetBehavioursInChildren<M1>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehavioursInChildren )}({nameof( M1 )})",
				top.GetBehavioursInChildren( typeof( M1 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( center.GetBehavioursInChildren )}<{nameof( M1 )}>",
				center.GetBehavioursInChildren<M1>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( center.GetBehavioursInChildren )}({nameof( M1 )})",
				center.GetBehavioursInChildren( typeof( M1 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( bottom.GetBehavioursInChildren )}<{nameof( M1 )}>",
				bottom.GetBehavioursInChildren<M1>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( bottom.GetBehavioursInChildren )}({nameof( M1 )})",
				bottom.GetBehavioursInChildren( typeof( M1 ) ) );


			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviourInParent )}<{nameof( M4 )}>",
				top.GetBehaviourInParent<M4>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviourInParent )}({nameof( M4 )})",
				top.GetBehaviourInParent( typeof( M4 ) ) );

			TestSMBehaviourUtility.LogBehaviour( $"{nameof( center.GetBehaviourInParent )}<{nameof( M4 )}>",
				center.GetBehaviourInParent<M4>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( center.GetBehaviourInParent )}({nameof( M4 )})",
				center.GetBehaviourInParent( typeof( M4 ) ) );

			TestSMBehaviourUtility.LogBehaviour( $"{nameof( bottom.GetBehaviourInParent )}<{nameof( M4 )}>",
				bottom.GetBehaviourInParent<M4>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( bottom.GetBehaviourInParent )}({nameof( M4 )})",
				bottom.GetBehaviourInParent( typeof( M4 ) ) );


			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehavioursInParent )}<{nameof( M4 )}>",
				top.GetBehavioursInParent<M4>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehavioursInParent )}({nameof( M4 )})",
				top.GetBehavioursInParent( typeof( M4 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( center.GetBehavioursInParent )}<{nameof( M4 )}>",
				center.GetBehavioursInParent<M4>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( center.GetBehavioursInParent )}({nameof( M4 )})",
				center.GetBehavioursInParent( typeof( M4 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( bottom.GetBehavioursInParent )}<{nameof( M4 )}>",
				bottom.GetBehavioursInParent<M4>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( bottom.GetBehavioursInParent )}({nameof( M4 )})",
				bottom.GetBehavioursInParent( typeof( M4 ) ) );


			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviourInChildren )}<{nameof( M4 )}>",
				top.GetBehaviourInChildren<M4>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviourInChildren )}({nameof( M4 )})",
				top.GetBehaviourInChildren( typeof( M4 ) ) );

			TestSMBehaviourUtility.LogBehaviour( $"{nameof( center.GetBehaviourInChildren )}<{nameof( M4 )}>",
				center.GetBehaviourInChildren<M4>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( center.GetBehaviourInChildren )}({nameof( M4 )})",
				center.GetBehaviourInChildren( typeof( M4 ) ) );

			TestSMBehaviourUtility.LogBehaviour( $"{nameof( bottom.GetBehaviourInChildren )}<{nameof( M4 )}>",
				bottom.GetBehaviourInChildren<M4>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( bottom.GetBehaviourInChildren )}({nameof( M4 )})",
				bottom.GetBehaviourInChildren( typeof( M4 ) ) );


			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehavioursInChildren )}<{nameof( M4 )}>",
				top.GetBehavioursInChildren<M4>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehavioursInChildren )}({nameof( M4 )})",
				top.GetBehavioursInChildren( typeof( M4 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( center.GetBehavioursInChildren )}<{nameof( M4 )}>",
				center.GetBehavioursInChildren<M4>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( center.GetBehavioursInChildren )}({nameof( M4 )})",
				center.GetBehavioursInChildren( typeof( M4 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( bottom.GetBehavioursInChildren )}<{nameof( M4 )}>",
				bottom.GetBehavioursInChildren<M4>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( bottom.GetBehavioursInChildren )}({nameof( M4 )})",
				bottom.GetBehavioursInChildren( typeof( M4 ) ) );

			while ( true )	{ yield return null; }
		}


/*
		・SMMonoBehaviour内の取得テスト
*/
		void CreateTestGetInSMMonoBehaviour() => CreateTestObjects();

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetInSMMonoBehaviour() => From( TestGetInSMMonoBehaviourSub() );
		IEnumerator TestGetInSMMonoBehaviourSub() {
			Log.Debug( $"{nameof( TestGetInSMMonoBehaviourSub )}" );

			var top = SceneManager.s_instance.GetBehaviour<M1>();
			var center = SceneManager.s_instance.GetBehaviour<M3>();
			TestSMObjectUtility.LogObject( $"{nameof( top )}",		top._object );
			TestSMObjectUtility.LogObject( $"{nameof( center )}",	center._object );


			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}<{nameof( M1 )}>",
				top.GetBehaviour<M1>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}({nameof( M1 )})",
				top.GetBehaviour( typeof( M1 ) ) );

			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}<{nameof( M2 )}>",
				top.GetBehaviour<M2>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}({nameof( M2 )})",
				top.GetBehaviour( typeof( M2 ) ) );

			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}<{nameof( M3 )}>",
				top.GetBehaviour<M3>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}({nameof( M3 )})",
				top.GetBehaviour( typeof( M3 ) ) );


			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}<{nameof( M1 )}>",
				top.GetBehaviours<M1>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}({nameof( M1 )})",
				top.GetBehaviours( typeof( M1 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}<{nameof( M2 )}>",
				top.GetBehaviours<M2>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}({nameof( M2 )})",
				top.GetBehaviours( typeof( M2 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}<{nameof( M3 )}>",
				top.GetBehaviours<M3>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}({nameof( M3 )})",
				top.GetBehaviours( typeof( M3 ) ) );


			TestSMBehaviourUtility.LogBehaviour( $"{nameof( center.GetBehaviourInParent )}<{nameof( M1 )}>",
				center.GetBehaviourInParent<M1>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( center.GetBehaviourInParent )}({nameof( M1 )})",
				center.GetBehaviourInParent( typeof( M1 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( center.GetBehavioursInParent )}<{nameof( M1 )}>",
				center.GetBehavioursInParent<M1>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( center.GetBehavioursInParent )}({nameof( M1 )})",
				center.GetBehavioursInParent( typeof( M1 ) ) );


			TestSMBehaviourUtility.LogBehaviour( $"{nameof( center.GetBehaviourInChildren )}<{nameof( M1 )}>",
				center.GetBehaviourInChildren<M1>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( center.GetBehaviourInChildren )}({nameof( M1 )})",
				center.GetBehaviourInChildren( typeof( M1 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( center.GetBehavioursInChildren )}<{nameof( M1 )}>",
				center.GetBehavioursInChildren<M1>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( center.GetBehavioursInChildren )}({nameof( M1 )})",
				center.GetBehavioursInChildren( typeof( M1 ) ) );


			TestSMBehaviourUtility.LogBehaviour( $"{nameof( center.GetBehaviourInParent )}<{nameof( M4 )}>",
				center.GetBehaviourInParent<M4>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( center.GetBehaviourInParent )}({nameof( M4 )})",
				center.GetBehaviourInParent( typeof( M4 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( center.GetBehavioursInParent )}<{nameof( M4 )}>",
				center.GetBehavioursInParent<M4>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( center.GetBehavioursInParent )}({nameof( M4 )})",
				center.GetBehavioursInParent( typeof( M4 ) ) );


			TestSMBehaviourUtility.LogBehaviour( $"{nameof( center.GetBehaviourInChildren )}<{nameof( M4 )}>",
				center.GetBehaviourInChildren<M4>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( center.GetBehaviourInChildren )}({nameof( M4 )})",
				center.GetBehaviourInChildren( typeof( M4 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( center.GetBehavioursInChildren )}<{nameof( M4 )}>",
				center.GetBehavioursInChildren<M4>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( center.GetBehavioursInChildren )}({nameof( M4 )})",
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

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetSMBehaviour() => From( TestGetSMBehaviourSub() );
		IEnumerator TestGetSMBehaviourSub() {
			Log.Debug( $"{nameof( TestGetSMBehaviourSub )}" );

			var top = SceneManager.s_instance.GetBehaviour<B1>()._object;
			TestSMObjectUtility.LogObject( $"{nameof( top )}",	top );

			TestSMObjectUtility.LogObjects( $"{nameof( top.GetAllChildren )}",	top.GetAllChildren() );
			TestSMObjectUtility.LogObjects( $"{nameof( top.GetAllParents )}",	top.GetAllParents() );

			
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}<{nameof( B1 )}>",
				top.GetBehaviour<B1>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}({nameof( B1 )})",
				top.GetBehaviour( typeof( B1 ) ) );

			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}<{nameof( B2 )}>",
				top.GetBehaviour<B2>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviour )}({nameof( B2 )})",
				top.GetBehaviour( typeof( B2 ) ) );


			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}<{nameof( B1 )}>",
				top.GetBehaviours<B1>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}({nameof( B1 )})",
				top.GetBehaviours( typeof( B1 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}<{nameof( B2 )}>",
				top.GetBehaviours<B2>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehaviours )}({nameof( B2 )})",
				top.GetBehaviours( typeof( B2 ) ) );


			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviourInParent )}<{nameof( B1 )}>",
				top.GetBehaviourInParent<B1>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviourInParent )}({nameof( B1 )})",
				top.GetBehaviourInParent( typeof( B1 ) ) );

			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviourInParent )}<{nameof( B2 )}>",
				top.GetBehaviourInParent<B2>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviourInParent )}({nameof( B2 )})",
				top.GetBehaviourInParent( typeof( B2 ) ) );


			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehavioursInParent )}<{nameof( B1 )}>",
				top.GetBehavioursInParent<B1>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehavioursInParent )}({nameof( B1 )})",
				top.GetBehavioursInParent( typeof( B1 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehavioursInParent )}<{nameof( B2 )}>",
				top.GetBehavioursInParent<B2>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehavioursInParent )}({nameof( B2 )})",
				top.GetBehavioursInParent( typeof( B2 ) ) );


			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviourInChildren )}<{nameof( B1 )}>",
				top.GetBehaviourInChildren<B1>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviourInChildren )}({nameof( B1 )})",
				top.GetBehaviourInChildren( typeof( B1 ) ) );

			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviourInChildren )}<{nameof( B2 )}>",
				top.GetBehaviourInChildren<B2>() );
			TestSMBehaviourUtility.LogBehaviour( $"{nameof( top.GetBehaviourInChildren )}({nameof( B2 )})",
				top.GetBehaviourInChildren( typeof( B2 ) ) );


			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehavioursInChildren )}<{nameof( B1 )}>",
				top.GetBehavioursInChildren<B1>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehavioursInChildren )}({nameof( B1 )})",
				top.GetBehavioursInChildren( typeof( B1 ) ) );

			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehavioursInChildren )}<{nameof( B2 )}>",
				top.GetBehavioursInChildren<B2>() );
			TestSMBehaviourUtility.LogBehaviours( $"{nameof( top.GetBehavioursInChildren )}({nameof( B2 )})",
				top.GetBehavioursInChildren( typeof( B2 ) ) );

			while ( true )	{ yield return null; }
		}
	}
}