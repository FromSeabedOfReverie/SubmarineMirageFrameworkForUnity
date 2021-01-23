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
	using KoganeUnityLib;
	using Task;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using TestBase;
	using TestTask;



	// TODO : コメント追加、整頓



	public class TestSMSceneManager : SMStandardTest {
		Text _text	{ get; set; }
		SMSceneManager _sceneManager	{ get; set; }



		protected override void Create() {
			Application.targetFrameRate = 30;

			_sceneManager = SMSceneManager.s_instance;

			Resources.Load<GameObject>( "TestCamera" ).Instantiate();
			var go = Resources.Load<GameObject>( "TestCanvas" ).Instantiate();
			go.DontDestroyOnLoad();
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
					$"    {nameof( _sceneManager._body._isRunInitialActive )} : {_sceneManager._body._isRunInitialActive}",
					")",
					$"{nameof( _sceneManager._fsm )} : {_sceneManager._fsm}"
				);
			} ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_createEvent.AddLast( async canceler => {
				SMLog.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestGetBehaviours ):	CreateTestGetBehaviours();	break;
				}
				SMLog.Debug( $"end {nameof( Create )}{_testName}" );
				await UTask.DontWait();
			} );
		}



/*
		・SMBehaviour取得テスト
		GetBehaviour,TScene,T,Type、GetBehaviours,TScene,T,Type、動作確認
*/
		void CreateTestGetBehaviours() => TestSMBehaviourSMUtility.CreateBehaviours( @"
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
			SMLog.Debug( $"{nameof( TestGetBehaviours )}" );


			SMLog.Debug( "・単体取得テスト" );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviour )}<M1>",
				_sceneManager.GetBehaviour<M1>() );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviour )}<M1>(Work)",
				_sceneManager.GetBehaviour<M1>( SMTaskType.Work ) );

			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviour )}<M1,UnknownScene>",
				_sceneManager.GetBehaviour<M1, UnknownMainSMScene>() );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviour )}<M1,UnknownScene>(Work)",
				_sceneManager.GetBehaviour<M1, UnknownMainSMScene>( SMTaskType.Work ) );

			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviour )}(M1)",
				_sceneManager.GetBehaviour( typeof( M1 ) ) );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviour )}(M1,UnknownScene)",
				_sceneManager.GetBehaviour( typeof( M1 ), typeof( UnknownMainSMScene ) ) );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviour )}(M1,UnknownScene,Work)",
				_sceneManager.GetBehaviour( typeof( M1 ), typeof( UnknownMainSMScene ), SMTaskType.Work ) );


			SMLog.Debug( "・複数取得テスト" );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviours )}<M1>",
				_sceneManager.GetBehaviours<M1>() );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviours )}<M1>(Work)",
				_sceneManager.GetBehaviours<M1>( SMTaskType.Work ) );

			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviours )}<M1,UnknownScene>",
				_sceneManager.GetBehaviours<M1, UnknownMainSMScene>() );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviours )}<M1,UnknownScene>(Work)",
				_sceneManager.GetBehaviours<M1, UnknownMainSMScene>( SMTaskType.Work ) );

			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviours )}(M1)",
				_sceneManager.GetBehaviours( typeof( M1 ) ) );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviours )}(M1,UnknownScene)",
				_sceneManager.GetBehaviours( typeof( M1 ), typeof( UnknownMainSMScene ) ) );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviours )}(M1,UnknownScene,Work)",
				_sceneManager.GetBehaviours( typeof( M1 ), typeof( UnknownMainSMScene ), SMTaskType.Work ) );


			await UTask.WaitWhile( _asyncCanceler, () => !Input.GetKey( KeyCode.Return ) );
			_sceneManager.GetBehaviour<M6>()._object.Dispose();


			SMLog.Debug( "・取得不可テスト" );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviour )}<M6>",
				_sceneManager.GetBehaviour<M6>() );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviour )}<M3>(Work)",
				_sceneManager.GetBehaviour<M3>( SMTaskType.Work ) );

			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviour )}<M2,ForeverScene>",
				_sceneManager.GetBehaviour<M2, ForeverSMScene>() );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviour )}<M2,ForeverScene>(FirstWork)",
				_sceneManager.GetBehaviour<M2, ForeverSMScene>( SMTaskType.FirstWork ) );

			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviour )}(M6)",
				_sceneManager.GetBehaviour( typeof( M6 ) ) );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviour )}(M2,ForeverScene)",
				_sceneManager.GetBehaviour( typeof( M2 ), typeof( ForeverSMScene ) ) );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviour )}(M2,ForeverScene,FirstWork)",
				_sceneManager.GetBehaviour( typeof( M2 ), typeof( ForeverSMScene ), SMTaskType.FirstWork ) );


			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviours )}<M6>",
				_sceneManager.GetBehaviours<M6>() );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviours )}<M6>(Work)",
				_sceneManager.GetBehaviours<M6>( SMTaskType.Work ) );

			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviours )}<M6,UnknownScene>",
				_sceneManager.GetBehaviours<M6, UnknownMainSMScene>() );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviours )}<M6,UnknownScene>(Work)",
				_sceneManager.GetBehaviours<M6, UnknownMainSMScene>( SMTaskType.Work ) );

			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviours )}(M6)",
				_sceneManager.GetBehaviours( typeof( M6 ) ) );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviours )}(M6,UnknownScene)",
				_sceneManager.GetBehaviours( typeof( M6 ), typeof( UnknownMainSMScene ) ) );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager )}.{nameof( SMSceneManager.GetBehaviours )}(M6,UnknownScene,Work)",
				_sceneManager.GetBehaviours( typeof( M6 ), typeof( UnknownMainSMScene ), SMTaskType.Work ) );


			await UTask.Never( _asyncCanceler );
		} );
	}
}