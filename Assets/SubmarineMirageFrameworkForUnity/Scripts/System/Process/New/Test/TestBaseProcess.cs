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
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class TestBaseProcess : BaseProcess {
		public override void Create() {
			_loadEvent.AddLast( async cancel => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "load 1 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 1 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 1 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 1 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 1 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"load 1 end {TimeManager.s_instance.StopMeasure()}" );
			} );
			_initializeEvent.AddLast( async cancel => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "initialize 1 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 1 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 1 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 1 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 1 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"initialize 1 end {TimeManager.s_instance.StopMeasure()}" );
			} );
			_updateEvent.AddLast( "1" ).Subscribe( _ => {
				Log.Debug( "update 1" );
			} );
			_finalizeEvent.AddLast( async cancel => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "finalize 1 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 1 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 1 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 1 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 1 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"finalize 1 end {TimeManager.s_instance.StopMeasure()}" );
			} );
		}
	}

	public class TestBaseProcess2 : TestBaseProcess {
		public override void Create() {
			base.Create();
			_loadEvent.AddLast( async cancel => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "load 2 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 2 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 2 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 2 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 2 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"load 2 end {TimeManager.s_instance.StopMeasure()}" );
			} );
			_initializeEvent.AddLast( async cancel => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "initialize 2 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 2 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 2 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 2 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 2 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"initialize 2 end {TimeManager.s_instance.StopMeasure()}" );
			} );
			_updateEvent.AddLast( "2" ).Subscribe( _ => {
				Log.Debug( "update 2" );
			} );
			_updateEvent.AddFirst( "0" ).Subscribe( _ => {
				Log.Debug( "update 0" );
			} );
			_updateEvent.InsertFirst( "1", "0.5" ).Subscribe( _ => {
				Log.Debug( "update 0.5" );
			} );
			_updateEvent.InsertLast( "1", "1.5" ).Subscribe( _ => {
				Log.Debug( "update 1.5" );
			} );
			_finalizeEvent.AddLast( async cancel => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "finalize 2 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 2 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 2 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 2 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 2 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"finalize 2 end {TimeManager.s_instance.StopMeasure()}" );
			} );
		}
		~TestBaseProcess2() {
			Log.Debug( $"Delete {this.GetAboutName()}" );
		}
	}

	public class TestBaseProcess3 : BaseProcess {
		public override ProcessBody.Type _type => ProcessBody.Type.FirstWork;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.Forever;
		public override void Create() {}
	}


	

	public class Hoge {
		Text _text;

		public Hoge() {
			_text = GameObject.Find( "Canvas/Text" ).GetComponent<Text>();
			var p = new TestBaseProcess3();

			p._disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ =>
				_text.text =
					$"{p.GetAboutName()}(\n"
					+ $"    {p._belongSceneName}\n"
					+ $"    _isInitialized {p._isInitialized}\n"
					+ $"    _isActive {p._isActive}\n"
					+ $"    {p._process._ranState}\n"
					+ $"    {p._process._activeState}\n"
					+ $")"
			) );

			p._disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( "key down 1" );
					p.RunStateEvent( ProcessBody.RanState.None ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down 2" );
					p.RunStateEvent( ProcessBody.RanState.Creating ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha3 ) ).Subscribe( _ => {
					Log.Warning( "key down 3" );
					p.RunStateEvent( ProcessBody.RanState.Created ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha4 ) ).Subscribe( _ => {
					Log.Warning( "key down 4" );
					p.RunStateEvent( ProcessBody.RanState.Loading ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha5 ) ).Subscribe( _ => {
					Log.Warning( "key down 5" );
					p.RunStateEvent( ProcessBody.RanState.Loaded ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha6 ) ).Subscribe( _ => {
					Log.Warning( "key down 6" );
					p.RunStateEvent( ProcessBody.RanState.Initializing ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha7 ) ).Subscribe( _ => {
					Log.Warning( "key down 7" );
					p.RunStateEvent( ProcessBody.RanState.Initialized ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha8 ) ).Subscribe( _ => {
					Log.Warning( "key down 8" );
					p.RunStateEvent( ProcessBody.RanState.FixedUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha9 ) ).Subscribe( _ => {
					Log.Warning( "key down 9" );
					p.RunStateEvent( ProcessBody.RanState.Update ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha0 ) ).Subscribe( _ => {
					Log.Warning( "key down 0" );
					p.RunStateEvent( ProcessBody.RanState.LateUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Minus ) ).Subscribe( _ => {
					Log.Warning( "key down -" );
					p.RunStateEvent( ProcessBody.RanState.Finalizing ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Quote ) ).Subscribe( _ => {
					Log.Warning( "key down ^" );
					p.RunStateEvent( ProcessBody.RanState.Finalized ).Forget();
				} )
			);
			p._disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Z ) ).Subscribe( _ => {
					Log.Warning( "key down Z" );
					p.ChangeActive( true ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.X ) ).Subscribe( _ => {
					Log.Warning( "key down X" );
					p.ChangeActive( false ).Forget();
				} )
			);

			p._disposables.AddLast( () => _text.text = string.Empty );

/*
			p.RunStateEvent();
			p.ChangeActive();
			p.Dispose();
			p.StopActiveAsync();
*/
		}
	}
}