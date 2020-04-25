//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Main.New {
	using System;
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using UniRx.Async;
	using Process.New;
	using Singleton.New;


	// TODO : コメント追加、整頓


	public static class SubmarineMirage {
		public static bool s_isInitialized	{ get; private set; }


// TODO : もっと良い開始名を考える
		public static async UniTask TakeOff( Func<UniTask> _initializePluginEvent,
												Func<UniTask> _registerProcessesEvent )
		{
			s_isInitialized = false;
			await Task.Delay( 1 );

			await _initializePluginEvent();

			MonoBehaviourSingletonManager.CreateInstance();
			ProcessRunner.CreateInstance();
			var h = new ProcessHierarchy(
				MonoBehaviourSingletonManager.s_instance.gameObject,
				new List<IProcess>() { MonoBehaviourSingletonManager.s_instance },
				null
			);
// TODO : 多分使わないので、未設定でもエラーにならない
//			ProcessRunner.s_instance._hierarchy = h;

			await ProcessRunner.s_instance.Create( () => _registerProcessesEvent() );

			s_isInitialized = true;
		}
	}
}