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
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using SMTask;
	using FSM.New;
	using Extension;
	using Utility;
	using Debug;
	using Test;
	using TestSMTask;
	using UnityObject = UnityEngine.Object;


	// TODO : コメント追加、整頓
/*
enable, state	→	state	→	enable
enable, state	→	state	→	disable
enable, state	→	enable	→	state
enable, state	→	disable	→	state

disable, state	→	state	→	enable
disable, state	→	state	→	disable
disable, state	→	enable	→	state
disable, state	→	disable	→	state


enable, state	→	null	→	enable
enable, state	→	null	→	disable
enable, state	→	enable	→	null
enable, state	→	disable	→	null

disable, state	→	null	→	enable
disable, state	→	null	→	disable
disable, state	→	enable	→	null
disable, state	→	disable	→	null


enable, null	→	null	→	enable
enable, null	→	null	→	disable
enable, null	→	enable	→	null
enable, null	→	disable	→	null

disable, null	→	null	→	enable
disable, null	→	null	→	disable
disable, null	→	enable	→	null
disable, null	→	disable	→	null


enable, null	→	state	→	enable
enable, null	→	state	→	disable
enable, null	→	enable	→	state
enable, null	→	disable	→	state

disable, null	→	state	→	enable
disable, null	→	state	→	disable
disable, null	→	enable	→	state
disable, null	→	disable	→	state
*/


	public class TestFiniteStateMachine : Test {
		Text _text;
		TestOwner _behaviour;


		protected override void Create() {
			Application.targetFrameRate = 30;
			_behaviour = new TestOwner( new BaseTestState[] {
				new TestStateA(),
				new TestStateB(),
				new TestStateC(),
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
					$"{_behaviour.GetAboutName()}(\n"
					+ $"    {nameof( _behaviour._isInitialized )} : {_behaviour._isInitialized}\n"
					+ $"    {nameof( _behaviour._isActive )} : {_behaviour._isActive}\n"
					+ $"    {nameof( _behaviour._body._ranState )} : {_behaviour._body._ranState}\n"
					+ $"    {nameof( _behaviour._body._activeState )} : {_behaviour._body._activeState}\n"
					+ $"    {nameof( _behaviour._body._nextActiveState )} : {_behaviour._body._nextActiveState}\n"
					+ ")\n";
				_text.text += $"{_behaviour._fsm}";
			} ) );

			_disposables.AddLast( _behaviour );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() {
			_disposables.AddLast( TestSMTaskUtility.SetRunKey( _behaviour ) );

			var i = 0;
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Space ) ).Subscribe( _ => {
					Log.Warning( "key down change state" );
					i = (i + 1) % 5;
					switch ( i ) {
						case 0:
							Log.Debug( $"{this.GetAboutName()} change {nameof( TestStateA )}" );
							_behaviour._fsm.ChangeState<TestStateA>().Forget();
							break;
						case 1:
							Log.Debug( $"{this.GetAboutName()} change {nameof( TestStateB )}" );
							_behaviour._fsm.ChangeState<TestStateB>().Forget();
							break;
						case 2:
							Log.Debug( $"{this.GetAboutName()} change {nameof( TestStateC )}" );
							_behaviour._fsm.ChangeState<TestStateC>().Forget();
							break;
						case 3:
							Log.Debug( $"{this.GetAboutName()} change null" );
							_behaviour._fsm.ChangeState( null ).Forget();
							break;
						case 4:
							Log.Debug( $"{this.GetAboutName()} change null to null" );
							_behaviour._fsm.ChangeState( null ).Forget();
							break;
					}
				} )
			);

			while ( true )	{ yield return null; }
		}
	}
}