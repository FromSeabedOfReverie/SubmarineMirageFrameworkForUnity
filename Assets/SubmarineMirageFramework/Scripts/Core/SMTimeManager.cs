//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System.Diagnostics;
	using UnityEngine;
	using UniRx;
	using Service;
	using Task;


	// TODO : コメント追加、整頓


	///====================================================================================================
	/// <summary>
	/// ■ 時間の管理クラス
	/// </summary>
	///====================================================================================================
	public class SMTimeManager : SMBehaviour, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		public override SMTaskType _type => SMTaskType.FirstWork;

		/// <summary>許容可能な、1フレームの最大更新秒数（これ以上更新に時間が掛かったら、強制補正）</summary>
		const float MAX_DELTA_TIME = 0.1f;

		/// <summary>時間加減速の影響を受けない、1フレームの更新秒数</summary>
		public float _unscaledDeltaTime	{ get; private set; }	// Time.unscaledDeltaTimeは値が大きく飛ぶので代用
		/// <summary>時間加減速の影響を受けない、アプリケーション起動時からの秒数</summary>
		public float _unscaledTime		{ get; private set; }	// Time.unscaledTimeは値が大きく飛ぶので代用
		/// <summary>処理時間計測</summary>
		readonly Stopwatch _stopwatch = new Stopwatch();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 作成
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Create() {
			var _lastUnscaledTime = 0f;

			// ● 更新
			_updateEvent.AddLast().Subscribe( _ => {
				_unscaledDeltaTime = Mathf.Min( MAX_DELTA_TIME, Time.unscaledDeltaTime );
				_unscaledTime += Mathf.Min( MAX_DELTA_TIME, Time.unscaledTime - _lastUnscaledTime );
				_lastUnscaledTime = Time.unscaledTime;
			} );
		}
		///------------------------------------------------------------------------------------------------
		/// ● 処理時間を計測
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 処理計測開始
		/// </summary>
		public void StartMeasure() {
			_stopwatch.Restart();
		}
		/// <summary>
		/// ● 処理計測終了
		///	 計測秒数が戻り値
		/// </summary>
		public float StopMeasure() {
			_stopwatch.Stop();
			return _stopwatch.ElapsedMilliseconds / 1000f;
		}
	}
}