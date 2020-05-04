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
					case "TestVariable":			CreateTestVariable();				break;
					case "TestMultiVariable":		CreateTestMultiVariable();			break;
					case "TestHierarchy":			CreateTestHierarchy();				break;
					case "TestGetHierarchies":		CreateTestGetHierarchies();			break;
					case "TestGetProcess":			CreateTestGetProcess();				break;
					case "TestGetHierarchyProcess":	CreateTestGetHierarchyProcess();	break;
					case "TestManual":				CreateTestManual();					break;
				}
				Log.Debug( $"end Create{_testName}" );
			} );
		}


/*
		・単体変数テスト
		_type、_lifeSpan、_sceneが、適切に設定されるか？
		_type、_lifeSpan、_sceneにより、適切に登録されるか？
		_ownerちゃんと設定？
*/
		void CreateTestVariable() {
#if false
			new Type[] { typeof( B1 ), typeof( B2 ), typeof( B3 ), typeof( B4 ), typeof( B5 ), typeof( B6 ) }
				.ForEach( t => t.Create() );
#else
			new Type[] { typeof( B1 ), typeof( B2 ), typeof( B3 ), typeof( B4 ), typeof( B5 ), typeof( B6 ) }
				.ForEach( t => t.Create() );
			new Type[] { typeof( M1 ), typeof( M2 ), typeof( M3 ), typeof( M4 ), typeof( M5 ), typeof( M6 ) }
				.ForEach( t => {
					var go = new GameObject( $"{t.Name}", t );
					go.SetActive( false );
				} );
#endif
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


/*
		・階層取得テスト
		GetHierarchiesInParent、GetHierarchiesInChildren、動作確認
*/
		void CreateTestGetHierarchies() {
			new Type[] { typeof( B1 ), typeof( B2 ), typeof( B3 ), typeof( B4 ), typeof( B5 ), typeof( B6 ) }
				.ForEach( t => t.Create() );
			TestProcessUtility.CreateMonoBehaviourProcess( @"
				M1, M1, M2,
					null,
						M2,
						M1,
							M3,
								null,
									M4,
										M5,
								null,
									M4, M4,
					null,
						M1,
							M6,
								M1, M1
					M1,
			" );
		}

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestGetHierarchies() => From( TestGetHierarchiesSub() );
		IEnumerator TestGetHierarchiesSub() {
			Log.Debug( "TestGetHierarchiesSub" );

			var top = SceneManager.s_instance.GetProcess<M1>()._hierarchy;
			var center = SceneManager.s_instance.GetProcess<M3>()._hierarchy;
			var bottom = SceneManager.s_instance.GetProcess<M5>()._hierarchy;
			LogHierarchy( "top", top );
			LogHierarchy( "center", center );
			LogHierarchy( "bottom", bottom );

			LogHierarchies( "top.GetHierarchiesInChildren",		top.GetHierarchiesInChildren() );
			LogHierarchies( "center.GetHierarchiesInChildren",	center.GetHierarchiesInChildren() );
			LogHierarchies( "bottom.GetHierarchiesInChildren",	bottom.GetHierarchiesInChildren() );

			LogHierarchies( "top.GetHierarchiesInParent",		top.GetHierarchiesInParent() );
			LogHierarchies( "center.GetHierarchiesInParent",	center.GetHierarchiesInParent() );
			LogHierarchies( "bottom.GetHierarchiesInParent",	bottom.GetHierarchiesInParent() );

			while ( true )	{ yield return null; }
		}


/*
		・取得テスト
		GetProcess<T>、GetProcess(type)、GetProcesses<T>、GetProcesses(type)、動作確認
*/
		void CreateTestGetProcess()
			=> CreateTestGetHierarchies();

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestGetProcess() => From( TestGetProcessSub() );
		IEnumerator TestGetProcessSub() {
			Log.Debug( "TestGetProcessSub" );

			var top = SceneManager.s_instance.GetProcess<M1>()._hierarchy;
			LogHierarchy( "top", top );

			LogProcess( "GetProcess<M1>",	top.GetProcess<M1>() );
			LogProcess( "GetProcess( M1 )",	top.GetProcess( typeof( M1 ) ) );
			LogProcess( "GetProcess<M2>",	top.GetProcess<M2>() );
			LogProcess( "GetProcess( M2 )",	top.GetProcess( typeof( M2 ) ) );
			LogProcess( "GetProcess<M3>",	top.GetProcess<M3>() );
			LogProcess( "GetProcess( M3 )",	top.GetProcess( typeof( M3 ) ) );

			LogProcesses( "GetProcesses<M1>",	top.GetProcesses<M1>() );
			LogProcesses( "GetProcesses( M1 )",	top.GetProcesses( typeof( M1 ) ) );
			LogProcesses( "GetProcesses<M2>",	top.GetProcesses<M2>() );
			LogProcesses( "GetProcesses( M2 )",	top.GetProcesses( typeof( M2 ) ) );
			LogProcesses( "GetProcesses<M3>",	top.GetProcesses<M3>() );
			LogProcesses( "GetProcesses( M3 )",	top.GetProcesses( typeof( M3 ) ) );

			while ( true )	{ yield return null; }
		}


/*
		・取得テスト
		GetProcessInParent<T>、GetProcessInParent(type)
		GetProcessesInParent<T>、GetProcessesInParent(type)
		GetProcessInChildren<T>、GetProcessInChildren(type)
		GetProcessesInChildren<T>、GetProcessesInChildren(type)
*/
		void CreateTestGetHierarchyProcess()
			=> CreateTestGetHierarchies();

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestGetHierarchyProcess() => From( TestGetHierarchyProcessSub() );
		IEnumerator TestGetHierarchyProcessSub() {
			Log.Debug( "TestGetHierarchyProcessSub" );

			var top = SceneManager.s_instance.GetProcess<M1>()._hierarchy;
			var center = SceneManager.s_instance.GetProcess<M3>()._hierarchy;
			var bottom = SceneManager.s_instance.GetProcess<M5>()._hierarchy;
			LogHierarchy( "top", top );
			LogHierarchy( "center", center );
			LogHierarchy( "bottom", bottom );


			LogProcess( "top.GetProcessInParent<M1>",		top.GetProcessInParent<M1>() );
			LogProcess( "top.GetProcessInParent( M1 )",		top.GetProcessInParent( typeof( M1 ) ) );
			LogProcess( "center.GetProcessInParent<M1>",	center.GetProcessInParent<M1>() );
			LogProcess( "center.GetProcessInParent( M1 )",	center.GetProcessInParent( typeof( M1 ) ) );
			LogProcess( "bottom.GetProcessInParent<M1>",	bottom.GetProcessInParent<M1>() );
			LogProcess( "bottom.GetProcessInParent( M1 )",	bottom.GetProcessInParent( typeof( M1 ) ) );

			LogProcesses( "top.GetProcessesInParent<M1>",		top.GetProcessesInParent<M1>() );
			LogProcesses( "top.GetProcessesInParent( M1 )",		top.GetProcessesInParent( typeof( M1 ) ) );
			LogProcesses( "center.GetProcessesInParent<M1>",	center.GetProcessesInParent<M1>() );
			LogProcesses( "center.GetProcessesInParent( M1 )",	center.GetProcessesInParent( typeof( M1 ) ) );
			LogProcesses( "bottom.GetProcessesInParent<M1>",	bottom.GetProcessesInParent<M1>() );
			LogProcesses( "bottom.GetProcessesInParent( M1 )",	bottom.GetProcessesInParent( typeof( M1 ) ) );

			LogProcess( "top.GetProcessInChildren<M1>",		top.GetProcessInChildren<M1>() );
			LogProcess( "top.GetProcessInChildren( M1 )",		top.GetProcessInChildren( typeof( M1 ) ) );
			LogProcess( "center.GetProcessInChildren<M1>",	center.GetProcessInChildren<M1>() );
			LogProcess( "center.GetProcessInChildren( M1 )",	center.GetProcessInChildren( typeof( M1 ) ) );
			LogProcess( "bottom.GetProcessInChildren<M1>",	bottom.GetProcessInChildren<M1>() );
			LogProcess( "bottom.GetProcessInChildren( M1 )",	bottom.GetProcessInChildren( typeof( M1 ) ) );

			LogProcesses( "top.GetProcessesInChildren<M1>",		top.GetProcessesInChildren<M1>() );
			LogProcesses( "top.GetProcessesInChildren( M1 )",		top.GetProcessesInChildren( typeof( M1 ) ) );
			LogProcesses( "center.GetProcessesInChildren<M1>",	center.GetProcessesInChildren<M1>() );
			LogProcesses( "center.GetProcessesInChildren( M1 )",	center.GetProcessesInChildren( typeof( M1 ) ) );
			LogProcesses( "bottom.GetProcessesInChildren<M1>",	bottom.GetProcessesInChildren<M1>() );
			LogProcesses( "bottom.GetProcessesInChildren( M1 )",	bottom.GetProcessesInChildren( typeof( M1 ) ) );


			LogProcess( "top.GetProcessInParent<M4>",		top.GetProcessInParent<M4>() );
			LogProcess( "top.GetProcessInParent( M4 )",		top.GetProcessInParent( typeof( M4 ) ) );
			LogProcess( "center.GetProcessInParent<M4>",	center.GetProcessInParent<M4>() );
			LogProcess( "center.GetProcessInParent( M4 )",	center.GetProcessInParent( typeof( M4 ) ) );
			LogProcess( "bottom.GetProcessInParent<M4>",	bottom.GetProcessInParent<M4>() );
			LogProcess( "bottom.GetProcessInParent( M4 )",	bottom.GetProcessInParent( typeof( M4 ) ) );

			LogProcesses( "top.GetProcessesInParent<M4>",		top.GetProcessesInParent<M4>() );
			LogProcesses( "top.GetProcessesInParent( M4 )",		top.GetProcessesInParent( typeof( M4 ) ) );
			LogProcesses( "center.GetProcessesInParent<M4>",	center.GetProcessesInParent<M4>() );
			LogProcesses( "center.GetProcessesInParent( M4 )",	center.GetProcessesInParent( typeof( M4 ) ) );
			LogProcesses( "bottom.GetProcessesInParent<M4>",	bottom.GetProcessesInParent<M4>() );
			LogProcesses( "bottom.GetProcessesInParent( M4 )",	bottom.GetProcessesInParent( typeof( M4 ) ) );

			LogProcess( "top.GetProcessInChildren<M4>",		top.GetProcessInChildren<M4>() );
			LogProcess( "top.GetProcessInChildren( M4 )",		top.GetProcessInChildren( typeof( M4 ) ) );
			LogProcess( "center.GetProcessInChildren<M4>",	center.GetProcessInChildren<M4>() );
			LogProcess( "center.GetProcessInChildren( M4 )",	center.GetProcessInChildren( typeof( M4 ) ) );
			LogProcess( "bottom.GetProcessInChildren<M4>",	bottom.GetProcessInChildren<M4>() );
			LogProcess( "bottom.GetProcessInChildren( M4 )",	bottom.GetProcessInChildren( typeof( M4 ) ) );

			LogProcesses( "top.GetProcessesInChildren<M4>",		top.GetProcessesInChildren<M4>() );
			LogProcesses( "top.GetProcessesInChildren( M4 )",		top.GetProcessesInChildren( typeof( M4 ) ) );
			LogProcesses( "center.GetProcessesInChildren<M4>",	center.GetProcessesInChildren<M4>() );
			LogProcesses( "center.GetProcessesInChildren( M4 )",	center.GetProcessesInChildren( typeof( M4 ) ) );
			LogProcesses( "bottom.GetProcessesInChildren<M4>",	bottom.GetProcessesInChildren<M4>() );
			LogProcesses( "bottom.GetProcessesInChildren( M4 )",	bottom.GetProcessesInChildren( typeof( M4 ) ) );

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


		void LogHierarchy( string text, ProcessHierarchy hierarchy ) {
			if ( hierarchy == null ) {
				Log.Debug( $"{text} : null" );
				return;
			}
			var name = hierarchy._owner != null ? hierarchy._owner.name : null;
			Log.Debug( $"{text} : " + string.Join( ", ",
				hierarchy._processes.Select( p => p.GetAboutName() )
			) + $" : {name}" );
		}

		void LogHierarchies( string text, IEnumerable<ProcessHierarchy> hierarchies ) {
			Log.Debug( $"{text} :\n" + string.Join( "\n",
				hierarchies.Select( h => {
					var name = h._owner != null ? h._owner.name : null;
					return string.Join( ", ",
						h._processes.Select( p => p.GetAboutName() )
					) + $" : {name}";
				} )
			) );
		}


		void LogProcess( string text, IProcess process ) {
			if ( process == null ) {
				Log.Debug( $"{text} : null" );
				return;
			}
			var name = process._hierarchy._owner != null ? process._hierarchy._owner.name : null;
			Log.Debug( $"{text} : {process.GetAboutName()} : {name}" );
		}

		void LogProcesses( string text, IEnumerable<IProcess> processes ) {
			Log.Debug( $"{text} :\n" + string.Join( "\n",
				processes.Select( p => {
					var name = p._hierarchy._owner != null ? p._hierarchy._owner.name : null;
					return p.GetAboutName() + $" : {name}";
				} )
			) );
		}
	}
}