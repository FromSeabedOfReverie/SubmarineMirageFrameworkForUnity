//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using KoganeUnityLib;
	using Service;
	using Setting;
	///====================================================================================================
	/// <summary>
	/// ■ ゲーム物の拡張クラス
	///		ゲーム物の拡張関数を定義し、内部的にゲーム物の便利クラスを呼ぶ。
	/// </summary>
	///====================================================================================================
	public static class GameObjectSMExtension {
		///------------------------------------------------------------------------------------------------
		/// ● インスタンス化
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● インスタンス生成
		/// </summary>
		public static GameObject Instantiate( this GameObject self, Transform parent, bool isWorldPositionStays )
			=> Object.Instantiate( self, parent, isWorldPositionStays );
		/// <summary>
		/// ● インスタンス生成
		/// </summary>
		public static GameObject Instantiate( this GameObject self, Transform parent )
			=> Object.Instantiate( self, parent );
		/// <summary>
		/// ● インスタンス生成
		/// </summary>
		public static GameObject Instantiate( this GameObject self, Vector3 position, Quaternion rotation,
												Transform parent
		) => Object.Instantiate( self, position, rotation, parent );
		/// <summary>
		/// ● インスタンス生成
		/// </summary>
		public static GameObject Instantiate( this GameObject self, Vector3 position, Quaternion rotation )
			=> Object.Instantiate( self, position, rotation );
		/// <summary>
		/// ● インスタンス生成
		/// </summary>
		public static GameObject Instantiate( this GameObject self )
			=> Object.Instantiate( self );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 削除（ゲーム物）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static void Destroy( this GameObject self, float waitSecond = 0 ) {
			if ( self == null )	{ return; }
			if ( waitSecond == 0 )	{ Object.Destroy( self ); }
			else					{ Object.Destroy( self, waitSecond ); }
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 指定層の子達のゲーム物を全取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static IEnumerable<GameObject> GetChildrenInLayer( this GameObject self, SMUnityLayer layer ) {
			var layerManager = SMServiceLocator.Resolve<SMUnityLayerManager>();
			var id = layerManager.ToInt( layer );
			return self.GetChildren()
				.Where( go => go.layer == id );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、子達の、部品達を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static IEnumerable<T> GetComponentsInChildrenUntilOneHierarchy<T>( this GameObject self,
																					bool isIncludeInactive = false
		) {
			// 自身が非活動中の場合、未処理
			if ( !isIncludeInactive && !self.activeInHierarchy )	{ yield break; }

			// 子階層から、走査開始を設定
			var currents = new Queue<Transform>();
			foreach ( Transform child in self.transform ) {
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
		/// ● 1階層までの、親の、部品達を取得
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、親の、部品達を取得（補助）
		/// </summary>
		static IEnumerable<T> GetComponentsInParentUntilOneHierarchySub<T>( this GameObject self,
																			bool isIncludeInactive,
																			bool isGetOnlyOne
		) {
			// 自身が非活動中の場合、未処理
			if ( !isIncludeInactive && !self.activeInHierarchy )	{ yield break; }

			// 親が存在する限り、再帰処理
			var parent = self.transform.parent;
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
		/// <summary>
		/// ● 1階層までの、親の、部品達を取得
		/// </summary>
		public static IEnumerable<T> GetComponentsInParentUntilOneHierarchy<T>( this GameObject self,
																				bool isIncludeInactive = false
		) => self.GetComponentsInParentUntilOneHierarchySub<T>( isIncludeInactive, false );
		/// <summary>
		/// ● 1階層までの、親の、部品を取得
		/// </summary>
		public static T GetComponentInParentUntilOneHierarchy<T>( this GameObject self,
																	bool isIncludeInactive = false
		) => self.GetComponentsInParentUntilOneHierarchySub<T>( isIncludeInactive, true )
				.FirstOrDefault();
	}
}