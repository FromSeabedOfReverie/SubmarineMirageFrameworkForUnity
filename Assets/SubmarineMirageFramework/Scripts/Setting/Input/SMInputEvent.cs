//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Setting {



	/// <summary>入力のイベント関数定数</summary>
	public enum SMInputEvent {
		/// <summary>決定キー</summary>
		Decide,
		/// <summary>終了キー</summary>
		Quit,
		/// <summary>再設定キー</summary>
		Reset,
		/// <summary>デバッグ表示</summary>
		DebugView,
		/// <summary>無に触れた</summary>
		Nothing,
		/// <summary>無に長時間触れた</summary>
		LongNothing,
		/// <summary>何らかの操作</summary>
		AnyOperation,
	}
}