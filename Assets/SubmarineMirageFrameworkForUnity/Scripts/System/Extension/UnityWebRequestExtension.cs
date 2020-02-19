//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Extension {
	using UnityEngine.Networking;
	///====================================================================================================
	/// <summary>
	/// ■ Unity配信の拡張クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public static class UnityWebRequestExtension {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 成功か？
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static bool IsSuccess( this UnityWebRequest self ) {
			return !self.isHttpError && !self.isNetworkError;
		}
	}
}