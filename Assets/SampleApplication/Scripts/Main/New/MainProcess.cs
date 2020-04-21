//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Main.New {
	using System.Threading;
	using System.Threading.Tasks;
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
		static CancellationToken s_asyncCancel => CoreProcessManager.s_instance._activeAsyncCancel;


		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
		static async void Main() {
			await Task.Delay( 1 );

			await CoreProcessManager.s_instance.Create(

				async () => {
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
					var p = await Resources.LoadAsync<GameObject>( "LunarConsole" )
						.ConfigureAwait( s_asyncCancel );
					var go = UnityObject.Instantiate( p );
					UnityObject.DontDestroyOnLoad( go );
#endif
					await UniTaskUtility.DontWait();
				},

				async () => {
					await MonoBehaviourSingletonManager.WaitForCreation();
					await CoreProcessManager.WaitForCreation();
					await SceneManager.WaitForCreation();

					await UniTaskUtility.DontWait();
				}
			);
		}
	}
}