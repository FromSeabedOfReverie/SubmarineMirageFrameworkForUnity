//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestSMTask {
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
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using SMTask;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using Test;
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public partial class TestSMObject : Test {
/*
		・単独実行テスト
		SMObject、1つの実行を確認

		RunStateSMObject、_type別、全stateを実行

		ChangeActiveSMObject、_type別、全isActive、isChangeOwnerを実行
		gameObject.SetActive順序を確認
		disable時に、実行反転を確認

		RunActiveSMObject、実行
		gameObject.SetActive順序を確認、多分弄らないのが正しい
		最初にenable指定の場合、gameObjectもenableで関数実行、
		最初にdisable指定の場合、gameObjectもdisableで関数実行しない

		イベント実行中、活動状態変更の、兼ね合いテスト
*/
		void CreateTestRunB1() {
			_behaviour = new B1();
			TestSMTaskUtility.SetEvent( _behaviour );
			_disposables.AddLast( TestSMTaskUtility.SetRunKey( _behaviour._object ) );
		}
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunB1() => From( RunForever() );

		void CreateTestRunB2() {
			_behaviour = new B2();
			TestSMTaskUtility.SetEvent( _behaviour );
			_disposables.AddLast( TestSMTaskUtility.SetRunKey( _behaviour._object ) );
		}
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunB2() => From( RunForever() );
		
		void CreateTestRunB3() {
			_behaviour = new B3();
			TestSMTaskUtility.SetEvent( _behaviour );
			_disposables.AddLast( TestSMTaskUtility.SetRunKey( _behaviour._object ) );
		}
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunB3() => From( RunForever() );


		void CreateTestRunM1()
			=> _behaviour = TestSMTaskUtility.CreateSMMonoBehaviour( @"false, M1" );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunM1() => From( TestRunM1Sub() );
		IEnumerator TestRunM1Sub() {
			TestSMTaskUtility.SetEvent( _behaviour );
			_disposables.AddLast( TestSMTaskUtility.SetRunKey( _behaviour._object ) );
			yield return RunForever();
		}

		void CreateTestRunM2()
			=> _behaviour = TestSMTaskUtility.CreateSMMonoBehaviour( @"false, M2" );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunM2() => From( TestRunM2Sub() );
		IEnumerator TestRunM2Sub() {
			TestSMTaskUtility.SetEvent( _behaviour );
			_disposables.AddLast( TestSMTaskUtility.SetRunKey( _behaviour._object ) );
			yield return RunForever();
		}
		
		void CreateTestRunM3()
			=> _behaviour = TestSMTaskUtility.CreateSMMonoBehaviour( @"true, M3" );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunM3() => From( TestRunM3Sub() );
		IEnumerator TestRunM3Sub() {
			TestSMTaskUtility.SetEvent( _behaviour );
			_disposables.AddLast( TestSMTaskUtility.SetRunKey( _behaviour._object ) );
			yield return RunForever();
		}

/*
		・兄弟実行テスト
		SMObject、兄弟2つの実行を確認

		RunStateSMObject、_type別、全stateを実行
		finalizeEventの、実行順序反転を確認

		ChangeActiveSMObject、_type別、全isActive、isChangeOwnerを実行
		gameObject.SetActive順序を確認
		disable時に、実行反転を確認

		RunActiveSMObject、実行
		gameObject.SetActive順序を確認、多分弄らないのが正しい
		最初にenable指定の場合、gameObjectもenableで関数実行、
		最初にdisable指定の場合、gameObjectもdisableで関数実行しない

		イベント実行中、活動状態変更の、兼ね合いテスト
*/
		void CreateTestRunBrothers1() => _behaviour = TestSMTaskUtility.CreateSMMonoBehaviour( @"
			true, M1, M4,
		" );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunBrothers1() => From( TestRunBrothers1Sub() );
		IEnumerator TestRunBrothers1Sub() {
			_behaviour._object.GetBehaviours().ForEach( b => TestSMTaskUtility.SetEvent( b ) );
			_disposables.AddLast( TestSMTaskUtility.SetRunKey( _behaviour._object ) );
			yield return RunForever();
		}

		void CreateTestRunBrothers2() => _behaviour = TestSMTaskUtility.CreateSMMonoBehaviour( @"
			true, M1, M2, M2,
		" );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunBrothers2() => From( TestRunBrothers2Sub() );
		IEnumerator TestRunBrothers2Sub() {
			_behaviour._object.GetBehaviours().ForEach( b => TestSMTaskUtility.SetEvent( b ) );
			_disposables.AddLast( TestSMTaskUtility.SetRunKey( _behaviour._object ) );
			yield return RunForever();
		}

		void CreateTestRunBrothers3() => _behaviour = TestSMTaskUtility.CreateSMMonoBehaviour( @"
			true, M1, M2, M3,
		" );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunBrothers3() => From( TestRunBrothers3Sub() );
		IEnumerator TestRunBrothers3Sub() {
			_behaviour._object.GetBehaviours().ForEach( b => TestSMTaskUtility.SetEvent( b ) );
			_disposables.AddLast( TestSMTaskUtility.SetRunKey( _behaviour._object ) );
			yield return RunForever();
		}



/*
		・親子実行テスト
		SMObject、親子3つの実行を確認

		RunStateSMObject、_type別、全stateを実行
		finalizeEventの、実行順序反転を確認

		ChangeActiveSMObject、_type別、全isActive、isChangeOwnerを実行
		gameObject.SetActive順序を確認
		disable時に、実行反転を確認

		RunActiveSMObject、実行
		gameObject.SetActive順序を確認、多分弄らないのが正しい
		最初にenable指定の場合、gameObjectもenableで関数実行、
		最初にdisable指定の場合、gameObjectもdisableで関数実行しない

		イベント実行中、活動状態変更の、兼ね合いテスト
*/
		void CreateTestRunChildren1() => _behaviour = TestSMTaskUtility.CreateSMMonoBehaviour( @"
			true, M1, M1,
				M1, M1,
					M1, M1,
		");
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunChildren1() => From( TestRunChildren1Sub() );
		IEnumerator TestRunChildren1Sub() {
			_behaviour._object.GetBehavioursInChildren<ISMBehaviour>().ForEach( b => TestSMTaskUtility.SetEvent( b ) );
			_disposables.AddLast( TestSMTaskUtility.SetRunKey( _behaviour._object ) );
			yield return RunForever();
		}

		void CreateTestRunChildren2() => _behaviour = TestSMTaskUtility.CreateSMMonoBehaviour( @"
			true, M1, M1,
				M2, M2,
					M2, M2,
		" );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunChildren2() => From( TestRunChildren2Sub() );
		IEnumerator TestRunChildren2Sub() {
			_behaviour._object.GetBehavioursInChildren<ISMBehaviour>().ForEach( b => TestSMTaskUtility.SetEvent( b ) );
			_disposables.AddLast( TestSMTaskUtility.SetRunKey( _behaviour._object ) );
			yield return RunForever();
		}

		void CreateTestRunChildren3() => _behaviour = TestSMTaskUtility.CreateSMMonoBehaviour( @"
			false, M1, M1,
				M2, M2,
					M3, M3,
		" );
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunChildren3() => From( TestRunChildren3Sub() );
		IEnumerator TestRunChildren3Sub() {
			_behaviour._object.GetBehavioursInChildren<ISMBehaviour>().ForEach( b => TestSMTaskUtility.SetEvent( b ) );
			_disposables.AddLast( TestSMTaskUtility.SetRunKey( _behaviour._object ) );
			yield return RunForever();
		}

/*
		・親子実行テスト
		子だけ、ChangeActiveSMObject( isChangeOwner )して、RunStateEvent等、上手く動作するか？
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
			_behaviour = TestSMTaskUtility.CreateSMMonoBehaviour( @"
				true, M1, M1,
					true, M1, M1,
			" );
		}
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRunByActiveState1() => From( TestRunByActiveState1Sub() );
		IEnumerator TestRunByActiveState1Sub() {
			_behaviour._object.GetBehavioursInChildren<ISMBehaviour>().ForEach( b => TestSMTaskUtility.SetEvent( b ) );
			_disposables.AddLast(
				TestSMTaskUtility.SetRunKey( _behaviour._object ),
				TestSMTaskUtility.SetChangeActiveKey( _behaviour._object._child )
			);
			yield return RunForever();
		}

		void CreateTestRunByActiveState2() {
			_behaviour = TestSMTaskUtility.CreateSMMonoBehaviour( @"
				true, M2, M2,
					true, M2, M2,
			" );
		}
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRunByActiveState2() => From( TestRunByActiveState2Sub() );
		IEnumerator TestRunByActiveState2Sub() {
			_behaviour._object.GetBehavioursInChildren<ISMBehaviour>().ForEach( b => TestSMTaskUtility.SetEvent( b ) );
			_disposables.AddLast(
				TestSMTaskUtility.SetRunKey( _behaviour._object ),
				TestSMTaskUtility.SetChangeActiveKey( _behaviour._object._child )
			);
			yield return RunForever();
		}

		void CreateTestRunByActiveState3() {
			_behaviour = TestSMTaskUtility.CreateSMMonoBehaviour( @"
				true, M3, M3,
					true, M3, M3,
			" );
		}
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRunByActiveState3() => From( TestRunByActiveState3Sub() );
		IEnumerator TestRunByActiveState3Sub() {
			_behaviour._object.GetBehavioursInChildren<ISMBehaviour>().ForEach( b => TestSMTaskUtility.SetEvent( b ) );
			_disposables.AddLast(
				TestSMTaskUtility.SetRunKey( _behaviour._object ),
				TestSMTaskUtility.SetChangeActiveKey( _behaviour._object._child )
			);
			yield return RunForever();
		}




		




		
/*
		・実行中エラーテスト
		実行中に、SMBehaviour内で、エラーが出たら？
		イベント実行中
		活動状態変更中
		活動イベント実行中
*/
	}
}