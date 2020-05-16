//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestProcess {
	using System;
	using System.Linq;
	using System.Threading;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.TestTools;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using Process.New;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using Test;
	using UnityObject = UnityEngine.Object;
	using RanState = Process.New.ProcessBody.RanState;
	using ActiveState = Process.New.ProcessBody.ActiveState;



	// TODO : コメント追加、整頓



	public partial class TestProcessHierarchy : Test {
		ProcessHierarchy _hierarchy;
		IProcess _process;


/*
		・単独実行テスト
		ProcessHierarchy、1つの実行を確認

		RunStateEvent、_type別、全stateを実行

		ChangeActive、_type別、全isActive、isChangeOwnerを実行
		gameObject.SetActive順序を確認
		disable時に、実行反転を確認

		RunActiveEvent、実行
		gameObject.SetActive順序を確認、多分弄らないのが正しい
		最初にenable指定の場合、gameObjectもenableで関数実行、
		最初にdisable指定の場合、gameObjectもdisableで関数実行しない

		イベント実行中、活動状態変更の、兼ね合いテスト
*/
		void CreateTestRunB1() {
			var p = new B1();
			_hierarchy = p._hierarchy;
			SetProcessEvent( p );
			SetTestRunKey();
		}
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunB1() => From( RunForever() );

		void CreateTestRunB2() {
			var p = new B2();
			_hierarchy = p._hierarchy;
			SetProcessEvent( p );
			SetTestRunKey();
		}
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunB2() => From( RunForever() );
		
		void CreateTestRunB3() {
			var p = new B3();
			_hierarchy = p._hierarchy;
			SetProcessEvent( p );
			SetTestRunKey();
		}
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunB3() => From( RunForever() );


		void CreateTestRunM1()
			=> _process = TestProcessUtility.CreateMonoBehaviourProcess( @"M1", false );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunM1() => From( TestRunM1Sub() );
		IEnumerator TestRunM1Sub() {
			_hierarchy = _process._hierarchy;
			SetProcessEvent( _process );
			SetTestRunKey();
			yield return RunForever();
		}

		void CreateTestRunM2()
			=> _process = TestProcessUtility.CreateMonoBehaviourProcess( @"M2", false );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunM2() => From( TestRunM2Sub() );
		IEnumerator TestRunM2Sub() {
			_hierarchy = _process._hierarchy;
			SetProcessEvent( _process );
			SetTestRunKey();
			yield return RunForever();
		}
		
		void CreateTestRunM3()
			=> _process = TestProcessUtility.CreateMonoBehaviourProcess( @"M3", true );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunM3() => From( TestRunM3Sub() );
		IEnumerator TestRunM3Sub() {
			_hierarchy = _process._hierarchy;
			SetProcessEvent( _process );
			SetTestRunKey();
			yield return RunForever();
		}

/*
		・兄弟実行テスト
		ProcessHierarchy、兄弟2つの実行を確認

		RunStateEvent、_type別、全stateを実行
		finalizeEventの、実行順序反転を確認

		ChangeActive、_type別、全isActive、isChangeOwnerを実行
		gameObject.SetActive順序を確認
		disable時に、実行反転を確認

		RunActiveEvent、実行
		gameObject.SetActive順序を確認、多分弄らないのが正しい
		最初にenable指定の場合、gameObjectもenableで関数実行、
		最初にdisable指定の場合、gameObjectもdisableで関数実行しない

		イベント実行中、活動状態変更の、兼ね合いテスト
*/
		void CreateTestRunBrothers1() => _process = TestProcessUtility.CreateMonoBehaviourProcess( @"
			M1, M4,
		" );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunBrothers1() => From( TestRunBrothers1Sub() );
		IEnumerator TestRunBrothers1Sub() {
			_hierarchy = _process._hierarchy;
			_hierarchy._processes.ForEach( p => SetProcessEvent( p ) );
			SetTestRunKey();
			yield return RunForever();
		}

		void CreateTestRunBrothers2() => _process = TestProcessUtility.CreateMonoBehaviourProcess( @"
			M1, M2, M2,
		", true );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunBrothers2() => From( TestRunBrothers2Sub() );
		IEnumerator TestRunBrothers2Sub() {
			_hierarchy = _process._hierarchy;
			_hierarchy._processes.ForEach( p => SetProcessEvent( p ) );
			SetTestRunKey();
			yield return RunForever();
		}

		void CreateTestRunBrothers3() => _process = TestProcessUtility.CreateMonoBehaviourProcess( @"
			M1, M2, M3,
		", true );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunBrothers3() => From( TestRunBrothers3Sub() );
		IEnumerator TestRunBrothers3Sub() {
			_hierarchy = _process._hierarchy;
			_hierarchy._processes.ForEach( p => SetProcessEvent( p ) );
			SetTestRunKey();
			yield return RunForever();
		}



/*
		・親子実行テスト
		ProcessHierarchy、親子3つの実行を確認

		RunStateEvent、_type別、全stateを実行
		finalizeEventの、実行順序反転を確認

		ChangeActive、_type別、全isActive、isChangeOwnerを実行
		gameObject.SetActive順序を確認
		disable時に、実行反転を確認
		enable、disable時、親の状態を考慮
		親enable　→　子enableの場合、内部で重複実行できない
		親enable　→　子disableの場合、正常実行
		親disable　→　子disableの場合、内部で重複実行できない
		親disable　→　子enableの場合、実行させないようにする
		子だけ、ChangeActive( isChangeOwner )して、RunStateEvent等、上手く動作するか？

		RunActiveEvent、実行
		gameObject.SetActive順序を確認、多分弄らないのが正しい
		最初にenable指定の場合、gameObjectもenableで関数実行、
		最初にdisable指定の場合、gameObjectもdisableで関数実行しない

		イベント実行中、活動状態変更の、兼ね合いテスト
*/
		void CreateTestRunChildren1() => _process = TestProcessUtility.CreateMonoBehaviourProcess( @"
			M1, M1,
				M1, M1,
					M1, M1,
		");
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunChildren1() => From( TestRunChildren1Sub() );
		IEnumerator TestRunChildren1Sub() {
			_hierarchy = _process._hierarchy;
			_hierarchy.GetProcessesInChildren<IProcess>().ForEach( p => SetProcessEvent( p ) );
			SetTestRunKey();
			yield return RunForever();
		}

		void CreateTestRunChildren2() => _process = TestProcessUtility.CreateMonoBehaviourProcess( @"
			M1, M1,
				M2, M2,
					M2, M2,
		");
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunChildren2() => From( TestRunChildren2Sub() );
		IEnumerator TestRunChildren2Sub() {
			_hierarchy = _process._hierarchy;
			_hierarchy.GetProcessesInChildren<IProcess>().ForEach( p => SetProcessEvent( p ) );
			SetTestRunKey();
			yield return RunForever();
		}

		void CreateTestRunChildren3() => _process = TestProcessUtility.CreateMonoBehaviourProcess( @"
			M1, M1,
				M2, M2,
					M3, M3,
		");
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunChildren3() => From( TestRunChildren3Sub() );
		IEnumerator TestRunChildren3Sub() {
			_hierarchy = _process._hierarchy;
			_hierarchy.GetProcessesInChildren<IProcess>().ForEach( p => SetProcessEvent( p ) );
			SetTestRunKey();
			yield return RunForever();
		}




		
/*
		・実行中エラーテスト
		実行中に、Process内で、エラーが出たら？
		イベント実行中
		活動状態変更中
		活動イベント実行中
*/



		void SetTestRunKey() {
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( "key down Creating" );
					_hierarchy.RunStateEvent( RanState.Creating ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down Loading" );
					_hierarchy.RunStateEvent( RanState.Loading ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha3 ) ).Subscribe( _ => {
					Log.Warning( "key down Initializing" );
					_hierarchy.RunStateEvent( RanState.Initializing ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha4 ) ).Subscribe( _ => {
					Log.Warning( "key down FixedUpdate" );
					_hierarchy.RunStateEvent( RanState.FixedUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha5 ) ).Subscribe( _ => {
					Log.Warning( "key down Update" );
					_hierarchy.RunStateEvent( RanState.Update ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha6 ) ).Subscribe( _ => {
					Log.Warning( "key down LateUpdate" );
					_hierarchy.RunStateEvent( RanState.LateUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha7 ) ).Subscribe( _ => {
					Log.Warning( "key down Finalizing" );
					_hierarchy.RunStateEvent( RanState.Finalizing ).Forget();
				} )
			);
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.A ) ).Subscribe( _ => {
					Log.Warning( "key down Enabling Owner" );
					_hierarchy.ChangeActive( true, true ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.S ) ).Subscribe( _ => {
					Log.Warning( "key down Disabling Owner" );
					_hierarchy.ChangeActive( false, true ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Z ) ).Subscribe( _ => {
					Log.Warning( "key down Enabling" );
					_hierarchy.ChangeActive( true, false ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.X ) ).Subscribe( _ => {
					Log.Warning( "key down Disabling" );
					_hierarchy.ChangeActive( false, false ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.C ) ).Subscribe( _ => {
					Log.Warning( "key down RunActiveEvent" );
					_hierarchy.RunActiveEvent().Forget();
				} )
			);
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down Dispose" );
					_hierarchy.Dispose();
					_hierarchy = null;
				} )
			);
		}


		void SetProcessEvent( IProcess process ) {
			var name = process.GetAboutName();
			var id = (
				process is BaseM	? ( (BaseM)process )._id :
				process is BaseB	? ( (BaseB)process )._id
									: (int?)null
			);

			process._loadEvent.AddLast( async cancel => {
				Log.Debug( $"{name}( {id} )._loadEvent : start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"{name}( {id} )._loadEvent : end" );
			} );
			process._initializeEvent.AddLast( async cancel => {
				Log.Debug( $"{name}( {id} )._initializeEvent : start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"{name}( {id} )._initializeEvent : end" );
			} );
			process._enableEvent.AddLast( async cancel => {
				Log.Debug( $"{name}( {id} )._enableEvent : start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"{name}( {id} )._enableEvent : end" );
			} );
			process._fixedUpdateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{name}( {id} )._fixedUpdateEvent" );
			} );
			process._updateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{name}( {id} )._updateEvent" );
			} );
			process._lateUpdateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{name}( {id} )._lateUpdateEvent" );
			} );
			process._disableEvent.AddLast( async cancel => {
				Log.Debug( $"{name}( {id} )._disableEvent : start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"{name}( {id} )._disableEvent : end" );
			} );
			process._finalizeEvent.AddLast( async cancel => {
				Log.Debug( $"{name}( {id} )._finalizeEvent : start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"{name}( {id} )._finalizeEvent : end" );
			} );
		}
	}
}