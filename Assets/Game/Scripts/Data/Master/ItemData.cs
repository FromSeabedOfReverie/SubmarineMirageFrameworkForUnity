//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using KoganeUnityLib;
using SubmarineMirage;
using SubmarineMirage.Data;
///========================================================================================================
/// <summary>
/// ■ アイテムの情報クラス
/// </summary>
///========================================================================================================
public class ItemData : SMCSVData<string> {
	///----------------------------------------------------------------------------------------------------
	/// ● 要素
	///----------------------------------------------------------------------------------------------------
	/// <summary>辞書への登録鍵</summary>
	public override string _registerKey => _name;

	/// <summary>番号</summary>
	[SMShow] public int _itemID			{ get; private set; }
	/// <summary>オブジェクト名</summary>
	[SMShow] public string _objectName	{ get; private set; }
	/// <summary>名前</summary>
	[SMShow] public string _name		{ get; private set; }
	/// <summary>消費値</summary>
	[SMShow] public int _consumption	{ get; private set; }
	/// <summary>説明文</summary>
	[SMShow] public string _explanation	{ get; private set; }

	///----------------------------------------------------------------------------------------------------
	/// ● 作成、削除
	///----------------------------------------------------------------------------------------------------
	/// <summary>
	/// ● 設定
	/// </summary>
	public override void Setup( string fileName, int index, List<string> texts ) {
		_itemID			= index;
		_objectName		= texts[0];
		_name			= texts[1];
		_consumption	= texts[2].ToInt();
		_explanation	= texts[3];
	}
}