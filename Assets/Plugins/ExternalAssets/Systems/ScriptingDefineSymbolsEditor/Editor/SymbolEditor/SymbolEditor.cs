//  SymbolEditor.cs
//  http://kan-kikuchi.hatenablog.com/entry/ScriptingDefineSymbolsEditor
//
//  Created by kan kikuchi on 2015.09.30.

using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// DefineSymbolを操作するクラス
/// </summary>
public static class SymbolEditor {

	//シンボルを区切る文字
	private const string SYMBOL_SEPARATOR = ";";

	//シンボル名一覧を保存する時のkey
	private const string SYMBOL_KEY_LIST_SAVE_KEY = "SymbolKeyListKey";

	//シンボル一覧
	private static List<DefineSymbol> _symbolList;
	public  static List<DefineSymbol>  SymbolList{
		get{
			if(_symbolList == null){
				Init ();
			}
			return _symbolList;
		}
	}

	//現在選ばれてるプラットフォーム
	private static BuildTargetGroup _targetGroup;
	public  static BuildTargetGroup  TargetGroup{
		get{return _targetGroup;}
	}

	//シンボルを編集したか
	private static bool _isEdited = false;
	public  static bool  IsEdited{
		get{return _isEdited;}
	}

	//重複したシンボルのkeyがあるか
	public static bool IsDuplicateSymbolKey{
		get{return _symbolList.Select (symbol => symbol.Key).Distinct().Count() != _symbolList.Count;}
	}

	//=================================================================================
	//初期化
	//=================================================================================

	//メニューからウィンドウを表示
	[MenuItem("Tools/Open/SymbolEditorWindow")]
	public static void Open (){
		SymbolEditorWindow.GetWindow (typeof(SymbolEditorWindow));
	}

	/// <summary>
	/// 初期化
	/// </summary>
	public static void Init(){
		_isEdited = false;

		//現在選ばれてるプラットフォームを取得
		_targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

		//シンボルのkeyをまとめるリスト作成
		List<string> symbolKeyList = new List<string>();

		//現在のプラットフォームに設定されてるシンボル名を取得し、リストに追加
		string[] settingSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup (_targetGroup).Split(new []{SYMBOL_SEPARATOR}, StringSplitOptions.None);
		symbolKeyList.AddRange (settingSymbols.ToList());

		//EditorUserSettingsに保存されているシンボル名一覧があれば、リストに追加
		string savingSymbolKeys = EditorUserSettings.GetConfigValue (SYMBOL_KEY_LIST_SAVE_KEY);

		List<string> savingSymbolKeyList = new List<string> ();;
		if(!string.IsNullOrEmpty(savingSymbolKeys)){
			savingSymbolKeyList = new List<string> (savingSymbolKeys.Split(new []{SYMBOL_SEPARATOR}, StringSplitOptions.None));
		}

		symbolKeyList.AddRange (savingSymbolKeyList);

		//空白及び重複しているシンボル名を除き、アルファベット順にソート
		symbolKeyList = symbolKeyList.Where(symbolKey => !string.IsNullOrEmpty(symbolKey)).Distinct().ToList();
		symbolKeyList.Sort ();

		//各シンボルkeyからシンボルを作成
		_symbolList = new List<DefineSymbol> ();
		foreach (string symbolKey in symbolKeyList) {

			bool isEnabled = settingSymbols.Contains (symbolKey);
			DefineSymbol symbol = new DefineSymbol (symbolKey, isEnabled);

			//EditorUserSettingsに保存されていなかったkeyは前のValueとInfoが残っている可能性があるので削除
			if(!savingSymbolKeyList.Contains(symbolKey)){
				symbol.DeleteValueAndInfo ();
			}

			_symbolList.Add (symbol);
		}

		//有効になっているシンボルを上に表示するように
		_symbolList = _symbolList.OrderBy (symbol => symbol.IsEnabled ? 0 : 1).ToList();

	}

	//=================================================================================
	//シンボル修正
	//=================================================================================

