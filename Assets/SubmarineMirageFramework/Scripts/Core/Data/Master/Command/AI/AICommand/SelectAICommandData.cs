//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Data {
	using System.Collections.Generic;
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ 選択会話のAI命令情報クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class SelectAICommandData : AICommandData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>文章</summary>
		public string _text;
		/// <summary>同意した際の、遷移先の節名</summary>
		public string _yesNodeName;
		/// <summary>否定した際の、遷移先の節名</summary>
		public string _noNodeName;
		/// <summary>無視した際の、遷移先の節名</summary>
		public string _ignoreNodeName;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public SelectAICommandData( List<string> commands ) : base( commands ) {
			_text = commands[0];
			_yesNodeName = commands[1];
			_noNodeName = commands[2];

			// 未設定の場合、否定節名を読込
			_ignoreNodeName = commands[ Mathf.Min( commands.Count, 3 ) ];
		}
	}
}