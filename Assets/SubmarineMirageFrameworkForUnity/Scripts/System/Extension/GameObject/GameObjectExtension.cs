//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Extension {
	using UnityEngine;
	using Utility;
	///====================================================================================================
	/// <summary>
	/// ■ ゲーム物の拡張クラス
	///----------------------------------------------------------------------------------------------------
	///		ゲーム物の拡張関数を定義し、内部的にゲーム物の便利クラスを呼ぶ。
	/// </summary>
	///====================================================================================================
	public static class GameObjectExtension {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 指定層の子供達のゲーム物を全取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static GameObject[] GetChildrenInLayer( this GameObject gameObject, LayerManager.Name layer ) {
			return GameObjectUtility.GetChildrenInLayer( gameObject, layer );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 指定名ゲーム物から、部品を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T FindComponent<T>( this GameObject gameObject, string name ) where T : Component {
			return GameObjectUtility.FindComponent<T>( name );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 指定付箋ゲーム物から、部品を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T FindComponentWithTag<T>( this GameObject gameObject, TagManager.Name tag )
			where T : Component
		{
			return GameObjectUtility.FindComponentWithTag<T>( tag );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● ゲーム物の場面名を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static string GetSceneNameOfGameObject( this GameObject gameObject ) {
			return GameObjectUtility.GetSceneNameOfGameObject( gameObject );
		}
	}
}