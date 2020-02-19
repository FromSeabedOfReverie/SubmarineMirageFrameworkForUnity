//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Extension {
	using UnityEngine;
	using Data;
	///====================================================================================================
	/// <summary>
	/// ■ テクスチャの拡張クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public static class TextureExtension {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>基本のスプライト中心</summary>
		static readonly Vector2 DEFAULT_SPRITE_PIVOT = new Vector2( 0.5f, 0.5f );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● スプライトに変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static Sprite ToSprite( this Texture2D texture, Vector2? pivot = null ) {
			return Sprite.Create(
				texture,
				new Rect( 0, 0, texture.width, texture.height ),
				pivot.HasValue ? pivot.Value : DEFAULT_SPRITE_PIVOT
			);
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 生情報に変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static TextureRawData ToRawData( this Texture2D texture,
												TextureRawData.Type type = TextureRawData.Type.PNG,
												int? encodeOption = null )
		{
			return new TextureRawData( texture, type, encodeOption );
		}
	}
}