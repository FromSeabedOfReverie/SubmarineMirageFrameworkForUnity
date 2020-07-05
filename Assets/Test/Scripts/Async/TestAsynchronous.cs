//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestAsync {
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using Cysharp.Threading.Tasks;
	using Extension;
	using Debug;
	using Test;
	///====================================================================================================
	/// <summary>
	/// ■ 非同期の試験
	///		C#のasync/awaitの挙動が不明だった為、作成した。
	///		UniRxと組み合わせ、やっと使い物になる。
	///		
	///		● 理解した事
	///			・呼び出し最初が async Task 関数の場合エラーが出ない為、最初は async void にする
	///			・最初以外は出来るだけ async UniTask を使う
	///			・async void Update() 等、絶対 async 出来ない物は普通に void Update() にする
	///			・async 関数を await せずに呼ぶ場合、後ろに .Forget() を付けると、UniRxが警告を出してくれる
	///			・空の async 関数には、await UniTask.Delay( 0 ) を入れると、コンパイラ警告が消える
	///			・呼び出し最初の async UniTask 関数を try catch で囲めばエラーが出る
	///			・await でエラー出ると以降実行されない、.Forget() で投げっぱなし実行だと以降も実行される
	/// </summary>
	///====================================================================================================
	public class TestAsynchronous : Test {
		protected override void Create() {
			Application.targetFrameRate = 30;
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() {
			var go = new GameObject( $"{nameof( TestMono )}" );
			go.AddComponent<TestMono>();
			while ( true )	{ yield return null; }
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestLinq() => From( async () => {
			Log.Debug( "TestLinq 1" );

			var asyncs = new List<TestAsyncObject>();
			for ( var i = 0; i < 10; i++ ) {
				asyncs.Add( new TestAsyncObject() );
			}
			await asyncs
				.Select( async a => await a.Initialize() );

			Log.Debug( "TestLinq 2" );
		} );



		public class TestMono : MonoBehaviourExtension {
			async void Start() {				// UniTaskだと、エラーが全く出なくなる
//				ShowError();					// エラーが出る
				ShowTime( "Start 1" );
//				StartSub().Forget();			// 中はエラー（表示は警告）でも、外は続行される
				await StartSub();				// 中はエラー（表示はエラー）だと、外も停止する
				ShowTime( "Start 2" );
				ShowError();					// エラーが出る
			}

			async UniTask StartSub() {
				ShowError();					// 呼出最初が、async UniTask ではなく、awaitされた場合、エラーが出る
												// 呼出最初が、async UniTask ではなく、.Forget()された場合、警告が出る
				await UniTask.DelayFrame( 60 );	// 60フレーム待機する
				ShowTime( "StartSub" );
			}


			void Update() {					// UniTaskだと、エラーが全く出なくなる
											// async void にしても、関係なく毎フレーム呼ばれる
				return;
//				ShowError();				// エラーが出る
				ShowTime( "Update 1" );
				UpdateSub().Forget();		// 中はエラー（表示は警告）でも、外は続行される
//				await UpdateSub();			// async で待機されるが、毎フレーム Update() 呼ばれる為、どんどん重なる
				ShowTime( "Update 2" );
			}

			async UniTask UpdateSub() {
				ShowError();					// 呼出最初が、async UniTask ではなく、awaitされた場合、エラーが出る
												// 呼出最初が、async UniTask ではなく、.Forget()された場合、警告が出る
				await UniTask.DelayFrame( 60 );	// 60フレーム待機する
				ShowTime( "UpdateSub" );
			}


			void ShowTime( string name ) {
				Log.Debug( $"{name} : {Time.frameCount}" );
			}

			void ShowError() {
				var a = new int[0];
				var b = a[3];
			}
		}



		public class TestAsyncObject {
			static int s_maxID;
			int _id;

			public TestAsyncObject() {
				_id = s_maxID++;
			}

			public async UniTask Initialize() {
				await UniTask.Delay( 1000 );
				Log.Debug( _id );
			}
		}
	}
}