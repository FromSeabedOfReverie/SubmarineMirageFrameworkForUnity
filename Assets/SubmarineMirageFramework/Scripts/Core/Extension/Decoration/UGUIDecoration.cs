//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Extension {
	using System.Collections.Generic;
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ UGUIの装飾クラス
	///----------------------------------------------------------------------------------------------------
	///		UGUIの文字描画の装飾を行う。
	/// </summary>
	///====================================================================================================
	public class UGUIDecoration : TextDecoration {
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
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public UGUIDecoration() : base() {
			// UGUI書式対応の、指定文字配列を設定
			_formatTexts = new List<string[]> {
				{ new string[] { "<b>",	"",	"</b>"		} },
				{ new string[] { "<i>",	"",	"</i>"		} },
				{ new string[] { "",	"",	"</color>"	} },
			};
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 色書式を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected override string ConvertColorFormat( Color c ) {
			return c.ToUGUIFormat();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 複数書式型で装飾
		///		色はType指定でなく、Colorオブジェクトを入れる。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override string ByType( string text, params object[] objects ) {
			text = base.ByType( text, objects );

			foreach ( var o in objects ) {
				if ( o is Type ) {
					switch ( (Type)o ) {
						case Type.Bold:		text = ByBold( text );		break;
						case Type.Italic:	text = ByItalic( text );	break;
					}
				}
			}
			return text;
		}
	}
}