//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestUTask {
	using System.Linq;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Extension;
	using Debug;
	using Test;



	public partial class TestUniTask : SMStandardTest {
		protected override void Create() {
			Application.targetFrameRate = 30;
		}



		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestYield() => From( async () => {
			SMLog.Debug( 1 );
			UniTask.Void( async () => {
				SMLog.Debug( 2 );
				await UniTask.Yield();
				SMLog.Debug( 3 );
			} );
			SMLog.Debug( 4 );

			await UniTask.Yield();
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestNextFrame() => From( async () => {
			SMLog.Debug( 1 );
			UniTask.Void( async () => {
				SMLog.Debug( 2 );
				await UniTask.NextFrame();
				SMLog.Debug( 3 );
			} );
			SMLog.Debug( 4 );

			await UniTask.Yield();
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestEmpty() => From( async () => {
			SMLog.Debug( 1 );
			UniTask.Void( async () => {
				SMLog.Debug( 2 );
				await new UniTask();
				SMLog.Debug( 3 );
			} );
			SMLog.Debug( 4 );

			await UniTask.Yield();
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestEmpty2() => From( async () => {
			var empty = new UniTask();
			SMLog.Debug( 1 );
			UniTask.Void( async () => {
				SMLog.Debug( 2 );
				await empty;
				SMLog.Debug( 3 );
				await empty;
				SMLog.Debug( 4 );
			} );
			SMLog.Debug( 5 );

			await UniTask.Yield();
		} );



		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestLinq() => From( async () => {
			SMLog.Debug( $"{nameof( TestLinq )} : start" );

			await Enumerable
				.Range( 0, 10 )
				.Select( _ => new TestAsyncObject() )
				.Select( a => a.Initialize() );

			SMLog.Debug( $"{nameof( TestLinq )} : end" );
		} );


		public class TestAsyncObject {
			static int s_maxID;
			int _id;

			public TestAsyncObject() => _id = s_maxID++;

			public async UniTask Initialize() {
				SMLog.Debug( $"{this.GetAboutName()}( {_id} ).{nameof( Initialize )} : start" );
				await UniTask.Delay( 1000 );
				SMLog.Debug( $"{this.GetAboutName()}( {_id} ).{nameof( Initialize )} : end" );
			}
		}
	}
}