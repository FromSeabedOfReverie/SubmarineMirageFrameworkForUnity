//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestScene {
	using System;
	using System.Linq;
	using UnityEngine;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Base;
	using Service;
	using Scene;
	using Extension;
	using Utility;
	using Debug;



	public class TestRegisterSMSceneManager : SMStandardBase {
		SMSceneManager _sceneManager;


		public TestRegisterSMSceneManager( SMSceneManager sceneManager ) {
			_sceneManager = sceneManager;

			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Return ) ).Subscribe( _ => {
					SMLog.Warning( "終了押下！！" );
					_sceneManager.AllChangeNullState().Forget();
				} )
			);
			
			var i = 0;
			var scenes = new Type[] {
				typeof( TitleSMScene ),
				typeof( GameSMScene ),
				typeof( GameOverSMScene ),
				typeof( GameClearSMScene ),
			};
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Space ) ).Subscribe( _ => {
					var t = scenes[i];
					SMLog.Warning( $"遷移押下 : {t.GetAboutName()}" );
					_sceneManager.GetFSM<MainSMScene>().ChangeState( t ).Forget();
					i = ( i + 1 ) % scenes.Count();
				} )
			);

//			return;

			UTask.Void( async () => {
				var debugDisplay =
					await SMServiceLocator.WaitResolve<SMDisplayLog>( _sceneManager._asyncCancelerOnDispose );
				while ( true ) {
					debugDisplay.Add( $"{_sceneManager.GetAboutName()} : { _sceneManager._ranState }" );
					_sceneManager.GetScenes().ForEach(
						s => debugDisplay.Add( $"{s.GetAboutName()} : { s._ranState }" )
					);
					await UTask.NextFrame( _sceneManager._asyncCancelerOnDispose );
				}
			} );
		}


		public void SetEvent() {
			_sceneManager._selfInitializeEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {_sceneManager.GetAboutName()}.{nameof( _sceneManager._selfInitializeEvent )}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {_sceneManager.GetAboutName()}.{nameof( _sceneManager._selfInitializeEvent )}" );
			} );
			_sceneManager._initializeEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {_sceneManager.GetAboutName()}.{nameof( _sceneManager._initializeEvent )}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {_sceneManager.GetAboutName()}.{nameof( _sceneManager._initializeEvent )}" );
			} );
			_sceneManager._finalizeEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {_sceneManager.GetAboutName()}.{nameof( _sceneManager._finalizeEvent )}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {_sceneManager.GetAboutName()}.{nameof( _sceneManager._finalizeEvent )}" );
			} );

			_sceneManager._enableEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{_sceneManager.GetAboutName()}.{nameof( _sceneManager._enableEvent )}" );
			} );
			_sceneManager._disableEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{_sceneManager.GetAboutName()}.{nameof( _sceneManager._disableEvent )}" );
			} );

			return;

			_sceneManager._fixedUpdateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{_sceneManager.GetAboutName()}.{nameof( _sceneManager._fixedUpdateEvent )}" );
			} );
			_sceneManager._updateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{_sceneManager.GetAboutName()}.{nameof( _sceneManager._updateEvent )}" );
			} );
			_sceneManager._lateUpdateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{_sceneManager.GetAboutName()}.{nameof( _sceneManager._lateUpdateEvent )}" );
			} );
		}
	}
}