//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask {
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using Task;
	using Scene;
	using Utility;
	using Debug;
	using Test;



	// TODO : コメント追加、整頓



	public partial class TestSMObject : SMStandardTest {
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
			SMLog.Debug( $"{nameof( TestGetFirstLastChild )}" );

			SMLog.Debug( "・最上階テスト" );
			var start = SMSceneManager.s_instance.GetBehaviour<M4>()._object._next;
			TestSMObjectUtility.LogObject( $"{nameof( SMObject.GetFirst )}",	start.GetFirst() );
			TestSMObjectUtility.LogObject( $"{nameof( SMObject.GetLast )}",		start.GetLast() );

			SMLog.Debug( "・子テスト" );
			var child = SMSceneManager.s_instance.GetBehaviour<M1>()._object._child._next;
			TestSMObjectUtility.LogObject( $"{nameof( SMObject.GetFirst )}",	child.GetFirst() );
			TestSMObjectUtility.LogObject( $"{nameof( SMObject.GetLast )}",		child.GetLast() );
			var parent = SMSceneManager.s_instance.GetBehaviour<M1>()._object;
			TestSMObjectUtility.LogObject( $"{nameof( SMObject.GetLastChild )}",	parent.GetLastChild() );

			SMLog.Debug( "・単体取得テスト" );
			start = SMSceneManager.s_instance.GetBehaviour<M2>()._object;
			TestSMObjectUtility.LogObject( $"{nameof( SMObject.GetFirst )}",		start.GetFirst() );
			TestSMObjectUtility.LogObject( $"{nameof( SMObject.GetLast )}",			start.GetLast() );

			SMLog.Debug( "・取得不可テスト" );
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
			SMLog.Debug( $"{nameof( TestGetBrothersChildren )}" );

			SMLog.Debug( "・最上階テスト" );
			var start = SMSceneManager.s_instance.GetBehaviour<M4>()._object._next;
			TestSMObjectUtility.LogObjects( $"{nameof( SMObject.GetBrothers )}",	start.GetBrothers() );

			SMLog.Debug( "・子テスト" );
			var child = SMSceneManager.s_instance.GetBehaviour<M1>()._object._child._next;
			TestSMObjectUtility.LogObjects( $"{nameof( SMObject.GetBrothers )}",	child.GetBrothers() );
			var parent = SMSceneManager.s_instance.GetBehaviour<M1>()._object;
			TestSMObjectUtility.LogObjects( $"{nameof( SMObject.GetChildren )}",	parent.GetChildren() );

			SMLog.Debug( "・単体取得テスト" );
			start = SMSceneManager.s_instance.GetBehaviour<M2>()._object;
			TestSMObjectUtility.LogObjects( $"{nameof( SMObject.GetBrothers )}",	start.GetBrothers() );

			SMLog.Debug( "・取得不可テスト" );
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
			SMLog.Debug( $"{nameof( TestGetAllParentsChildren )}" );

			SMLog.Debug( "・複数取得テスト" );
			var start = SMSceneManager.s_instance.GetBehaviour<M4>()._object;
			SMObject lastChild = null;
			for ( lastChild = start._child._next; lastChild?._child != null; lastChild = lastChild._child ) {}
			TestSMObjectUtility.LogObjects( $"{nameof( SMObject.GetAllParents )}",	lastChild.GetAllParents() );
			TestSMObjectUtility.LogObjects( $"{nameof( SMObject.GetAllChildren )}",	start.GetAllChildren() );

			SMLog.Debug( "・単体取得テスト" );
			TestSMObjectUtility.LogObjects( $"{nameof( SMObject.GetAllParents )}",	start._next.GetAllParents() );
			TestSMObjectUtility.LogObjects( $"{nameof( SMObject.GetAllChildren )}",	start._next.GetAllChildren() );

			await UTask.Never( _asyncCanceler );
		} );


