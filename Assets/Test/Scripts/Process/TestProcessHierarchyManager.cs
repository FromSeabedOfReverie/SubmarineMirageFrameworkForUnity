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



	public class TestProcessHierarchyManager : Test {
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

			_disposables.AddLast( _process );

			CreateObject();
		}


		void CreateObject() {
			var top = new GameObject( "TestMono top" );
			top.AddComponent<TestMono1>();
			top.SetActive( false );
			var parent = top.transform;
			var id = 0;

			3.Times( brotherIndex => {
				3.Times( hierarchyIndex => {
					var go = new GameObject();
					go.name = $"TestMono {id++}";
					go.SetParent( parent );
					parent = go.transform;

					if ( hierarchyIndex == 0 )	{ go.SetActive( false ); }
					if ( hierarchyIndex == 0 && brotherIndex == 0 )	{ go.AddComponent<TestMono1>(); }
					if ( hierarchyIndex == 1 && brotherIndex == 1 )	{ go.AddComponent<TestMono2>(); }
					if ( hierarchyIndex == 2 && brotherIndex == 2 )	{ go.AddComponent<TestMono3>(); }
					if ( hierarchyIndex == 2 && brotherIndex == 0 )	{
						go.AddComponent<TestMono1>();
						go.AddComponent<TestMono2>();
						go.AddComponent<TestMono3>();
					}
				} );
				parent = top.transform;
			} );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( TestManualSub() );
		IEnumerator TestManualSub() {
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( "key down Creating" );
					_process.RunStateEvent( SMTaskRanState.Creating ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down Loading" );
					_process.RunStateEvent( SMTaskRanState.Loading ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha3 ) ).Subscribe( _ => {
					Log.Warning( "key down Initializing" );
					_process.RunStateEvent( SMTaskRanState.Initializing ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha4 ) ).Subscribe( _ => {
					Log.Warning( "key down FixedUpdate" );
					_process.RunStateEvent( SMTaskRanState.FixedUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha5 ) ).Subscribe( _ => {
					Log.Warning( "key down Update" );
					_process.RunStateEvent( SMTaskRanState.Update ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha6 ) ).Subscribe( _ => {
					Log.Warning( "key down LateUpdate" );
					_process.RunStateEvent( SMTaskRanState.LateUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha7 ) ).Subscribe( _ => {
					Log.Warning( "key down Finalizing" );
					_process.RunStateEvent( SMTaskRanState.Finalizing ).Forget();
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



		public class TestMono1 : SMMonoBehaviour {
			public override SMTaskType _type => SMTaskType.DontWork;
			public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;
			static int s_count;
			public int _id	{ get; private set; }
			public override void Create() {
				_id = s_count++;
			}
		}
		public class TestMono2 : TestMono1 {
			public override SMTaskType _type => SMTaskType.Work;
			public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;
		}
		public class TestMono3 : TestMono1 {
			public override SMTaskType _type => SMTaskType.FirstWork;
			public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;
		}
	}
}