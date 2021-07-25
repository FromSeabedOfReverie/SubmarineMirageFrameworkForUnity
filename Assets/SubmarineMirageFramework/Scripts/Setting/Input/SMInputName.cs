//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Setting {



	/// <summary>入力名</summary>
	public enum SMInputName {
		/// <summary>移動用のX軸</summary>
		MoveAxisX,
		/// <summary>移動用のY軸</summary>
		MoveAxisY,
		/// <summary>回転用のX軸</summary>
		RotateAxisX,
		/// <summary>回転用のY軸</summary>
		RotateAxisY,
		/// <summary>デバッグ用のX軸</summary>
		DebugAxisX,
		/// <summary>デバッグ用のY軸</summary>
		DebugAxisY,

		/// <summary>視点スクロール</summary>
		CameraScroll,
		/// <summary>視点回転</summary>
		CameraRotate,
		/// <summary>行動</summary>
		Action,
		/// <summary>歩行</summary>
		Walk,
		/// <summary>決定</summary>
		Decide,
		/// <summary>終了</summary>
		Quit,
		/// <summary>リセット</summary>
		Reset,
		/// <summary>デバッグ表示</summary>
		DebugView,
	}
}