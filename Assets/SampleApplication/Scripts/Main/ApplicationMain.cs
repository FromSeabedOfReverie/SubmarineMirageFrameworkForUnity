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
using SubmarineMirage.Scene;
using SubmarineMirage.Extension;
using SubmarineMirage.Utility;
using SubmarineMirage.Debug;



// TODO : コメント追加、整頓



public static class ApplicationMain {
	static async UniTask InitializePlugin() {
		UniTaskScheduler.UnobservedExceptionWriteLogType = LogType.Error;

		DOTween.Init(
			false,
#if DEVELOP
			false,
#else
			true,
#endif
			LogBehaviour.ErrorsOnly );
		DOTween.defaultAutoPlay = AutoPlay.None;

#if DEVELOP
		var prefab = Resources.Load<GameObject>( "LunarConsole" );
		var go = prefab.Instantiate();
		go.DontDestroyOnLoad();
#endif

		await UTask.DontWait();
	}


	static async UniTask RegisterSettings() {
		SMServiceLocator.Register<BaseSMSceneSetting>( new SMSceneSetting() );

		await UTask.DontWait();
	}


	static async UniTask RegisterBehaviours() {
		await UTask.DontWait();
	}


	[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
	static async void Main() {
		var framework = new SubmarineMirageFramework();
		await framework.TakeOff( InitializePlugin, RegisterSettings, RegisterBehaviours );
	}
}