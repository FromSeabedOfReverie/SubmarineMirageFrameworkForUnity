//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ スプライトの拡張クラス
	/// </summary>
	///====================================================================================================
	public static class SpriteSMExtension {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 生情報に変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static SMTextureRawData ToRawData( this Sprite sprite,
													SMTextureRawData.Type type = SMTextureRawData.Type.PNG,
													int? encodeOption = null
		) => sprite.texture.ToRawData( type, encodeOption );
	}
}