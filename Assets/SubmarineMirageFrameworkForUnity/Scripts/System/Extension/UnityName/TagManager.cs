//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Extension {
	///====================================================================================================
	/// <summary>
	/// ■ 付箋の管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class TagManager : UnityName<TagManager, TagManager.Name> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>付箋名</summary>
		public enum Name {
			/// <summary>未設定</summary>
			Untagged,
			/// <summary>シングルトン管理</summary>
			Singletons,

			// TODO : リファクタリング次第で、以下タグは不要になるかもしれない

			/// <summary>中心カメラ</summary>
			MainCamera,
			/// <summary>プレイヤー</summary>
			Player,
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

		/// <summary>登録するか？</summary>
		public override bool _isRegister => false;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public TagManager() {
		}
	}
}