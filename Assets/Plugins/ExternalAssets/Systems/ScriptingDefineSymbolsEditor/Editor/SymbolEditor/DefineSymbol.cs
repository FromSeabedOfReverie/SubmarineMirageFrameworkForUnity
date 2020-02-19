//  DefineSymbol.cs
//  http://kan-kikuchi.hatenablog.com/entry/ScriptingDefineSymbolsEditor
//
//  Created by kan kikuchi on 2015.09.30.

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// Symbol名、対応する値、説明を持ったSymbolのクラス
/// </summary>
public class DefineSymbol {

	//シンボルのkey
	private string _key;
	public  string  Key{
		get{return _key;}
	}

	//シンボルに対応した値
	private string _value = "";
	public  string  Value{
		get{return _value;}
	}

	//シンボルが有効か
	private bool _isEnabled = true;
	public  bool  IsEnabled{
		get{return _isEnabled;}
	}

	//シンボルの説明
	private string _info = "";
	public  string  Info{
		get{return _info;}
	}

	//保存する時のkeyの形式
	private const string SYMBOL_VALUE_SAVE_KEY_FORMAT = "SymbolValueKeyFor{0}";
	private const string SYMBOL_INFO_SAVE_KEY_FORMAT  = "SymbolInfoKeyFor{0}";

	//=================================================================================
	//初期化
	//=================================================================================

	/// <summary>
	/// keyと有効かどうか指定して初期化する
	/// </summary>
	public DefineSymbol (string key, bool isEnabled){

		_key       = key;
		_isEnabled = isEnabled;

		//シンボルに対応した値を取得
		_value = EditorUserSettings.GetConfigValue (GetSymbolValueSaveKey(_key));
		if(string.IsNullOrEmpty(_value)){
			_value = "";
		}

		//シンボルの説明を取得
		_info  = EditorUserSettings.GetConfigValue (GetSymbolInfoSaveKey(_key));
		if(string.IsNullOrEmpty(_info)){
			_info = "";
		}
	}

	//=================================================================================
	//内部
	//=================================================================================

	//シンボルに対応した値を保存する時に使うkeyを取得
	private string GetSymbolValueSaveKey(string symbol){
		return string.Format (SYMBOL_VALUE_SAVE_KEY_FORMAT, symbol);
	}

	//シンボルの説明を保存する時に使うkeyを取得
	private string GetSymbolInfoSaveKey(string symbol){
		return string.Format (SYMBOL_INFO_SAVE_KEY_FORMAT, symbol);
	}

	//シンボル名を半角英数字+アンダースコア以外を除き、大文字で設定
	private static  string ModifyKey(string symbol){
		Regex regex = new Regex(@"[^_a-zA-Z0-9]");
		symbol = regex.Replace(symbol, "");
		return symbol.ToUpper ();
	}

	//=================================================================================
	//変更
	//=================================================================================

	/// <summary>
	/// シンボルに対応した値と情報を削除する
	/// </summary>
	public void DeleteValueAndInfo(){
		//シンボルに対応する値を削除
		_value = "";
		EditorUserSettings.SetConfigValue (GetSymbolValueSaveKey(_key), "");

		//シンボルの説明を削除
		_info = "";
		EditorUserSettings.SetConfigValue (GetSymbolInfoSaveKey(_key), "");
	}

	/// <summary>
	/// シンボルの設定を編集する
	/// </summary>
	public void Edit(string key, string symbolValue, string symbolInfo, bool isEnabled){
		_key       = ModifyKey (key);
		_value     = symbolValue;
		_info      = symbolInfo;
		_isEnabled = isEnabled;
	}

	/// <summary>
	/// シンボルの設定を編集する
	/// </summary>
	public void Edit(bool isEnabled){
		_isEnabled = isEnabled;
	}

	//=================================================================================
	//保存
	//=================================================================================

	/// <summary>
	/// シンボルの設定を保存する
	/// </summary>
	public void Save(){
		//シンボルに対応する値を保存
		EditorUserSettings.SetConfigValue (GetSymbolValueSaveKey(_key), _value);

		//シンボルの説明を保存
		EditorUserSettings.SetConfigValue (GetSymbolInfoSaveKey(_key), _info);
	}

}
