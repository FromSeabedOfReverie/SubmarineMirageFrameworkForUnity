//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestUTask {
	using System;
	using System.Threading;
	using System.Diagnostics;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using UTask;
	using Debug;
	using Test;

	public partial class TestUTaskCanceler : Test {
		Stopwatch _stopwatch = new Stopwatch();
		int _index;


		protected override void Create() {
			Application.targetFrameRate = 30;
		}


		void StartMeasure() => _stopwatch.Restart();

		float StopMeasure() {
			_stopwatch.Stop();
			return _stopwatch.ElapsedMilliseconds / 1000f;
		}

		async UniTask TestCancel( UTaskCanceler canceler, UTaskCanceler runCanceler, string name ) {
			for ( var i = 0; i < 2; i++ ) {
				UTask.Void( async () => {
					await UTask.Delay( _asyncCanceler, 500 );
					Log.Debug( $"停止{_index}" );
					canceler.Cancel();
				} );
				try {
					Log.Debug( $"{name}トークン、待機開始{i}" );
					StartMeasure();
					await UTask.Delay( runCanceler, 2000 );
					Log.Debug( $"{name}トークン、待機終了{i} : {StopMeasure()}" );
				} catch ( OperationCanceledException ) {}
				_index++;
			}
		}

		async UniTask TestDelete2( UTaskCanceler delete1, UTaskCanceler delete2, string name1, string name2 ) {
			delete1.Dispose();
			Log.Debug( $"{name1}のみ削除 : {name2} : {delete2}" );
			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 500 );
				Log.Debug( "停止" );
				delete2.Cancel();
			} );
			try {
				Log.Debug( "待機開始" );
				await UTask.Delay( delete2, 2000 );
				Log.Debug( "待機終了" );
			} catch ( OperationCanceledException ) {}
			delete2.Dispose();
		}

		async UniTask TestDelete3( UTaskCanceler delete1, UTaskCanceler delete2, UTaskCanceler delete3,
									string name1, string name2, string name3 ) {
			delete1.Dispose();
			Log.Debug( $"{name1}のみ削除 : {name2} : {delete2}" );

			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 500 );
				Log.Debug( $"{name2}停止" );
				delete2.Cancel();
			} );
			try {
				Log.Debug( $"{name2}待機開始" );
				await UTask.Delay( delete2, 2000 );
				Log.Debug( $"{name2}待機終了" );
			} catch ( OperationCanceledException ) {}
			delete2.Dispose();
			Log.Debug( $"{name2}のみ削除 : {name3} : {delete3}" );

			UTask.Void( async () => {
				await UTask.Delay( _asyncCanceler, 500 );
				Log.Debug( $"{name3}停止" );
				delete3.Cancel();
			} );
			try {
				Log.Debug( $"{name3}待機開始" );
				await UTask.Delay( delete3, 2000 );
				Log.Debug( $"{name3}待機終了" );
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
			var canceler = new UTaskCanceler();
			canceler._cancelEvent.AddLast().Subscribe( _ => Log.Debug( "停止イベント" ) );

			Log.Debug( "・リンクテスト" );
			Log.Debug( canceler );

			Log.Debug( "・停止リロードテスト" );
			_index = 0;
			await TestCancel( canceler, canceler, "" );

			Log.Debug( "・削除テスト" );
			canceler.Dispose();
			canceler = null;
		} );


/*
		・子供テスト
		親子リンク、親停止リロード、子停止リロード、停止イベント、停止順、文章表示、親削除、子削除を確認
*/
		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestChild() => From( async () => {
			var createCanceler = new Func<( UTaskCanceler, UTaskCanceler )>( () => {
				var p = new UTaskCanceler();
				var c = p.CreateChild();
				p._cancelEvent.AddLast().Subscribe( _ => Log.Debug( "親停止イベント" ) );
				c._cancelEvent.AddLast().Subscribe( _ => Log.Debug( "子停止イベント" ) );
				return ( p, c );
			} );
			var ( parent, child ) = createCanceler();

			Log.Debug( "・リンクテスト" );
			Log.Debug( $"{nameof( parent )} : {parent}" );
			Log.Debug( $"{nameof( child )} : {child}" );

			Log.Debug( "・親停止リロードテスト" );
			_index = 0;
			await TestCancel( parent, parent, "親" );
			await TestCancel( parent, child, "子" );

			Log.Debug( "・子停止リロードテスト" );
			_index = 0;
			await TestCancel( child, parent, "親" );
			await TestCancel( child, child, "子" );

			Log.Debug( "・親から削除テスト" );
			await TestDelete2( parent, child, "親", "子" );
			parent = null;
			child = null;

			Log.Debug( "・子から削除テスト" );
			( parent, child ) = createCanceler();
			await TestDelete2( child, parent, "子", "親" );
			parent = null;
			child = null;
		} );


