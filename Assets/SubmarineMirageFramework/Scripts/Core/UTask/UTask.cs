//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.UTask {
	using System;
	using System.Threading;
	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using Cysharp.Threading.Tasks;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.Networking;


	// TODO : コメント追加、整頓


	///====================================================================================================
	/// <summary>
	/// ■ UniTaskの便利クラス。
	///		UniTaskの変更波及防止、停止処理の厳密化の為、ラッパー化。
	///		UniTaskUtilityだと長い為、UTaskと省略。
	///		
	///		TODO : DOTween対応
	///				Addressables、TextMeshPro、AsyncLINQは、使用しない。
	/// </summary>
	///====================================================================================================
	public static class UTask {
		public static readonly UniTask Empty = new UniTask();


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask DontWait() => Empty;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask Create( Func<UniTask> factory )
			=> UniTask.Create( factory );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask<T> Create<T>( Func< UniTask<T> > factory )
			=> UniTask.Create( factory );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask Defer( Func<UniTask> factory )
			=> UniTask.Defer( factory );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask<T> Defer<T>( Func< UniTask<T> > factory )
			=> UniTask.Defer( factory );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static AsyncLazy Lazy( Func<UniTask> factory )
			=> UniTask.Lazy( factory );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static AsyncLazy<T> Lazy<T>( Func< UniTask<T> > factory )
			=> UniTask.Lazy( factory );


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Void( CancellationToken cancellationToken,
									Func<CancellationToken, UniTaskVoid> asyncAction
		) => UniTask.Void( asyncAction, cancellationToken );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Action Action( CancellationToken cancellationToken,
										Func<CancellationToken, UniTaskVoid> asyncAction
		) => UniTask.Action( asyncAction, cancellationToken );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UnityAction UnityAction( CancellationToken cancellationToken,
												Func<CancellationToken, UniTaskVoid> asyncAction
		) => UniTask.UnityAction( asyncAction, cancellationToken );


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask Never( CancellationToken cancellationToken )
			=> UniTask.Never( cancellationToken );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask<T> Never<T>( CancellationToken cancellationToken )
			=> UniTask.Never<T>( cancellationToken );


		// 引数が無いのが、高速らしい
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Cysharp.Threading.Tasks.YieldAwaitable Yield() => UniTask.Yield();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask Yield( CancellationToken cancellationToken,
										PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => UniTask.Yield( timing, cancellationToken );


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask NextFrame( CancellationToken cancellationToken,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => UniTask.NextFrame( timing, cancellationToken );


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask WaitForEndOfFrame( CancellationToken cancellationToken )
			=> UniTask.WaitForEndOfFrame( cancellationToken );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask WaitForFixedUpdate( CancellationToken cancellationToken )
			=> UniTask.WaitForFixedUpdate( cancellationToken );


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask Delay( CancellationToken cancellationToken, int millisecondsDelay,
										bool ignoreTimeScale = false,
										PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			if ( millisecondsDelay == 0 )	{ return Empty; }
			return UniTask.Delay( millisecondsDelay, ignoreTimeScale, delayTiming, cancellationToken );
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask Delay( CancellationToken cancellationToken, TimeSpan delayTimeSpan,
										bool ignoreTimeScale = false,
										PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			if ( delayTimeSpan == TimeSpan.Zero )	{ return Empty; }
			return UniTask.Delay( delayTimeSpan, ignoreTimeScale, delayTiming, cancellationToken );
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask Delay( CancellationToken cancellationToken, int millisecondsDelay,
										DelayType delayType,
										PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			if ( millisecondsDelay == 0 )	{ return Empty; }
			return UniTask.Delay( millisecondsDelay, delayType, delayTiming, cancellationToken );
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask Delay( CancellationToken cancellationToken, TimeSpan delayTimeSpan,
										DelayType delayType,
										PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			if ( delayTimeSpan == TimeSpan.Zero )	{ return Empty; }
			return UniTask.Delay( delayTimeSpan, delayType, delayTiming, cancellationToken );
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask DelayFrame( CancellationToken cancellationToken, int delayFrameCount,
											PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			if ( delayFrameCount == 0 )	{ return Empty; }
			return UniTask.DelayFrame( delayFrameCount, delayTiming, cancellationToken );
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask WaitWhile( CancellationToken cancellationToken, Func<bool> predicate,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) {
			if ( !predicate() )	{ return Empty; }
			return UniTask.WaitWhile( predicate, timing, cancellationToken );
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask WaitUntil( CancellationToken cancellationToken, Func<bool> predicate,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) {
			if ( predicate() )	{ return Empty; }
			return UniTask.WaitUntil( predicate, timing, cancellationToken );
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask<U> WaitUntilValueChanged<T, U>( CancellationToken cancellationToken,
																T target, Func<T, U> monitorFunction,
													PlayerLoopTiming monitorTiming = PlayerLoopTiming.Update,
																IEqualityComparer<U> equalityComparer = null
		) where T : class
			=> UniTask.WaitUntilValueChanged(
				target, monitorFunction, monitorTiming, equalityComparer, cancellationToken
			);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask WaitUntilCanceled( CancellationToken cancellationToken,
													PlayerLoopTiming timing = PlayerLoopTiming.Update
		) {
			if ( cancellationToken.IsCancellationRequested )	{ return Empty; }
			return UniTask.WaitUntilCanceled( cancellationToken, timing );
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask FromCanceled( CancellationToken cancellationToken ) {
			if ( cancellationToken.IsCancellationRequested )	{ return Empty; }
			return UniTask.FromCanceled( cancellationToken );
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask<T> FromCanceled<T>( CancellationToken cancellationToken ) {
			if ( cancellationToken.IsCancellationRequested )	{ return new UniTask<T>(); }
			return UniTask.FromCanceled<T>( cancellationToken );
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask ToUniTask( this AsyncOperation asyncOperation,
											CancellationToken cancellationToken,
											IProgress<float> progress = null,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => asyncOperation.ToUniTask( progress, timing, cancellationToken );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask<UnityEngine.Object> ToUniTask( this ResourceRequest asyncOperation,
																CancellationToken cancellationToken,
																IProgress<float> progress = null,
															PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => asyncOperation.ToUniTask( progress, timing, cancellationToken );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask<UnityWebRequest> ToUniTask( this UnityWebRequestAsyncOperation asyncOperation,
															CancellationToken cancellationToken,
															IProgress<float> progress = null,
															PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => asyncOperation.ToUniTask( progress, timing, cancellationToken );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask ToUniTask( this IEnumerator enumerator,
											CancellationToken cancellationToken,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => enumerator.ToUniTask( timing, cancellationToken );


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerator ToCoroutine( Func<UniTask> taskFactory )
			=> UniTask.ToCoroutine( taskFactory );
	}
}