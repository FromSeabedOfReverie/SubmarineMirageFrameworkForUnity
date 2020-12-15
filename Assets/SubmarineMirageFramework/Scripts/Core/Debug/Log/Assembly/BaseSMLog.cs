//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Debug {
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ デバッグ記録の基盤クラス
	///		tagは、Enumで作成して登録し、必ず最初は未登録となる定数を定義する。（NONE等）
	///		UnityエディタのConsole窓から、クリックで飛べない為、DLL化する。
	/// </summary>
	///====================================================================================================
	public abstract class BaseSMLog<T> : IDisposable where T : Enum {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>タグ装飾一覧の辞書</summary>
		static readonly Dictionary<T, string> s_tagFormats = new Dictionary<T, string>();
		/// <summary>表示するか？</summary>
		static bool s_isEnabled	{ get; set; }
		/// <summary>Unityエディタで実行中か？</summary>
		bool _isEditor	{ get; set; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected BaseSMLog( bool isEnabled, bool isEditor ) {
			// DLLにビルドするので、プリプロセッサで囲めない為、ここで情報を渡す
			s_isEnabled = isEnabled;
			_isEditor = isEditor;

			// NONE以外の付箋色を登録
			var max = Enum.GetValues( typeof( T ) ).Length - 1;
			var delta = 1.0f / max;
			// HSV色相環で、順当に設定
			for ( var i = 0; i < max; i++ ) {
				RegisterTag( (T)(object)( i + 1 ), Color.HSVToRGB( i * delta, 1, 1 ) );
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● デストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		~BaseSMLog() => Dispose();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 解放
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Dispose() => s_tagFormats.Clear();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● タグを登録
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void RegisterTag( T tag, Color color ) {
			var format = $"[{tag}]";
			if ( _isEditor )	{ format = DecorationFormat( format, color ); }
			format += " {0}";
			s_tagFormats[tag] = format;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● フォーマットを装飾
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected abstract string DecorationFormat( string format, Color color );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文章を整形
		/// </summary>
		///------------------------------------------------------------------------------------------------
		static object FormatText( object text, T tag ) {
			// タグが設定されている場合
			if ( !tag.Equals( default( T ) ) ) {
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