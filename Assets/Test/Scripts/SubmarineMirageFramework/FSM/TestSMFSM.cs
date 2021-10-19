//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System;
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;



	public class TestSMFSM : SMStandardTest {
		protected override void Create() {
			Application.targetFrameRate = 30;
		}



		// ちゃんと作成される？
		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestCreate() => From( async () => {
			{
				var fsm = new SMFSM<TestState>();	// 作成
				fsm = new SMFSM<TestState>();		// 破棄、作成
				fsm.Dispose();						// 破棄
				fsm.Dispose();						// 何もしない

				var task = new TestOwnerTask();		// 作成
				task.Dispose();						// 破棄
			}
			await UTask.DontWait();
		} );


		// Setup機能するか？
		// Type指定できる？、違うType指定だとどうなる？
		// 破棄済の場合
		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestSetup() => From( async () => {
			var types = new Type[] { typeof( TestStateA ), typeof( TestStateB ), typeof( TestStateC ) };
			var states = types.Select( t => t.Create<TestState>() ).ToArray();

			var fsm = new SMFSM<TestState>();
			fsm.Setup( this, states );		// 設定、状態を使用
			try {
				fsm.Setup( this, states );	// 二度目の設定、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			try {
				states.First().Setup( this, fsm );  // 状態が二度目の設定、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			var s = new TestState2();
			var temp = 0;
			try {
				s.Setup( temp, fsm );       // 状態が_owner設定失敗、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			try {
				s.Setup( this, temp );		// 状態が_fsm設定失敗、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			s.Dispose();
			fsm.Dispose();

			fsm = new SMFSM<TestState>();
			fsm.Setup( this, types );		// 設定、型を使用
			try {
				fsm.Setup( this, types );   // 二度目の設定、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			fsm.Dispose();

			fsm = new SMFSM<TestState>();
			try {
				fsm.Setup( this, new Type[] { typeof( TestDummyState ) } ); // 違う状態で設定、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			fsm.Dispose();

			try {
				states.First().Setup( this, fsm );  // 状態が破棄済の設定、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			try {
				fsm.Setup( this, types );	// 破棄後の設定、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}

			await UTask.DontWait();
		} );


		// update系イベントの、多種多様な指定、破棄、手動呼び出し
		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestUpdates() => From( async () => {
			var types = new Type[] { typeof( TestStateA ), typeof( TestStateB ), typeof( TestStateC ) };

			var fsm = new SMFSM<TestState>();
			fsm.Setup( this, types );
			fsm.FixedUpdateState();				// 実行
			fsm.UpdateState();					// 実行
			fsm.LateUpdateState();				// 実行
			fsm.Dispose();

			var task = new TestOwnerTask();
			task._fsm.Setup( task, types );
			await UTask.WaitWhile( _asyncCanceler, () => task._ranState != SMTaskRunState.LateUpdate ); // 実行
			task.Dispose();

			task = new TestOwnerTask();
			var updates = new SMSubject[] { new SMSubject(), new SMSubject(), new SMSubject() };
			updates.ForEach( ( e, i ) => e.AddLast().Subscribe( _ => SMLog.Debug( $"updates[{i}]" ) ) );
			task._fsm.Setup( task, types, updates[0], updates[1], updates[2] );
			await UTask.WaitWhile( _asyncCanceler, () => task._ranState != SMTaskRunState.LateUpdate ); // 未実行
			updates.ForEach( e => e.Run() );	// 実行
			task.Dispose();
			updates.ForEach( e => e.Run() );    // 未実行
			updates.ForEach( e => e.Dispose() );

			task = new TestOwnerTask();
			updates = new SMSubject[] { new SMSubject(), new SMSubject(), new SMSubject() };
			updates.ForEach( ( e, i ) => e.AddLast().Subscribe( _ => SMLog.Debug( $"updates[{i}]" ) ) );
			task._fsm.Setup( task, types, updates[0], updates[1], updates[2] );
			await UTask.WaitWhile( _asyncCanceler, () => task._ranState != SMTaskRunState.LateUpdate ); // 未実行
			updates.ForEach( e => e.Run() );    // 実行
			updates.ForEach( e => e.Dispose() );
			task.Dispose();						// エラーにならない
		} );


		// GetState関連、ちゃんと動く？
		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestGets() => From( async () => {
			var types = new Type[] { typeof( TestStateA ), typeof( TestStateB ), typeof( TestStateC ) };

			var fsm = new SMFSM<TestState>();
			fsm.Setup( this, types );
			SMLog.Debug( fsm.GetStates().ToShowString() );			// 全取得
			SMLog.Debug( fsm.GetState( typeof( TestStateB ) ) );	// 取得
			SMLog.Debug( fsm.GetState<TestStateB>() );				// 取得
			fsm.Dispose();

			try {
				SMLog.Debug( fsm.GetStates().ToShowString() );		// 破棄済、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			try {
				SMLog.Debug( fsm.GetState( typeof( TestStateB ) ) );	// 破棄済、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			try {
				SMLog.Debug( fsm.GetState<TestStateB>() );          // 破棄済、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}

			fsm = new SMFSM<TestState>();
			fsm.Setup( this, new Type[] {} );
			SMLog.Debug( fsm.GetStates().ToShowString() );			// 無取得
			SMLog.Debug( fsm.GetState( typeof( TestStateB ) ) );    // 無取得
			SMLog.Debug( fsm.GetState<TestStateB>() );              // 無取得
			fsm.Dispose();

			await UTask.DontWait();
		} );


// TODO : ここから
		// 正しくイベント実行される？
		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestEvent() => From( async () => {
			var types = new Type[] { typeof( TestStateA ), typeof( TestStateB ), typeof( TestStateC ) };

			var task = new TestOwnerTask();
			task._fsm.Setup( task, types );				// 実行
			await UTask.WaitWhile( _asyncCanceler, () => task._ranState != SMTaskRunState.LateUpdate );
			await task._fsm.ChangeState<TestStateA>();  // 実行
			await UTask.Delay( _asyncCanceler, 500 );   // _asyncUpdateEvent、途中停止
			await task._fsm.ChangeState<TestStateA>();  // 実行
			await UTask.Delay( _asyncCanceler, 1000 );  // _asyncUpdateEvent、実行完了
			await task._fsm.ChangeState<TestStateB>();  // 実行
			await task._fsm.ChangeState<TestStateC>();  // 実行
			task.Dispose();
		} );


		// _isActiveオフで、モディファイアがちゃんと詰まる？
		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestActive() => From( async () => {
			var types = new Type[] { typeof( TestStateA ), typeof( TestStateB ), typeof( TestStateC ) };

			var fsm = new SMFSM<TestState>();

			var t = fsm.ChangeState<TestStateA>();	// 未実行
			fsm.Setup( this, types );
			await t;								// 実行

			await fsm.ChangeState<TestStateB>();	// 実行

			fsm._isActive = false;
			t = fsm.ChangeState<TestStateC>();		// 未実行
			SMLog.Debug( "待機開始" );
			await UTask.Delay( _asyncCanceler, 1000 );
			SMLog.Debug( "待機終了" );
			fsm._isActive = true;
			await t;                                // 実行

			t = fsm.ChangeState<TestStateA>();		// 実行
			await UTask.Delay( _asyncCanceler, 500 );
			var t2 = fsm.ChangeState<TestStateB>();	// キャッシュに積まれる
			fsm._isActive = false;
			SMLog.Debug( fsm );
			await t;								// 実行中のみ、実行
			fsm._isActive = true;
			await t2;								// 実行

			fsm.Dispose();
		} );


		// _isInitializedの挙動確認
		// ChangeStateのテスト
		// キャンセルも確認
		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestChangeState() => From( async () => {

			var task = new TestOwnerTask();
			var displayLog = SMServiceLocator.Resolve<SMDisplayLog>();
			task._updateEvent.AddLast().Subscribe( _ => {
				task.ToString().Split( "\n" ).ForEach( s => displayLog.Add( s ) );
			} );
			task.Dispose();

			await UTask.DontWait();
		} );



		public class TestOwnerTask : SMTask {
			[SMShowLine] public override SMTaskRunType _type => SMTaskRunType.Sequential;
			[SMShow] public readonly SMFSM<TestState> _fsm = new SMFSM<TestState>();

			public TestOwnerTask() {
				_disposables.AddFirst( () => {
					_fsm.Dispose();
				} );
			}
			public override void Create() {}
		}

		public abstract class TestState : SMState<object, TestState> {
			public TestState() {
				_setupEvent.AddLast().Subscribe( _ => {
					SMLog.Debug( $"{this.GetAboutName()}.{nameof( _setupEvent )}\n{this}" );
				} );

				_enterEvent.AddLast( async cancel => {
					SMLog.Debug( $"{this.GetAboutName()}.{nameof( _enterEvent )} : start\n{this}" );
					await UTask.Delay( cancel, 1000 );
					SMLog.Debug( $"{this.GetAboutName()}.{nameof( _enterEvent )} : end\n{this}" );
				} );
				_exitEvent.AddLast( async cancel => {
					SMLog.Debug( $"{this.GetAboutName()}.{nameof( _exitEvent )} : start\n{this}" );
					await UTask.Delay( cancel, 1000 );
					SMLog.Debug( $"{this.GetAboutName()}.{nameof( _exitEvent )} : end\n{this}" );
				} );

				_asyncUpdateEvent.AddLast( async cancel => {
					SMLog.Debug( $"{this.GetAboutName()}.{nameof( _asyncUpdateEvent )} : start\n{this}" );
					await UTask.Delay( cancel, 1000 );
					SMLog.Debug( $"{this.GetAboutName()}.{nameof( _asyncUpdateEvent )} : end\n{this}" );
				} );

				_fixedUpdateEvent.AddLast().Subscribe( _ => {
					SMLog.Debug( $"{this.GetAboutName()}.{nameof( _fixedUpdateEvent )}\n{this}" );
				} );
				_updateEvent.AddLast().Subscribe( _ => {
					SMLog.Debug( $"{this.GetAboutName()}.{nameof( _updateEvent )}\n{this}" );
				} );
				_lateUpdateEvent.AddLast().Subscribe( _ => {
					SMLog.Debug( $"{this.GetAboutName()}.{nameof( _lateUpdateEvent )}\n{this}" );
				} );
			}
		}

		public class TestStateA : TestState {
		}

		public class TestStateB : TestState {
		}

		public class TestStateC : TestState {
		}

		public class TestDummyState : SMState<object, TestDummyState> {
		}

		public class TestState2 : SMState<TestSMFSM, TestState2> {
		}
	}
}