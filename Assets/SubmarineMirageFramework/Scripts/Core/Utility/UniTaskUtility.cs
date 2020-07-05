//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Utility {
	using System;
	using System.Threading;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using UnityEngine;
	using UnityEngine.Networking;
	using System.Collections;


	// TODO : コメント追加、整頓


	///====================================================================================================
	/// <summary>
	/// ■ UniTaskの便利クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public static class UniTaskUtility {


		public static readonly UniTask None = new UniTask();
		public static readonly UniTask<int> NoneInt = new UniTask<int>( 0 );


		public static UniTask DontWait() => None;


		public static UniTask Delay( CancellationToken cancellationToken, int millisecondsDelay,
										bool ignoreTimeScale = false,
										PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			if ( millisecondsDelay == 0 )	{ return None; }
			return UniTask.Delay( millisecondsDelay, ignoreTimeScale, delayTiming, cancellationToken );
		}

		public static UniTask Delay( CancellationToken cancellationToken, TimeSpan delayTimeSpan,
										bool ignoreTimeScale = false,
										PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			if ( delayTimeSpan == TimeSpan.Zero )	{ return None; }
			return UniTask.Delay( delayTimeSpan, ignoreTimeScale, delayTiming, cancellationToken );
		}

		public static UniTask Delay( CancellationToken cancellationToken, int millisecondsDelay,
										DelayType delayType,
										PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			if ( millisecondsDelay == 0 )	{ return None; }
			return UniTask.Delay( millisecondsDelay, delayType, delayTiming, cancellationToken );
		}

		public static UniTask Delay( CancellationToken cancellationToken, TimeSpan delayTimeSpan,
										DelayType delayType,
										PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			if ( delayTimeSpan == TimeSpan.Zero )	{ return None; }
			return UniTask.Delay( delayTimeSpan, delayType, delayTiming, cancellationToken );
		}


		public static UniTask DelayFrame( CancellationToken cancellationToken, int delayFrameCount,
											PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			if ( delayFrameCount == 0 )	{ return None; }
			return UniTask.DelayFrame( delayFrameCount, delayTiming, cancellationToken );
		}


		// 高速らしい
		public static YieldAwaitable Yield() => UniTask.Yield();

		public static UniTask Yield( CancellationToken cancellationToken,
										PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => UniTask.Yield( timing, cancellationToken );


		public static UniTask NextFrame( CancellationToken cancellationToken,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => UniTask.NextFrame( timing, cancellationToken );


		public static UniTask WaitForEndOfFrame( CancellationToken cancellationToken )
			=> UniTask.WaitForEndOfFrame( cancellationToken );

		public static UniTask WaitForFixedUpdate( CancellationToken cancellationToken )
			=> UniTask.WaitForFixedUpdate( cancellationToken );


		public static UniTask WaitWhile( CancellationToken cancellationToken, Func<bool> predicate,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) {
			if ( !predicate() )	{ return None; }
			return UniTask.WaitWhile( predicate, timing, cancellationToken );
		}

		public static UniTask WaitUntil( CancellationToken cancellationToken, Func<bool> predicate,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) {
			if ( predicate() )	{ return None; }
			return UniTask.WaitUntil( predicate, timing, cancellationToken );
		}

		public static UniTask<U> WaitUntilValueChanged<T, U>( CancellationToken cancellationToken,
																T target, Func<T, U> monitorFunction,
													PlayerLoopTiming monitorTiming = PlayerLoopTiming.Update,
																IEqualityComparer<U> equalityComparer = null
		) where T : class
			=> UniTask.WaitUntilValueChanged(
				target, monitorFunction, monitorTiming, equalityComparer, cancellationToken
			);

		public static UniTask WaitUntilCanceled( CancellationToken cancellationToken,
													PlayerLoopTiming timing = PlayerLoopTiming.Update
		) {
			if ( cancellationToken.IsCancellationRequested )	{ return None; }
			return UniTask.WaitUntilCanceled( cancellationToken, timing );
		}


		public static UniTask FromCanceled( CancellationToken cancellationToken ) {
			if ( cancellationToken.IsCancellationRequested )	{ return None; }
			return UniTask.FromCanceled( cancellationToken );
		}

		public static UniTask<T> FromCanceled<T>( CancellationToken cancellationToken ) {
			if ( cancellationToken.IsCancellationRequested )	{ return new UniTask<T>(); }
			return UniTask.FromCanceled<T>( cancellationToken );
		}


		public static UniTask Never( CancellationToken cancellationToken )
			=> UniTask.Never( cancellationToken );

		public static UniTask<T> Never<T>( CancellationToken cancellationToken )
			=> UniTask.Never<T>( cancellationToken );


		public static UniTask ToUniTask( this AsyncOperation asyncOperation,
											CancellationToken cancellationToken,
											IProgress<float> progress = null,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => asyncOperation.ToUniTask( progress, timing, cancellationToken );

		public static UniTask<UnityEngine.Object> ToUniTask( this ResourceRequest asyncOperation,
																CancellationToken cancellationToken,
																IProgress<float> progress = null,
															PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => asyncOperation.ToUniTask( progress, timing, cancellationToken );

		public static UniTask<UnityWebRequest> ToUniTask( this UnityWebRequestAsyncOperation asyncOperation,
															CancellationToken cancellationToken,
															IProgress<float> progress = null,
															PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => asyncOperation.ToUniTask( progress, timing, cancellationToken );

		public static UniTask ToUniTask( this IEnumerator enumerator,
											CancellationToken cancellationToken,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) => enumerator.ToUniTask( timing, cancellationToken );
	}
}