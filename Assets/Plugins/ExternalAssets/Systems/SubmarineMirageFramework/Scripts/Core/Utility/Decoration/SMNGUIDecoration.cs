//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System.Collections.Generic;
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ NGUIの装飾クラス
	///		NGUIの文字描画の装飾を行う。
	/// </summary>
	///====================================================================================================
	public class SMNGUIDecoration : SMTextDecoration {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>書式型</summary>
		public new enum Type {
			/// <summary>太字</summary>
			Bold,
			/// <summary>斜体</summary>
			Italic,
			/// <summary>色</summary>
			Color,
			/// <summary>下線</summary>
			Underline,
			/// <summary>打消線</summary>
			Strikethrough,
			/// <summary>下付き文字</summary>
			Subscript,
			/// <summary>上付き文字</summary>
			Superscript,
			/// <summary>リンク</summary>
			Hyperlink,
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public SMNGUIDecoration() {
			// NGUI書式対応の、指定文字配列を設定
			_formatTexts = new List<string[]> {
				{ new string[] { "[b]",		"",		"[/b]" } },
				{ new string[] { "[i]",		"",		"[/i]" } },
				{ new string[] { "",		"",		"[-]" } },
				{ new string[] { "[u]",		"",		"[/u]" } },
				{ new string[] { "[s]",		"",		"[/s]" } },
				{ new string[] { "[sub]",	"",		"[/sub]" } },
				{ new string[] { "[sup]",	"",		"[/sup]" } },
				{ new string[] { "[url=",	"]",	"[/url]" } },
			};
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 書式文字を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		string GetFormat( Type code, Order order )
			=> GetFormat( (int)code, order );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 書式文字に整形
		/// </summary>
		///------------------------------------------------------------------------------------------------
		string By( Type code, string text )
			=> By( (int)code, text );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 下線で装飾
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public string ByUnderline( string text )
			=> By( Type.Underline, text );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 打消線で装飾
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public string ByStrikethrough( string text )
			=> By( Type.Strikethrough, text );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 下付き文字で装飾
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public string BySubscript( string text )
			=> By( Type.Subscript, text );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 上付き文字で装飾
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public string BySuperscript( string text )
			=> By( Type.Superscript, text );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● URLで装飾
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public string ByHyperlink( string text, string url )
			=> (
				GetFormat( Type.Hyperlink, Order.Start ) +
				url +
				GetFormat( Type.Hyperlink, Order.StartEnd ) +
				text +
				GetFormat( Type.Hyperlink, Order.End )
			);
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 色書式を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected override string ConvertColorFormat( Color c )
			=> c.ToNGUIFormat();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 複数書式型で装飾
		///		URL、色はType指定でなく、string、Colorオブジェクトを入れる。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override string ByType( string text, params object[] objects ) {
			text = base.ByType( text, objects );

			foreach ( var o in objects ) {
				if ( o is Type ) {
					switch ( (Type)o ) {
						case Type.Bold:				text = ByBold( text );			break;
						case Type.Italic:			text = ByItalic( text );		break;
						case Type.Underline:		text = ByUnderline( text );		break;
						case Type.Strikethrough:	text = ByStrikethrough( text );	break;
						case Type.Subscript:		text = BySubscript( text );		break;
						case Type.Superscript:		text = BySuperscript( text );	break;
					}

				} else if ( o is string ) {
					text = ByHyperlink( text, (string)o );
				}
			}
			return text;
		}
	}
}