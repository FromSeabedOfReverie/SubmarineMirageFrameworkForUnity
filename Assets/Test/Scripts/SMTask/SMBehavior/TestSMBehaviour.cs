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
	using SMTask;
	using Extension;
	using Utility;
	using Debug;
	using Test;
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public class TestSMBehaviour : Test {
		SMBehaviour _behaviour;
		Text _text;


		protected override void Create() {
			Application.targetFrameRate = 30;
			_behaviour = new B6();

			_createEvent.AddLast( async cancel => {
				TestSMTaskUtility.SetEvent( _behaviour );
				await UniTaskUtility.DontWait();
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
					+ $"    {_behaviour._body._ranState}\n"
					+ $"    {_behaviour._body._activeState}\n"
					+ $"    next : {_behaviour._body._nextActiveState}\n"
					+ $")";
			} ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_disposables.AddLast( _behaviour );
		}


		[UnityTest]
		public IEnumerator TestRunErrorState() => From( async () => {
			var errorRuns = new SMTaskRanState[] {
				SMTaskRanState.None, SMTaskRanState.Created, SMTaskRanState.Loaded, SMTaskRanState.Initialized,
				SMTaskRanState.Finalized,
			};
			foreach ( var state in errorRuns ) {
				try						{ await _behaviour.RunStateEvent( state ); }
				catch ( Exception e )	{ Log.Error( e ); }
			}
		} );


		[UnityTest]
		public IEnumerator TestRunStateEvent() => From( async () => {
			var states = new SMTaskRanState[] {
				SMTaskRanState.Creating, SMTaskRanState.Loading, SMTaskRanState.Initializing,
				SMTaskRanState.FixedUpdate, SMTaskRanState.Update, SMTaskRanState.LateUpdate,
				SMTaskRanState.Finalizing,
			};
			foreach( var run in states ) {
				Log.Debug( $"request : {run}" );
				await _behaviour.RunStateEvent( run );
			}
		} );


		[UnityTest]
		public IEnumerator TestStopActiveAsync() => From( async () => {
			UniTask.Void( async () => {
				await UniTaskUtility.Delay( _asyncCancel, 3000 );
				Log.Debug( $"{nameof( _behaviour.StopActiveAsync )}" );
				_behaviour.StopActiveAsync();
			} );
			try {
				while ( true ) {
					Log.Debug( "Runnning" );
					await UniTaskUtility.Delay( _behaviour._activeAsyncCancel, 1000 );
				}
			} catch ( OperationCanceledException ) {
			}
			Log.Debug( $"end {nameof( TestStopActiveAsync )}" );
		} );


		[UnityTest]
		public IEnumerator TestDispose() => From( async () => {
			UniTask.Void( async () => {
				await UniTaskUtility.Delay( _asyncCancel, 3000 );
				Log.Debug( $"{nameof( _behaviour.Dispose )}" );
				_behaviour.Dispose();
			} );
			try {
				while ( true ) {
					Log.Debug( "Runnning" );
					await UniTaskUtility.Delay( _behaviour._activeAsyncCancel, 1000 );
				}
			} catch ( OperationCanceledException ) {
			}
			Log.Debug( $"end {nameof( TestDispose )}" );
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( TestManualSub() );
		IEnumerator TestManualSub() {
			_disposables.AddLast( TestSMTaskUtility.SetRunKey( _behaviour ) );
			while ( true )	{ yield return null; }
		}
	}
}