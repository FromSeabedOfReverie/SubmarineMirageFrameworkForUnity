//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using UnityEngine;
	using Data;
	///====================================================================================================
	/// <summary>
	/// ■ スプライトの拡張クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public static class SpriteExtension {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 生情報に変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static TextureRawData ToRawData( this Sprite sprite,
												TextureRawData.Type type = TextureRawData.Type.PNG,
												int? encodeOption = null )
		{
			return sprite.texture.ToRawData( type, encodeOption );
		}
	}
}