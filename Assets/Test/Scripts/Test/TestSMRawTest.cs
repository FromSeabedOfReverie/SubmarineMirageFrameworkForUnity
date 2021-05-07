//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestBase.Test {
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using UniRx;
	using Extension;
	using Utility;
	using Debug;


	public class TestSMRawTest : SMRawTest {
		TestSMTestBody _body	{ get; set; }


		protected override void Awake() {
			base.Awake();
			_body = new TestSMTestBody( this );
			_disposables.Add( _body );
		}

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
		public IEnumerator TestTask() => _body.TestTask();

		[UnityTest]
		public IEnumerator TestCoroutine() => _body.TestCoroutine();


		[UnityTest]
		public IEnumerator TestCancelTask() => _body.TestCancelTask();

		[UnityTest]
		public IEnumerator TestCancelCoroutine() => _body.TestCancelCoroutine();


		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestDisposeTask() => _body.TestDisposeTask();

		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestDisposeCoroutine() => _body.TestDisposeCoroutine();
	}
}