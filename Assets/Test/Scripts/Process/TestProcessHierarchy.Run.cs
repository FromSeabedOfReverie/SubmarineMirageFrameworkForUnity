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
		IProcess _process;


/*
		・単独実行テスト
		ProcessHierarchy、1つの実行を確認

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
		void CreateTestRunB1() {
			SetProcessEvent( _process = (B1)TestProcessUtility.CreateMonoBehaviourProcess( @"B1") );
			SetTestRunKey();
		}
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunB1() => From( RunForever() );

		void CreateTestRunB2() {
			SetProcessEvent( _process = (B2)TestProcessUtility.CreateMonoBehaviourProcess( @"B2") );
			SetTestRunKey();
		}
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunB2() => From( RunForever() );
		
		void CreateTestRunB3() {
			SetProcessEvent( _process = (B3)TestProcessUtility.CreateMonoBehaviourProcess( @"B3") );
			SetTestRunKey();
		}
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunB3() => From( RunForever() );

		void CreateTestRunM1() {
			SetProcessEvent( _process = (M1)TestProcessUtility.CreateMonoBehaviourProcess( @"M1") );
			SetTestRunKey();
		}
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunM1() => From( RunForever() );

		void CreateTestRunM2() {
			SetProcessEvent( _process = (M2)TestProcessUtility.CreateMonoBehaviourProcess( @"M2") );
			SetTestRunKey();
		}
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunM2() => From( RunForever() );
		
		void CreateTestRunM3() {
			SetProcessEvent( _process = (M3)TestProcessUtility.CreateMonoBehaviourProcess( @"M3") );
			SetTestRunKey();
		}
		[UnityTest] [Timeout( int.MaxValue )] public IEnumerator TestRunM3() => From( RunForever() );

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
		void CreateTestRunBrothers() {
			TestProcessUtility.CreateMonoBehaviourProcess( @"
				M1, M2
			");
			_process = SceneManager.s_instance.GetProcess<M1>();
			SetProcessEvent( _process );
			SetTestRunKey();
		}

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestRunBrothers() => From( RunForever() );



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

		RunActiveEvent、実行
		gameObject.SetActive順序を確認、多分弄らないのが正しい
		最初にenable指定の場合、gameObjectもenableで関数実行、
		最初にdisable指定の場合、gameObjectもdisableで関数実行しない

		イベント実行中、活動状態変更の、兼ね合いテスト
*/
		void CreateTestParentAndChild() {
			TestProcessUtility.CreateMonoBehaviourProcess( @"
				M1,
					M2,
						M3,
			");
			_process = SceneManager.s_instance.GetProcess<M1>();
			SetProcessEvent( _process );
			SetTestRunKey();
		}

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestParentAndChild() => From( RunForever() );





		
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
					_process.RunStateEvent( RanState.Creating ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down Loading" );
					_process.RunStateEvent( RanState.Loading ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha3 ) ).Subscribe( _ => {
					Log.Warning( "key down Initializing" );
					_process.RunStateEvent( RanState.Initializing ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha4 ) ).Subscribe( _ => {
					Log.Warning( "key down FixedUpdate" );
					_process.RunStateEvent( RanState.FixedUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha5 ) ).Subscribe( _ => {
					Log.Warning( "key down Update" );
					_process.RunStateEvent( RanState.Update ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha6 ) ).Subscribe( _ => {
					Log.Warning( "key down LateUpdate" );
					_process.RunStateEvent( RanState.LateUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha7 ) ).Subscribe( _ => {
					Log.Warning( "key down Finalizing" );
					_process.RunStateEvent( RanState.Finalizing ).Forget();
				} )
			);
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Z ) ).Subscribe( _ => {
					Log.Warning( "key down Enabling" );
					_process.ChangeActive( true ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.X ) ).Subscribe( _ => {
					Log.Warning( "key down Disabling" );
					_process.ChangeActive( false ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.C ) ).Subscribe( _ => {
					Log.Warning( "key down RunActiveEvent" );
					_process.RunActiveEvent().Forget();
				} )
			);
		}


		void SetProcessEvent( IProcess process ) {
			process._loadEvent.AddLast( async cancel => {
				Log.Debug( $"start {process.GetAboutName()}._loadEvent" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"end {process.GetAboutName()}._loadEvent" );
			} );
			process._initializeEvent.AddLast( async cancel => {
				Log.Debug( $"start {process.GetAboutName()}._initializeEvent" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"end {process.GetAboutName()}._initializeEvent" );
			} );
			process._enableEvent.AddLast( async cancel => {
				Log.Debug( $"start {process.GetAboutName()}._enableEvent" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"end {process.GetAboutName()}._enableEvent" );
			} );
			process._fixedUpdateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{process.GetAboutName()}._fixedUpdateEvent" );
			} );
			process._updateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{process.GetAboutName()}._updateEvent" );
			} );
			process._lateUpdateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{process.GetAboutName()}._lateUpdateEvent" );
			} );
			process._disableEvent.AddLast( async cancel => {
				Log.Debug( $"start {process.GetAboutName()}._disableEvent" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"end {process.GetAboutName()}._disableEvent" );
			} );
			process._finalizeEvent.AddLast( async cancel => {
				Log.Debug( $"start {process.GetAboutName()}._finalizeEvent" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"end {process.GetAboutName()}._finalizeEvent" );
			} );
		}
	}
}