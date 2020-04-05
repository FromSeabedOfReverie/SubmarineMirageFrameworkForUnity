//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Editor.Build {
	using UnityEditor;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ ビルド管理テストの編集クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class BuildEditor : EditorWindow {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>シングルトン</summary>
		static BuildEditor s_instanceObject;
		/// <summary>ビルド管理</summary>
		public readonly BuildManager _build = new BuildManager();
		///------------------------------------------------------------------------------------------------
		/// ● アクセサ
		///------------------------------------------------------------------------------------------------
		/// <summary>シングルトン取得</summary>
		static BuildEditor s_instance {
			get {
// TODO : Application.dataPathが、エディタから呼べない、エラー
				if ( s_instanceObject == null )	{ s_instanceObject = new BuildEditor(); }
				Log.Debug( "BuildEditor" );
				return s_instanceObject;
			}
		}
		///------------------------------------------------------------------------------------------------
		/// ● ビルド処理
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● ビルド前処理
		/// </summary>
		[MenuItem( "File/Before Build", priority = 222 )]
		static void BeforeBuild() {
			Log.Debug( "Before Build" );
			s_instance._build.OnPreprocessBuild( null );
		}
		/// <summary>
		/// ● ビルド後処理
		/// </summary>
		[MenuItem( "File/After Build", priority = 223 )]
		static void AfterBuild() {
			Log.Debug( "After Build" );
			s_instance._build.OnPostprocessBuild( null );
		}
	}
}