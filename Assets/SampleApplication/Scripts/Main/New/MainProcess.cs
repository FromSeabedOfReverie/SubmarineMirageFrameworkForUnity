//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Main.New {
	using System.Threading;
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using UnityEngine;
	using DG.Tweening;
	using UniRx;
	using UniRx.Async;
	using Process.New;
	using MultiEvent;
	using Singleton.New;
	using FSM.New;
	using Scene;
	using Utility;
	using Debug;
	using UnityObject = UnityEngine.Object;


	// TODO : コメント追加、整頓


	public static class MainProcess {
		public static bool s_isInitialized	{ get; private set; }


		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
		static async void Main() {
			s_isInitialized = false;
			await Task.Delay( 1 );


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


			MonoBehaviourSingletonManager.CreateInstance();
			ProcessHierarchyManager.CreateInstance();
			var h = new ProcessHierarchy(
				MonoBehaviourSingletonManager.s_instance.gameObject,
				new List<IProcess>() { MonoBehaviourSingletonManager.s_instance },
				null
			);
// TODO : 多分使わないので、未設定でもエラーにならない
//			ProcessHierarchyManager.s_instance._hierarchy = h;


			await ProcessHierarchyManager.s_instance.Create(
				async () => {
					new Log();
					await SceneManager.WaitForCreation();
					await UniTaskUtility.DontWait();
				}
			);


			s_isInitialized = true;
		}
	}
}