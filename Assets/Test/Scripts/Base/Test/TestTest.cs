//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using UniRx;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class TestTest : Test {
		protected override void Create() {
			Log.Debug( "Create" );
			_createEvent.AddLast( async cancel => {
				Log.Debug( "start _createEvent" );
				await UniTaskUtility.Delay( _asyncCancel, 1000 );
				Log.Debug( "end _createEvent" );
			} );
			_initializeEvent.AddLast( async cancel => {
				Log.Debug( "start _initializeEvent" );
				await UniTaskUtility.Delay( _asyncCancel, 1000 );
				Log.Debug( "end _initializeEvent" );
			} );
			_finalizeEvent.AddLast().Subscribe( _ => Log.Debug( "_finalizeEvent" ) );
			_disposables.AddLast( () => Log.Debug( "Dispose" ) );
		}


		[UnityTest]
		public IEnumerator Test() => From( async () => {
			Log.Debug( " Test" );
			await UniTaskUtility.DontWait();
		} );
	}
}