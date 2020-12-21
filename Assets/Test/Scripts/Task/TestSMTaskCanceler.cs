//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask {
	using System;
	using System.Diagnostics;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Task;
	using Utility;
	using Debug;
	using TestBase;



	public partial class TestSMTaskCanceler : SMStandardTest {
		readonly Stopwatch _stopwatch = new Stopwatch();
		int _index	{ get; set; }


		protected override void Create() {
			Application.targetFrameRate = 30;
		}


		void StartMeasure() => _stopwatch.Restart();

		float StopMeasure() {
			_stopwatch.Stop();
			return _stopwatch.ElapsedMilliseconds / 1000f;
		}

		async UniTask TestCancel( SMTaskCanceler canceler, SMTaskCanceler runCanceler, string name ) {
			for ( var i = 0; i < 2; i++ ) {
				UTask.Void( async () => {
					await UTask.Delay( _asyncCanceler, 500 );
					SMLog.Debug( $"停止{_index}" );
					canceler.Cancel();
				} );
				try {
					SMLog.Debug( $"{name}トークン、待機開始{i}" );
					StartMeasure();
					await UTask.Delay( runCanceler, 2000 );
					SMLog.Debug( $"{name}トークン、待機終了{i} : {StopMeasure()}" );
				} catch ( OperationCanceledException ) {}
				_index++;
			}
		}

		async UniTask TestDelete2( SMTaskCanceler delete1, SMTaskCanceler delete2, string name1, string name2 ) {
			delete1.Dispose();
			SMLog.Debug( $"{name1}のみ削除 : {name2} : {delete2}" );
			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 500 );
				SMLog.Debug( "停止" );
				delete2.Cancel();
			} );
			try {
				SMLog.Debug( "待機開始" );
				await UTask.Delay( delete2, 2000 );
				SMLog.Debug( "待機終了" );
			} catch ( OperationCanceledException ) {}
			delete2.Dispose();
		}

		async UniTask TestDelete3( SMTaskCanceler delete1, SMTaskCanceler delete2, SMTaskCanceler delete3,
									string name1, string name2, string name3 ) {
			delete1.Dispose();
			SMLog.Debug( $"{name1}のみ削除 : {name2} : {delete2}" );

			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 500 );
				SMLog.Debug( $"{name2}停止" );
				delete2.Cancel();
			} );
			try {
				SMLog.Debug( $"{name2}待機開始" );
				await UTask.Delay( delete2, 2000 );
				SMLog.Debug( $"{name2}待機終了" );
			} catch ( OperationCanceledException ) {}
			delete2.Dispose();
			SMLog.Debug( $"{name2}のみ削除 : {name3} : {delete3}" );

			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 500 );
				SMLog.Debug( $"{name3}停止" );
				delete3.Cancel();
			} );
			try {
				SMLog.Debug( $"{name3}待機開始" );
				await UTask.Delay( delete3, 2000 );
				SMLog.Debug( $"{name3}待機終了" );
			} catch ( OperationCanceledException ) {}
			delete3.Dispose();
		}

		
/*
		・単独テスト
		親子リンク、停止、停止リロード、停止イベント、文章表示、削除時リンクを確認
*/
		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestSingle() => From( async () => {
			var canceler = new SMTaskCanceler();
			canceler._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( "停止イベント" ) );

			SMLog.Debug( "・リンクテスト" );
			SMLog.Debug( canceler );

			SMLog.Debug( "・停止テスト" );
			_index = 0;
			await TestCancel( canceler, canceler, "" );

			SMLog.Debug( "・削除テスト" );
			canceler.Dispose();
			canceler = null;
		} );


