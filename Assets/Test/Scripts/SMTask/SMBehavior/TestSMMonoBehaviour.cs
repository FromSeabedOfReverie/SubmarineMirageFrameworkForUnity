//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestSMTask {
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using UTask;
	using SMTask;
	using Scene;
	using Debug;
	using Test;
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public class TestSMMonoBehaviour : Test {
		TestSMBehaviourBody _testBody;
		Text _text;


		protected override void Create() {
			Application.targetFrameRate = 30;
			_testBody = new TestSMBehaviourBody();
			_disposables.AddLast( _testBody );

			UnityObject.Instantiate( Resources.Load<GameObject>( "TestCamera" ) );
			var go = UnityObject.Instantiate( Resources.Load<GameObject>( "TestCanvas" ) );
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
		void CreateTestDestroy() => TestSMBehaviourUtility.CreateBehaviours( @"
			M4,
			M4, M4,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDestroy() => From( async () => {
			Log.Debug( $"{nameof( TestDestroy )}" );

			var b = (SMMonoBehaviour)SceneManager.s_instance.GetBehaviour<M4>();
			UnityObject.Destroy( b.gameObject );
			await UTask.NextFrame( _asyncCanceler );

			b = SceneManager.s_instance.GetBehaviour<M4>();
			UnityObject.Destroy( b );

			await UTask.Never( _asyncCanceler );
		} );


/*
		・SMMonoBehaviour取得テスト
		GetBehaviour,T,Type、GetBehaviours,T,Type、動作確認
*/
		void CreateTestGetBehaviours() => TestSMBehaviourUtility.CreateBehaviours( @"
			M1, M4, M4, M1,
			M4, M4,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetBehaviours() => From( async () => {
			Log.Debug( $"{nameof( TestGetBehaviours )}" );

			Log.Debug( "・単体取得テスト" );
			var start = (SMMonoBehaviour)SceneManager.s_instance.GetBehaviour<M1>();
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviour )}<M4>",	start.GetBehaviour<M4>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviour )}(M4)",	start.GetBehaviour( typeof( M4 ) ) );

			Log.Debug( "・複数取得テスト" );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehaviours )}<M4>",	start.GetBehaviours<M4>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehaviours )}(M4)",	start.GetBehaviours( typeof( M4 ) ) );

			Log.Debug( "・取得不可テスト" );
			start = (SMMonoBehaviour)start._object._next._behaviour;
			TestSMObjectUtility.LogObject( $"{nameof( start )}",	start._object );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviour )}<M1>",	start.GetBehaviour<M1>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviour )}(M1)",	start.GetBehaviour( typeof( M1 ) ) );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehaviours )}<M1>",	start.GetBehaviours<M1>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehaviours )}(M1)",	start.GetBehaviours( typeof( M1 ) ) );

			await UTask.Never( _asyncCanceler );
		} );


/*
		・SMMonoBehaviour取得テスト
		GetBehaviourInParent,T,type、GetBehaviourInChildren,T,type、
		GetBehavioursInParent,T,type、GetBehavioursInChildren,T,type、動作確認
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
			Log.Debug( $"{nameof( TestGetBehavioursParentChildren )}" );


			Log.Debug( "・単体取得テスト" );
			var start = (SMMonoBehaviour)SceneManager.s_instance.GetBehaviour<M1>();
			SMMonoBehaviour lastChild = null;
			for ( var last = start._object; last._child != null; ) {
				last = last._child;
				lastChild = (SMMonoBehaviour)last._behaviour;
			}
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviourInParent )}<M4>",
				lastChild.GetBehaviourInParent<M4>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviourInParent )}(M4)",
				lastChild.GetBehaviourInParent( typeof( M4 ) ) );

			start = (SMMonoBehaviour)start._object._next._behaviour;
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviourInChildren )}<M4>",
				start.GetBehaviourInChildren<M4>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviourInChildren )}(M4)",
				start.GetBehaviourInChildren( typeof( M4 ) ) );


			Log.Debug( "・複数取得テスト" );
			start = (SMMonoBehaviour)start._object._next._behaviour;
			for ( var last = start._object; last._child != null; ) {
				last = last._child;
				lastChild = (SMMonoBehaviour)last._behaviour;
			}
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehavioursInParent )}<M4>",
				lastChild.GetBehavioursInParent<M4>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehavioursInParent )}(M4)",
				lastChild.GetBehavioursInParent( typeof( M4 ) ) );

			start = (SMMonoBehaviour)start._object._next._behaviour;
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehavioursInChildren )}<M4>",
				start.GetBehavioursInChildren<M4>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehavioursInChildren )}(M4)",
				start.GetBehavioursInChildren( typeof( M4 ) ) );


			Log.Debug( "・取得不可テスト" );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviourInParent )}<M2>",
				lastChild.GetBehaviourInParent<M2>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviourInParent )}(M2)",
				lastChild.GetBehaviourInParent( typeof( M2 ) ) );

			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviourInChildren )}<M2>",
				start.GetBehaviourInChildren<M2>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( SMMonoBehaviour.GetBehaviourInChildren )}(M2)",
				start.GetBehaviourInChildren( typeof( M2 ) ) );

			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehavioursInParent )}<M2>",
				lastChild.GetBehavioursInParent<M2>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehavioursInParent )}(M2)",
				lastChild.GetBehavioursInParent( typeof( M2 ) ) );

			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehavioursInChildren )}<M2>",
				start.GetBehavioursInChildren<M2>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( SMMonoBehaviour.GetBehavioursInChildren )}(M2)",
				start.GetBehavioursInChildren( typeof( M2 ) ) );


			await UTask.Never( _asyncCanceler );
		} );
	}
}