//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System.Collections.Generic;
	using Singleton;
	using Extension;


	// TODO : コメント追加、整頓


	public class SMTestManager : SMRawSingleton<SMTestManager> {
		readonly List<SMBaseTest> _tests = new List<SMBaseTest>();


		public SMTestManager() {
			_disposables.Add( () => {
				_tests.ForEach( t => t.Dispose() );
				_tests.Clear();
			} );
		}


		public void Register( SMBaseTest test ) => _tests.Add( test );

		public void Unregister( SMBaseTest test ) => _tests.Remove( test );
	}
}