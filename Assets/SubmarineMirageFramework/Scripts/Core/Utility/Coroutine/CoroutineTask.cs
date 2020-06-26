//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Utility {
	using System;
	using System.Collections;
	using UnityEngine;
	using UniRx;
	using MultiEvent;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ コルーチンの仕事クラス
	///		UniRXを用いたコルーチンのラッパーで、処理を監視する。
	/// </summary>
	///====================================================================================================
	public class CoroutineTask : CustomYieldInstruction, IDisposableExtension {
		///------------------------------------------------------------------------------------------------
		/// 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>コルーチン処理中フラグ</summary>
		public override bool keepWaiting => _isRunning;

		/// <summary>管理クラスに登録するか？</summary>
		bool _isRegister;
		/// <summary>実行中か？</summary>
		bool _isRunning;
		/// <summary>コルーチン処理の本体</summary>
		IEnumerator _coroutine;
		/// <summary>処理完了時の呼戻関数</summary>
		Action _onCompleted;
		/// <summary>複数解放</summary>
		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();
		///------------------------------------------------------------------------------------------------
		/// 生成
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public CoroutineTask( IEnumerator coroutine, Action onCompleted = null,
								bool isPlay = true, bool isRegister = false )
		{
			_coroutine = coroutine;
			_onCompleted = onCompleted;
			_isRegister = isRegister;
			if ( isPlay )	{ Play(); }

			_disposables.AddLast( () => {
				_isRunning = false;
				if ( _isRegister )	{ CoroutineTaskManager.s_instance.UnRegister( this ); }
			} );
		}
		/// <summary>
		/// ● デストラクタ
		/// </summary>
		~CoroutineTask() => Dispose();
		/// <summary>
		/// ● 解放
		/// </summary>
		public void Dispose() => _disposables.Dispose();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 再生
		///		※中断箇所から再生され、最後まで再生されても、最初から再実行される事はない。（何も実行されない）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Play() {
			CheckDisposeError();
			if ( _isRunning )	{ return; }

			if ( _isRegister )	{ CoroutineTaskManager.s_instance.Register( this ); }
			var isDontWait = false;
			_disposables.AddFirst( "Coroutine",
				Observable.FromCoroutine( () => _coroutine )
					.DoOnCompleted( () => {
						Pause();
						_onCompleted?.Invoke();
						isDontWait = true;
					} )
					.Subscribe()
			);
			_isRunning = true;
			if ( isDontWait )	{ Pause(); }
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 一時停止
		///		※一時停止後、再生すると途中再開出来るが、最初に巻き戻せない。（停止関数はない）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Pause() {
			CheckDisposeError();
			if ( !_isRunning )	{ return; }
			_isRunning = false;
			
			if ( _isRegister )	{ CoroutineTaskManager.s_instance.UnRegister( this ); }
			_disposables.Remove( "Coroutine" );
		}
/*
		///------------------------------------------------------------------------------------------------
		/// 実現不可能関数
		///		これらの関数は、IEnumerator型のせいで実装できない。
		///		UnityのコルーチンはIEnumeratorを削るように再生し、最初の状態に戻せず複製も出来ない為、実装困難。
		///		よってこのクラスでは、再生、一時停止だけ提供し、コルーチンは使い捨てる物と割り切り、
		///		このクラスの変数を確保して、使い回すような使用方法は、できない。
		///------------------------------------------------------------------------------------------------
		/// <summary>● 停止</summary>
		public void Stop() {}
		/// <summary>● 最初から再生</summary>
		public void RePlay() {}
*/
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 解放済失敗を判定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void CheckDisposeError() {
			if ( !_disposables._isDispose )	{ return; }
			throw new ObjectDisposedException( this.GetAboutName(), "既に解放済" );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文字列に変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override string ToString()
			=> $"{this.GetAboutName()}( {nameof( _isRegister )} : {_isRegister}, "
				+ $"{nameof( _isRunning )} : {_isRunning} )";
	}
}