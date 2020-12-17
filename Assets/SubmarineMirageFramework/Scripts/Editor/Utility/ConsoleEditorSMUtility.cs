//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.EditorUtility {
	using System.Reflection;
	using UnityEditor;


	// TODO : コメント追加、整頓


	// TODO : ライセンスに未記入
	// 参考URL : https://gist.github.com/baba-s/4db42702de8b4254251dc05f15ac6a10
	public static class ConsoleEditorSMUtility {
		public static void Clear() {
			var type = Assembly.GetAssembly( typeof( SceneView ) )
#if UNITY_2017_1_OR_NEWER
				.GetType( "UnityEditor.LogEntries" )
#else
				.GetType( "UnityEditorInternal.LogEntries" )
#endif
			;
			var method = type.GetMethod( "Clear", BindingFlags.Static | BindingFlags.Public );
			method.Invoke( null, null );
		}
	}
}