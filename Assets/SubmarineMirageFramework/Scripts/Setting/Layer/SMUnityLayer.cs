//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Setting {



	/// <summary>Unityのレイヤー名</summary>
	public enum SMUnityLayer {
		/// <summary>通常</summary>
		Default,
		/// <summary>未設定</summary>
		None,
		/// <summary>地面</summary>
		Ground,
		/// <summary>水</summary>
		Water,
		/// <summary>AI</summary>
		AI,
		/// <summary>プレイヤー</summary>
		Player,
		/// <summary>物理衝突</summary>
		Collider,
		/// <summary>遠景</summary>
		DistantView,
	}
}