/*
		・子供達テスト
		親子リンク、親停止リロード、子停止リロード、停止イベント、停止順、文章表示、親削除、子削除を確認
*/
		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestChildren() => From( async () => {
			var createCanceler = new Func<( UTaskCanceler, UTaskCanceler, UTaskCanceler )>( () => {
				var p = new UTaskCanceler();
				var c1 = p.CreateChild();
				var c2 = p.CreateChild();
				p._cancelEvent.AddLast().Subscribe( _ => Log.Debug( "親停止イベント" ) );
				c1._cancelEvent.AddLast().Subscribe( _ => Log.Debug( "子1停止イベント" ) );
				c2._cancelEvent.AddLast().Subscribe( _ => Log.Debug( "子2停止イベント" ) );
				return ( p, c1, c2 );
			} );
			var ( parent, child1, child2 ) = createCanceler();

			Log.Debug( "・リンクテスト" );
			Log.Debug( $"{nameof( parent )} : {parent}" );
			Log.Debug( $"{nameof( child1 )} : {child1}" );
			Log.Debug( $"{nameof( child2 )} : {child2}" );

			Log.Debug( "・親停止リロードテスト" );
			_index = 0;
			await TestCancel( parent, parent, "親" );
			await TestCancel( parent, child1, "子1" );
			await TestCancel( parent, child2, "子2" );

			Log.Debug( "・子1停止リロードテスト" );
			_index = 0;
			await TestCancel( child1, parent, "親" );
			await TestCancel( child1, child1, "子1" );
			await TestCancel( child1, child2, "子2" );

			Log.Debug( "・親から削除テスト" );
			await TestDelete3( parent, child1, child2, "親", "子1", "子2" );
			parent = null;
			child1 = null;
			child2 = null;

			Log.Debug( "・子2から削除テスト" );
			( parent, child1, child2 ) = createCanceler();
			await TestDelete3( child2, child1, parent, "子2", "子1", "親" );
			parent = null;
			child1 = null;
			child2 = null;
		} );


/*
		・祖親子テスト
		祖親子リンク、祖停止リロード、親停止リロード、子停止リロード、停止イベント、停止順、文章表示、
		祖削除、子削除を確認
*/
		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator Test3Generations() => From( async () => {
			var createCanceler = new Func<( UTaskCanceler, UTaskCanceler, UTaskCanceler )>( () => {
				var g = new UTaskCanceler();
				var p = g.CreateChild();
				var c = p.CreateChild();
				g._cancelEvent.AddLast().Subscribe( _ => Log.Debug( "祖停止イベント" ) );
				p._cancelEvent.AddLast().Subscribe( _ => Log.Debug( "親停止イベント" ) );
				c._cancelEvent.AddLast().Subscribe( _ => Log.Debug( "子停止イベント" ) );
				return ( g, p, c );
			} );
			var ( grandparent, parent, child ) = createCanceler();

			Log.Debug( "・リンクテスト" );
			Log.Debug( $"{nameof( grandparent )} : {grandparent}" );
			Log.Debug( $"{nameof( parent )} : {parent}" );
			Log.Debug( $"{nameof( child )} : {child}" );

			Log.Debug( "・祖停止リロードテスト" );
			_index = 0;
			await TestCancel( grandparent, grandparent, "祖" );
			await TestCancel( grandparent, parent, "親" );
			await TestCancel( grandparent, child, "子" );

			Log.Debug( "・親停止リロードテスト" );
			_index = 0;
			await TestCancel( parent, grandparent, "祖" );
			await TestCancel( parent, parent, "親" );
			await TestCancel( parent, child, "子" );

			Log.Debug( "・子停止リロードテスト" );
			_index = 0;
			await TestCancel( child, grandparent, "祖" );
			await TestCancel( child, parent, "親" );
			await TestCancel( child, child, "子" );

			Log.Debug( "・祖から削除テスト" );
			await TestDelete3( grandparent, parent, child, "祖", "親", "子" );
			grandparent = null;
			parent = null;
			child = null;

			Log.Debug( "・子から削除テスト" );
			( grandparent, parent, child ) = createCanceler();
			await TestDelete3( child, parent, grandparent, "子", "親", "祖" );
			grandparent = null;
			parent = null;
			child = null;
		} );
	}
}