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
	/// ■ 他アプリケーション宣伝情報の管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class ApplicationCMDataManager : CSVDataManager<int, ApplicationCMData> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public ApplicationCMDataManager() : base (
			"https://docs.google.com/spreadsheets/d/e/2PACX-1vQZjl0KQ3qdx1ghjDLczrLpmWQ11Ao75IdaSobLMoFHjuzhG4pTCX0bXvZgGl_P4-2fjLCdCbBKHaRE/pub?gid=1057403382&single=true&output=csv",
			"",
			FileLoader.Type.Server, 1 )
		{
		}
	}
}