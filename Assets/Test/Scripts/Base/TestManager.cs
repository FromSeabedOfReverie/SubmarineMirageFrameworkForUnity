//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System.Collections.Generic;
	using Singleton.New;


	// TODO : コメント追加、整頓


	public class TestManager : RawSingleton<TestManager> {
		readonly List<BaseTest> _tests = new List<BaseTest>();


		public TestManager() {
			_disposables.AddLast( () => {
				_tests.ForEach( t => t.Dispose() );
				_tests.Clear();
			} );
		}


		public void Register( BaseTest test ) => _tests.Add( test );

		public void Unregister( BaseTest test ) => _tests.Remove( test );
	}
}