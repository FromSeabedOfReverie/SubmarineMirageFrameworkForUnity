//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Data.Server {
	using File;
	///====================================================================================================
	/// <summary>
	/// ■ 製品版の宣伝情報の管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class CMDataManager : CSVDataManager<CMData.Platform, CMData> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public CMDataManager() : base (
			"https://docs.google.com/spreadsheets/d/e/2PACX-1vQZjl0KQ3qdx1ghjDLczrLpmWQ11Ao75IdaSobLMoFHjuzhG4pTCX0bXvZgGl_P4-2fjLCdCbBKHaRE/pub?gid=1475705626&single=true&output=csv",
			"",
			FileLoader.Type.Server, 1 )
		{
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public CMData Get() {
			return Get(
#if UNITY_ANDROID
				CMData.Platform.Android
#elif UNITY_STANDALONE_WIN || UNITY_WSA
				CMData.Platform.Windows
#elif UNITY_IOS
				CMData.Platform.IOS
#elif UNITY_STANDALONE_OSX
				CMData.Platform.MacOSX
#elif UNITY_STANDALONE_LINUX
				CMData.Platform.Linux
#endif
			);
		}
	}
}