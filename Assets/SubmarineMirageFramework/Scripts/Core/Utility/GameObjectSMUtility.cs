//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Utility {
	using UnityEngine;
	using KoganeUnityLib;
	using Service;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ ゲーム物の便利クラス
	///		ゲーム物、部品の拡張関数から、呼ばれる。
	/// </summary>
	///====================================================================================================
	public static class GameObjectSMUtility {
		///------------------------------------------------------------------------------------------------
		/// ● インスタンス化
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● インスタンス生成
		/// </summary>
		public static T Instantiate<T>() where T : MonoBehaviourSMExtension {
			var type = typeof( T );
			var go = new GameObject( type.GetAboutName() );
			var b = go.AddComponent( type ) as T;

// TODO : SMSceneManagerコメント
/*
			if ( scene == null ) {
				var _sceneManager = SMServiceLocator.Resolve<SMSceneManager>();
				if ( _sceneManager != null )	{ _sceneManager._body.MoveForeverScene( go ); }
				else							{ go.DontDestroyOnLoad(); }
			}
*/
			go.DontDestroyOnLoad();
			return b;
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
		public static T FindComponentWithTag<T>( SMTagManager.Name tag ) where T : Component {
			var tagManager = SMServiceLocator.Resolve<SMTagManager>();
			var s = tagManager.Get( tag );
			var go = GameObject.FindWithTag( s );
			return go != null ? go.GetComponent<T>() : null;
		}
	}
}