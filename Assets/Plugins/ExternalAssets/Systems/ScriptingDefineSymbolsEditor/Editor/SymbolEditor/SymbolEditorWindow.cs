//  DefineSymbolController.cs
//  http://kan-kikuchi.hatenablog.com/entry/ScriptingDefineSymbolsEditor
//
//  Created by kan kikuchi on 2015.09.30.

using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// DefineSymbolを操作する時に使うウィンドウ
/// </summary>
public class SymbolEditorWindow : EditorWindow {

	private Vector2 _scrollPosition = Vector2.zero;

	//=================================================================================
	//初期化
	//=================================================================================

	private void OnEnable (){
		SymbolEditor.Init ();
	}

	//=================================================================================
	//表示するGUIの設定
	//=================================================================================

	private void OnGUI(){

		if(EditorApplication.isPlaying || Application.isPlaying || EditorApplication.isCompiling){
			EditorGUILayout.HelpBox("コンパイル中、実行中は変更できません", MessageType.Warning);
			return;
		}

		//変更があり、セーブしていない時は上に注意を表示
		if(SymbolEditor.IsEdited){
			EditorGUILayout.HelpBox("保存されていない情報があります", MessageType.Warning);
		}
		else{
			EditorGUILayout.HelpBox ("保存済みです", MessageType.Info);
		}

		_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUI.skin.scrollView);
		EditorGUILayout.PrefixLabel("Current Target : " + SymbolEditor.TargetGroup.ToString());

		//シンボル一覧表示
		EditorGUILayout.BeginVertical( GUI.skin.box );
		{
			EditorGUILayout.PrefixLabel("Scripting Define Symbols");
			CreateSymbolMenu ();
		}
		EditorGUILayout.EndVertical();

		GUILayout.Space (20);

		//Saveメニューを並べる
		EditorGUILayout.BeginVertical( GUI.skin.box );
		{
			EditorGUILayout.PrefixLabel("Save");
			CreateSaveMenu ();
		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.EndScrollView();
	}

	//シンボルのメニューを作成
	private void CreateSymbolMenu(){

		//シンボル分メニュー作成
		List<DefineSymbol> symbolList = SymbolEditor.SymbolList;
		for (int i = 0; i < symbolList.Count; i++) {
			CreateSymbolMenuParts (symbolList [i], i);
			GUILayout.Space (5);
		}

		//新規に追加できるよう、空白のメニューを追加
		DefineSymbol newSymbol = new DefineSymbol ("", false);
		CreateSymbolMenuParts (newSymbol, symbolList.Count);

		GUILayout.Space (10);

		//保存されている状態に戻すボタン
		if(GUILayout.Button ("Reset")){
			SymbolEditor.Init();
		}

		GUILayout.Space (10);

		//全部無効にするボタン
		if(GUILayout.Button ("All Invalid")){
			SymbolEditor.SetAllEnabled(false);
		}

		//全部有効にするボタン
		if(GUILayout.Button ("All Valid")){
			SymbolEditor.SetAllEnabled(true);
		}

		GUILayout.Space (10);

		//全部削除するボタン
		if(GUILayout.Button ("All Delete")){
			SymbolEditor.AllDelete();
		}
	}

	//各シンボルのメニューを作成
	private void CreateSymbolMenuParts(DefineSymbol symbol, int symbolNo){

		//有効になっているかどうかでスキンを変える
		EditorGUILayout.BeginVertical(symbol.IsEnabled ? GUI.skin.button : GUI.skin.textField);
		{

			//内容の変更チェック開始
			EditorGUI.BeginChangeCheck ();

			string symbolKey = symbol.Key;
			bool isEnabled   = symbol.IsEnabled;

			EditorGUILayout.BeginHorizontal ();
			{
				//内容の変更チェック開始
				EditorGUI.BeginChangeCheck ();

				//チェックボックス作成
				isEnabled = EditorGUILayout.Toggle (isEnabled, GUILayout.Width(15));

				//シンボル名
				EditorGUILayout.LabelField("Symbol", GUILayout.Width(45));
				symbolKey = GUILayout.TextField(symbolKey);

				//最後の新規入力欄以外は削除ボタンを表示
				if(symbolNo < SymbolEditor.SymbolList.Count){
					if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(14))){
						SymbolEditor.Delete (symbolNo);
						return;
					}
				}

			}
			EditorGUILayout.EndHorizontal();

			EditorGUI.indentLevel = 2;

			//シンボルに対応する値
			string symbolValue = symbol.Value;
			EditorGUILayout.BeginHorizontal ();
			{
				EditorGUILayout.LabelField("Value", GUILayout.Width(64));
				symbolValue = GUILayout.TextField(symbolValue);
			}
			EditorGUILayout.EndHorizontal();

			//シンボルの説明
			string symbolInfo = symbol.Info;
			EditorGUILayout.BeginHorizontal ();
			{
				EditorGUILayout.LabelField("Info", GUILayout.Width(64));
				symbolInfo = GUILayout.TextField(symbolInfo);
			}
			EditorGUILayout.EndHorizontal();

			//内容が変更されていれば、シンボルを編集
			if (EditorGUI.EndChangeCheck ()){
				SymbolEditor.EditSymbol (symbolNo, symbolKey, symbolValue, symbolInfo, isEnabled);
			}

		}
		EditorGUILayout.EndVertical();

		EditorGUI.indentLevel = 0;
	}

	//セーブメニュー作成
	private void CreateSaveMenu(){
		if(SymbolEditor.IsDuplicateSymbolKey){
			EditorGUILayout.HelpBox("Symbolが重複しているので保存できません。", MessageType.Warning);
			return;
		}

		if(GUILayout.Button ("Current Target")){
			SymbolEditor.Save (SymbolEditor.TargetGroup);
		}
		if(GUILayout.Button ("All Target")){
			SymbolEditor.SaveAll();
		}
	}

}
