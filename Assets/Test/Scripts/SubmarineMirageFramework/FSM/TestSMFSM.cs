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


		// Initialize機能するか？
		// Type指定できる？、違うType指定だとどうなる？
		// 破棄済の場合
		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestInitialize() => From( async () => {
			var types = new Type[] { typeof( TestStateA ), typeof( TestStateB ), typeof( TestStateC ) };
			var states = types.Select( t => t.Create<TestState>() ).ToArray();

			var fsm = new SMFSM<TestState>();
			fsm.Initialize( this, states );		// 設定、状態を使用
			try {
				fsm.Initialize( this, states );	// 二度目の設定、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			try {
				states.First().Initialize( this, fsm );  // 状態が二度目の設定、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			var s = new TestState2();
			var temp = 0;
			try {
				s.Initialize( temp, fsm );       // 状態が_owner設定失敗、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			try {
				s.Initialize( this, temp );		// 状態が_fsm設定失敗、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			s.Dispose();
			fsm.Dispose();

			fsm = new SMFSM<TestState>();
			fsm.Initialize( this, types );		// 設定、型を使用
			try {
				fsm.Initialize( this, types );   // 二度目の設定、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			fsm.Dispose();

			fsm = new SMFSM<TestState>();
			var t = fsm.ChangeState<TestStateA>();	// 未実行
			SMLog.Debug( fsm );
			fsm.Initialize( this, types );
			SMLog.Debug( fsm );
			await t;								// 実行
			SMLog.Debug( fsm );
			fsm.Dispose();

			fsm = new SMFSM<TestState>();
			try {
				fsm.Initialize( this, new Type[] { typeof( TestDummyState ) } ); // 違う状態で設定、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			fsm.Dispose();

			try {
				states.First().Initialize( this, fsm );  // 状態が破棄済の設定、エラー
			} catch ( Exception e ) {
				SMLog.Error( e );
			}
			try {
				fsm.Initialize( this, types );	// 破棄後の設定、エラー
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
			fsm.Initialize( this, types );
			fsm.FixedUpdateState();				// 実行
			fsm.UpdateState();					// 実行
			fsm.LateUpdateState();				// 実行
			fsm.Dispose();

			var task = new TestOwnerTask();
			task._fsm.Initialize( task, types );
			await UTask.WaitWhile( _asyncCanceler, () => task._ranState != SMTaskRunState.LateUpdate ); // 実行
			task.Dispose();

			task = new TestOwnerTask();
			var updates = new SMSubject[] { new SMSubject(), new SMSubject(), new SMSubject() };
			updates.ForEach( ( e, i ) => e.AddLast().Subscribe( _ => SMLog.Debug( $"updates[{i}]" ) ) );
			task._fsm.Initialize( task, types, updates[0], updates[1], updates[2] );
			await UTask.WaitWhile( _asyncCanceler, () => task._ranState != SMTaskRunState.LateUpdate ); // 未実行
			updates.ForEach( e => e.Run() );	// 実行
			task.Dispose();
			updates.ForEach( e => e.Run() );    // 未実行
			updates.ForEach( e => e.Dispose() );

			task = new TestOwnerTask();
			updates = new SMSubject[] { new SMSubject(), new SMSubject(), new SMSubject() };
			updates.ForEach( ( e, i ) => e.AddLast().Subscribe( _ => SMLog.Debug( $"updates[{i}]" ) ) );
			task._fsm.Initialize( task, types, updates[0], updates[1], updates[2] );
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
			fsm.Initialize( this, types );
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
			fsm.Initialize( this, new Type[] {} );
			SMLog.Debug( fsm.GetStates().ToShowString() );			// 無取得
			SMLog.Debug( fsm.GetState( typeof( TestStateB ) ) );    // 無取得
			SMLog.Debug( fsm.GetState<TestStateB>() );              // 無取得
			fsm.Dispose();

			await UTask.DontWait();
		} );


		// 正しくイベント実行される？
		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestEvent() => From( async () => {
			var types = new Type[] { typeof( TestStateA ), typeof( TestStateB ), typeof( TestStateC ) };

			var task = new TestOwnerTask();
			task._fsm.Initialize( task, types );		// 実行
			await UTask.WaitWhile( _asyncCanceler, () => task._ranState != SMTaskRunState.LateUpdate );
			await task._fsm.ChangeState<TestStateA>();  // 実行
			await UTask.Delay( _asyncCanceler, 500 );   // _asyncUpdateEvent、途中停止
			await task._fsm.ChangeState<TestStateA>();  // 実行
			await UTask.Delay( _asyncCanceler, 1000 );  // _asyncUpdateEvent、実行完了
			await task._fsm.ChangeState<TestStateB>();  // 実行
			await task._fsm.ChangeState<TestStateC>();  // 実行
			task.Dispose();
		} );


		// ChangeStateのテスト
		// キャンセルも確認
		[UnityTest, Timeout( int.MaxValue )]
		public IEnumerator TestChangeState() => From( async () => {
			var types = new Type[] { typeof( TestStateA ), typeof( TestStateB ), typeof( TestStateC ) };
			var task = new TestOwnerTask();
			task._fsm._asyncCancelerOnExit._cancelEvent.AddLast().Subscribe( _ =>
				SMLog.Debug( $"{nameof( task._fsm._asyncCancelerOnExit )}" ) );
			task._fsm._asyncCancelerOnDispose._cancelEvent.AddLast().Subscribe( _ =>
				SMLog.Debug( $"{nameof( task._fsm._asyncCancelerOnDispose )}" ) );

			var t = task._fsm.ChangeState<TestStateA>();	// 未実行
			task._fsm.Initialize( task, types );
			await t;										// 実行

			await task._fsm.ChangeState( null );			// 実行
			await task._fsm.ChangeState( null );			// 未実行

			await task._fsm.ChangeState<TestStateA>();		// 実行
			await task._fsm.ChangeState<TestStateA>();		// 実行

			task._fsm.ChangeState<TestStateB>().Forget();	// 実行
			await task._fsm.ChangeState<TestStateC>();		// 実行

			task._fsm.ChangeState<TestStateA>().Forget();   // 実行
			SMLog.Debug( task._fsm );
			task._fsm.ChangeState<TestStateB>().Forget();   // 未実行
			SMLog.Debug( task._fsm );
			task._fsm.ChangeState<TestStateC>().Forget();   // 未実行
			SMLog.Debug( task._fsm );
			task._fsm.ChangeState<TestStateA>().Forget();   // 未実行
			SMLog.Debug( task._fsm );
			task._fsm.ChangeState<TestStateB>().Forget();   // 未実行
			SMLog.Debug( task._fsm );
			task._fsm.ChangeState<TestStateC>().Forget();   // 未実行
			SMLog.Debug( task._fsm );
			await task._fsm.ChangeState<TestStateA>();		// 実行

			task._fsm.ChangeState( null ).Forget();			// 実行
			await task._fsm.ChangeState<TestStateA>();      // 実行

			task._fsm.ChangeState<TestStateB>().Forget();   // 実行
			await task._fsm.ChangeState( null );            // 実行

			task._fsm.ChangeState<TestStateA>().Forget();   // 実行
			await task._fsm.ChangeState( null );            // 実行

			task._fsm.ChangeState( null ).Forget();         // 実行
			await task._fsm.ChangeState<TestStateA>();      // 実行

			task.Dispose();
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
				_initializeEvent.AddLast().Subscribe( _ => {
					SMLog.Debug( $"{this.GetName()}.{nameof( _initializeEvent )}\n{this}" );
				} );

				_enterEvent.AddLast( async cancel => {
					SMLog.Debug( $"{this.GetName()}.{nameof( _enterEvent )} : start\n{this}" );
					await UTask.Delay( cancel, 1000 );
					SMLog.Debug( $"{this.GetName()}.{nameof( _enterEvent )} : end\n{this}" );
				} );
				_exitEvent.AddLast( async cancel => {
					SMLog.Debug( $"{this.GetName()}.{nameof( _exitEvent )} : start\n{this}" );
					await UTask.Delay( cancel, 1000 );
					SMLog.Debug( $"{this.GetName()}.{nameof( _exitEvent )} : end\n{this}" );
				} );

				_asyncUpdateEvent.AddLast( async cancel => {
					SMLog.Debug( $"{this.GetName()}.{nameof( _asyncUpdateEvent )} : start\n{this}" );
					await UTask.Delay( cancel, 1000 );
					SMLog.Debug( $"{this.GetName()}.{nameof( _asyncUpdateEvent )} : end\n{this}" );
				} );

				_fixedUpdateEvent.AddLast().Subscribe( _ => {
					SMLog.Debug( $"{this.GetName()}.{nameof( _fixedUpdateEvent )}\n{this}" );
				} );
				_updateEvent.AddLast().Subscribe( _ => {
					SMLog.Debug( $"{this.GetName()}.{nameof( _updateEvent )}\n{this}" );
				} );
				_lateUpdateEvent.AddLast().Subscribe( _ => {
					SMLog.Debug( $"{this.GetName()}.{nameof( _lateUpdateEvent )}\n{this}" );
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