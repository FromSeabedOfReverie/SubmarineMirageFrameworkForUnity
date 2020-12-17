//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask {
	using System.Linq;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Task;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using TestBase;
	using TestScene;
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public class TestSMObjectManager : SMStandardTest {
		Text _text	{ get; set; }
		SMSceneManager _sceneManager	{ get; set; }



		protected override void Create() {
			Application.targetFrameRate = 30;

			_sceneManager = SMSceneManager.s_instance;

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
				SMLog.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestGetAll ):			CreateTestGetAll();			break;
					case nameof( TestGetBehaviours ):	CreateTestGetBehaviours();	break;
				}
				SMLog.Debug( $"end {nameof( Create )}{_testName}" );
				await UTask.DontWait();
			} );
		}



/*
		・SMObject取得テスト
		GetAll、動作確認
*/
		void CreateTestGetAll() => TestSMBehaviourSMUtility.CreateBehaviours( @"
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
			SMLog.Debug( $"{nameof( TestGetAll )}" );


			SMLog.Debug( "・全取得テスト" );
			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetAllGroups )}",
				_sceneManager._fsm._foreverScene._groups.GetAllGroups() );
			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetAllGroups )},isReverse",
				_sceneManager._fsm._foreverScene._groups.GetAllGroups( null, true ) );

			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMGroupManager.GetAllGroups )}",
				_sceneManager._fsm._scene._groups.GetAllGroups() );
			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMGroupManager.GetAllGroups )},isReverse",
				_sceneManager._fsm._scene._groups.GetAllGroups( null, true ) );


			SMLog.Debug( $"・{SMTaskType.DontWork} 取得テスト" );
			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetAllGroups )}",
				_sceneManager._fsm._foreverScene._groups.GetAllGroups( SMTaskType.DontWork ) );
			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetAllGroups )},isReverse",
				_sceneManager._fsm._foreverScene._groups.GetAllGroups( SMTaskType.DontWork, true ) );

			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMGroupManager.GetAllGroups )}",
				_sceneManager._fsm._scene._groups.GetAllGroups( SMTaskType.DontWork ) );
			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMGroupManager.GetAllGroups )},isReverse",
				_sceneManager._fsm._scene._groups.GetAllGroups( SMTaskType.DontWork, true ) );


			SMLog.Debug( $"・{SMTaskType.Work} 取得テスト" );
			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetAllGroups )}",
				_sceneManager._fsm._foreverScene._groups.GetAllGroups( SMTaskType.Work ) );
			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetAllGroups )},isReverse",
				_sceneManager._fsm._foreverScene._groups.GetAllGroups( SMTaskType.Work, true ) );

			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMGroupManager.GetAllGroups )}",
				_sceneManager._fsm._scene._groups.GetAllGroups( SMTaskType.Work ) );
			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMGroupManager.GetAllGroups )},isReverse",
				_sceneManager._fsm._scene._groups.GetAllGroups( SMTaskType.Work, true ) );


			SMLog.Debug( $"・{SMTaskType.FirstWork} 取得テスト" );
			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetAllGroups )}",
				_sceneManager._fsm._foreverScene._groups.GetAllGroups( SMTaskType.FirstWork ) );
			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetAllGroups )},isReverse",
				_sceneManager._fsm._foreverScene._groups.GetAllGroups( SMTaskType.FirstWork, true ) );

			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMGroupManager.GetAllGroups )}",
				_sceneManager._fsm._scene._groups.GetAllGroups( SMTaskType.FirstWork ) );
			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMGroupManager.GetAllGroups )},isReverse",
				_sceneManager._fsm._scene._groups.GetAllGroups( SMTaskType.FirstWork, true ) );


			await UTask.WaitWhile( _asyncCanceler, () => !Input.GetKey( KeyCode.Return ) );


			SMLog.Debug( $"・取得不可テスト" );
			_sceneManager.GetBehaviour<M1>()._object.Dispose();
			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMGroupManager.GetAllGroups )}",
				_sceneManager._fsm._scene._groups.GetAllGroups( SMTaskType.DontWork ) );
			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMGroupManager.GetAllGroups )},isReverse",
				_sceneManager._fsm._scene._groups.GetAllGroups( SMTaskType.DontWork, true ) );

			_sceneManager.GetBehaviour<M2>()._object.Dispose();
			_sceneManager.GetBehaviour<M3>()._object.Dispose();
			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMGroupManager.GetAllGroups )}",
				_sceneManager._fsm._scene._groups.GetAllGroups() );
			TestSMObjectSMUtility.LogObjects(
				$"{nameof( _sceneManager._fsm._scene )}.{nameof( SMGroupManager.GetAllGroups )},isReverse",
				_sceneManager._fsm._scene._groups.GetAllGroups( null, true ) );


			await UTask.Never( _asyncCanceler );
		} );



