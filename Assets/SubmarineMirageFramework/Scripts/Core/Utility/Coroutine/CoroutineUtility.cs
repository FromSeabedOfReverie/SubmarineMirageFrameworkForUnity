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
		///		CoroutineProcessクラスを簡単に呼べる。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static CoroutineProcess Start( IEnumerator coroutine, Action onCompleted = null ) {
			return new CoroutineProcess( coroutine, onCompleted, true, true );
		}
	}
}