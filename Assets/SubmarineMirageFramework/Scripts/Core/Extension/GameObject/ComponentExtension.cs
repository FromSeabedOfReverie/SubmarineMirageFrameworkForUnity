//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using System.Collections.Generic;
	using UnityEngine;
	using KoganeUnityLib;
	using Utility;
	///====================================================================================================
	/// <summary>
	/// ■ 部品の拡張クラス
	///----------------------------------------------------------------------------------------------------
	///		部品の拡張関数を定義し、内部的にゲーム物の便利クラスを呼ぶ。
	/// </summary>
	///====================================================================================================
	public static class ComponentExtension {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 指定層の子達のゲーム物を全取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static IEnumerable<GameObject> GetChildrenInLayer( this Component self, LayerManager.Name layer )
			=> GameObjectSMUtility.GetChildrenInLayer( self.gameObject, layer );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 親を含まない、全子階層の部品達を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T[] GetComponentsInChildrenWithoutSelf<T>( this Component self, bool isIncludeInactive )
			where T : Component
			=> self.gameObject.GetComponentsInChildrenWithoutSelf<T>( isIncludeInactive );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、子達の、部品達を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static IEnumerable<T> GetComponentsInChildrenUntilOneHierarchy<T>( this Component self,
																					bool isIncludeInactive = false
		) => GameObjectSMUtility.GetComponentsInChildrenUntilOneHierarchy<T>(
				self.gameObject, isIncludeInactive );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、親の、部品達を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static IEnumerable<T> GetComponentsInParentUntilOneHierarchy<T>( this Component self,
																				bool isIncludeInactive = false
		) => GameObjectSMUtility.GetComponentsInParentUntilOneHierarchy<T>(
				self.gameObject, isIncludeInactive );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、親の、部品を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T GetComponentInParentUntilOneHierarchy<T>( this Component self,
																	bool isIncludeInactive = false
		) => GameObjectSMUtility.GetComponentInParentUntilOneHierarchy<T>(
				self.gameObject, isIncludeInactive );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 指定名ゲーム物から、部品を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T FindComponent<T>( this Component self, string name ) where T : Component
			=> GameObjectSMUtility.FindComponent<T>( name );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 指定付箋ゲーム物から、部品を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T FindComponentWithTag<T>( this Component self, TagManager.Name tag ) where T : Component
			=> GameObjectSMUtility.FindComponentWithTag<T>( tag );
	}
}