/*
		・SMBehaviour取得テスト
		GetBehaviour,T,Type、GetBehaviourAtLast、GetBehaviours,T,Type、動作確認
*/
		void CreateTestGetBehavioursLast() => TestSMBehaviourUtility.CreateBehaviours( @"
			M1, M4, M4, M1,
			M4, M4,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetBehavioursLast() => From( async () => {
			SMLog.Debug( $"{nameof( TestGetBehavioursLast )}" );

			SMLog.Debug( "・単体取得テスト" );
			var start = SMSceneManager.s_instance.GetBehaviour<M1>()._object;
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMObject.GetBehaviour )}<M4>",	start.GetBehaviour<M4>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMObject.GetBehaviour )}(M4)",	start.GetBehaviour( typeof( M4 ) ) );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMObject.GetBehaviourAtLast )}",	start.GetBehaviourAtLast() );

			SMLog.Debug( "・複数取得テスト" );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMObject.GetBehaviours )}",		start.GetBehaviours() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMObject.GetBehaviours )}<M4>",	start.GetBehaviours<M4>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMObject.GetBehaviours )}(M4)",	start.GetBehaviours( typeof( M4 ) ) );

			SMLog.Debug( "・取得不可テスト" );
			start = start._next;
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMObject.GetBehaviour )}<M1>",	start.GetBehaviour<M1>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMObject.GetBehaviour )}(M1)",	start.GetBehaviour( typeof( M1 ) ) );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMObject.GetBehaviours )}<M1>",	start.GetBehaviours<M1>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMObject.GetBehaviours )}(M1)",	start.GetBehaviours( typeof( M1 ) ) );

			await UTask.Never( _asyncCanceler );
		} );


/*
		・SMBehaviour取得テスト
		GetBehaviourInParent,T,Type、GetBehaviourInChildren,T,Type、
		GetBehavioursInParent,T,Type、GetBehavioursInChildren,T,Type、動作確認
*/
		void CreateTestGetBehavioursParentChildren() => TestSMBehaviourUtility.CreateBehaviours( @"
			M1, M4,
				M1, M1,
					M1, M1,
			M1, M1,
				M1, M1,
					M1, M4,

			M4, M4,
				M1, M1,
					M1, M4,
						M1, M1,
					M1, M4,
			M1, M1,
				M1, M4,
					M1, M1,
						M4, M4,
				M1, M4,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetBehavioursParentChildren() => From( async () => {
			SMLog.Debug( $"{nameof( TestGetBehavioursParentChildren )}" );


			SMLog.Debug( "・単体取得テスト" );
			var start = SMSceneManager.s_instance.GetBehaviour<M1>()._object;
			SMObject lastChild = null;
			for ( lastChild = start._child; lastChild?._child != null; lastChild = lastChild._child ) {}
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMObject.GetBehaviourInParent )}<M4>",
				lastChild.GetBehaviourInParent<M4>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMObject.GetBehaviourInParent )}(M4)",
				lastChild.GetBehaviourInParent( typeof( M4 ) ) );

			start = start._next;
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMObject.GetBehaviourInChildren )}<M4>",
				start.GetBehaviourInChildren<M4>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMObject.GetBehaviourInChildren )}(M4)",
				start.GetBehaviourInChildren( typeof( M4 ) ) );


			SMLog.Debug( "・複数取得テスト" );
			start = start._next;
			for ( lastChild = start._child; lastChild?._child != null; lastChild = lastChild._child ) {}
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMObject.GetBehavioursInParent )}<M4>",
				lastChild.GetBehavioursInParent<M4>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMObject.GetBehavioursInParent )}(M4)",
				lastChild.GetBehavioursInParent( typeof( M4 ) ) );

			start = start._next;
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMObject.GetBehavioursInChildren )}<M4>",
				start.GetBehavioursInChildren<M4>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMObject.GetBehavioursInChildren )}(M4)",
				start.GetBehavioursInChildren( typeof( M4 ) ) );


			SMLog.Debug( "・取得不可テスト" );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMObject.GetBehaviourInParent )}<M2>",
				lastChild.GetBehaviourInParent<M2>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMObject.GetBehaviourInParent )}(M2)",
				lastChild.GetBehaviourInParent( typeof( M2 ) ) );

			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMObject.GetBehaviourInChildren )}<M2>",
				start.GetBehaviourInChildren<M2>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMObject.GetBehaviourInChildren )}(M2)",
				start.GetBehaviourInChildren( typeof( M2 ) ) );

			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMObject.GetBehavioursInParent )}<M2>",
				lastChild.GetBehavioursInParent<M2>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMObject.GetBehavioursInParent )}(M2)",
				lastChild.GetBehavioursInParent( typeof( M2 ) ) );

			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMObject.GetBehavioursInChildren )}<M2>",
				start.GetBehavioursInChildren<M2>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMObject.GetBehavioursInChildren )}(M2)",
				start.GetBehavioursInChildren( typeof( M2 ) ) );


			await UTask.Never( _asyncCanceler );
		} );
	}
}