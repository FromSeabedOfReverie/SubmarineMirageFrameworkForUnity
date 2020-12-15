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
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class TestSMRawTest : SMRawTest {
		protected override void Create() {
			SMLog.Debug( $"{nameof( Create )}" );

			_createEvent += async canceler => {
				SMLog.Debug( $"{nameof( _createEvent )} : start" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"{nameof( _createEvent )} : end" );
			};

			_initializeEvent += async canceler => {
				SMLog.Debug( $"{nameof( _initializeEvent )} : start" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"{nameof( _initializeEvent )} : end" );
			};

			_finalizeEvent.Subscribe( _ => SMLog.Debug( $"{nameof( _finalizeEvent )}" ) );

			_disposables.Add( () => SMLog.Debug( $"{nameof( Dispose )}" ) );
		}


		[UnityTest]
		public IEnumerator Test() => From( async () => {
			SMLog.Debug( $"{nameof( Test )}" );
			await UTask.DontWait();
		} );
	}
}