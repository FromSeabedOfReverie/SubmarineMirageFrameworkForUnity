//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Debug {
	using System;
	using System.Collections.Generic;
	///====================================================================================================
	/// <summary>
	/// ■ デバッグ記録の基盤クラス
	///----------------------------------------------------------------------------------------------------
	///		tagは、Enumで作成して登録し、必ず最初は未登録となる定数を定義する。（NONE等）
	///		UnityエディタのConsole窓から、クリックで飛べない為、DLL化する。
	/// </summary>
	///====================================================================================================
	public abstract class BaseLog<T> where T : Enum {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>タグ装飾一覧の辞書</summary>
		static readonly Dictionary<T, string> s_tagFormats = new Dictionary<T, string>();
		/// <summary>表示するか？</summary>
		static bool s_isEnabled = true;
		/// <summary>基本形式</summary>
		protected static string s_defaultFormat { get; private set; } = "[{0}] ";	// 見難いから1文字空ける
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected BaseLog( bool isEnabled ) {
			// DLLにビルドするので、プリプロセッサで囲めない為、ここで情報を渡す
			s_isEnabled = isEnabled;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● タグを登録
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected static void RegisterTag( T tag, string format = null ) {
			// 形式未設定の場合、基本形式を設定
			if ( string.IsNullOrEmpty( format ) ) {
				// 代入タイミングが異なるので、整形後、再度形式を設定
				format = string.Format( s_defaultFormat, tag ) + "{0}";
			}
			s_tagFormats[tag] = format;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文章を整形
		/// </summary>
		///------------------------------------------------------------------------------------------------
		static object FormatText( object text, T tag ) {
			// タグが設定されている場合
			if ( !tag.Equals( default( T ) ) ) {
				// タグが未登録の場合、登録
				if ( !s_tagFormats.ContainsKey( tag ) ) {
					RegisterTag( tag );
				}
				text = string.Format( s_tagFormats[tag], text );
			}
			return text;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 通常文を表示
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static void Debug( object text, T tag = default( T ) ) {
			if ( s_isEnabled ) {
				UnityEngine.Debug.Log( FormatText( text, tag ) );
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 警告文を表示
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static void Warning( object text, T tag = default( T ) ) {
			if ( s_isEnabled ) {
				UnityEngine.Debug.LogWarning( FormatText( text, tag ) );
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 失敗文を表示
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static void Error( object text, T tag = default( T ) ) {
			if ( s_isEnabled ) {
				UnityEngine.Debug.LogError( FormatText( text, tag ) );
			}
		}
	}
}