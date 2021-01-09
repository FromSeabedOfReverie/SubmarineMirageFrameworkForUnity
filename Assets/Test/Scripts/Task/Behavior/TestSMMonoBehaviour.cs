//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask {
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.TestTools;
	using UniRx;
	using Task.Behaviour;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using TestBase;



	// TODO : コメント追加、整頓



	public class TestSMMonoBehaviour : SMStandardTest {
		TestSMBehaviourBody _testBody	{ get; set; }
		Text _text	{ get; set; }


		protected override void Create() {
			Application.targetFrameRate = 30;
			_testBody = new TestSMBehaviourBody();
			_disposables.AddLast( _testBody );

			Resources.Load<GameObject>( "TestCamera" ).Instantiate();
			var go = Resources.Load<GameObject>( "TestCanvas" ).Instantiate();
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ =>
				_text.text = _testBody._viewText
			) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_createEvent.AddLast( async canceler => {
				switch ( _testName ) {
					case nameof( TestDestroy ):						CreateTestDestroy();		break;
					case nameof( TestGetBehaviours ):				CreateTestGetBehaviours();	break;
					case nameof( TestGetBehavioursParentChildren ):	CreateTestGetBehavioursParentChildren();break;
				}
				await UTask.DontWait();
			} );
		}


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCreateMono() => From( () => _testBody.TestCreateMono<M6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRunErrorState() => From( () => _testBody.TestRunErrorState<M6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestTaskType() => From( () => _testBody.TestTaskType<M4, M5, M6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestStopActiveAsync() => From( () => _testBody.TestStopActiveAsync<M6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDispose() => From( () => _testBody.TestDispose<M6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDelete() => From( () => _testBody.TestDelete<M6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestManualAtActive() => From( _testBody.TestManual<M6>( true ) );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestManualAtInActive() => From( _testBody.TestManual<M6>( false ) );


/*
		・破棄テスト
		破棄される？
*/
		void CreateTestDestroy() => TestSMBehaviourSMUtility.CreateBehaviours( @"
			M4,
			M4, M4,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDestroy() => From( async () => {
			SMLog.Debug( $"{nameof( TestDestroy )}" );

			var b = (SMMonoBehaviour)SMSceneManager.s_instance.GetBehaviour<M4>();
			b.gameObject.Destroy();
			await UTask.NextFrame( _asyncCanceler );

			b = SMSceneManager.s_instance.GetBehaviour<M4>();
			b.Destroy();

			await UTask.Never( _asyncCanceler );
		} );


/*
		・SMMonoBehaviour取得テスト
		GetBehaviour,T,Type、GetBehaviours,T,Type、動作確認
*/
		void CreateTestGetBehaviours() => TestSMBehaviourSMUtility.CreateBehaviours( @"
			M1, M4, M4, M1,
			M4, M4,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetBehaviours() => From( async () => {
			SMLog.Debug( $"{nameof( TestGetBehaviours )}" );

			SMLog.Debug( "・単体取得テスト" );
			var start = (SMMonoBehaviour)SMSceneManager.s_instance.GetBehaviour<M1>();
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviour )}<M4>",	start.GetBehaviour<M4>() );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviour )}(M4)",	start.GetBehaviour( typeof( M4 ) ) );

			SMLog.Debug( "・複数取得テスト" );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehaviours )}<M4>",	start.GetBehaviours<M4>() );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehaviours )}(M4)",	start.GetBehaviours( typeof( M4 ) ) );

			SMLog.Debug( "・取得不可テスト" );
			start = (SMMonoBehaviour)start._object._next._behaviour;
			TestSMObjectSMUtility.LogObject( $"{nameof( start )}",	start._object );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviour )}<M1>",	start.GetBehaviour<M1>() );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviour )}(M1)",	start.GetBehaviour( typeof( M1 ) ) );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehaviours )}<M1>",	start.GetBehaviours<M1>() );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehaviours )}(M1)",	start.GetBehaviours( typeof( M1 ) ) );

			await UTask.Never( _asyncCanceler );
		} );


/*
		・SMMonoBehaviour取得テスト
		GetBehaviourInParent,T,type、GetBehaviourInChildren,T,type、
		GetBehavioursInParent,T,type、GetBehavioursInChildren,T,type、動作確認
*/
		void CreateTestGetBehavioursParentChildren() => TestSMBehaviourSMUtility.CreateBehaviours( @"
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
			var start = (SMMonoBehaviour)SMSceneManager.s_instance.GetBehaviour<M1>();
			SMMonoBehaviour lastChild = null;
			for ( var last = start._object; last._child != null; ) {
				last = last._child;
				lastChild = (SMMonoBehaviour)last._behaviour;
			}
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviourInParent )}<M4>",
				lastChild.GetBehaviourInParent<M4>() );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviourInParent )}(M4)",
				lastChild.GetBehaviourInParent( typeof( M4 ) ) );

			start = (SMMonoBehaviour)start._object._next._behaviour;
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviourInChildren )}<M4>",
				start.GetBehaviourInChildren<M4>() );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviourInChildren )}(M4)",
				start.GetBehaviourInChildren( typeof( M4 ) ) );


			SMLog.Debug( "・複数取得テスト" );
			start = (SMMonoBehaviour)start._object._next._behaviour;
			for ( var last = start._object; last._child != null; ) {
				last = last._child;
				lastChild = (SMMonoBehaviour)last._behaviour;
			}
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehavioursInParent )}<M4>",
				lastChild.GetBehavioursInParent<M4>() );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehavioursInParent )}(M4)",
				lastChild.GetBehavioursInParent( typeof( M4 ) ) );

			start = (SMMonoBehaviour)start._object._next._behaviour;
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehavioursInChildren )}<M4>",
				start.GetBehavioursInChildren<M4>() );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehavioursInChildren )}(M4)",
				start.GetBehavioursInChildren( typeof( M4 ) ) );


			SMLog.Debug( "・取得不可テスト" );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviourInParent )}<M2>",
				lastChild.GetBehaviourInParent<M2>() );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviourInParent )}(M2)",
				lastChild.GetBehaviourInParent( typeof( M2 ) ) );

			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviourInChildren )}<M2>",
				start.GetBehaviourInChildren<M2>() );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviourInChildren )}(M2)",
				start.GetBehaviourInChildren( typeof( M2 ) ) );

			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehavioursInParent )}<M2>",
				lastChild.GetBehavioursInParent<M2>() );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehavioursInParent )}(M2)",
				lastChild.GetBehavioursInParent( typeof( M2 ) ) );

			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehavioursInChildren )}<M2>",
				start.GetBehavioursInChildren<M2>() );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehavioursInChildren )}(M2)",
				start.GetBehavioursInChildren( typeof( M2 ) ) );


			await UTask.Never( _asyncCanceler );
		} );
	}
}