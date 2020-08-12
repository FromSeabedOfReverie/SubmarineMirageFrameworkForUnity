//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestSMTask {
	using System.Linq;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using UTask;
	using SMTask;
	using Scene;
	using Extension;
	using Debug;
	using Test;
	using TestScene;
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public class TestSMObjectManager : Test {
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
					$"    {nameof( _sceneManager._body._activeState )} : {_sceneManager._body._activeState}",
					$"    {nameof( _sceneManager._body._nextActiveState )} : {_sceneManager._body._nextActiveState}",
					")",
					$"{nameof( _sceneManager._fsm )} : {_sceneManager._fsm}"
				);
			} ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_createEvent.AddLast( async canceler => {
				Log.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestGetAll ):			CreateTestGetAll();			break;
					case nameof( TestGetBehaviours ):	CreateTestGetBehaviours();	break;
				}
				Log.Debug( $"end {nameof( Create )}{_testName}" );

				await UTask.DontWait();
			} );
		}



/*
		・SMObject取得テスト
		GetAll、動作確認
*/
		void CreateTestGetAll() => TestSMBehaviourUtility.CreateBehaviours( @"
			M1, M1,
				M1, M1,
			M2, M2,
				M2, M2,
			M3, M3,
				M3, M3,
			M4, M4,
				M4, M4,
			M5, M5,
				M5, M5,
			M6, M6,
				M6, M6,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetAll() => From( async () => {
			Log.Debug( $"{nameof( TestGetAll )}" );


			Log.Debug( "・全取得テスト" );
			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetAllTops )}",
				_sceneManager._fsm._foreverScene._objects.GetAllTops() );
			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetAllTops )},isReverse",
				_sceneManager._fsm._foreverScene._objects.GetAllTops( null, true ) );

			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMObjectManager.GetAllTops )}",
				_sceneManager._fsm._scene._objects.GetAllTops() );
			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMObjectManager.GetAllTops )},isReverse",
				_sceneManager._fsm._scene._objects.GetAllTops( null, true ) );


			Log.Debug( $"・{SMTaskType.DontWork} 取得テスト" );
			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetAllTops )}",
				_sceneManager._fsm._foreverScene._objects.GetAllTops( SMTaskType.DontWork ) );
			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetAllTops )},isReverse",
				_sceneManager._fsm._foreverScene._objects.GetAllTops( SMTaskType.DontWork, true ) );

			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMObjectManager.GetAllTops )}",
				_sceneManager._fsm._scene._objects.GetAllTops( SMTaskType.DontWork ) );
			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMObjectManager.GetAllTops )},isReverse",
				_sceneManager._fsm._scene._objects.GetAllTops( SMTaskType.DontWork, true ) );


			Log.Debug( $"・{SMTaskType.Work} 取得テスト" );
			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetAllTops )}",
				_sceneManager._fsm._foreverScene._objects.GetAllTops( SMTaskType.Work ) );
			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetAllTops )},isReverse",
				_sceneManager._fsm._foreverScene._objects.GetAllTops( SMTaskType.Work, true ) );

			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMObjectManager.GetAllTops )}",
				_sceneManager._fsm._scene._objects.GetAllTops( SMTaskType.Work ) );
			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMObjectManager.GetAllTops )},isReverse",
				_sceneManager._fsm._scene._objects.GetAllTops( SMTaskType.Work, true ) );


			Log.Debug( $"・{SMTaskType.FirstWork} 取得テスト" );
			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetAllTops )}",
				_sceneManager._fsm._foreverScene._objects.GetAllTops( SMTaskType.FirstWork ) );
			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetAllTops )},isReverse",
				_sceneManager._fsm._foreverScene._objects.GetAllTops( SMTaskType.FirstWork, true ) );

			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMObjectManager.GetAllTops )}",
				_sceneManager._fsm._scene._objects.GetAllTops( SMTaskType.FirstWork ) );
			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMObjectManager.GetAllTops )},isReverse",
				_sceneManager._fsm._scene._objects.GetAllTops( SMTaskType.FirstWork, true ) );


			await UTask.WaitWhile( _asyncCanceler, () => !Input.GetKey( KeyCode.Return ) );


			Log.Debug( $"・取得不可テスト" );
			_sceneManager.GetBehaviour<M1>()._object.Dispose();
			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMObjectManager.GetAllTops )}",
				_sceneManager._fsm._scene._objects.GetAllTops( SMTaskType.DontWork ) );
			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMObjectManager.GetAllTops )},isReverse",
				_sceneManager._fsm._scene._objects.GetAllTops( SMTaskType.DontWork, true ) );

			_sceneManager.GetBehaviour<M2>()._object.Dispose();
			_sceneManager.GetBehaviour<M3>()._object.Dispose();
			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMObjectManager.GetAllTops )}",
				_sceneManager._fsm._scene._objects.GetAllTops() );
			TestSMObjectUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMObjectManager.GetAllTops )},isReverse",
				_sceneManager._fsm._scene._objects.GetAllTops( null, true ) );


			await UTask.Never( _asyncCanceler );
		} );



