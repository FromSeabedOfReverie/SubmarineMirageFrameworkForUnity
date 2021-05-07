//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Utility {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using Cysharp.Threading.Tasks;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.Networking;

	// 本当はこう書かなければならないが、記述を省く為、省略
	// 全書類に跨る、別名定義機能は、C#に無い
	using UniTaskSMUtility = UTask;



	///====================================================================================================
	/// <summary>
	/// ■ UniTaskの便利クラス。
	///		UniTaskの変更波及防止、停止処理の厳密化の為、ラッパー化。
	///		UniTaskSMUtilityだと長い為、UTaskと省略。
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
		public static Func<UniTask> Func( Func<UniTask> factory )
			=> factory;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Func< UniTask<T> > Func<T>( Func< UniTask<T> > factory )
			=> factory;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Void( Func<UniTaskVoid> asyncAction )
			=> UniTask.Void( asyncAction );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Action Action( Func<UniTaskVoid> asyncAction )
			=> UniTask.Action( asyncAction );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UnityAction UnityAction( Func<UniTaskVoid> asyncAction )
			=> UniTask.UnityAction( asyncAction );


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask Never( SMAsyncCanceler canceler )
			=> UniTask.Never( canceler.ToToken() );


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask Yield( SMAsyncCanceler canceler, PlayerLoopTiming timing = PlayerLoopTiming.Update )
			=> UniTask.Yield( timing, canceler.ToToken() );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask NextFrame( SMAsyncCanceler canceler,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => UniTask.NextFrame( timing, canceler.ToToken() );


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask Delay( SMAsyncCanceler canceler, int millisecondsDelay, bool ignoreTimeScale = false,
										PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			if ( millisecondsDelay == 0 )	{ return Empty; }
			return UniTask.Delay( millisecondsDelay, ignoreTimeScale, delayTiming, canceler.ToToken() );
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask Delay( SMAsyncCanceler canceler, TimeSpan delayTimeSpan, bool ignoreTimeScale = false,
										PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			if ( delayTimeSpan == TimeSpan.Zero )	{ return Empty; }
			return UniTask.Delay( delayTimeSpan, ignoreTimeScale, delayTiming, canceler.ToToken() );
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask Delay( SMAsyncCanceler canceler, int millisecondsDelay, DelayType delayType,
										PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			if ( millisecondsDelay == 0 )	{ return Empty; }
			return UniTask.Delay( millisecondsDelay, delayType, delayTiming, canceler.ToToken() );
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask Delay( SMAsyncCanceler canceler, TimeSpan delayTimeSpan, DelayType delayType,
										PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			if ( delayTimeSpan == TimeSpan.Zero )	{ return Empty; }
			return UniTask.Delay( delayTimeSpan, delayType, delayTiming, canceler.ToToken() );
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask DelayFrame( SMAsyncCanceler canceler, int delayFrameCount,
											PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			if ( delayFrameCount == 0 )	{ return Empty; }
			return UniTask.DelayFrame( delayFrameCount, delayTiming, canceler.ToToken() );
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask WaitWhile( SMAsyncCanceler canceler, Func<bool> predicate,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) {
			if ( !predicate() )	{ return Empty; }
			return UniTask.WaitWhile( predicate, timing, canceler.ToToken() );
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask WaitUntil( SMAsyncCanceler canceler, Func<bool> predicate,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) {
			if ( predicate() )	{ return Empty; }
			return UniTask.WaitUntil( predicate, timing, canceler.ToToken() );
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask<U> WaitUntilValueChanged<T, U>( SMAsyncCanceler canceler, T target,
																Func<T, U> monitorFunction,
													PlayerLoopTiming monitorTiming = PlayerLoopTiming.Update,
																IEqualityComparer<U> equalityComparer = null
		) where T : class
			=> UniTask.WaitUntilValueChanged(
				target, monitorFunction, monitorTiming, equalityComparer, canceler.ToToken()
			);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask WaitUntilCanceled( SMAsyncCanceler canceler,
													PlayerLoopTiming timing = PlayerLoopTiming.Update
		) {
			if ( canceler.ToToken().IsCancellationRequested )	{ return Empty; }
			return UniTask.WaitUntilCanceled( canceler.ToToken(), timing );
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask ToUniTask( this AsyncOperation asyncOperation, SMAsyncCanceler canceler,
											IProgress<float> progress = null,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => asyncOperation.ToUniTask( progress, timing, canceler.ToToken() );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask<UnityEngine.Object> ToUniTask( this ResourceRequest asyncOperation,
																SMAsyncCanceler canceler,
																IProgress<float> progress = null,
															PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => asyncOperation.ToUniTask( progress, timing, canceler.ToToken() );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask<UnityWebRequest> ToUniTask( this UnityWebRequestAsyncOperation asyncOperation,
															SMAsyncCanceler canceler,
															IProgress<float> progress = null,
															PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => asyncOperation.ToUniTask( progress, timing, canceler.ToToken() );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask ToUniTask( this SMAsyncCanceler canceler,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => WaitUntilCanceled( canceler, timing );

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UniTask ToUniTask( this IEnumerator enumerator, SMAsyncCanceler canceler,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => enumerator.ToUniTask( timing, canceler.ToToken() );


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerator ToCoroutine( Func<UniTask> taskFactory )
			=> UniTask.ToCoroutine( taskFactory );
	}
}