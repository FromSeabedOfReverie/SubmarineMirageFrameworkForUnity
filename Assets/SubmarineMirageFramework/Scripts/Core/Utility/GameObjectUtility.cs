//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Utility {
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
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
		public static List<GameObject> GetChildrenInLayer( GameObject gameObject, LayerManager.Name layer ) {
			var id = LayerManager.s_instance.ToInt( layer );
			return gameObject.GetChildren()
				.Where( go => go.layer == id )
				.ToList();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、子供達の、部品達を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static List<T> GetComponentsInChildrenUntilOneHierarchy<T>( GameObject gameObject,
																			bool isIncludeInactive = false
		) {
			var results = new List<T>();
			// 自身が非活動中の場合、未処理
			if ( !isIncludeInactive && !gameObject.activeInHierarchy )	{ return results; }

			var transforms = new List<Transform> { gameObject.transform };
			// 自身の同一階層達が無くなるまで、再帰処理
			while ( !transforms.IsEmpty() ) {
				// 自身の、同一階層達の、全子供達の順で走査
				var children = new List<Transform>();
				transforms.ForEach( transform => {
					foreach ( Transform child in transform ) {
						// 子が活動中の場合
						if ( isIncludeInactive || child.gameObject.activeInHierarchy ) {
							var cs = child.GetComponents<T>();
							// 部品が無い場合、更に子供を処理
							if ( !cs.IsEmpty() )	{ results.Add( cs ); }
							else					{ children.Add( child ); }
						}
					}
				} );
				transforms = children;	// 子供達を自身の階層とし、再帰処理
			}

			return results;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、親の、部品達を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static List<T> GetComponentsInParentUntilOneHierarchy<T>( GameObject gameObject,
																			bool isIncludeInactive = false,
																			bool isGetOnlyOne = false
		) {
			var results = new List<T>();
			// 自身が非活動中の場合、未処理
			if ( !isIncludeInactive && !gameObject.activeInHierarchy )	{ return results; }

			var parent = gameObject.transform.parent;
			// 親が無いか、部品が存在するまで、再帰処理
			while ( parent != null && results.IsEmpty() ) {
				// 親が活動中の場合、部品達を取得
				if ( isIncludeInactive || parent.gameObject.activeInHierarchy ) {
					if ( isGetOnlyOne )	{ results.Add( parent.GetComponent<T>() ); }
					else				{ results.Add( parent.GetComponents<T>() ); }
					parent = parent.parent;	// 部品達が無い場合に備え、親を再指定し、再帰処理
				} else {
					parent = null;	// 親が非活動中の場合、再帰終了
				}
			}

			return results;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、親の、部品を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T GetComponentInParentUntilOneHierarchy<T>( GameObject gameObject,
																	bool isIncludeInactive = false
		) {
			return GetComponentsInParentUntilOneHierarchy<T>( gameObject, isIncludeInactive, true )
				.FirstOrDefault();
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