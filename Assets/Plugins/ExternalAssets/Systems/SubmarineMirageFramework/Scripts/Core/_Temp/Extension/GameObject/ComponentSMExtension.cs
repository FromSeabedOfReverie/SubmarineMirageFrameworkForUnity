//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using System.Collections.Generic;
	using UnityEngine;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 部品の拡張クラス
	///		部品の拡張関数を定義し、内部的にゲーム物の便利クラスを呼ぶ。
	/// </summary>
	///====================================================================================================
	public static class ComponentSMExtension {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 削除（部品）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static void Destroy( this Component self, float waitSecond = 0 ) {
			if ( self == null )	{ return; }
			if ( waitSecond == 0 )	{ Object.Destroy( self ); }
			else					{ Object.Destroy( self, waitSecond ); }
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 指定層の子達のゲーム物を全取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static IEnumerable<GameObject> GetChildrenInLayer( this Component self, SMUnityLayer layer )
			=> self.gameObject.GetChildrenInLayer( layer );
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
		) => self.gameObject.GetComponentsInChildrenUntilOneHierarchy<T>( isIncludeInactive );
		///------------------------------------------------------------------------------------------------
		/// ● 1階層までの、親の、部品達を取得
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 1階層までの、親の、部品達を取得
		/// </summary>
		public static IEnumerable<T> GetComponentsInParentUntilOneHierarchy<T>( this Component self,
																				bool isIncludeInactive = false
		) => self.gameObject.GetComponentsInParentUntilOneHierarchy<T>( isIncludeInactive );
		/// <summary>
		/// ● 1階層までの、親の、部品を取得
		/// </summary>
		public static T GetComponentInParentUntilOneHierarchy<T>( this Component self,
																	bool isIncludeInactive = false
		) => self.gameObject.GetComponentInParentUntilOneHierarchy<T>( isIncludeInactive );
	}
}