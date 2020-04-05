//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using System.Collections.Generic;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 自動実行のAI命令情報クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class AutoAICommandData : AICommandData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>自動実行か？</summary>
		public bool _isAuto;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public AutoAICommandData( List<string> commands ) : base( commands ) {
			_isAuto = commands[0].ToBooleanOrDefault();
		}
	}
}