//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using System.Collections.Generic;
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
		/// ● 指定層の子達のゲーム物を全取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static IEnumerable<GameObject> GetChildrenInLayer( this GameObject self, LayerManager.Name layer )
			=> GameObjectUtility.GetChildrenInLayer( self, layer );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、子達の、部品達を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static IEnumerable<T> GetComponentsInChildrenUntilOneHierarchy<T>( this GameObject self,
																					bool isIncludeInactive = false
		) => GameObjectUtility.GetComponentsInChildrenUntilOneHierarchy<T>( self, isIncludeInactive );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、親の、部品達を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static IEnumerable<T> GetComponentsInParentUntilOneHierarchy<T>( this GameObject self,
																				bool isIncludeInactive = false
		) => GameObjectUtility.GetComponentsInParentUntilOneHierarchy<T>( self, isIncludeInactive );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、親の、部品を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T GetComponentInParentUntilOneHierarchy<T>( this GameObject self,
																	bool isIncludeInactive = false
		) => GameObjectUtility.GetComponentInParentUntilOneHierarchy<T>( self, isIncludeInactive );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 指定名ゲーム物から、部品を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T FindComponent<T>( this GameObject self, string name ) where T : Component
			=> GameObjectUtility.FindComponent<T>( name );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 指定付箋ゲーム物から、部品を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T FindComponentWithTag<T>( this GameObject self, TagManager.Name tag ) where T : Component
			=> GameObjectUtility.FindComponentWithTag<T>( tag );
	}
}