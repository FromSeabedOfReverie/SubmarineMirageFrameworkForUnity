//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestBase {
	using System.Collections.Generic;
	using Singleton;
	using Extension;


	// TODO : コメント追加、整頓


	public class SMTestManager : SMRawSingleton<SMTestManager> {
		readonly List<BaseSMTest> _tests = new List<BaseSMTest>();


		public SMTestManager() {
			_disposables.Add( () => {
				_tests.ForEach( t => t.Dispose() );
				_tests.Clear();
			} );
		}


		public void Register( BaseSMTest test ) => _tests.Add( test );

		public void Unregister( BaseSMTest test ) => _tests.Remove( test );
	}
}