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



	public class TestProcessHierarchy : Test {
		SceneManager _process;
		Text _text;


		protected override void Create() {
			Application.targetFrameRate = 30;
			_process = SceneManager.s_instance;

			UnityObject.Instantiate( Resources.Load<GameObject>( "TestCamera" ) );
			var go = UnityObject.Instantiate( Resources.Load<GameObject>( "TestCanvas" ) );
			UnityObject.DontDestroyOnLoad( go );
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ => {
				if ( _process == null ) {
					_text.text = string.Empty;
					return;
				}
				_text.text =
					$"{_process.GetAboutName()}(\n"
					+ $"    _isInitialized : {_process._isInitialized}\n"
					+ $"    _isActive : {_process._isActive}\n"
					+ $"    _ranState : {_process._body._ranState}\n"
					+ $"    _activeState : {_process._body._activeState}\n"
					+ $"    next : {_process._body._nextActiveState}\n"
					+ $")\n"
					+ $"_fsm : {_process._fsm}";
			} ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_createEvent.AddLast( async cancel => {
				Log.Debug( $"start Create{_testName}" );
				switch ( _testName ) {
					case "TestVariable":		CreateTestVariable();		break;
					case "TestMultiVariable":	CreateTestMultiVariable();	break;
					case "TestHierarchy":		CreateTestHierarchy();		break;
					case "TestManual":			CreateTestManual();			break;
				}
				Log.Debug( $"end Create{_testName}" );
//				await UniTaskUtility.Yield( _asyncCancel );
			} );
		}


/*
		・単体変数テスト
		_type、_lifeSpan、_sceneが、適切に設定されるか？
		_type、_lifeSpan、_sceneにより、適切に登録されるか？
		_ownerちゃんと設定？
*/
		void CreateTestVariable() {
			new Type[] { typeof( B1 ), typeof( B2 ), typeof( B3 ), typeof( B4 ), typeof( B5 ), typeof( B6 ) }
				.ForEach( t => t.Create() );
			new Type[] { typeof( M1 ), typeof( M2 ), typeof( M3 ), typeof( M4 ), typeof( M5 ), typeof( M6 ) }
				.ForEach( t => {
					var go = new GameObject( $"{t.Name}", t );
					go.SetActive( false );
				} );
		}

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestVariable() => From( TestVariableSub() );
		IEnumerator TestVariableSub() {
			Log.Debug( "TestVariableSub" );
			while ( true )	{ yield return null; }
		}


/*
		・複数変数テスト
		_type、_lifeSpan、_sceneが、適切に設定されるか？
		_type、_lifeSpan、_sceneにより、適切に登録されるか？
		_ownerちゃんと設定？
		_processes、複数もちゃんと設定？
*/
		void CreateTestMultiVariable() => TestProcessUtility.CreateMonoBehaviourProcess( @"
			M1, M1, M1,
			M2, M2, M2,
			M3, M3, M3,
			M4, M4, M4,
			M5, M5, M5,
			M6, M6, M6,

			M1,
			M1, M2,
			M1, M2, M3,
			M1, M2, M3, M4,
			M1, M2, M3, M4, M5,
			M1, M2, M3, M4, M5, M6,

			M1, M4,
			M1, M5,
			M1, M6,
		" );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestMultiVariable() => From( TestMultiVariableSub() );
		IEnumerator TestMultiVariableSub() {
			Log.Debug( "TestMultiVariableSub" );
			while ( true )	{ yield return null; }
		}

/*
		・階層テスト
		_top、_parent、_children、親子兄弟関係、が正しく設定されるか？
		SetParent、SetChildren、SetBrothers、それぞれ1回だけ呼ばれる？
		SetTop、SetAllData、親子階層含めて、1回だけ呼ばれる？
*/
		void CreateTestHierarchy() => TestProcessUtility.CreateMonoBehaviourProcess(
#if false
		@"
			M1,
				null,
					M2,
					M1,
						M3,
							null,
							null,
								M4,
				null,
					M1,
				M1,
		"
#else
		@"
			M1,
				M1,
					M1,
			M2,
				null,
					null,
						M2,
							null,
								null,
									M2,
										null,
											null,
			null,
				M3,

			M1, M1, M1,
				null,
					M1, M1, M1,
						null
							M1, M1, M1,
			M1,
			M1,
				M2,
			M1,
				M2,
					M3,
			M1,
				M2,
					M3,
						M4,
			M1,
				M2,
					M3,
						M4,
							M5,
			M1,
				M2,
					M3,
						M4,
							M5,
								M6,
			M1, M1,
				M1, M2,
			M1, M1,
				M1, M3,
			M1, M1,
				M1, M4,
			M1, M1,
				M1, M5,
			M1, M1,
				M1, M6,

			M1,
				null,
					M2,
					M1,
						M3,
							null,
							null,
								M4,
				null,
					M1,
				M1,
		"
#endif
		);

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestHierarchy() => From( TestHierarchySub() );
		IEnumerator TestHierarchySub() {
			Log.Debug( "TestHierarchySub" );
			while ( true )	{ yield return null; }
		}


		void CreateTestManual() {
		}

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( TestManualSub() );
		IEnumerator TestManualSub() {
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
			var i = 0;
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Space ) ).Subscribe( _ => {
					Log.Warning( "key down change scene" );
					i = (i + 1) % 2;
					switch ( i ) {
						case 0:
							Log.Debug( $"{this.GetAboutName()} change TestChange1Scene" );
							_process._fsm.ChangeScene<TestChange1Scene>().Forget();
							break;
						case 1:
							Log.Debug( $"{this.GetAboutName()} change TestChange2Scene" );
							_process._fsm.ChangeScene<TestChange2Scene>().Forget();
							break;
						case 2:
							Log.Debug( $"{this.GetAboutName()} change UnknownScene" );
							_process._fsm.ChangeScene<UnknownScene>().Forget();
							break;
					}
				} )
			);
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down Dispose" );
					_process.Dispose();
					_process = null;
				} )
			);

			while ( true )	{ yield return null; }
		}
	}
}