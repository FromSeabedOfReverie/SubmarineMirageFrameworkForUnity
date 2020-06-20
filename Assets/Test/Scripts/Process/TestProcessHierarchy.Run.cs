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
	using SMTask;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using Test;
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public partial class TestProcessHierarchy : Test {
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
			_process = new B1();
			TestProcessUtility.SetEvent( _process );
			_disposables.AddLast( TestProcessUtility.SetRunKey( _process._hierarchy ) );
		}
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunB1() => From( RunForever() );

		void CreateTestRunB2() {
			_process = new B2();
			TestProcessUtility.SetEvent( _process );
			_disposables.AddLast( TestProcessUtility.SetRunKey( _process._hierarchy ) );
		}
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunB2() => From( RunForever() );
		
		void CreateTestRunB3() {
			_process = new B3();
			TestProcessUtility.SetEvent( _process );
			_disposables.AddLast( TestProcessUtility.SetRunKey( _process._hierarchy ) );
		}
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunB3() => From( RunForever() );


		void CreateTestRunM1()
			=> _process = TestProcessUtility.CreateMonoBehaviourProcess( @"false, M1" );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunM1() => From( TestRunM1Sub() );
		IEnumerator TestRunM1Sub() {
			TestProcessUtility.SetEvent( _process );
			_disposables.AddLast( TestProcessUtility.SetRunKey( _process._hierarchy ) );
			yield return RunForever();
		}

		void CreateTestRunM2()
			=> _process = TestProcessUtility.CreateMonoBehaviourProcess( @"false, M2" );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunM2() => From( TestRunM2Sub() );
		IEnumerator TestRunM2Sub() {
			TestProcessUtility.SetEvent( _process );
			_disposables.AddLast( TestProcessUtility.SetRunKey( _process._hierarchy ) );
			yield return RunForever();
		}
		
		void CreateTestRunM3()
			=> _process = TestProcessUtility.CreateMonoBehaviourProcess( @"true, M3" );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunM3() => From( TestRunM3Sub() );
		IEnumerator TestRunM3Sub() {
			TestProcessUtility.SetEvent( _process );
			_disposables.AddLast( TestProcessUtility.SetRunKey( _process._hierarchy ) );
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
			true, M1, M4,
		" );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunBrothers1() => From( TestRunBrothers1Sub() );
		IEnumerator TestRunBrothers1Sub() {
			_process._hierarchy._processes.ForEach( p => TestProcessUtility.SetEvent( p ) );
			_disposables.AddLast( TestProcessUtility.SetRunKey( _process._hierarchy ) );
			yield return RunForever();
		}

		void CreateTestRunBrothers2() => _process = TestProcessUtility.CreateMonoBehaviourProcess( @"
			true, M1, M2, M2,
		" );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunBrothers2() => From( TestRunBrothers2Sub() );
		IEnumerator TestRunBrothers2Sub() {
			_process._hierarchy._processes.ForEach( p => TestProcessUtility.SetEvent( p ) );
			_disposables.AddLast( TestProcessUtility.SetRunKey( _process._hierarchy ) );
			yield return RunForever();
		}

		void CreateTestRunBrothers3() => _process = TestProcessUtility.CreateMonoBehaviourProcess( @"
			true, M1, M2, M3,
		" );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunBrothers3() => From( TestRunBrothers3Sub() );
		IEnumerator TestRunBrothers3Sub() {
			_process._hierarchy._processes.ForEach( p => TestProcessUtility.SetEvent( p ) );
			_disposables.AddLast( TestProcessUtility.SetRunKey( _process._hierarchy ) );
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

		RunActiveEvent、実行
		gameObject.SetActive順序を確認、多分弄らないのが正しい
		最初にenable指定の場合、gameObjectもenableで関数実行、
		最初にdisable指定の場合、gameObjectもdisableで関数実行しない

		イベント実行中、活動状態変更の、兼ね合いテスト
*/
		void CreateTestRunChildren1() => _process = TestProcessUtility.CreateMonoBehaviourProcess( @"
			true, M1, M1,
				M1, M1,
					M1, M1,
		");
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunChildren1() => From( TestRunChildren1Sub() );
		IEnumerator TestRunChildren1Sub() {
			_process._hierarchy.GetProcessesInChildren<ISMBehavior>().ForEach( p => TestProcessUtility.SetEvent( p ) );
			_disposables.AddLast( TestProcessUtility.SetRunKey( _process._hierarchy ) );
			yield return RunForever();
		}

		void CreateTestRunChildren2() => _process = TestProcessUtility.CreateMonoBehaviourProcess( @"
			true, M1, M1,
				M2, M2,
					M2, M2,
		" );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunChildren2() => From( TestRunChildren2Sub() );
		IEnumerator TestRunChildren2Sub() {
			_process._hierarchy.GetProcessesInChildren<ISMBehavior>().ForEach( p => TestProcessUtility.SetEvent( p ) );
			_disposables.AddLast( TestProcessUtility.SetRunKey( _process._hierarchy ) );
			yield return RunForever();
		}

		void CreateTestRunChildren3() => _process = TestProcessUtility.CreateMonoBehaviourProcess( @"
			false, M1, M1,
				M2, M2,
					M3, M3,
		" );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunChildren3() => From( TestRunChildren3Sub() );
		IEnumerator TestRunChildren3Sub() {
			_process._hierarchy.GetProcessesInChildren<ISMBehavior>().ForEach( p => TestProcessUtility.SetEvent( p ) );
			_disposables.AddLast( TestProcessUtility.SetRunKey( _process._hierarchy ) );
			yield return RunForever();
		}

/*
		・親子実行テスト
		子だけ、ChangeActive( isChangeOwner )して、RunStateEvent等、上手く動作するか？
		enable、disable時、親の状態を考慮

		e, e	→	e, d'	d'可能
		e, d'	→	e, e	e可能
		d',e	→	d',d'	不可（遷移元の状態にならない）
		d',d	→	d',e	不可

		e, e, e		→	e, e, e		不可（重複）
					→	e, d',d		d'd可能
		e, e, d'	→	e, e, d'	不可（重複）
					→	e, d',d'	d'可能
		e, d',e		→	e, d',d		不可（遷移元の状態にならない）
					→	e, e, e		不可（遷移元の状態にならない）
		e, d',d		→	e, d',d		不可（重複）
					→	e, e, e		ee可能
		e, d',d'	→	e, d',d'	不可（重複）
					→	e, e, d'	e可能
		d',e, e		→	d',e, e		不可（遷移元の状態にならない）
					→	d',d',d		不可（遷移元の状態にならない）
		d',e, d'	→	d',e, d'	不可（遷移元の状態にならない）
					→	d',d',d'	不可（遷移元の状態にならない）
		d',d, e		→	d',d',d		不可（遷移元の状態にならない）
					→	d',e, d		不可（遷移元の状態にならない）
		d',d, d		→	d',d',d		不可（重複）
					→	d',e, d		不可
		d',d',d		→	d',d',d		不可（重複）
					→	d',e, d		不可
		d',d',d'	→	d',d',d'	不可（重複）
					→	d',e, d'	不可
*/
		void CreateTestRunByActiveState1() {
			_process = TestProcessUtility.CreateMonoBehaviourProcess( @"
				true, M1, M1,
					true, M1, M1,
			" );
		}
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRunByActiveState1() => From( TestRunByActiveState1Sub() );
		IEnumerator TestRunByActiveState1Sub() {
			_process._hierarchy.GetProcessesInChildren<ISMBehavior>().ForEach( p => TestProcessUtility.SetEvent( p ) );
			_disposables.AddLast(
				TestProcessUtility.SetRunKey( _process._hierarchy ),
				TestProcessUtility.SetChangeActiveKey( _process._hierarchy._children.First() )
			);
			yield return RunForever();
		}

		void CreateTestRunByActiveState2() {
			_process = TestProcessUtility.CreateMonoBehaviourProcess( @"
				true, M2, M2,
					true, M2, M2,
			" );
		}
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRunByActiveState2() => From( TestRunByActiveState2Sub() );
		IEnumerator TestRunByActiveState2Sub() {
			_process._hierarchy.GetProcessesInChildren<ISMBehavior>().ForEach( p => TestProcessUtility.SetEvent( p ) );
			_disposables.AddLast(
				TestProcessUtility.SetRunKey( _process._hierarchy ),
				TestProcessUtility.SetChangeActiveKey( _process._hierarchy._children.First() )
			);
			yield return RunForever();
		}

		void CreateTestRunByActiveState3() {
			_process = TestProcessUtility.CreateMonoBehaviourProcess( @"
				true, M3, M3,
					true, M3, M3,
			" );
		}
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRunByActiveState3() => From( TestRunByActiveState3Sub() );
		IEnumerator TestRunByActiveState3Sub() {
			_process._hierarchy.GetProcessesInChildren<ISMBehavior>().ForEach( p => TestProcessUtility.SetEvent( p ) );
			_disposables.AddLast(
				TestProcessUtility.SetRunKey( _process._hierarchy ),
				TestProcessUtility.SetChangeActiveKey( _process._hierarchy._children.First() )
			);
			yield return RunForever();
		}




		




		
/*
		・実行中エラーテスト
		実行中に、Process内で、エラーが出たら？
		イベント実行中
		活動状態変更中
		活動イベント実行中
*/
	}
}