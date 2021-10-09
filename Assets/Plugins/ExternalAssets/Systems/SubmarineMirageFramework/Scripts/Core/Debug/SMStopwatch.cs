//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Debug {
	using System.Diagnostics;
	using Base;
	///====================================================================================================
	/// <summary>
	/// ■ 処理時間の計測クラス
	/// </summary>
	///====================================================================================================
	public class SMStopwatch : SMStandardBase {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>処理時間計測</summary>
		readonly Stopwatch _stopwatch = new Stopwatch();
		/// <summary>計測秒数</summary>
		[SMShowLine] public float _seconds	{ get; private set; }

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public SMStopwatch() {
			_disposables.AddFirst( () => {
				Stop();
				_seconds = 0;
			} );
		}
		///------------------------------------------------------------------------------------------------
		/// ● 計測
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 計測開始
		/// </summary>
		public void Start()
			=> _stopwatch.Restart();

		/// <summary>
		/// ● 計測終了
		///	 計測秒数を戻す。
		/// </summary>
		public float Stop() {
			_stopwatch.Stop();
			_seconds = _stopwatch.ElapsedMilliseconds / 1000f;
			return _seconds;
		}
	}
}