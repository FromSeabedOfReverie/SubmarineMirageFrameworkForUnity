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
	/// ■ アイテムの情報クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class ItemData : CSVData<string> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>辞書への登録鍵</summary>
		public override string _registerKey => _name;

		/// <summary>番号</summary>
		public int _id;
		/// <summary>オブジェクト名</summary>
		public string _objectName;
		/// <summary>名前</summary>
		public string _name;
		/// <summary>消費値</summary>
		public int _consumption;
		/// <summary>説明文</summary>
		public string _explanation;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Set( string fileName, int index, List<string> texts ) {
			_id				= index;
			_objectName		= texts[0];
			_name			= texts[1];
			_consumption	= texts[2].ToInt();
			_explanation	= texts[3];
		}
	}
}