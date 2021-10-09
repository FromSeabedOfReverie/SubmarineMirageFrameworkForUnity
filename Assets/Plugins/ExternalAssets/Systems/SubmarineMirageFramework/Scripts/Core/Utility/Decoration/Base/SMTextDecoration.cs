//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Utility {
	using System.Collections.Generic;
	using UnityEngine;
	using Base;
	///====================================================================================================
	/// <summary>
	/// ■ 文字の装飾基盤クラス
	/// </summary>
	///====================================================================================================
	public abstract class SMTextDecoration : SMLightBase {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>書式型</summary>
		public enum Type {
			/// <summary>太字</summary>
			Bold,
			/// <summary>斜体</summary>
			Italic,
			/// <summary>色</summary>
			Color,
		}
		/// <summary>順序</summary>
		protected enum Order {
			/// <summary>先頭書式</summary>
			Start,
			/// <summary>先頭の終了書式</summary>
			StartEnd,
			/// <summary>終了書式</summary>
			End,
		}

		/// <summary>書式と指定文字の配列</summary>
		protected List<string[]> _formatTexts	{ get; set; } = new List<string[]>();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 解放
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Dispose() => _formatTexts.Clear();
		///------------------------------------------------------------------------------------------------
		/// ● 書式文字を取得
		///------------------------------------------------------------------------------------------------
		/// <summary>● 書式文字を取得（整数）</summary>
		protected string GetFormat( int code, Order order )
			=> _formatTexts[code][(int)order];
		/// <summary>● 書式文字を取得（定数）</summary>
		string GetFormat( Type code, Order order )
			=> GetFormat( (int)code, order );
		///------------------------------------------------------------------------------------------------
		/// ● 書式文字に整形
		///------------------------------------------------------------------------------------------------
		/// <summary>● 書式文字に整形（整数）</summary>
		protected string By( int code, string text )
			=> (
				GetFormat( code, Order.Start ) +
				text +
				GetFormat( code, Order.End )
			);
		/// <summary>● 書式文字に整形（定数）</summary>
		string By( Type code, string text )
			=> By( (int)code, text );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 太文字で装飾
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public string ByBold( string text )
			=> By( Type.Bold, text );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 斜体で装飾
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public string ByItalic( string text )
			=> By( Type.Italic, text );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 色で装飾
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public string ByColor( string text, Color color )
			=> (
				ConvertColorFormat(color) +
				text +
				GetFormat( Type.Color, Order.End )
			);
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 色書式を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected virtual string ConvertColorFormat( Color c )
			=> string.Empty;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 複数書式型で装飾
		///		色はType指定でなく、Colorオブジェクトを入れる。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual string ByType( string text, params object[] objects ) {
			foreach ( var o in objects ) {
				if ( o is Type ) {
					switch ( (Type)o ) {
						case Type.Bold:		text = ByBold( text );		break;
						case Type.Italic:	text = ByItalic( text );	break;
					}

				} else if ( o is Color ) {
					text = ByColor( text, (Color)o );
				}
			}
			return text;
		}
	}
}