//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Main {
	using System.Threading.Tasks;
	using UnityEngine;
	using DG.Tweening;
	using UniRx;
	using UniRx.Async;
	using Process;
	using Data;
	using Data.File;
	using Extension;
	using Build;
	using Debug;
	using Test.Audio;
	///====================================================================================================
	/// <summary>
	/// ★ メイン処理のクラス
	///----------------------------------------------------------------------------------------------------
	///		一番最初に実行される、メイン関数を定義している。
	/// </summary>
	///====================================================================================================
	public class MainProcess {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ★ メイン関数（最初期実行）
		///		一番最初に実行される処理。
		///		謎の指定子でAwake以前に呼ばれるが、多用すると危険な為、重要物にのみ、1つだけ使うべき。
		///		
		///		しかし、UniRxと競合しており、無理矢理awaitさせている。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
		static async void Main() {
			// UniRxも同名指定子で、最初期実行関数を指定している為、UniRx初期化まで待機
			await Task.Delay( 1 );
//			await UniTask.Delay( 1 );	// 当関数は毎回実行順序が変わり、UniRx未初期化の場合、エラー


			// 中心処理管理クラスを初期化
			await CoreProcessManager.s_instance.Constructor( InitializePlugin, RegisterProcesses );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 初期化（外部プラグイン）
		///		※ここに、外部ライブラリを順序付けて、初期化する。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		static async UniTask InitializePlugin() {
			// DOTweenを初期化
			DOTween.Init(
				false,
#if DEVELOP
				false,
#else
				true,
#endif
				LogBehaviour.ErrorsOnly );
			DOTween.defaultAutoPlay = AutoPlay.None;

			// コンソールを初期化
#if DEVELOP
			var go = Object.Instantiate( Resources.Load<GameObject>( "LunarConsole" ) );
			Object.DontDestroyOnLoad( go );
#endif

			await UniTask.Delay( 0 );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 処理を登録
		///		※ここに、処理を順序付けて、登録する。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		static async UniTask RegisterProcesses() {
//			new TestProcess();
//			new WriteLicense();
			new TestHogeProcess2();
			return;

			await DebugManager.WaitForCreation();

			await CoroutineProcessManager.WaitForCreation();
			await InputManager.WaitForCreation();
			await NetworkManager.WaitForCreation();
			await TimeManager.WaitForCreation();

			await FileManager.WaitForCreation();
			await AllDataManager.WaitForCreation();
			await TestAudioManager.WaitForCreation();
			await SplashScreenExtension.WaitForCreation();

#if DEVELOP
			await DebugDisplay.WaitForCreation();
#endif

			// 登録は、継承先コンストラクタ実行後になるよう、1ミリ秒遅延させた為、全遅延登録完了まで待機が必要
			// 指定時間間隔が短過ぎる為か、Delay( 10 )でもDelay( 1 )より早く実行される為、
			// 念の為、次のフレームまで待機させた
			await UniTask.DelayFrame( 1 );
		}
	}
}