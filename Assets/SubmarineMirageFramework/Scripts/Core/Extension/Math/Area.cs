//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ 範囲のクラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class Area {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>範囲の開始位置</summary>
		public Vector2 _startPosition;
		/// <summary>範囲の終了位置</summary>
		public Vector2 _endPosition;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public Area( Vector2 startPosition, Vector2 endPosition ) {
			_startPosition = startPosition;
			_endPosition = endPosition;
		}
		///------------------------------------------------------------------------------------------------
		/// ● 範囲内か？
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 範囲内か？
		/// </summary>
		public bool IsInside( Vector2 position ) {
			return (
				_startPosition.IsLess( position ) &&
				position.IsLess( _endPosition )
			);
		}
		/// <summary>
		/// ● 範囲内か？（bool配列を返す）
		/// </summary>
		public bool IsInside( Vector2 position, out bool[] isDetails ) {
			var isStartGreaters = _startPosition.IsLessDetails( position );
			var isEndLesses = position.IsLessDetails( _endPosition );

			isDetails = new bool[] {
				isStartGreaters[0] && isEndLesses[0],
				isStartGreaters[1] && isEndLesses[1]
			};

			return isDetails[0] && isDetails[1];
		}
		///------------------------------------------------------------------------------------------------
		/// ● 範囲外か？
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 範囲外か？
		/// </summary>
		public bool IsOutside( Vector2 position ) {
			return !IsInside( position );
		}
		/// <summary>
		/// ● 範囲外か？（bool配列を返す）
		/// </summary>
		public bool IsOutside( Vector2 position, out bool[] isDetails ) {
			var isInside = IsInside( position, out isDetails );
			isDetails[0] = !isDetails[0];
			isDetails[1] = !isDetails[1];

			return !isInside;
		}
	}
}