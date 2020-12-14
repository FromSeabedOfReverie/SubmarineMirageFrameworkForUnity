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
	using UTask;
	using Debug;


	// TODO : コメント追加、整頓


	public class TestSMStandardTest : SMStandardTest {
		protected override void Create() {
			Log.Debug( $"{nameof( Create )}" );

			_createEvent.AddLast( async canceler => {
				Log.Debug( $"{nameof( _createEvent )} : start" );
				await UTask.Delay( canceler, 1000 );
				Log.Debug( $"{nameof( _createEvent )} : end" );
			} );

			_initializeEvent.AddLast( async canceler => {
				Log.Debug( $"{nameof( _initializeEvent )} : start" );
				await UTask.Delay( canceler, 1000 );
				Log.Debug( $"{nameof( _initializeEvent )} : end" );
			} );

			_finalizeEvent.AddLast().Subscribe( _ => Log.Debug( $"{nameof( _finalizeEvent )}" ) );

			_disposables.AddLast( () => Log.Debug( $"{nameof( Dispose )}" ) );
		}


		[UnityTest]
		public IEnumerator Test() => From( async () => {
			Log.Debug( $"{nameof( Test )}" );
			await UTask.DontWait();
		} );
	}
}