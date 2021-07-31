//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using UnityEngine;
using DG.Tweening;
using UniRx;
using Cysharp.Threading.Tasks;
using KoganeUnityLib;
using SubmarineMirage;
using SubmarineMirage.Service;
using SubmarineMirage.Data;
using SubmarineMirage.Scene;
using SubmarineMirage.Extension;
using SubmarineMirage.Utility;
using SubmarineMirage.Debug;



public static class ApplicationMain {
	static async UniTask InitializePlugin() {
		UniTaskScheduler.UnobservedExceptionWriteLogType = LogType.Error;

		DOTween.Init(
			false,
			!SMDebugManager.IS_DEVELOP,
			LogBehaviour.ErrorsOnly
		);
		DOTween.defaultAutoPlay = AutoPlay.None;

		if ( SMDebugManager.IS_DEVELOP ) {
/*
			var prefab = Resources.Load<GameObject>( "LunarConsole" );
			var go = prefab.Instantiate();
			go.DontDestroyOnLoad();
*/
		}

		await UTask.DontWait();
	}


	static async UniTask RegisterSettings() {
		SMServiceLocator.Register<BaseSMDataSetting>( new SMDataSetting() );
		SMServiceLocator.Register<BaseSMSceneSetting>( new SMSceneSetting() );

		await UTask.DontWait();
	}


	[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
	static async void Main() {
//		return;

		var framework = SMServiceLocator.Register( new SubmarineMirageFramework() );
		await framework.TakeOff( InitializePlugin, RegisterSettings );
	}
}