//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestUTask {
	using System;
	using System.Collections;
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
	public partial class TestUniTask : SMStandardTest {
		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestMonoBehaviour() => From( TestMonoBehaviourSub() );
		IEnumerator TestMonoBehaviourSub() {
			var go = new GameObject( $"{nameof( TestMono )}" );
			go.AddComponent<TestMono>();
			while ( true )	{ yield return null; }
		}



		public class TestMono : MonoBehaviourSMExtension {
			async void Start() {				// UniTaskだと、エラーが全く出なくなる
//				throw new Exception();			// エラーが出る
				ShowTime( "Start 1" );
//				StartSub().Forget();			// 中はエラー（表示は警告）でも、外は続行される
				await StartSub();				// 中はエラー（表示はエラー）だと、外も停止する
				ShowTime( "Start 2" );
				throw new Exception();			// エラーが出る
			}

			async UniTask StartSub() {
				throw new Exception();			// 呼出最初が、async UniTask ではなく、awaitされた場合、エラーが出る
												// 呼出最初が、async UniTask ではなく、.Forget()された場合、警告が出る
				await UniTask.DelayFrame( 60 );	// 60フレーム待機する
				ShowTime( "StartSub" );
			}


			void Update() {					// UniTaskだと、エラーが全く出なくなる
											// async void にしても、関係なく毎フレーム呼ばれる
				return;
//				throw new Exception();		// エラーが出る
				ShowTime( "Update 1" );
				UpdateSub().Forget();		// 中はエラー（表示は警告）でも、外は続行される
//				await UpdateSub();			// async で待機されるが、毎フレーム Update() 呼ばれる為、どんどん重なる
				ShowTime( "Update 2" );
			}

			async UniTask UpdateSub() {
				throw new Exception();			// 呼出最初が、async UniTask ではなく、awaitされた場合、エラーが出る
												// 呼出最初が、async UniTask ではなく、.Forget()された場合、警告が出る
				await UniTask.DelayFrame( 60 );	// 60フレーム待機する
				ShowTime( "UpdateSub" );
			}


			void ShowTime( string name ) => SMLog.Debug( $"{name} : {Time.frameCount}" );


			public override void Dispose()	{}
		}
	}
}