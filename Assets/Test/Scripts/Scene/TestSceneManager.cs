//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestScene {
	using System;
	using System.Linq;
	using System.Threading;
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
	using Extension;
	using Debug;
	using Test;
	using TestSMTask;
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public class TestSceneManager : SMStandardTest {
		Text _text;
		SceneManager _sceneManager;



		protected override void Create() {
			Application.targetFrameRate = 30;

			_sceneManager = SceneManager.s_instance;

			UnityObject.Instantiate( Resources.Load<GameObject>( "TestCamera" ) );
			var go = UnityObject.Instantiate( Resources.Load<GameObject>( "TestCanvas" ) );
			UnityObject.DontDestroyOnLoad( go );
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ => {
				if ( _sceneManager == null ) {
					_text.text = string.Empty;
					return;
				}
				_text.text = string.Join( "\n",
					$"{_sceneManager.GetAboutName()}(",
					$"    {nameof( _sceneManager._isInitialized )} : {_sceneManager._isInitialized}",
					$"    {nameof( _sceneManager._isActive )} : {_sceneManager._isActive}",
					$"    {nameof( _sceneManager._body._ranState )} : {_sceneManager._body._ranState}",
					$"    {nameof( _sceneManager._body._isActive )} : {_sceneManager._body._isActive}",
					$"    {nameof( _sceneManager._body._isInitialActive )} : {_sceneManager._body._isInitialActive}",
					")",
					$"{nameof( _sceneManager._fsm )} : {_sceneManager._fsm}"
				);
			} ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_createEvent.AddLast( async canceler => {
				Log.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestGetBehaviours ):	CreateTestGetBehaviours();	break;
				}
				Log.Debug( $"end {nameof( Create )}{_testName}" );
				await UTask.DontWait();
			} );
		}



/*
		・SMBehaviour取得テスト
		GetBehaviour,TScene,T,Type、GetBehaviours,TScene,T,Type、動作確認
*/
		void CreateTestGetBehaviours() => TestSMBehaviourUtility.CreateBehaviours( @"
			M2, M2,
				M2, M1,
					M2, M1,
			M3, M3,
				M3, M1,
					M3, M1,
			M4, M4,
				M4, M1,
					M4, M1,
			M5, M5,
				M5, M1,
					M5, M1,
			M6, M6,
				M6, M1,
					M6, M1,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetBehaviours() => From( async () => {
			Log.Debug( $"{nameof( TestGetBehaviours )}" );


			Log.Debug( "・単体取得テスト" );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviour )}<M1>",
				_sceneManager.GetBehaviour<M1>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviour )}<M1>(Work)",
				_sceneManager.GetBehaviour<M1>( SMTaskType.Work ) );

			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviour )}<M1,UnknownScene>",
				_sceneManager.GetBehaviour<M1, UnknownScene>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviour )}<M1,UnknownScene>(Work)",
				_sceneManager.GetBehaviour<M1, UnknownScene>( SMTaskType.Work ) );

			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviour )}(M1)",
				_sceneManager.GetBehaviour( typeof( M1 ) ) );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviour )}(M1,UnknownScene)",
				_sceneManager.GetBehaviour( typeof( M1 ), typeof( UnknownScene ) ) );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviour )}(M1,UnknownScene,Work)",
				_sceneManager.GetBehaviour( typeof( M1 ), typeof( UnknownScene ), SMTaskType.Work ) );


			Log.Debug( "・複数取得テスト" );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviours )}<M1>",
				_sceneManager.GetBehaviours<M1>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviours )}<M1>(Work)",
				_sceneManager.GetBehaviours<M1>( SMTaskType.Work ) );

			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviours )}<M1,UnknownScene>",
				_sceneManager.GetBehaviours<M1, UnknownScene>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviours )}<M1,UnknownScene>(Work)",
				_sceneManager.GetBehaviours<M1, UnknownScene>( SMTaskType.Work ) );

			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviours )}(M1)",
				_sceneManager.GetBehaviours( typeof( M1 ) ) );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviours )}(M1,UnknownScene)",
				_sceneManager.GetBehaviours( typeof( M1 ), typeof( UnknownScene ) ) );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviours )}(M1,UnknownScene,Work)",
				_sceneManager.GetBehaviours( typeof( M1 ), typeof( UnknownScene ), SMTaskType.Work ) );


			await UTask.WaitWhile( _asyncCanceler, () => !Input.GetKey( KeyCode.Return ) );
			_sceneManager.GetBehaviour<M6>()._object.Dispose();


			Log.Debug( "・取得不可テスト" );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviour )}<M6>",
				_sceneManager.GetBehaviour<M6>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviour )}<M3>(Work)",
				_sceneManager.GetBehaviour<M3>( SMTaskType.Work ) );

			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviour )}<M2,ForeverScene>",
				_sceneManager.GetBehaviour<M2, ForeverScene>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviour )}<M2,ForeverScene>(FirstWork)",
				_sceneManager.GetBehaviour<M2, ForeverScene>( SMTaskType.FirstWork ) );

			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviour )}(M6)",
				_sceneManager.GetBehaviour( typeof( M6 ) ) );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviour )}(M2,ForeverScene)",
				_sceneManager.GetBehaviour( typeof( M2 ), typeof( ForeverScene ) ) );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviour )}(M2,ForeverScene,FirstWork)",
				_sceneManager.GetBehaviour( typeof( M2 ), typeof( ForeverScene ), SMTaskType.FirstWork ) );


			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviours )}<M6>",
				_sceneManager.GetBehaviours<M6>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviours )}<M6>(Work)",
				_sceneManager.GetBehaviours<M6>( SMTaskType.Work ) );

			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviours )}<M6,UnknownScene>",
				_sceneManager.GetBehaviours<M6, UnknownScene>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviours )}<M6,UnknownScene>(Work)",
				_sceneManager.GetBehaviours<M6, UnknownScene>( SMTaskType.Work ) );

			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviours )}(M6)",
				_sceneManager.GetBehaviours( typeof( M6 ) ) );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviours )}(M6,UnknownScene)",
				_sceneManager.GetBehaviours( typeof( M6 ), typeof( UnknownScene ) ) );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SceneManager.GetBehaviours )}(M6,UnknownScene,Work)",
				_sceneManager.GetBehaviours( typeof( M6 ), typeof( UnknownScene ), SMTaskType.Work ) );


			await UTask.Never( _asyncCanceler );
		} );
	}
}