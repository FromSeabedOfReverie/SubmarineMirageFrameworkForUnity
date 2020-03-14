//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Utility {
	using System.Linq;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using UniRx;
	using KoganeUnityLib;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ ゲーム物の便利クラス
	///----------------------------------------------------------------------------------------------------
	///		ゲーム物、部品の拡張関数から、呼ばれる。
	/// </summary>
	///====================================================================================================
	public static class GameObjectUtility {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 指定層の子供達のゲーム物を全取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static GameObject[] GetChildrenInLayer( GameObject gameObject, LayerManager.Name layer ) {
			var id = LayerManager.s_instance.ToInt( layer );
			return gameObject.GetChildren()
				.Where( go => go.layer == id )
				.ToArray();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 指定名ゲーム物から、部品を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T FindComponent<T>( string name ) where T : Component {
			var go = GameObject.Find( name );
			return go != null ? go.GetComponent<T>() : null;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 指定付箋ゲーム物から、部品を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T FindComponentWithTag<T>( TagManager.Name tag ) where T : Component {
			var s = TagManager.s_instance.Get( tag );
			var go = GameObject.FindWithTag( s );
			return go != null ? go.GetComponent<T>() : null;
		}
	}
}