/*
		・SMBehaviour取得テスト
		GetBehaviour,T,Type、GetBehaviours,T,Type、動作確認
*/
		void CreateTestGetBehaviours() => TestSMBehaviourUtility.CreateBehaviours( @"
			M1, M1,
				M1, M4,
					M1, M4,
			M1, M4, M4,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetBehaviours() => From( async () => {
			Log.Debug( $"{nameof( TestGetBehaviours )}" );


			Log.Debug( "・単体取得テスト" );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviour )}<M4>",
				_sceneManager._fsm._foreverScene._objects.GetBehaviour<M4>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviour )}<M4>",
				_sceneManager._fsm._foreverScene._objects.GetBehaviour<M4>( SMTaskType.DontWork ) );

			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviour )}(M4)",
				_sceneManager._fsm._foreverScene._objects.GetBehaviour( typeof( M4 ) ) );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviour )}(M4)",
				_sceneManager._fsm._foreverScene._objects.GetBehaviour( typeof( M4 ), SMTaskType.DontWork ) );


			Log.Debug( "・複数取得テスト" );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviours )}<M4>",
				_sceneManager._fsm._foreverScene._objects.GetBehaviours<M4>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviours )}<M4>",
				_sceneManager._fsm._foreverScene._objects.GetBehaviours<M4>( SMTaskType.DontWork ) );

			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviours )}(M4)",
				_sceneManager._fsm._foreverScene._objects.GetBehaviours( typeof( M4 ) ) );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviours )}(M4)",
				_sceneManager._fsm._foreverScene._objects.GetBehaviours( typeof( M4 ), SMTaskType.DontWork ) );


			Log.Debug( "・取得不可テスト" );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviour )}<M2>",
				_sceneManager._fsm._foreverScene._objects.GetBehaviour<M2>() );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviour )}<M2>",
				_sceneManager._fsm._foreverScene._objects.GetBehaviour<M2>( SMTaskType.DontWork ) );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviour )}<M4>",
				_sceneManager._fsm._foreverScene._objects.GetBehaviour<M4>( SMTaskType.Work ) );

			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviour )}(M2)",
				_sceneManager._fsm._foreverScene._objects.GetBehaviour( typeof( M2 ) ) );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviour )}(M2)",
				_sceneManager._fsm._foreverScene._objects.GetBehaviour( typeof( M2 ), SMTaskType.DontWork ) );
			TestSMBehaviourUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviour )}(M4)",
				_sceneManager._fsm._foreverScene._objects.GetBehaviour( typeof( M4 ), SMTaskType.Work ) );

			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviours )}<M2>",
				_sceneManager._fsm._foreverScene._objects.GetBehaviours<M2>() );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviours )}<M2>",
				_sceneManager._fsm._foreverScene._objects.GetBehaviours<M2>( SMTaskType.DontWork ) );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviours )}<M4>",
				_sceneManager._fsm._foreverScene._objects.GetBehaviours<M4>( SMTaskType.Work ) );

			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviours )}(M2)",
				_sceneManager._fsm._foreverScene._objects.GetBehaviours( typeof( M2 ) ) );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviours )}(M2)",
				_sceneManager._fsm._foreverScene._objects.GetBehaviours( typeof( M2 ), SMTaskType.DontWork ) );
			TestSMBehaviourUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMObjectManager.GetBehaviours )}(M4)",
				_sceneManager._fsm._foreverScene._objects.GetBehaviours( typeof( M4 ), SMTaskType.Work ) );


			await UTask.Never( _asyncCanceler );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( TestManualSub() );
		IEnumerator TestManualSub() {
			_disposables.AddLast( TestSMBehaviourUtility.SetRunKey( _sceneManager ) );

			var i = 0;
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Space ) ).Subscribe( _ => {
					Log.Warning( "key down change scene" );
					i = (i + 1) % 2;
					switch ( i ) {
						case 0:
							Log.Debug( $"{this.GetAboutName()} change {nameof( TestChange1Scene )}" );
							_sceneManager._fsm.ChangeScene<TestChange1Scene>().Forget();
							break;
						case 1:
							Log.Debug( $"{this.GetAboutName()} change {nameof( TestChange2Scene )}" );
							_sceneManager._fsm.ChangeScene<TestChange2Scene>().Forget();
							break;
						case 2:
							Log.Debug( $"{this.GetAboutName()} change {nameof( UnknownScene )}" );
							_sceneManager._fsm.ChangeScene<UnknownScene>().Forget();
							break;
					}
				} )
			);

			while ( true )	{ yield return null; }
		}
	}
}