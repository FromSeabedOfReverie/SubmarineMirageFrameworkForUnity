//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Utility {
	using System;
	using System.Threading;
	using System.Collections.Generic;
	using UniRx.Async;
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

		public static UniTask DontWait( CancellationToken cancellationToken ) {
			return Delay( cancellationToken, 0 );
		}

		public static UniTask Yield( CancellationToken cancellationToken,
										PlayerLoopTiming timing = PlayerLoopTiming.Update
		) {
			return UniTask.Yield( timing, cancellationToken );
		}

		public static UniTask Delay( CancellationToken cancellationToken, int millisecondsDelay,
										bool ignoreTimeScale = false,
										PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			return UniTask.Delay( millisecondsDelay, ignoreTimeScale, delayTiming, cancellationToken );
		}
		public static UniTask Delay( CancellationToken cancellationToken, TimeSpan delayTimeSpan,
										bool ignoreTimeScale = false,
										PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			return UniTask.Delay( delayTimeSpan, ignoreTimeScale, delayTiming, cancellationToken );
		}

		public static UniTask<int> DelayFrame( CancellationToken cancellationToken, int delayFrameCount,
												PlayerLoopTiming delayTiming = PlayerLoopTiming.Update
		) {
			return UniTask.DelayFrame( delayFrameCount, delayTiming, cancellationToken );
		}

		public static UniTask WaitUntil( CancellationToken cancellationToken, Func<bool> predicate,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) {
			return UniTask.WaitUntil( predicate, timing, cancellationToken );
		}

		public static UniTask WaitWhile( CancellationToken cancellationToken, Func<bool> predicate,
											PlayerLoopTiming timing = PlayerLoopTiming.Update
		) {
			return UniTask.WaitWhile( predicate, timing, cancellationToken );
		}

		public static UniTask<U> WaitUntilValueChanged<T, U>( CancellationToken cancellationToken,
																T target, Func<T, U> monitorFunction,
													PlayerLoopTiming monitorTiming = PlayerLoopTiming.Update,
																IEqualityComparer<U> equalityComparer = null
		) where T : class {
			return UniTask.WaitUntilValueChanged(
				target, monitorFunction, monitorTiming, equalityComparer, cancellationToken
			);
		}

		public static UniTask ConfigureAwait( this AsyncOperation asyncOperation,
												CancellationToken cancellation,
												IProgress<float> progress = null,
												PlayerLoopTiming timing = PlayerLoopTiming.Update
		) {
			return asyncOperation.ConfigureAwait( progress, timing, cancellation );
		}
		public static UniTask<UnityEngine.Object> ConfigureAwait( this ResourceRequest resourceRequest,
																	CancellationToken cancellation,
																	IProgress<float> progress = null,
															PlayerLoopTiming timing = PlayerLoopTiming.Update
		) {
			return resourceRequest.ConfigureAwait( progress, timing, cancellation );
		}
		public static UniTask<UnityEngine.Object> ConfigureAwait( this AssetBundleRequest resourceRequest,
																	CancellationToken cancellation,
																	IProgress<float> progress = null,
															PlayerLoopTiming timing = PlayerLoopTiming.Update
        ) {
			return resourceRequest.ConfigureAwait( progress, timing, cancellation );
		}
		public static UniTask ConfigureAwait( this WWW www, CancellationToken cancellation,
												IProgress<float> progress = null,
												PlayerLoopTiming timing = PlayerLoopTiming.Update
        ) {
			return www.ConfigureAwait( progress, timing, cancellation );
		}
		public static UniTask<UnityWebRequest> ConfigureAwait(
															this UnityWebRequestAsyncOperation asyncOperation,
															CancellationToken cancellation,
															IProgress<float> progress = null,
															PlayerLoopTiming timing = PlayerLoopTiming.Update
        ) {
			return asyncOperation.ConfigureAwait( progress, timing, cancellation );
		}
		public static UniTask ConfigureAwait( this IEnumerator enumerator, CancellationToken cancellationToken,
												PlayerLoopTiming timing = PlayerLoopTiming.Update
        ) {
			return enumerator.ConfigureAwait( timing, cancellationToken );
		}
	}
}