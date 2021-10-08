//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Setting {



	/// <summary>Unityのタグ名</summary>
	public enum SMUnityTag {
		/// <summary>未設定</summary>
		Untagged,
		Respawn,
		Finish,
		EditorOnly,
		/// <summary>中心カメラ</summary>
		MainCamera,
		/// <summary>プレイヤー</summary>
		Player,
		GameController,

// TODO : リファクタリング次第で、以下タグは不要になるかもしれない
		/// <summary>中心光</summary>
		MainLight,
		/// <summary>時間</summary>
		Time,
		/// <summary>雨</summary>
		Rain,
		/// <summary>風</summary>
		Wind,
		/// <summary>雲</summary>
		Cloud,
		/// <summary>水</summary>
		Water,
		/// <summary>地面</summary>
		Ground,
	}
}