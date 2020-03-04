//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Debug {
	using System.Linq;
	using UnityEngine.SceneManagement;
#if UNITY_EDITOR
	using UnityEditor;
#endif
	using Singleton;
	///====================================================================================================
	/// <summary>
	/// ■ デバッグの管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class DebugManager : Singleton<DebugManager> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>デバッグ記録</summary>
		Log _log;
#if DEVELOP
		/// <summary>デバッグ肝心情報</summary>
		DebugMainInfo _debugMainInfo;
#endif
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public DebugManager() {
			_log = new Log();
#if DEVELOP
			_debugMainInfo = new DebugMainInfo();
#endif
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 現在場面が、ビルド場面に含まれているか？
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public bool isInBuildScene() {
#if UNITY_EDITOR
			var path = SceneManager.GetActiveScene().path;
			return EditorBuildSettings.scenes
				.Any( scene => scene.path == path );
#else
			return true;
#endif
		}
	}
}