//  ConstantsClassCreator.cs
//  http://kan-kikuchi.hatenablog.com/entry/ScriptingDefineSymbolsEditor
//
//  Created by kan kikuchi on 2015.09.30.

using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

/// <summary>
/// 定数を管理するクラスを生成するクラス
/// </summary>
public static class ConstantsClassCreator{

	//無効な文字を管理する配列
	private static readonly string[] INVALUD_CHARS = {
		" ", "!", "\"", "#", "$",
		"%", "&", "\'", "(", ")",
		"-", "=", "^",  "~", "\\",
		"|", "[", "{",  "@", "`",
		"]", "}", ":",  "*", ";",
		"+", "/", "?",  ".", ">",
		",", "<"
	};

	//定数の区切り文字
	private const char DELIMITER = '_';

	//スクリプトの拡張子
	private const string SCRIPT_EXTENSION = ".cs";

	//作成したスクリプトを書き出す先
	private const string EXPORT_DIRECTORY_PATH = "Assets/Scripts/Constants/";

	//型名
	private const string STRING_NAME = "string";
	private const string INT_NAME    = "int";
	private const string FLOAT_NAME  = "float";

	/// <summary>
	/// 定数を管理するクラスを自動生成する
	/// </summary>
	/// <param name="className">クラスの名前</param>
	/// <param name="classInfo">なんのクラスか説明するコメント文</param>
	/// <param name="variableDic">定数名とその値をセットで登録したDictionary</param>
	/// <typeparam name="T">定数の型、stringかintかfloat</typeparam>
	public static void Create<T> (string className, string classInfo, Dictionary<string, T> valueDict){
		//入力された型の判定
		string typeName = null;

		if(typeof(T) == typeof(string)){
			typeName = STRING_NAME;
		}
		else if(typeof(T) == typeof(int)){
			typeName = INT_NAME;
		}
		else if(typeof(T) == typeof(float)){
			typeName = FLOAT_NAME;
		}
		else{
			Debug.Log (className + SCRIPT_EXTENSION +"の作成に失敗しました.想定外の型" + typeof(T).Name  + "が入力されました");
			return;
		}

		//ディクショナリーをソートしたものに
		SortedDictionary<string, T> sortDict = new SortedDictionary<string, T> (valueDict);

		//入力された辞書のkeyから無効な文字列を削除して、大文字に_を設定した定数名と同じものに変更し新たな辞書に登録
		//次の定数の最大長求めるところで、_を含めたものを取得したいので、先に実行
		Dictionary<string, T> newValueDict = new Dictionary<string, T> ();

		foreach (KeyValuePair<string, T> valuePair in sortDict) {
			string newKey = RemoveInvalidChars(valuePair.Key);
			newKey = SetDelimiterBeforeUppercase(newKey);
			newValueDict [newKey] = valuePair.Value;
		}

		//定数名の最大長を取得し、空白数を決定
		int keyLengthMax = 0;
		if(newValueDict.Count > 0){
			keyLengthMax = 1 + newValueDict.Keys.Select (key => key.Length).Max ();
		}

		//コメント文とクラス名を入力
		StringBuilder builder = new StringBuilder ();

		builder.AppendLine ("/// <summary>");
		builder.AppendFormat ("/// {0}", classInfo).AppendLine ();
		builder.AppendLine ("/// </summary>");
		builder.AppendFormat ("public static class {0}", className).AppendLine ("{").AppendLine ();

		//入力された定数とその値のペアを書き出していく
		string[] keyArray = newValueDict.Keys.ToArray();
		foreach (string key in keyArray) {

			if (string.IsNullOrEmpty (key)) {
				continue;
			}
			//数字だけのkeyだったらスルー
			else if (System.Text.RegularExpressions.Regex.IsMatch(key ,@"^[0-9]+$")){
				continue;
			}
			//keyに半英数字と_以外が含まれていたらスルー
			else if (!System.Text.RegularExpressions.Regex.IsMatch(key, @"^[_a-zA-Z0-9]+$")){
				continue;
			}

			//イコールが並ぶ用に空白を調整する
			string EqualStr = String.Format("{0, " + (keyLengthMax - key.Length).ToString() + "}", "=");

			//上記で判定した型と定数名を入力
			builder.Append ("\t").AppendFormat (@"public const {0} {1} {2} ", typeName, key, EqualStr);

			//Tがstringの場合は値の前後に"を付ける
			if (typeName == STRING_NAME) {
				builder.AppendFormat (@"""{0}"";", newValueDict[key]).AppendLine ();
			} 

			//Tがfloatの場合は値の後にfを付ける
			else if (typeName == FLOAT_NAME) {
				builder.AppendFormat (@"{0}f;", newValueDict[key]).AppendLine ();
			}

			else {
				builder.AppendFormat (@"{0};", newValueDict[key]).AppendLine ();
			}

		}

		builder.AppendLine().AppendLine ("}");

		//書き出し、ファイル名はクラス名.cs
		string exportPath = EXPORT_DIRECTORY_PATH + className + SCRIPT_EXTENSION;

		//書き出し先のディレクトリが無ければ作成
		string directoryName = Path.GetDirectoryName (exportPath);
		if (!Directory.Exists (directoryName)) {
			Directory.CreateDirectory(directoryName);
		}

		File.WriteAllText (exportPath, builder.ToString (), Encoding.UTF8);
		AssetDatabase.Refresh (ImportAssetOptions.ImportRecursive);

		Debug.Log (className + SCRIPT_EXTENSION + "の作成が完了しました");
	}


	/// <summary>
	/// 無効な文字を削除します
	/// </summary>
	private static string RemoveInvalidChars(string str){
		Array.ForEach(INVALUD_CHARS, c => str = str.Replace(c, string.Empty));
		return str;
	}

	/// <summary>
	/// 区切り文字を大文字の前に設定する
	/// </summary>
	private static string SetDelimiterBeforeUppercase(string str){
		string conversionStr = "";

		for(int strNo = 0; strNo < str.Length; strNo++){

			bool isSetDelimiter = true;

			//最初には設定しない
			if(strNo == 0){
				isSetDelimiter = false;
			}
			//小文字か数字なら設定しない
			else if(char.IsLower(str[strNo]) || char.IsNumber(str[strNo])){
				isSetDelimiter = false;
			}
			//判定してるの前が大文字なら設定しない(連続大文字の時)
			else if(char.IsUpper(str[strNo - 1]) && !char.IsNumber(str[strNo])){
				isSetDelimiter = false;
			}
			//判定してる文字かその文字の前が区切り文字なら設定しない
			else if(str[strNo] == DELIMITER || str[strNo - 1] == DELIMITER){
				isSetDelimiter = false;
			}

			//文字設定
			if(isSetDelimiter){
				conversionStr += DELIMITER.ToString();
			}
			conversionStr += str.ToUpper() [strNo];

		}

		return conversionStr;
	}

}