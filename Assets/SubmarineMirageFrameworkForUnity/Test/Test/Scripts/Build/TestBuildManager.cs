//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
namespace SubmarineMirageFramework.Test.Build {
	using UnityEditor;
	using UnityEngine;
	using SubmarineMirageFramework.Build;
	using SubmarineMirageFramework.Process;
	///====================================================================================================
	/// <summary>
	/// ■ ビルド管理のテストクラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class TestBuildManager : MonoBehaviourProcess {
	}



	///====================================================================================================
	/// <summary>
	/// ■ ビルド管理テストの編集クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	[CustomEditor( typeof( TestBuildManager ) )]
	public class TestBuildManagerEditor : Editor {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>ビルド管理</summary>
		BuildManager _buildManager;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● エディタ描画
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if ( _buildManager == null ) {
				_buildManager = new BuildManager();
			}

			if ( GUILayout.Button( "ビルド前" ) )	{ _buildManager.OnPreprocessBuild( null ); }
			if ( GUILayout.Button( "ビルド後" ) )	{ _buildManager.OnPostprocessBuild( null ); }
		}
	}
}
#endif