	/// <summary>
	/// シンボルを修正
	/// </summary>
	public static void EditSymbol(int symbolNo, string symbolKey, string symbolValue, string symbolInfo, bool isEnabled){
		_isEdited = true;

		//新たなシンボル作成
		if(symbolNo >= _symbolList.Count){
			DefineSymbol newSymbol = new DefineSymbol (symbolKey, isEnabled);
			_symbolList.Add (newSymbol);
		}

		DefineSymbol symbol = _symbolList[symbolNo];
		symbol.Edit (symbolKey, symbolValue, symbolInfo, isEnabled);
	}

	/// <summary>
	/// 全てのシンボルを有効or無効にする
	/// </summary>
	public static void SetAllEnabled(bool isEnabled){
		_isEdited = true;

		foreach (DefineSymbol symbol in _symbolList) {
			symbol.Edit (isEnabled);
		}
	}

	/// <summary>
	/// 全てのシンボルを削除する
	/// </summary>
	public static void AllDelete(){
		_isEdited = true;

		_symbolList.Clear ();
	}

	/// <summary>
	/// シンボルを削除する
	/// </summary>
	public static void Delete(int symbolNo){
		if(symbolNo >= _symbolList.Count){
			return;
		}
		_isEdited = true;

		_symbolList [symbolNo].DeleteValueAndInfo ();
		_symbolList.RemoveAt (symbolNo);
	}

	//=================================================================================
	//シンボルの保存
	//=================================================================================

	/// <summary>
	/// 指定したプラットフォームにシンボルを保存
	/// </summary>
	public static void Save(BuildTargetGroup targetGroup, bool needToCreateDefineValue = true){
		_isEdited = false;

		//シンボルのkeyを空白を無視かつ重複しないようにに取得
		List<string> symbolKeyList = _symbolList
			.Select(symbol => symbol.Key).Where (symbolKey => !string.IsNullOrEmpty(symbolKey))
			.Distinct().ToList ();

		//シンボルを一つの文字列にまとめ、EditorUserSettingsに保存
		EditorUserSettings.SetConfigValue (SYMBOL_KEY_LIST_SAVE_KEY, string.Join(SYMBOL_SEPARATOR ,symbolKeyList.ToArray()));

		//各シンボルの対応した設定を保存
		string enabledSymbols = "";
		Dictionary<string, string> _defineValueDic = new Dictionary<string, string>();

		foreach (DefineSymbol symbol in _symbolList) {

			symbol.Save ();

			//valueが設定されている場合は定数クラスに書き出すためにDictにまとめる
			if(needToCreateDefineValue && !string.IsNullOrEmpty(symbol.Value)){
				_defineValueDic[symbol.Key] = symbol.Value;
			}

			//有効になっているシンボルは設定するように;区切りでenabledSymbolsにまとめる
			if(symbol.IsEnabled){

				if(!string.IsNullOrEmpty(enabledSymbols)){
					enabledSymbols += SYMBOL_SEPARATOR;
				}
				enabledSymbols += symbol.Key;
			}

		}
		 
		//設定するグループが不明だとエラーがでるので設定しないように
		if(targetGroup != BuildTargetGroup.Unknown){
			PlayerSettings.SetScriptingDefineSymbolsForGroup (targetGroup, enabledSymbols);
		}

		//Symbolに対応した値を定数クラス、DefineValueを書き出す
		if(needToCreateDefineValue){
			ConstantsClassCreator.Create<string>("DefineValue", "Symbolに対応した値を定数で管理するクラス", _defineValueDic);
		}

	}

	/// <summary>
	/// 全プラットフォームに同じシンボルを保存
	/// </summary>
    public static void SaveAll(){

        //Symbolに対応した値をまとめたDefineValueを作成するかのフラグ
        bool needToCreateDefineValue = true;

        foreach (BuildTargetGroup buildTarget in Enum.GetValues(typeof(BuildTargetGroup))){

            if (!IsObsolete(buildTarget)){
                Save(buildTarget, needToCreateDefineValue);

                //一度書き出したら、DefineValueを再度書き出さないように
                needToCreateDefineValue = false;
            }
        }
    }

    public static bool IsObsolete(Enum value){
        var fi = value.GetType().GetField(value.ToString());
        var attributes = (ObsoleteAttribute[])
            fi.GetCustomAttributes(typeof(ObsoleteAttribute), false);
        return (attributes != null && attributes.Length > 0);
    }
}
