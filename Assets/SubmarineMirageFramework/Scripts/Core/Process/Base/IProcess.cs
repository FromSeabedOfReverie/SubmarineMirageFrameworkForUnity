//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process {
	using System;
	using UniRx.Async;
	///====================================================================================================
	/// <summary>
	/// ■ 処理のインターフェース
	///----------------------------------------------------------------------------------------------------
	///		各種処理のインターフェースが、継承して使用する。
	/// </summary>
	///====================================================================================================
	public interface IProcess {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>実行済処理の状態</summary>
		CoreProcessManager.ExecutedState _executedState	{ get; set; }
		/// <summary>登録するか？</summary>
		bool _isRegister	{ get; }
		/// <summary>場面内だけ存在するか？</summary>
		bool _isInSceneOnly { get; }
		/// <summary>中心処理初期化後まで待機するか？</summary>
		bool _isWaitInitializedCoreProcesses	{ get; }
		/// <summary>初期化済か？</summary>
		bool _isInitialized	{ get; }
		/// <summary>初期化時のイベント</summary>
		Func<UniTask> _initializeEvent	{ get; }
		/// <summary>終了時のイベント</summary>
		Func<UniTask> _finalizeEvent	{ get; }
		///------------------------------------------------------------------------------------------------
		/// ● 仮想関数
		///------------------------------------------------------------------------------------------------
		/// <summary>● 初期化</summary>
		UniTask Initialize();
		/// <summary>● 終了</summary>
		UniTask Finalize();
	}
}