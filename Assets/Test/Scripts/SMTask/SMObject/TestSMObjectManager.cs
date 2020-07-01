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



	public class TestSMObjectManagerManager : Test {
		SceneManager _behaviour;
		Text _text;


		protected override void Create() {
			Application.targetFrameRate = 30;
			_behaviour = SceneManager.s_instance;

			UnityObject.Instantiate( Resources.Load<GameObject>( "TestCamera" ) );
			var go = UnityObject.Instantiate( Resources.Load<GameObject>( "TestCanvas" ) );
			UnityObject.DontDestroyOnLoad( go );
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ => {
				if ( _behaviour == null ) {
					_text.text = string.Empty;
					return;
				}
				_text.text =
					$"{_behaviour.GetAboutName()}(\n"
					+ $"    {nameof( _behaviour._isInitialized )} : {_behaviour._isInitialized}\n"
					+ $"    {nameof( _behaviour._isActive )} : {_behaviour._isActive}\n"
					+ $"    {nameof( _behaviour._body._ranState )} : {_behaviour._body._ranState}\n"
					+ $"    {nameof( _behaviour._body._activeState )} : {_behaviour._body._activeState}\n"
					+ $"    next : {_behaviour._body._nextActiveState}\n"
					+ $")\n"
					+ $"{nameof( _behaviour._fsm )} : {_behaviour._fsm}";
			} ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_disposables.AddLast( _behaviour );

			CreateObject();
		}


		void CreateObject() {
			var top = new GameObject( $"Test top" );
			top.AddComponent<M1>();
			top.SetActive( false );
			var parent = top.transform;
			var id = 0;

			3.Times( brotherIndex => {
				3.Times( hierarchyIndex => {
					var go = new GameObject();
					go.name = $"Test {id++}";
					go.SetParent( parent );
					parent = go.transform;

					if ( hierarchyIndex == 0 )	{ go.SetActive( false ); }
					if ( hierarchyIndex == 0 && brotherIndex == 0 )	{ go.AddComponent<M1>(); }
					if ( hierarchyIndex == 1 && brotherIndex == 1 )	{ go.AddComponent<M2>(); }
					if ( hierarchyIndex == 2 && brotherIndex == 2 )	{ go.AddComponent<M3>(); }
					if ( hierarchyIndex == 2 && brotherIndex == 0 )	{
						go.AddComponent<M1>();
						go.AddComponent<M2>();
						go.AddComponent<M3>();
					}
				} );
				parent = top.transform;
			} );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( TestManualSub() );
		IEnumerator TestManualSub() {
			_disposables.AddLast( TestSMTaskUtility.SetRunKey( _behaviour ) );

			var i = 0;
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Space ) ).Subscribe( _ => {
					Log.Warning( "key down change scene" );
					i = (i + 1) % 2;
					switch ( i ) {
						case 0:
							Log.Debug( $"{this.GetAboutName()} change TestChange1Scene" );
							_behaviour._fsm.ChangeScene<TestChange1Scene>().Forget();
							break;
						case 1:
							Log.Debug( $"{this.GetAboutName()} change TestChange2Scene" );
							_behaviour._fsm.ChangeScene<TestChange2Scene>().Forget();
							break;
						case 2:
							Log.Debug( $"{this.GetAboutName()} change UnknownScene" );
							_behaviour._fsm.ChangeScene<UnknownScene>().Forget();
							break;
					}
				} )
			);

			while ( true )	{ yield return null; }
		}
	}
}