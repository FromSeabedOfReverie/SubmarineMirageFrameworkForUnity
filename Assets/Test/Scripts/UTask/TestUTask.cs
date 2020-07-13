//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestUTask {
	using System;
	using System.Linq;
	using System.Threading;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using UTask;
	using Extension;
	using Debug;
	using Test;

	public partial class TestUTask : Test {
		protected override void Create() {
			Application.targetFrameRate = 30;
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestEmpty() => From( async () => {
			Log.Debug( 1 );
			UTask.Void( _asyncCancel, async c => {
				Log.Debug( 2 );
				await UTask.Empty;
				Log.Debug( 3 );
				await UTask.Empty;
				Log.Debug( 4 );
			} );
			Log.Debug( 5 );
			await UTask.DontWait();
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestDontWait() => From( async () => {
			Log.Debug( 1 );
			UTask.Void( _asyncCancel, async c => {
				Log.Debug( 2 );
				await UTask.DontWait();
				Log.Debug( 3 );
				await UTask.DontWait();
				Log.Debug( 4 );
			} );
			Log.Debug( 5 );
			await UTask.DontWait();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestCreate() => From( async () => {
			Log.Debug( 1 );
			var t = UTask.Create( async () => {
				Log.Debug( 2 );
				await UTask.Yield();
				Log.Debug( 3 );
			} );
			Log.Debug( 4 );
			await t;
			Log.Debug( 5 );
			await t;		// 2回待機できず、エラー
			Log.Debug( 6 );
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestDefer() => From( async () => {
			Log.Debug( 1 );
			var t = UTask.Defer( async () => {
				Log.Debug( 2 );
				await UTask.Yield();
				Log.Debug( 3 );
			} );
			Log.Debug( 4 );
			await t;
			Log.Debug( 5 );
			await t;		// 2回待機できず、エラー
			Log.Debug( 6 );
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestLazy() => From( async () => {
			Log.Debug( 1 );
			var t = UTask.Lazy( async () => {
				Log.Debug( 2 );
				await UTask.Yield();
				Log.Debug( 3 );
			} );
			Log.Debug( 4 );
			await t;
			Log.Debug( 5 );
			await t;		// 2回目は、空を待機
			Log.Debug( 6 );
		} );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestFunc() => From( async () => {
			Log.Debug( 1 );
			var t = new Func<UniTask>( async () => {
				Log.Debug( 2 );
				await UTask.Delay( _asyncCancel, 1000 );
				Log.Debug( 3 );
			} );
			Log.Debug( 4 );
			await t();
			Log.Debug( 5 );
			await t();		// きちんと、2回目も待機される
			Log.Debug( 6 );
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestNever() => From( async () => {
			var canceler = new CancellationTokenSource();
			_disposables.AddLast( canceler );
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Return ) ).Subscribe( _ =>
					canceler.Cancel()
				)
			);
			Log.Debug( 1 );
			try {
				await UTask.Never( canceler.Token );
			} finally {
				Log.Debug( 2 );
			}
		} );

	}
}