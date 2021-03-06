//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Data {
	using System.Collections.Generic;
	///====================================================================================================
	/// <summary>
	/// ■ AI命令情報の基盤クラス
	///----------------------------------------------------------------------------------------------------
	///		様々なAI命令クラスに、継承される。
	/// </summary>
	///====================================================================================================
	public abstract class AICommandData : BaseData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public AICommandData( List<string> commands ) {
		}
		/// <summary>
		/// ● コンストラクタ
		///		デバッグ表示時の、文字列変換で必要
		/// </summary>
		public AICommandData() {
		}
	}
}