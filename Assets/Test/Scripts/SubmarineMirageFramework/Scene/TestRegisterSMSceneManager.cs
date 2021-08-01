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