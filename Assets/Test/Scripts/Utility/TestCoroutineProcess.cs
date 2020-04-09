//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestUtility {
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.TestTools;
	using UniRx;
	using Utility;
	using Debug;
	using Test;
	///====================================================================================================
	/// <summary>
	/// ■ コルーチン処理の試験クラス
	/// </summary>
	///====================================================================================================
	public class TestCoroutineProcess : Test {
		Text _text;


		protected override void Create() {
			Application.targetFrameRate = 60;

			Object.Instantiate( Resources.Load<GameObject>( "TestCamera" ) );
			var go = Object.Instantiate( Resources.Load<GameObject>( "TestCanvas" ) );
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ =>
				_text.text = CoroutineProcessManager.s_instance.ToString()
			) );
		}


		[UnityTest]
		[Timeout(int.MaxValue)]
		public IEnumerator TestDontRegisterCoroutine() => From( async () => {
			var c = new CoroutineProcess(
				TestCoroutineBody( 0 ), () => Log.Debug( $"onCompleted : {0}" ), false, false );

			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Return ) ).Subscribe( _ => {
					Log.Warning( "key down Play()" );
					c.Play();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.RightShift ) ).Subscribe( _ => {
					Log.Warning( "key down Pause()" );
					c.Pause();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down Dispose()" );
					c.Dispose();
				} )
			);

			await UniTaskUtility.WaitWhile( _asyncCancel, () => true );
		} );


		[UnityTest]
		[Timeout(int.MaxValue)]
		public IEnumerator TestAutoCoroutine() => From( async () => {
			var c = new CoroutineProcess(
				TestCoroutineBody( 0 ), () => Log.Debug( $"onCompleted : {0}" ), true, true );

			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Return ) ).Subscribe( _ => {
					Log.Warning( "key down Play()" );
					c.Play();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.RightShift ) ).Subscribe( _ => {
					Log.Warning( "key down Pause()" );
					c.Pause();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down Dispose()" );
					c.Dispose();
				} )
			);

			await UniTaskUtility.WaitWhile( _asyncCancel, () => true );
		} );


		[UnityTest]
		[Timeout(int.MaxValue)]
		public IEnumerator TestCoroutine() => From( async () => {
			var c = new CoroutineProcess(
				TestCoroutineBody( 0 ), () => Log.Debug( $"onCompleted : {0}" ), false, true );

			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Return ) ).Subscribe( _ => {
					Log.Warning( "key down Play()" );
					c.Play();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.RightShift ) ).Subscribe( _ => {
					Log.Warning( "key down Pause()" );
					c.Pause();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down Dispose()" );
					c.Dispose();
				} )
			);

			await UniTaskUtility.WaitWhile( _asyncCancel, () => true );
		} );


		[UnityTest]
		[Timeout(int.MaxValue)]
		public IEnumerator TestCoroutineManager() => From( async () => {
			var coroutines = new List<CoroutineProcess>();
			CoroutineProcess coroutine = null;
			int maxID = 0;

			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( "key down Create" );
					var id = maxID++;
					var c = new CoroutineProcess(
						TestCoroutineBody( id ), () => Log.Debug( $"onCompleted : {id}" ), false, true );
					coroutines.Add( c );
					c.Play();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down Play()" );
					coroutine?.Play();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha3 ) ).Subscribe( _ => {
					Log.Warning( "key down Pause()" );
					coroutine?.Pause();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha0 ) ).Subscribe( _ => {
					Log.Warning( "key down Get LastOrDefault" );
					coroutine = coroutines.LastOrDefault();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down Remove" );
					if ( coroutine != null )	{ coroutines.Remove( coroutine ); }
				} )
			);

			await UniTaskUtility.WaitWhile( _asyncCancel, () => true );
		} );


		IEnumerator TestCoroutineBody( int id ) {
			Log.Debug( $"coroutine start : {id}" );
			for ( var i = 0; i < 3; i++ ) {
				Log.Debug( $"coroutine wait {i} : {id}" );
				yield return new WaitForSeconds( 1 );
			}
			Log.Debug( $"coroutine end : {id}" );
		}
	}
}