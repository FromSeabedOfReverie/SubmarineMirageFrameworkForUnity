//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {



	/// <summary>情報の種類</summary>
	public enum SMDataType {
		/// <summary>テクスチャ画像</summary>
		Texture,
		/// <summary>スプライト画像</summary>
		Sprite,
		/// <summary>音</summary>
		Audio,
		/// <summary>シリアル化情報</summary>
		Serialize,
		/// <summary>文章</summary>
		Text,
		/// <summary>生情報（どれにも当て嵌まらない時用）</summary>
		Raw,
	}
}