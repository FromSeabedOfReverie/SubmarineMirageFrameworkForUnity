//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestCSharp {
	using System;
	using System.Linq;
	using System.Threading;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Extension;
	using Utility;
	using Debug;
	using TestBase;


	// 停止識別子の試験


	public partial class TestSMUniTask : SMStandardTest {

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
				SMLog.Debug( "-----------------" );
				cancelers.Add( new CancellationTokenSource() );
				cancelers.Add( new CancellationTokenSource() );
				cancelers.Add( cancelers[0].Token.Link( cancelers[1].Token ) );
				cancelers.ForEach( ( c, i ) => c.Token.Register( () => SMLog.Debug( $"callback {i}" ) ) );
			} );
			var logEvent = new Action<string>( logText => SMLog.Debug(
				$"{logText}\n"
				+ string.Join( "\n", cancelers.Select( ( c, i ) => $"{i} : {c.IsCancellationRequested}" ) )
			) );

			resetEvent();
			logEvent( "初期状態" );

			resetEvent();
			cancelers[0].Token.Register( () => SMLog.Debug( $"other token callback {0}" ) );
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
					SMLog.Debug( "waiting" );
					await UniTask.Delay( 200, cancellationToken: cancelers[2].Token );
				}
			} );
			await UTask.Delay( _asyncCanceler, 1000 );
			cancelers[0].Dispose();
			cancelers[1].Dispose();
			cancelers.RemoveRange( 0, 2 );
			cancelers[0].Cancel();
			logEvent( $"0, 1番目を削除、2番目をキャンセル" );

			await UTask.WaitWhile( _asyncCanceler, () => true );
		} );
	}
}