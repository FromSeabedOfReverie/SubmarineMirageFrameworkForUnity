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
		/// <summary>自身</summary>
		static BaseSMLog<T> s_instance	{ get; set; }
		/// <summary>自身が有効か？</summary>
		public static bool s_isEnable	{ get; set; }

		/// <summary>タグ装飾一覧の辞書</summary>
		readonly Dictionary<T, string> _tagFormats = new Dictionary<T, string>();
		/// <summary>Unityエディタで実行中か？</summary>
		bool _isEditor	{ get; set; }

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public BaseSMLog( bool isEditor ) {
			s_instance = this;

			// DLLにビルドするので、プリプロセッサで囲めない為、ここで情報を渡す
			_isEditor = isEditor;

			// NONE以外の付箋色を登録
			var max = Enum.GetValues( typeof( T ) ).Length - 1;
			var delta = 1.0f / max;
			// HSV色相環で、順当に設定
			for ( var i = 0; i < max; i++ ) {
				RegisterTag( (T)(object)( i + 1 ), Color.HSVToRGB( i * delta, 1, 1 ) );
			}
		}

		/// <summary>
		/// ● デストラクタ
		/// </summary>
		~BaseSMLog() => Dispose();

		/// <summary>
		/// ● 廃棄
		/// </summary>
		public virtual void Dispose() {
			_tagFormats.Clear();
			s_instance = null;
		}

		///------------------------------------------------------------------------------------------------
		/// ● 登録
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● タグを登録
		/// </summary>
		void RegisterTag( T tag, Color color ) {
			var format = $"[{tag}]";
			if ( _isEditor )	{ format = DecorationFormat( format, color ); }
			format += " {0}";
			_tagFormats[tag] = format;
		}

		/// <summary>
		/// ● フォーマットを装飾
		/// </summary>
		protected abstract string DecorationFormat( string format, Color color );

		///------------------------------------------------------------------------------------------------
		/// ● 文章表示
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文章を整形
		/// </summary>
		object FormatText( object text, T tag ) {
			// タグが設定されている場合
			if ( !tag.Equals( default( T ) ) ) {
				text = string.Format( _tagFormats[tag], text );
			}
			return text;
		}

		/// <summary>
		/// ● 通常文を表示
		/// </summary>
		public static void Debug( object text, T tag = default ) {
			if ( !s_isEnable )	{ return; }

			UnityEngine.Debug.Log( s_instance?.FormatText( text, tag ) ?? text );
		}

		/// <summary>
		/// ● 警告文を表示
		/// </summary>
		public static void Warning( object text, T tag = default ) {
			if ( !s_isEnable )	{ return; }

			UnityEngine.Debug.LogWarning( s_instance?.FormatText( text, tag ) ?? text );
		}

		/// <summary>
		/// ● 失敗文を表示
		/// </summary>
		public static void Error( object text, T tag = default ) {
			if ( !s_isEnable )	{ return; }

			UnityEngine.Debug.LogError( s_instance?.FormatText( text, tag ) ?? text );
		}
	}
}