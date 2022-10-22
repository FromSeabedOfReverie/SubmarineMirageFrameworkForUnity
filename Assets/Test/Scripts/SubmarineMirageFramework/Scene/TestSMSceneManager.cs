//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Test {
	using System;
	using System.Linq;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;
	using Game;



	public class TestSMSceneManager : SMUnitTest {
		bool _isInitialized	{ get; set; }

		readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();

		SMSceneManager _sceneManager	{ get; set; }
		SMInputManager _inputManager	{ get; set; }
		SMDisplayLog _displayLog		{ get; set; }



		protected override void Create() {
			UTask.Void( async () => {
				var taskManager = await SMServiceLocator.WaitResolve<SMTaskManager>( _canceler );
				_displayLog = await SMServiceLocator.WaitResolve<SMDisplayLog>( _canceler );
				_inputManager = await SMServiceLocator.WaitResolve<SMInputManager>( _canceler );
				_sceneManager = await SMServiceLocator.WaitResolve<SMSceneManager>( _canceler );

				taskManager._updateEvent.AddLast()
					.Where( _ => !_sceneManager._isDispose )
					.Subscribe( _ => {
						_displayLog.Add( $"{_sceneManager.GetName()} : { _sceneManager._ranState }" );
						_sceneManager.GetScenes().ForEach(
							s => _displayLog.Add( $"    {s.GetName()} : { s._ranState }" )
						);
					} );

				var testManager = new TestRegisterSMSceneManager( _sceneManager );
				testManager.SetEvent();
				_disposables.AddFirst( testManager );

				await UTask.WaitWhile( _canceler, () => _sceneManager._ranState < SMTaskRunState.Create );
				_sceneManager.GetScenes().ForEach( s => {
					var t = new TestRegisterSMScene( s );
					t.SetEvent();
					_disposables.AddFirst( t );
				} );

				_isInitialized = true;
			} );

			_disposables.AddFirst( () => {
				_canceler.Dispose();
			} );
		}



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( async () => {
			await UTask.WaitWhile( _canceler, () => !_isInitialized );

			_inputManager.GetKey( SMInputKey.Quit )._enabledEvent.AddLast().Subscribe( _ => {
				SMLog.Warning( "終了押下！！" );
				UTask.Void( async () => {
					await _sceneManager.Destroy();
					SMServiceLocator.Unregister<SMSceneManager>();
				} );
			} );

			var i = 0;
			var scenes = new Type[] {
				typeof( TitleSMScene ),
				typeof( GameSMScene ),
				typeof( GameOverSMScene ),
				typeof( GameClearSMScene ),
			};
			_inputManager.GetKey( SMInputKey.Reset )._enabledEvent.AddLast().Subscribe( _ => {
				var t = scenes[i];
				SMLog.Warning( $"遷移押下 : {t.GetName()}" );
				_sceneManager.GetFSM<MainSMScene>().ChangeState( t ).Forget();
				i = ( i + 1 ) % scenes.Count();
			} );

			await UTask.Never( _canceler );
		} );
	}
}