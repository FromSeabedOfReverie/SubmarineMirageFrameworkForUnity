//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestAsync {
	using System;
	using System.Linq;
	using System.Threading;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using Extension;
	using Utility;
	using Debug;
	using Test;


	// 停止識別子の試験


	public class TestCancellationToken : Test {
		protected override void Create() {
			Application.targetFrameRate = 30;
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestLinkedToken() => From( async () => {
			var cancelers = new List<CancellationTokenSource>();
			_disposables.AddLast( cancelers );

			var resetEvent = new Action( () => {
				cancelers.ForEach( c => {
					c.Cancel();
					c.Dispose();
				} );
				cancelers.Clear();
				Log.Debug( "-----------------" );
				cancelers.Add( new CancellationTokenSource() );
				cancelers.Add( new CancellationTokenSource() );
				cancelers.Add( cancelers[0].Token.Add( cancelers[1].Token ) );
				cancelers.ForEach( ( c, i ) => c.Token.Register( () => Log.Debug( $"callback {i}" ) ) );
			} );
			var logEvent = new Action<string>( logText => Log.Debug(
				$"{logText}\n"
				+ string.Join( "\n", cancelers.Select( ( c, i ) => $"{i} : {c.IsCancellationRequested}" ) )
			) );

			resetEvent();
			logEvent( "初期状態" );

			resetEvent();
			cancelers[0].Token.Register( () => Log.Debug( $"other token callback {0}" ) );
			cancelers[0].Cancel();
			logEvent( "別Tokenの、callbackが実行されるか？" );

			3.Times( i => {
				resetEvent();
				cancelers[i].Cancel();
				logEvent( $"{i} 番目をキャンセル" );
			} );

			resetEvent();
			cancelers[0].Dispose();
			logEvent( $"{0} 番目をキャンセルせずに、解放" );
			cancelers.ForEach( c => c.Dispose() );
			cancelers.Clear();

			resetEvent();
			cancelers[0].Dispose();
			cancelers[1].Dispose();
			logEvent( $"0, 1番目をキャンセルせずに、解放" );
			cancelers.ForEach( c => c.Dispose() );
			cancelers.Clear();

			resetEvent();
			UniTask.Void( async () => {
				while ( true ) {
					Log.Debug( "waiting" );
					await UniTaskUtility.Delay( cancelers[2].Token, 200 );
				}
			} );
			await UniTaskUtility.Delay( _asyncCancel, 1000 );
			cancelers[0].Dispose();
			cancelers[1].Dispose();
			cancelers.RemoveRange( 0, 2 );
			cancelers[0].Cancel();
			logEvent( $"0, 1番目を削除、2番目をキャンセル" );

			await UniTaskUtility.WaitWhile( _asyncCancel, () => true );
		} );
	}
}