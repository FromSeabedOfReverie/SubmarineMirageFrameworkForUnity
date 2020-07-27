//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Main {
	using System.Threading.Tasks;
	using UnityEngine;
	using DG.Tweening;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using UTask;
	using Debug;
	using UnityObject = UnityEngine.Object;


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
			var go = UnityObject.Instantiate( prefab );
			UnityObject.DontDestroyOnLoad( go );
#endif

			await UTask.DontWait();
		}


		static async UniTask RegisterBehaviours() {
			new Log();

			await UTask.DontWait();
		}


// TODO : 戻す
//		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
//		static async void Main()
//			=> await SubmarineMirage.s_instance.TakeOff( InitializePlugin, RegisterBehaviours );
// TODO : 消す
		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
		static async void Main() {
			await Task.Delay( 1 );
			await InitializePlugin();
			var canceler = new UTaskCanceler();
			await UTask.WaitWhile( canceler, () => !SubmarineMirage.s_instance._isRegisterCreateTestEvent );
			await SubmarineMirage.s_instance._createTestEvent.Run( canceler );
			SubmarineMirage.s_instance._isInitialized = true;
			canceler.Dispose();
		}
	}
}