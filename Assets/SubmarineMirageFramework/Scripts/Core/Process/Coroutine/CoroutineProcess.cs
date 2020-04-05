//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process {
	using System;
	using System.Collections;
	using UnityEngine;
	using UniRx;
	///====================================================================================================
	/// <summary>
	/// ■ コルーチン処理のクラス
	///----------------------------------------------------------------------------------------------------
	///		UniRXを用いたコルーチンのラッパーで、処理を監視する。
	/// </summary>
	///====================================================================================================
	public class CoroutineProcess : CustomYieldInstruction {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>コルーチン中断中フラグ</summary>
		public override bool keepWaiting { get { return _isRunning; } }

		/// <summary>管理クラスに登録するか？</summary>
		bool _isRegister;
		/// <summary>実行中か？</summary>
		bool _isRunning;
		/// <summary>コルーチン処理の本体</summary>
		IEnumerator _coroutine;
		/// <summary>処理完了時の呼戻関数</summary>
		Action _onCompleted;
		/// <summary>処理削除用の変数</summary>
		IDisposable _disposable;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public CoroutineProcess( IEnumerator coroutine, Action onCompleted = null,
									bool isPlay = true, bool isRegister = false )
		{
			_coroutine = coroutine;
			_onCompleted = onCompleted;
			_isRegister = isRegister;
			if ( isPlay )	{ Play(); }
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 再生
		///		※中断箇所から再生され、最後まで再生されても、最初から再実行される事はない。（何も実行されない）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Play() {
			if ( _isRunning )	{ return; }
			_isRunning = true;
			
			if ( _isRegister )	{ CoroutineProcessManager.s_instance.Register( this ); }
			_disposable = Observable
				.FromCoroutine( () => _coroutine )
				.DoOnCompleted( () => PlayComplete() )
				.Subscribe();
		}
		/// <summary>
		/// ● 再生完了
		/// </summary>
		void PlayComplete() {
			Pause();
			_onCompleted?.Invoke();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 一時停止
		///		※一時停止後、再生すると途中再開出来るが、最初に巻き戻せない。（停止関数はない）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Pause() {
			if ( !_isRunning )	{ return; }
			_isRunning = false;
			
			if ( _isRegister )	{ CoroutineProcessManager.s_instance.UnRegister( this ); }
			_disposable?.Dispose();
		}
/*
		///------------------------------------------------------------------------------------------------
		/// ● 実現不可能関数
		//		これらの関数は、IEnumerator型のせいで実装できない。
		//		UnityのコルーチンはIEnumeratorを削るように再生し、最初の状態に戻せず複製も出来ない為、実装困難。
		//		よってこのクラスでは、再生、一時停止だけ提供し、コルーチンは使い捨てる物と割り切り、
		//		このクラスの変数を確保して、使い回すような使用方法は、できない。
		///------------------------------------------------------------------------------------------------
		/// <summary>停止</summary>
		public void Stop() {}
		/// <summary>最初から再生</summary>
		public void RePlay() {}
*/
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● デストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		~CoroutineProcess() {
			Pause();
		}
	}
}