/*
		・SMBehaviour取得テスト
		GetBehaviour,T,Type、GetBehaviours,T,Type、動作確認
*/
		void CreateTestGetBehaviours() => TestSMBehaviourSMUtility.CreateBehaviours( @"
			M1, M1,
				M1, M4,
					M1, M4,
			M1, M4, M4,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetBehaviours() => From( async () => {
			SMLog.Debug( $"{nameof( TestGetBehaviours )}" );


			SMLog.Debug( "・単体取得テスト" );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviour )}<M4>",
				_sceneManager._fsm._foreverScene._groups.GetBehaviour<M4>() );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviour )}<M4>",
				_sceneManager._fsm._foreverScene._groups.GetBehaviour<M4>( SMTaskType.DontWork ) );

			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviour )}(M4)",
				_sceneManager._fsm._foreverScene._groups.GetBehaviour( typeof( M4 ) ) );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviour )}(M4)",
				_sceneManager._fsm._foreverScene._groups.GetBehaviour( typeof( M4 ), SMTaskType.DontWork ) );


			SMLog.Debug( "・複数取得テスト" );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviours )}<M4>",
				_sceneManager._fsm._foreverScene._groups.GetBehaviours<M4>() );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviours )}<M4>",
				_sceneManager._fsm._foreverScene._groups.GetBehaviours<M4>( SMTaskType.DontWork ) );

			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviours )}(M4)",
				_sceneManager._fsm._foreverScene._groups.GetBehaviours( typeof( M4 ) ) );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviours )}(M4)",
				_sceneManager._fsm._foreverScene._groups.GetBehaviours( typeof( M4 ), SMTaskType.DontWork ) );


			SMLog.Debug( "・取得不可テスト" );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviour )}<M2>",
				_sceneManager._fsm._foreverScene._groups.GetBehaviour<M2>() );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviour )}<M2>",
				_sceneManager._fsm._foreverScene._groups.GetBehaviour<M2>( SMTaskType.DontWork ) );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviour )}<M4>",
				_sceneManager._fsm._foreverScene._groups.GetBehaviour<M4>( SMTaskType.Work ) );

			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviour )}(M2)",
				_sceneManager._fsm._foreverScene._groups.GetBehaviour( typeof( M2 ) ) );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviour )}(M2)",
				_sceneManager._fsm._foreverScene._groups.GetBehaviour( typeof( M2 ), SMTaskType.DontWork ) );
			TestSMBehaviourSMUtility.LogBehaviour(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviour )}(M4)",
				_sceneManager._fsm._foreverScene._groups.GetBehaviour( typeof( M4 ), SMTaskType.Work ) );

			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviours )}<M2>",
				_sceneManager._fsm._foreverScene._groups.GetBehaviours<M2>() );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviours )}<M2>",
				_sceneManager._fsm._foreverScene._groups.GetBehaviours<M2>( SMTaskType.DontWork ) );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviours )}<M4>",
				_sceneManager._fsm._foreverScene._groups.GetBehaviours<M4>( SMTaskType.Work ) );

			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviours )}(M2)",
				_sceneManager._fsm._foreverScene._groups.GetBehaviours( typeof( M2 ) ) );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviours )}(M2)",
				_sceneManager._fsm._foreverScene._groups.GetBehaviours( typeof( M2 ), SMTaskType.DontWork ) );
			TestSMBehaviourSMUtility.LogBehaviours(
				$"{nameof( _sceneManager._fsm._foreverScene )}.{nameof( SMGroupManager.GetBehaviours )}(M4)",
				_sceneManager._fsm._foreverScene._groups.GetBehaviours( typeof( M4 ), SMTaskType.Work ) );


			await UTask.Never( _asyncCanceler );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( TestManualSub() );
		IEnumerator TestManualSub() {
			_disposables.AddLast( TestSMBehaviourSMUtility.SetRunKey( _sceneManager ) );

			var i = 0;
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Space ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {nameof( _sceneManager._fsm.ChangeScene )}" );
					i = (i + 1) % 2;
					switch ( i ) {
						case 0:
							SMLog.Debug( $"{this.GetAboutName()} change {nameof( TestChange1Scene )}" );
							_sceneManager._fsm.ChangeScene<TestChange1Scene>().Forget();
							break;
						case 1:
							SMLog.Debug( $"{this.GetAboutName()} change {nameof( TestChange2Scene )}" );
							_sceneManager._fsm.ChangeScene<TestChange2Scene>().Forget();
							break;
						case 2:
							SMLog.Debug( $"{this.GetAboutName()} change {nameof( UnknownSMScene )}" );
							_sceneManager._fsm.ChangeScene<UnknownSMScene>().Forget();
							break;
					}
				} )
			);

			while ( true )	{ yield return null; }
		}
	}
}