/*
		・子供テスト
		親子リンク、親停止、子停止、停止イベント、停止順、文章表示、親削除、子削除を確認
*/
		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestChild() => From( async () => {
			var createCanceler = new Func<( SMTaskCanceler, SMTaskCanceler )>( () => {
				var p = new SMTaskCanceler();
				var c = p.CreateChild();
				p._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( "親停止イベント" ) );
				c._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( "子停止イベント" ) );
				return ( p, c );
			} );
			var ( parent, child ) = createCanceler();

			SMLog.Debug( "・リンクテスト" );
			SMLog.Debug( $"{nameof( parent )} : {parent}" );
			SMLog.Debug( $"{nameof( child )} : {child}" );

			SMLog.Debug( "・親停止テスト" );
			_index = 0;
			await TestCancel( parent, parent, "親" );
			await TestCancel( parent, child, "子" );

			SMLog.Debug( "・子停止テスト" );
			_index = 0;
			await TestCancel( child, parent, "親" );
			await TestCancel( child, child, "子" );

			SMLog.Debug( "・親から削除テスト" );
			await TestDelete2( parent, child, "親", "子" );
			parent = null;
			child = null;

			SMLog.Debug( "・子から削除テスト" );
			( parent, child ) = createCanceler();
			await TestDelete2( child, parent, "子", "親" );
			parent = null;
			child = null;
		} );


/*
		・子供達テスト
		親子リンク、親停止、子停止、停止イベント、停止順、文章表示、親削除、子削除を確認
*/
		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestChildren() => From( async () => {
			var createCanceler = new Func<( SMTaskCanceler, SMTaskCanceler, SMTaskCanceler )>( () => {
				var p = new SMTaskCanceler();
				var c1 = p.CreateChild();
				var c2 = p.CreateChild();
				p._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( "親停止イベント" ) );
				c1._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( "子1停止イベント" ) );
				c2._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( "子2停止イベント" ) );
				return ( p, c1, c2 );
			} );
			var ( parent, child1, child2 ) = createCanceler();

			SMLog.Debug( "・リンクテスト" );
			SMLog.Debug( $"{nameof( parent )} : {parent}" );
			SMLog.Debug( $"{nameof( child1 )} : {child1}" );
			SMLog.Debug( $"{nameof( child2 )} : {child2}" );

			SMLog.Debug( "・親停止テスト" );
			_index = 0;
			await TestCancel( parent, parent, "親" );
			await TestCancel( parent, child1, "子1" );
			await TestCancel( parent, child2, "子2" );

			SMLog.Debug( "・子1停止テスト" );
			_index = 0;
			await TestCancel( child1, parent, "親" );
			await TestCancel( child1, child1, "子1" );
			await TestCancel( child1, child2, "子2" );

			SMLog.Debug( "・親から削除テスト" );
			await TestDelete3( parent, child1, child2, "親", "子1", "子2" );
			parent = null;
			child1 = null;
			child2 = null;

			SMLog.Debug( "・子2から削除テスト" );
			( parent, child1, child2 ) = createCanceler();
			await TestDelete3( child2, child1, parent, "子2", "子1", "親" );
			parent = null;
			child1 = null;
			child2 = null;
		} );


/*
		・祖親子テスト
		祖親子リンク、祖停止、親停止、子停止、停止イベント、停止順、文章表示、
		祖削除、子削除を確認
*/
		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator Test3Generations() => From( async () => {
			var createCanceler = new Func<( SMTaskCanceler, SMTaskCanceler, SMTaskCanceler )>( () => {
				var g = new SMTaskCanceler();
				var p = g.CreateChild();
				var c = p.CreateChild();
				g._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( "祖停止イベント" ) );
				p._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( "親停止イベント" ) );
				c._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( "子停止イベント" ) );
				return ( g, p, c );
			} );
			var ( grandparent, parent, child ) = createCanceler();

			SMLog.Debug( "・リンクテスト" );
			SMLog.Debug( $"{nameof( grandparent )} : {grandparent}" );
			SMLog.Debug( $"{nameof( parent )} : {parent}" );
			SMLog.Debug( $"{nameof( child )} : {child}" );

			SMLog.Debug( "・祖停止テスト" );
			_index = 0;
			await TestCancel( grandparent, grandparent, "祖" );
			await TestCancel( grandparent, parent, "親" );
			await TestCancel( grandparent, child, "子" );

			SMLog.Debug( "・親停止テスト" );
			_index = 0;
			await TestCancel( parent, grandparent, "祖" );
			await TestCancel( parent, parent, "親" );
			await TestCancel( parent, child, "子" );

			SMLog.Debug( "・子停止テスト" );
			_index = 0;
			await TestCancel( child, grandparent, "祖" );
			await TestCancel( child, parent, "親" );
			await TestCancel( child, child, "子" );

			SMLog.Debug( "・祖から削除テスト" );
			await TestDelete3( grandparent, parent, child, "祖", "親", "子" );
			grandparent = null;
			parent = null;
			child = null;

			SMLog.Debug( "・子から削除テスト" );
			( grandparent, parent, child ) = createCanceler();
			await TestDelete3( child, parent, grandparent, "子", "親", "祖" );
			grandparent = null;
			parent = null;
			child = null;
		} );


