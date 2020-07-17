//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestSMTask {
	using System.Linq;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.TestTools;
	using UniRx;
	using UTask;
	using SMTask;
	using Extension;
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

			_behaviour._loadEvent.AddFirst( async canceler => {
				Log.Debug( $"start : {this.GetAboutName()}.{nameof( _behaviour._loadEvent )}" );
				await UTask.DontWait();
			} );
			_behaviour._loadEvent.AddLast( async canceler => {
				Log.Debug( $"end : {this.GetAboutName()}.{nameof( _behaviour._loadEvent )}" );
				await UTask.DontWait();
			} );

			_behaviour._initializeEvent.AddFirst( async canceler => {
				Log.Debug( $"start : {this.GetAboutName()}.{nameof( _behaviour._initializeEvent )}" );
				await UTask.DontWait();
			} );
			_behaviour._initializeEvent.AddLast( async canceler => {
				Log.Debug( $"end : {this.GetAboutName()}.{nameof( _behaviour._initializeEvent )}" );
				await UTask.DontWait();
			} );

			_behaviour._enableEvent.AddFirst( async canceler => {
				Log.Debug( $"start : {this.GetAboutName()}.{nameof( _behaviour._enableEvent )}" );
				await UTask.DontWait();
			} );
			_behaviour._enableEvent.AddLast( async canceler => {
				Log.Debug( $"end : {this.GetAboutName()}.{nameof( _behaviour._enableEvent )}" );
				await UTask.DontWait();
			} );

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

			_behaviour._disableEvent.AddFirst( async canceler => {
				Log.Debug( $"start : {this.GetAboutName()}.{nameof( _behaviour._disableEvent )}" );
				await UTask.DontWait();
			} );
			_behaviour._disableEvent.AddLast( async canceler => {
				Log.Debug( $"end : {this.GetAboutName()}.{nameof( _behaviour._disableEvent )}" );
				await UTask.DontWait();
			} );

			_behaviour._finalizeEvent.AddFirst( async canceler => {
				Log.Debug( $"start : {this.GetAboutName()}.{nameof( _behaviour._finalizeEvent )}" );
				await UTask.DontWait();
			} );
			_behaviour._finalizeEvent.AddLast( async canceler => {
				Log.Debug( $"end : {this.GetAboutName()}.{nameof( _behaviour._finalizeEvent )}" );
				await UTask.DontWait();
			} );

			while ( true )	{ yield return null; }
		}
	}
}