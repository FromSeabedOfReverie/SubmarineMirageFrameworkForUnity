//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Data {
	using System;
	using System.Collections.Generic;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 結果の情報クラス
	///----------------------------------------------------------------------------------------------------
	///		様々なプログラム処理の実行結果の情報を保存。
	///		失敗、成功、通知等に使用。
	/// </summary>
	///====================================================================================================
	public class ResultData<T> : CSVData<T> where T : struct, Enum {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>辞書への登録鍵</summary>
		public override T _registerKey => _type;

		/// <summary>型</summary>
		public T _type { get; private set; }
		/// <summary>文章</summary>
		public string _text { get; private set; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Set( string fileName, int index, List<string> texts ) {
			_type = texts[0].ToEnum<T>();
			_text = texts[1];
		}
	}
}