/*
		・停止識別子の比較テスト
		IsCanceledBy、確認
*/
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestIsCanceledBy() => From( async () => {
			SMLog.Debug( $"{nameof( TestIsCanceledBy )}" );


			SMLog.Debug( "・単体テスト" );
			using ( var c = new SMTaskCanceler() ) {
				c._cancelEvent.AddLast().Subscribe( _ => SMLog.Debug( "停止" ) );
				SMLog.Debug( $"等しい？ : {c.IsCanceledBy( c.ToToken() )}" );
				try {
					UTask.Void( async () => {
						await UTask.Delay( c, 500 );
						c.Cancel();
					} );
					await UTask.Delay( c, 1000 );
					SMLog.Debug( "処理完了" );

				} catch ( OperationCanceledException e ) {
					SMLog.Debug( $"等しい？ : {c.IsCanceledBy( e.CancellationToken )}" );
				}
			}


			SMLog.Debug( "・親子、親停止テスト" );
			using ( var c = new SMTaskCanceler() ) {
				using ( var cc = c.CreateChild() ) {
					c._cancelEvent.AddLast().Subscribe(_ => SMLog.Debug("親停止"));
					cc._cancelEvent.AddLast().Subscribe(_ => SMLog.Debug("子停止"));
					SMLog.Debug( string.Join( "\n",
						$"親 : {c.ToToken().GetHashCode()}",
						$"子 : {cc.ToToken().GetHashCode()}"
					) );
					SMLog.Debug( string.Join( "\n",
						$"親子等しい？ : {c.IsCanceledBy( cc.ToToken() )}",
						$"子親等しい？ : {cc.IsCanceledBy( c.ToToken() )}"
					) );
					try {
						UTask.Void( async () => {
							await UTask.Delay( c, 500 );
							c.Cancel();
						} );
						await UTask.Delay( cc, 1000 );
						SMLog.Debug( "処理完了" );

					} catch ( OperationCanceledException e ) {
						SMLog.Debug( string.Join( "\n",
							$"親等しい？ : {c.IsCanceledBy( e.CancellationToken )}",
							$"子等しい？ : {cc.IsCanceledBy( e.CancellationToken )}"
						) );
					}
				}
			}


			SMLog.Debug( "・親子、子停止テスト" );
			using ( var c = new SMTaskCanceler() ) {
				using ( var cc = c.CreateChild() ) {
					c._cancelEvent.AddLast().Subscribe(_ => SMLog.Debug("親停止"));
					cc._cancelEvent.AddLast().Subscribe(_ => SMLog.Debug("子停止"));
					SMLog.Debug( string.Join( "\n",
						$"親 : {c.ToToken().GetHashCode()}",
						$"子 : {cc.ToToken().GetHashCode()}"
					) );
					SMLog.Debug( string.Join( "\n",
						$"親子等しい？ : {c.IsCanceledBy( cc.ToToken() )}",
						$"子親等しい？ : {cc.IsCanceledBy( c.ToToken() )}"
					) );
					try {
						UTask.Void( async () => {
							await UTask.Delay( cc, 500 );
							cc.Cancel();
						} );
						await UTask.Delay( c, 1000 );
						SMLog.Debug( "処理完了" );

					} catch ( OperationCanceledException e ) {
						SMLog.Debug( string.Join( "\n",
							$"親等しい？ : {c.IsCanceledBy( e.CancellationToken )}",
							$"子等しい？ : {cc.IsCanceledBy( e.CancellationToken )}"
						) );
					}
				}
			}
		} );


/*
		・解放後のテスト
		_isCancel、確認
*/
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDisposed() => From( async () => {
			SMLog.Debug( $"{nameof( TestDisposed )}" );

			var c = new SMTaskCanceler();
			c.Dispose();
			SMLog.Debug( string.Join( "\n",
				$"{nameof( c._isDispose )} : {c._isDispose}",
				$"{nameof( c._isCancel )} : {c._isCancel}"
			) );

			await UTask.DontWait();
		} );
	}
}