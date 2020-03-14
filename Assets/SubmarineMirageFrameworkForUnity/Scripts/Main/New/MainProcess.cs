//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Main.New {
	using System.Threading;
	using System.Threading.Tasks;
	using UnityEngine;
	using DG.Tweening;
	using UniRx;
	using UniRx.Async;
	using Process.New;
	using Singleton.New;
	using FSM.New;
	using Utility;
	using UnityObject = UnityEngine.Object;


	// TODO : コメント追加、整頓


	public static class MainProcess {
		static CancellationToken s_asyncCancel => CoreProcessManager.s_instance._activeAsyncCancel;

		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
		static async void Main() {
			await Task.Delay( 1 );
			await CoreProcessManager.s_instance.Create( InitializePlugin, RegisterProcesses );
		}

		static async UniTask InitializePlugin() {
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
			var go = UnityObject.Instantiate( Resources.Load<GameObject>( "LunarConsole" ) );
			UnityObject.DontDestroyOnLoad( go );
#endif
			await UniTaskUtility.Delay( s_asyncCancel, 0 );
		}

		static async UniTask RegisterProcesses() {
//			new TestProcess2();
			new TestOwner();
			await UniTaskUtility.DelayFrame( s_asyncCancel, 1 );
		}
	}
}