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



	public class TestProcessRunner : Test {
		SMTaskRunner _process;
		Text _text;


		protected override void Create() {
			Application.targetFrameRate = 30;
			_process = SMTaskRunner.s_instance;

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
					+ $")";
			} ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_disposables.AddLast( _process );
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
//						_process.DeleteForeverHierarchies().Forget();
					} )
			);

			_process._loadEvent.AddFirst(
				async cancel => Log.Debug( $"{this.GetAboutName()}._loadEvent start" ) );
			_process._loadEvent.AddLast(
				async cancel => Log.Debug( $"{this.GetAboutName()}._loadEvent end" ) );

			_process._initializeEvent.AddFirst(
				async cancel => Log.Debug( $"{this.GetAboutName()}._initializeEvent start" ) );
			_process._initializeEvent.AddLast(
				async cancel => Log.Debug( $"{this.GetAboutName()}._initializeEvent end" ) );

			_process._enableEvent.AddFirst(
				async cancel => Log.Debug( $"{this.GetAboutName()}._enableEvent start" ) );
			_process._enableEvent.AddLast(
				async cancel => Log.Debug( $"{this.GetAboutName()}._enableEvent end" ) );

			_process._fixedUpdateEvent.AddFirst().Subscribe(
				_ => Log.Debug( $"{this.GetAboutName()}._fixedUpdateEvent start" ) );
			_process._fixedUpdateEvent.AddLast().Subscribe(
				_ => Log.Debug( $"{this.GetAboutName()}._fixedUpdateEvent end" ) );

			_process._updateEvent.AddFirst().Subscribe(
				_ => Log.Debug( $"{this.GetAboutName()}._updateEvent start" ) );
			_process._updateEvent.AddLast().Subscribe(
				_ => Log.Debug( $"{this.GetAboutName()}._updateEvent end" ) );

			_process._lateUpdateEvent.AddFirst().Subscribe(
				_ => Log.Debug( $"{this.GetAboutName()}._lateUpdateEvent start" ) );
			_process._lateUpdateEvent.AddLast().Subscribe(
				_ => Log.Debug( $"{this.GetAboutName()}._lateUpdateEvent end" ) );

			_process._disableEvent.AddFirst(
				async cancel => Log.Debug( $"{this.GetAboutName()}._disableEvent start" ) );
			_process._disableEvent.AddLast(
				async cancel => Log.Debug( $"{this.GetAboutName()}._disableEvent end" ) );

			_process._finalizeEvent.AddFirst(
				async cancel => Log.Debug( $"{this.GetAboutName()}._finalizeEvent start" ) );
			_process._finalizeEvent.AddLast(
				async cancel => Log.Debug( $"{this.GetAboutName()}._finalizeEvent end" ) );

			while ( true )	{ yield return null; }
		}
	}
}