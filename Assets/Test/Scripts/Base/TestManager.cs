//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System.Collections.Generic;
	using SubmarineMirage.Editor;


	// TODO : コメント追加、整頓


	public static class TestManager {
		static List<BaseTest> _tests = new List<BaseTest>();

		static TestManager() {
			ConsoleEditorUtility.Clear();
			Dispose();
			PlayerExtensionEditor._playStopEvent = () => Dispose();
		}

		static void Dispose() {
			_tests.ForEach( t => t.Dispose() );
			_tests.Clear();
		}

		public static void Register( BaseTest test ) => _tests.Add( test );

		public static void UnRegister( BaseTest test ) => _tests.Remove( test );
	}
}