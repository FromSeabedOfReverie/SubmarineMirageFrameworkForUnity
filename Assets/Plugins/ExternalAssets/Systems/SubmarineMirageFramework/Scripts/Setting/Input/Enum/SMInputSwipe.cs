//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {



	/// <summary>入力のスワイプ定数</summary>
	// キーパッド数字と方向を対応定義
	public enum SMInputSwipe {
		/// <summary>左下にスワイプ</summary>
		DownerLeft = 1,
		/// <summary>下にスワイプ</summary>
		Down,
		/// <summary>右下にスワイプ</summary>
		DownerRight,
		/// <summary>左にスワイプ</summary>
		Left,
		/// <summary>スワイプ無し</summary>
		None,   // キーパッド5が中央の為、この位置で定義
		/// <summary>右にスワイプ</summary>
		Right,
		/// <summary>左上にスワイプ</summary>
		UpperLeft,
		/// <summary>上にスワイプ</summary>
		Up,
		/// <summary>右上にスワイプ</summary>
		UpperRight,
	}
}