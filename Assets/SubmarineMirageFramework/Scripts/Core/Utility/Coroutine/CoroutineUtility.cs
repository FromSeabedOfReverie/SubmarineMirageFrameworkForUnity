//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Utility {
	using System;
	using System.Collections;
	///====================================================================================================
	/// <summary>
	/// ■ コルーチンの便利クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public static class CoroutineUtility {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 開始
		///		コルーチンの仕事クラスを簡単に呼べる。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static CoroutineTask Start( IEnumerator coroutine, Action onCompleted = null ) {
			return new CoroutineTask( coroutine, onCompleted, true, true );
		}
	}
}