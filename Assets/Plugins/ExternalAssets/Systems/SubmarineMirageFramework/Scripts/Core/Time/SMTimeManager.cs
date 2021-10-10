//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using UnityEngine;
	using UniRx;
	///====================================================================================================
	/// <summary>
	/// ■ 時間の管理クラス
	/// </summary>
	///====================================================================================================
	public class SMTimeManager : SMTask, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>実行型</summary>
		public override SMTaskRunType _type => SMTaskRunType.Sequential;

		/// <summary>許容可能な、1フレームの最大更新秒数（これ以上更新に時間が掛かったら、強制補正）</summary>
		const float MAX_DELTA_TIME = 0.1f;

		/// <summary>時間加減速の影響を受けない、1フレームの更新秒数</summary>
		public float _unscaledDeltaTime	{ get; private set; }	// Time.unscaledDeltaTimeは値が大きく飛ぶので代用
		/// <summary>時間加減速の影響を受けない、アプリケーション起動時からの秒数</summary>
		public float _unscaledTime		{ get; private set; }	// Time.unscaledTimeは値が大きく飛ぶので代用
		/// <summary>処理時間計測</summary>
		readonly SMStopwatch _stopwatch = new SMStopwatch();
		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 作成
		/// </summary>
		public override void Create() {
			var _lastUnscaledTime = 0f;

			// ● 更新
			_updateEvent.AddLast().Subscribe( _ => {
				_unscaledDeltaTime = Mathf.Min( MAX_DELTA_TIME, Time.unscaledDeltaTime );
				_unscaledTime += Mathf.Min( MAX_DELTA_TIME, Time.unscaledTime - _lastUnscaledTime );
				_lastUnscaledTime = Time.unscaledTime;
			} );

			_disposables.AddFirst( () => {
				_stopwatch.Dispose();
			} );
		}
		///------------------------------------------------------------------------------------------------
		/// ● 処理時間を計測
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 処理計測開始
		/// </summary>
		public void StartMeasure() => _stopwatch.Start();
		/// <summary>
		/// ● 処理計測終了
		///	 計測秒数を戻す。
		/// </summary>
		public float StopMeasure() => _stopwatch.Stop();
	}
}