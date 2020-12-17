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
	using Task;
	using Task.Modifyler;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using TestBase;
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public class TestSMScene : SMStandardTest {
		SMSceneManager _sceneManager	{ get; set; }
		Text _text	{ get; set; }


		protected override void Create() {
			Application.targetFrameRate = 30;
			_sceneManager = SMSceneManager.s_instance;

			UnityObject.Instantiate( Resources.Load<GameObject>( "TestCamera" ) );
			var go = UnityObject.Instantiate( Resources.Load<GameObject>( "TestCanvas" ) );
			UnityObject.DontDestroyOnLoad( go );
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast(Observable.EveryLateUpdate().Subscribe( (Action<long>)(_ => {
				if (_sceneManager == null ) {
					_text.text = string.Empty;
					return;
				}
				var b = _sceneManager;
				_text.text = string.Join( "\n",
					$"{b.GetAboutName()}(",
					$"    {nameof( b._isInitialized )} : {b._isInitialized}",
					$"    {nameof( b._isActive )} : {b._isActive}",
					$"    {nameof( b._body._ranState )} : {b._body._ranState}",
					$"    {nameof( b._body._isActive )} : {b._body._isActive}",
					$"    {nameof( b._body._isInitialActive )} : {b._body._isInitialActive}",
					")",
					$"{nameof( b._fsm )} : {b._fsm}"
				);
			}) ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_disposables.AddLast( _sceneManager );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() {
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskRunState.Create}" );
					RunStateSMBehaviour.RegisterAndRun( _sceneManager, SMTaskRunState.Create ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskRunState.SelfInitializing}" );
					RunStateSMBehaviour.RegisterAndRun( _sceneManager, SMTaskRunState.SelfInitializing ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha3 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskRunState.Initializing}" );
					RunStateSMBehaviour.RegisterAndRun( _sceneManager, SMTaskRunState.Initializing ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha4 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskRunState.FixedUpdate}" );
					RunStateSMBehaviour.RegisterAndRun( _sceneManager, SMTaskRunState.FixedUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha5 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskRunState.Update}" );
					RunStateSMBehaviour.RegisterAndRun( _sceneManager, SMTaskRunState.Update ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha6 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskRunState.LateUpdate}" );
					RunStateSMBehaviour.RegisterAndRun( _sceneManager, SMTaskRunState.LateUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha7 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskRunState.Finalizing}" );
					RunStateSMBehaviour.RegisterAndRun( _sceneManager, SMTaskRunState.Finalizing ).Forget();
				} )
			);
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Z ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskActiveState.Enable}" );
					ChangeActiveSMBehaviour.RegisterAndRun( _sceneManager, true ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.X ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskActiveState.Disable}" );
					ChangeActiveSMBehaviour.RegisterAndRun( _sceneManager, false ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.C ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {nameof( ChangeActiveSMBehaviour.RegisterAndRunInitial )}" );
					ChangeActiveSMBehaviour.RegisterAndRunInitial( _sceneManager ).Forget();
				} )
			);
			var i = 0;
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Space ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {nameof( _sceneManager._fsm.ChangeScene )}" );
					i = (i + 1) % 2;
					switch ( i ) {
						case 0:
							SMLog.Debug( $"{this.GetAboutName()} change TestChange1Scene" );
							_sceneManager._fsm.ChangeScene<TestChange1Scene>().Forget();
							break;
						case 1:
							SMLog.Debug( $"{this.GetAboutName()} change TestChange2Scene" );
							_sceneManager._fsm.ChangeScene<TestChange2Scene>().Forget();
							break;
						case 2:
							SMLog.Debug( $"{this.GetAboutName()} change UnknownScene" );
							_sceneManager._fsm.ChangeScene<UnknownSMScene>().Forget();
							break;
					}
				} )
			);
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {nameof( _sceneManager.Dispose )}" );
					_sceneManager.Dispose();
					_sceneManager = null;
				} )
			);

			while ( true )	{ yield return null; }
		}
	}



	public class TestChange1Scene : SMScene {}
	public class TestChange2Scene : SMScene {}
}