//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using UnityEngine;
	using UniRx;
	using Singleton;
	///====================================================================================================
	/// <summary>
	/// ■ 時間の管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class TimeManager : Singleton<TimeManager> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>許容可能な、1フレームの最大更新秒数（これ以上更新に時間が掛かったら、強制補正）</summary>
		const float MAX_DELTA_TIME = 0.1f;

		/// <summary>時間加減速の影響を受けない、1フレームの更新秒数</summary>
		public float _unscaledDeltaTime	{ get; private set; }	// Time.unscaledDeltaTimeは値が大きく飛ぶので代用
		/// <summary>時間加減速の影響を受けない、アプリケーション起動時からの秒数</summary>
		public float _unscaledTime		{ get; private set; }	// Time.unscaledTimeは値が大きく飛ぶので代用
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public TimeManager() {
			var _lastUnscaledTime = 0f;

			// ● 更新
			_updateEvent.Subscribe( _ => {
				_unscaledDeltaTime = Mathf.Min( MAX_DELTA_TIME, Time.unscaledDeltaTime );
				_unscaledTime += Mathf.Min( MAX_DELTA_TIME, Time.unscaledTime - _lastUnscaledTime );
				_lastUnscaledTime = Time.unscaledTime;
			} );
		}
	}
}