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



	public class TestSMTaskRunner : Test {
		SMTaskRunner _behaviour;
		Text _text;


		protected override void Create() {
			Application.targetFrameRate = 30;
			_behaviour = SMTaskRunner.s_instance;

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
					+ $")";
			} ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_disposables.AddLast( _behaviour );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( TestManualSub() );
		IEnumerator TestManualSub() {
			_disposables.AddFirst(
				Observable.EveryUpdate()
					.Where( _ => Input.GetKeyDown( KeyCode.Return ) )
					.Take( 1 )
					.Subscribe( _ => {
						Log.Debug( "KeyDown Return" );
					} )
			);

			_behaviour._loadEvent.AddFirst(
				async cancel => Log.Debug( $"start : {this.GetAboutName()}.{nameof( _behaviour._loadEvent )}" ) );
			_behaviour._loadEvent.AddLast(
				async cancel => Log.Debug( $"end : {this.GetAboutName()}.{nameof( _behaviour._loadEvent )}" ) );

			_behaviour._initializeEvent.AddFirst(
				async cancel => Log.Debug( $"start : {this.GetAboutName()}.{nameof( _behaviour._initializeEvent )}" ) );
			_behaviour._initializeEvent.AddLast(
				async cancel => Log.Debug( $"end : {this.GetAboutName()}.{nameof( _behaviour._initializeEvent )}" ) );

			_behaviour._enableEvent.AddFirst(
				async cancel => Log.Debug( $"start : {this.GetAboutName()}.{nameof( _behaviour._enableEvent )}" ) );
			_behaviour._enableEvent.AddLast(
				async cancel => Log.Debug( $"end : {this.GetAboutName()}.{nameof( _behaviour._enableEvent )}" ) );

			_behaviour._fixedUpdateEvent.AddFirst().Subscribe(
				_ => Log.Debug( $"start : {this.GetAboutName()}.{nameof( _behaviour._fixedUpdateEvent )}" ) );
			_behaviour._fixedUpdateEvent.AddLast().Subscribe(
				_ => Log.Debug( $"end : {this.GetAboutName()}.{nameof( _behaviour._fixedUpdateEvent )}" ) );

			_behaviour._updateEvent.AddFirst().Subscribe(
				_ => Log.Debug( $"start : {this.GetAboutName()}.{nameof( _behaviour._updateEvent )}" ) );
			_behaviour._updateEvent.AddLast().Subscribe(
				_ => Log.Debug( $"end : {this.GetAboutName()}.{nameof( _behaviour._updateEvent )}" ) );

			_behaviour._lateUpdateEvent.AddFirst().Subscribe(
				_ => Log.Debug( $"start : {this.GetAboutName()}.{nameof( _behaviour._lateUpdateEvent )}" ) );
			_behaviour._lateUpdateEvent.AddLast().Subscribe(
				_ => Log.Debug( $"end : {this.GetAboutName()}.{nameof( _behaviour._lateUpdateEvent )}" ) );

			_behaviour._disableEvent.AddFirst(
				async cancel => Log.Debug( $"start : {this.GetAboutName()}.{nameof( _behaviour._disableEvent )}" ) );
			_behaviour._disableEvent.AddLast(
				async cancel => Log.Debug( $"end : {this.GetAboutName()}.{nameof( _behaviour._disableEvent )}" ) );

			_behaviour._finalizeEvent.AddFirst(
				async cancel => Log.Debug( $"start : {this.GetAboutName()}.{nameof( _behaviour._finalizeEvent )}" ) );
			_behaviour._finalizeEvent.AddLast(
				async cancel => Log.Debug( $"end : {this.GetAboutName()}.{nameof( _behaviour._finalizeEvent )}" ) );

			while ( true )	{ yield return null; }
		}
	}
}