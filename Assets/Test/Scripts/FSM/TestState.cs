//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestFSM {
	using System;
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.TestTools;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using SMTask;
	using FSM.New;
	using Extension;
	using Utility;
	using Debug;
	using Test;
	using UnityObject = UnityEngine.Object;
	using RunState = FSM.New.FiniteStateMachineRunState;


	// TODO : コメント追加、整頓


	public class TestState : Test {
		Text _text;
		TestOwner _behaviour;
		BaseTestState _state => _behaviour._fsm._state;


		protected override void Create() {
			Application.targetFrameRate = 30;
			_behaviour = new TestOwner( new BaseTestState[] {
				new TestStateA(),
			} );

			UnityObject.Instantiate( Resources.Load<GameObject>( "TestCamera" ) );
			var go = UnityObject.Instantiate( Resources.Load<GameObject>( "TestCanvas" ) );
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ => {
				if ( _behaviour == null ) {
					_text.text = string.Empty;
					return;
				}
				_text.text =
					$"{nameof( _behaviour )} : {_behaviour._body._activeState}\n" +
					$"\n" +
					$"{_behaviour._fsm}";
			} ) );

			_disposables.AddLast( _behaviour );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestError() {
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( "key down None" );
					_state.RunBehaviourStateEvent( SMTaskRanState.None ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down Creating" );
					_state.RunBehaviourStateEvent( SMTaskRanState.Creating ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha3 ) ).Subscribe( _ => {
					Log.Warning( "key down Created" );
					_state.RunBehaviourStateEvent( SMTaskRanState.Created ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha4 ) ).Subscribe( _ => {
					Log.Warning( "key down Loaded" );
					_state.RunBehaviourStateEvent( SMTaskRanState.Loaded ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha5 ) ).Subscribe( _ => {
					Log.Warning( "key down Initialized" );
					_state.RunBehaviourStateEvent( SMTaskRanState.Initialized ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha6 ) ).Subscribe( _ => {
					Log.Warning( "key down Finalized" );
					_state.RunBehaviourStateEvent( SMTaskRanState.Finalized ).Forget();
				} )
			);
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.A ) ).Subscribe( _ => {
					Log.Warning( "key down Entered" );
					_state.RunStateEvent( RunState.Entered ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.S ) ).Subscribe( _ => {
					Log.Warning( "key down BeforeUpdate" );
					_state.RunStateEvent( RunState.BeforeUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.D ) ).Subscribe( _ => {
					Log.Warning( "key down Exited" );
					_state.RunStateEvent( RunState.Exited ).Forget();
				} )
			);

			while ( true )	{ yield return null; }
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() {
			yield return From( async () => {
				await _behaviour.RunStateEvent( SMTaskRanState.Creating );
			} );

			_disposables.AddLast( TestFSMUtility.SetRunKey( _state ) );
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Z ) ).Subscribe( _ => {
					Log.Warning( "key down Owner enable" );
					_behaviour.ChangeActive( true ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.X ) ).Subscribe( _ => {
					Log.Warning( "key down Owner disable" );
					_behaviour.ChangeActive( false ).Forget();
				} )
			);
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Delete ) ).Subscribe( _ => {
					Log.Warning( "key down Owner StopActiveAsync" );
					_behaviour.StopActiveAsync();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.RightShift ) ).Subscribe( _ => {
					Log.Warning( "key down Owner finalize" );
					_behaviour.RunStateEvent( SMTaskRanState.Finalizing ).Forget();
				} )
			);

			yield return From( async () => {
				await _behaviour.RunStateEvent( SMTaskRanState.Loading );
				await _behaviour.RunStateEvent( SMTaskRanState.Initializing );
			} );

			while ( true )	{ yield return null; }
		}
	}
}