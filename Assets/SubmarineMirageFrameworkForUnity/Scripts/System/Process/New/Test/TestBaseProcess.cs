//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process.New {
	using System;
	using System.Linq;
	using System.Threading;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using Singleton.New;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class TestBaseProcess : BaseProcess {
		public override ProcessBody.Type _type => ProcessBody.Type.FirstWork;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.Forever;
		public override void Create() {
			Log.Debug( "Create()" );
			_loadEvent.AddLast( async cancel => {
				Log.Debug( "_loadEvent start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( "_loadEvent end" );
			} );
			_initializeEvent.AddLast( async cancel => {
				Log.Debug( "_initializeEvent start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( "_initializeEvent end" );
			} );
			_enableEvent.AddLast( async cancel => {
				Log.Debug( "_enableEvent start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( "_enableEvent end" );
			} );
			_fixedUpdateEvent.AddLast().Subscribe( _ => {
				Log.Debug( "_fixedUpdateEvent" );
			} );
			_updateEvent.AddLast().Subscribe( _ => {
				Log.Debug( "_updateEvent" );
			} );
			_lateUpdateEvent.AddLast().Subscribe( _ => {
				Log.Debug( "_lateUpdateEvent" );
			} );
			_disableEvent.AddLast( async cancel => {
				Log.Debug( "_disableEvent start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( "_disableEvent end" );
			} );
			_finalizeEvent.AddLast( async cancel => {
				Log.Debug( "_finalizeEvent start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( "_finalizeEvent end" );
			} );
		}
	}


	

	public class Hoge : Singleton<Hoge> {
		new IProcess _process;

		public Hoge() {
			var text = GameObject.Find( "Canvas/Text" ).GetComponent<Text>();
			_process = new TestBaseProcess();

			_process._disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ =>
				text.text =
					$"{_process.GetAboutName()}(\n"
					+ $"    {_process._belongSceneName}\n"
					+ $"    _isInitialized {_process._isInitialized}\n"
					+ $"    _isActive {_process._isActive}\n"
					+ $"    {_process._process._ranState}\n"
					+ $"    {_process._process._activeState}\n"
					+ $")"
			) );

			_process._disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( "key down Creating" );
					_process.RunStateEvent( ProcessBody.RanState.Creating ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down Loading" );
					_process.RunStateEvent( ProcessBody.RanState.Loading ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha3 ) ).Subscribe( _ => {
					Log.Warning( "key down Initializing" );
					_process.RunStateEvent( ProcessBody.RanState.Initializing ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha4 ) ).Subscribe( _ => {
					Log.Warning( "key down FixedUpdate" );
					_process.RunStateEvent( ProcessBody.RanState.FixedUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha5 ) ).Subscribe( _ => {
					Log.Warning( "key down Update" );
					_process.RunStateEvent( ProcessBody.RanState.Update ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha6 ) ).Subscribe( _ => {
					Log.Warning( "key down LateUpdate" );
					_process.RunStateEvent( ProcessBody.RanState.LateUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha7 ) ).Subscribe( _ => {
					Log.Warning( "key down Finalizing" );
					_process.RunStateEvent( ProcessBody.RanState.Finalizing ).Forget();
				} )
			);
			_process._disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Z ) ).Subscribe( _ => {
					Log.Warning( "key down Enabling" );
					_process.ChangeActive( true ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.X ) ).Subscribe( _ => {
					Log.Warning( "key down Disabling" );
					_process.ChangeActive( false ).Forget();
				} )
			);
			_process._disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down Dispose" );
					_process.Dispose();
				} )
			);

			new Func<UniTask>( async () => {
				while ( true ) {
					Log.Debug(
						$"IsCancellationRequested : {_process._activeAsyncCancel.IsCancellationRequested}" );
					await UniTaskUtility.Delay( _process._activeAsyncCancel, 1000 );
				}
			} )();

			_process._disposables.AddLast( () => text.text = string.Empty );

			_disposables.AddLast( _process );

/*
			p.ChangeActive();
*/
		}

		~Hoge() => Log.Debug( "~Hoge" );

		public override void Create() {}
	}
}