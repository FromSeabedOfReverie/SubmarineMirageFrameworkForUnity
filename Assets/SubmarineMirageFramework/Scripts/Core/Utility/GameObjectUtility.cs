//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Utility {
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
		/// <summary>
		/// ● 指定層の子達のゲーム物を全取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static IEnumerable<GameObject> GetChildrenInLayer( GameObject gameObject, LayerManager.Name layer ) {
			var id = LayerManager.s_instance.ToInt( layer );
			return gameObject.GetChildren()
				.Where( go => go.layer == id );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、子達の、部品達を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static IEnumerable<T> GetComponentsInChildrenUntilOneHierarchy<T>( GameObject gameObject,
																					bool isIncludeInactive = false
		) {
			// 自身が非活動中の場合、未処理
			if ( !isIncludeInactive && !gameObject.activeInHierarchy )	{ yield break; }

			// 子階層から、走査開始を設定
			var currents = new Queue<Transform>();
			foreach ( Transform child in gameObject.transform ) {
				currents.Enqueue( child );
			}

			// 階層達が存在する限り、再帰処理
			while ( !currents.IsEmpty() ) {
				var current = currents.Dequeue();

				// 非活動時に取得せず、子が非活動中の場合、未取得
				if ( !isIncludeInactive && !current.gameObject.activeInHierarchy )	{ continue; }

				// 部品達が存在する場合、部品達を返す
				var cs = current.GetComponents<T>();
				if ( !cs.IsEmpty() ) {
					foreach ( var c in cs )	{ yield return c; }

				// 部品が無い場合、子階層を追加
				} else {
					foreach ( Transform child in current ) {
						currents.Enqueue( child );
					}
				}
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、親の、部品達を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static IEnumerable<T> GetComponentsInParentUntilOneHierarchy<T>( GameObject gameObject,
																				bool isIncludeInactive = false,
																				bool isGetOnlyOne = false
		) {
			// 自身が非活動中の場合、未処理
			if ( !isIncludeInactive && !gameObject.activeInHierarchy )	{ yield break; }

			// 親が存在する限り、再帰処理
			var parent = gameObject.transform.parent;
			while ( parent != null ) {
				// 非活動時に取得せず、親が非活動中の場合、再帰終了
				if ( !isIncludeInactive && !parent.gameObject.activeInHierarchy )	{ yield break; }

				// 1つだけ取得する場合
				if ( isGetOnlyOne ) {
					var c = parent.GetComponent<T>();
					if ( c != null ) {
						yield return c;
						yield break;
					}

				// 複数取得する場合
				} else {
					var cs = parent.GetComponents<T>();
					if ( !cs.IsEmpty() ) {
						foreach ( var c in cs )	{ yield return c; }
						yield break;
					}
				}

				// 部品が無い場合、親を再指定し、再帰処理
				parent = parent.parent;
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、親の、部品を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T GetComponentInParentUntilOneHierarchy<T>( GameObject gameObject,
																	bool isIncludeInactive = false
		) => GetComponentsInParentUntilOneHierarchy<T>( gameObject, isIncludeInactive, true )
				.FirstOrDefault();
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