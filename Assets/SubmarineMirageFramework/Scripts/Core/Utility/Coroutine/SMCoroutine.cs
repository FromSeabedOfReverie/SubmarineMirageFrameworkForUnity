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
	using Base;
	using Service;
	using Extension;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ コルーチンのクラス
	///		UniRXを用いたコルーチンのラッパーで、処理を監視する。
	/// </summary>
	///====================================================================================================
	public class SMCoroutine : CustomYieldInstruction, ISMLightBase {
		///------------------------------------------------------------------------------------------------
		/// 要素
		///------------------------------------------------------------------------------------------------
		[SMShowLine] public uint _id		{ get; private set; }
		[SMShowLine] public bool _isDispose { get; private set; }

		/// <summary>コルーチン処理中フラグ</summary>
		public override bool keepWaiting => _isRunning;

		/// <summary>コルーチンの管理者</summary>
		SMCoroutineManager _coroutineTaskManager	{ get; set; }

		/// <summary>管理クラスに登録するか？</summary>
		[SMShow] bool _isRegister		{ get; set; }
		/// <summary>実行中か？</summary>
		[SMShowLine] bool _isRunning => _coroutineObserver != null;

		/// <summary>コルーチン本体</summary>
		IEnumerator _coroutine			{ get; set; }
		/// <summary>コルーチンの解放識別子</summary>
		IDisposable _coroutineObserver	{ get; set; }
		/// <summary>処理完了時の呼戻関数</summary>
		Action _completedEvent		{ get; set; }
		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMCoroutine( IEnumerator coroutine, Action completedEvent = null,
								bool isPlay = true, bool isRegister = true ) {
			_id = SMIDCounter.GetNewID( this );

			_coroutineTaskManager = SMServiceLocator.Resolve<SMCoroutineManager>();

			_coroutine = coroutine;
			_completedEvent = completedEvent;
			_isRegister = isRegister;
			if ( isPlay )	{ Play(); }
		}

		/// <summary>
		/// ● デストラクタ
		/// </summary>
		~SMCoroutine()
			=> Dispose();

		/// <summary>
		/// ● 解放
		/// </summary>
		public void Dispose() {
			if ( _isDispose )	{ return; }

			Pause();
			_coroutine = null;
			_completedEvent = null;
			_coroutineTaskManager = null;

			_isDispose = true;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 再生
		///		※中断箇所から再生され、最後まで再生されても、最初から再実行される事はない。（何も実行されない）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Play() {
			CheckDisposeError();
			if ( _isRunning )	{ return; }


			if ( _isRegister )	{ _coroutineTaskManager.Register( this ); }

			var isDontWait = false;

			_coroutineObserver = Observable.FromCoroutine( () => _coroutine )
				.DoOnCompleted( () => {
					Pause();
					_completedEvent?.Invoke();
					isDontWait = true;
				} )
				.Subscribe();

			// 実行後、待機無しで即完了した場合、Pause内_isRunningが設定前で、早期リターンされる為、
			// FromCoroutine外側の下記で、改めて呼ぶ必要がある
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

			
			if ( _isRegister )	{ _coroutineTaskManager.Unregister( this ); }

			_coroutineObserver?.Dispose();
			_coroutineObserver = null;
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
			if ( !_isDispose )	{ return; }

			throw new ObjectDisposedException( nameof( SMCoroutine ), $"既に解放済 : {this}" );
		}
		///------------------------------------------------------------------------------------------------
		/// ● 文章変換
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文章に変換
		/// </summary>
		public override string ToString() => ToString( 0 );

		/// <summary>
		/// ● 文章に変換（インデントを整列）
		/// </summary>
		public virtual string ToString( int indent, bool isUseHeadIndent = true )
			=> this.ToShowString( indent, false, false, isUseHeadIndent );

		/// <summary>
		/// ● 1行文章に変換（インデントを整列）
		/// </summary>
		public virtual string ToLineString( int indent = 0 )
			=> ObjectSMExtension.ToLineString( this, indent );

		/// <summary>
		/// ● 文章追加を設定
		/// </summary>
		public virtual string AddToString( int indent )
			=> string.Empty;
	}
}