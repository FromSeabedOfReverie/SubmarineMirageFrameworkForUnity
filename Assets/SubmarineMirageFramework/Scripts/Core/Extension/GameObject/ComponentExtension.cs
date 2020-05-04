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
		/// ● 指定層の子供達のゲーム物を全取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static List<GameObject> GetChildrenInLayer( this Component component, LayerManager.Name layer )
			=> GameObjectUtility.GetChildrenInLayer( component.gameObject, layer );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 親を含まない、全子階層の部品達を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T[] GetComponentsInChildrenWithoutSelf<T>( this Component component, bool isIncludeInactive )
			where T : Component
			=> component.gameObject.GetComponentsInChildrenWithoutSelf<T>( isIncludeInactive );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、子供達の、部品達を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static List<T> GetComponentsInChildrenUntilOneHierarchy<T>( this Component component,
																			bool isIncludeInactive = false
		)
			=> GameObjectUtility.GetComponentsInChildrenUntilOneHierarchy<T>(
				component.gameObject, isIncludeInactive );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、親の、部品達を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static List<T> GetComponentsInParentUntilOneHierarchy<T>( this Component component,
																			bool isIncludeInactive = false
		)
			=> GameObjectUtility.GetComponentsInParentUntilOneHierarchy<T>(
				component.gameObject, isIncludeInactive );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、親の、部品を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T GetComponentInParentUntilOneHierarchy<T>( this Component component,
																	bool isIncludeInactive = false
		)
			=> GameObjectUtility.GetComponentInParentUntilOneHierarchy<T>(
				component.gameObject, isIncludeInactive );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 指定名ゲーム物から、部品を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T FindComponent<T>( this Component component, string name ) where T : Component
			=> GameObjectUtility.FindComponent<T>( name );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 指定付箋ゲーム物から、部品を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T FindComponentWithTag<T>( this Component component, TagManager.Name tag )
			where T : Component
			=> GameObjectUtility.FindComponentWithTag<T>( tag );
	}
}