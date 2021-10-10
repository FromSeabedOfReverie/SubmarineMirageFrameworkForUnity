//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using UniRx;
	using KoganeUnityLib;



	public class TestSMInputManager : SMStandardTest {
		SMInputManager _inputManager	{ get; set; }



		protected override void Create() {
			_initializeEvent.AddLast( async c => {
				_inputManager = SMServiceLocator.Resolve<SMInputManager>();
				await UTask.DontWait();
			} );
		}



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestAxis() => From( async () => {
			var displayLog = SMServiceLocator.Resolve<SMDisplayLog>();
			_inputManager._updateEvent.AddLast().Subscribe( _ => {
				displayLog.Add( $"{_inputManager.GetAxis( SMInputAxis.Move )}" );
				displayLog.Add( $"{_inputManager.GetAxis( SMInputAxis.Rotate )}" );
				displayLog.Add( $"{_inputManager.GetAxis( SMInputAxis.Mouse )}" );
				displayLog.Add( $"{_inputManager.GetAxis( SMInputAxis.Debug )}" );
			} );

			await UTask.Never( _asyncCanceler );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestKey() => From( async () => {
			var displayLog = SMServiceLocator.Resolve<SMDisplayLog>();

			var key = _inputManager.GetKey( SMInputKey.Debug );
			_inputManager._updateEvent.AddLast().Subscribe( _ => {
				var text = $"{key}";
				text.Split( "\n" ).ForEach( line => displayLog.Add( line ) );
			} );
			key._enablingEvent.AddLast().Subscribe( _ =>		displayLog.Add( nameof( key._enablingEvent ) ) );
			key._enabledEvent.AddLast().Subscribe( _ =>			SMLog.Debug( nameof( key._enabledEvent ) ) );
			key._thinOutEnablingEvent.AddLast().Subscribe( _ =>	SMLog.Debug( nameof( key._thinOutEnablingEvent ) ) );
			key._disabledEvent.AddLast().Subscribe( _ =>		SMLog.Debug( nameof( key._disabledEvent ) ) );

			await UTask.Never( _asyncCanceler );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestMoveRange() => From( async () => {
			var displayLog = SMServiceLocator.Resolve<SMDisplayLog>();

			_inputManager._updateEvent.AddLast().Subscribe( _ => {
				displayLog.Add(
					$"{nameof( SMInputAxis.Mouse )} : {_inputManager.GetAxis( SMInputAxis.Mouse ).sqrMagnitude}" );
				displayLog.Add( $"{nameof( _inputManager._noMoveSqrRange )} : {_inputManager._noMoveSqrRange}" );
				displayLog.Add( $"{nameof( _inputManager._swipeSqrRange )} : {_inputManager._swipeSqrRange}" );
			} );

			await UTask.Never( _asyncCanceler );
		} );



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestSwipe() => From( async () => {
			var displayLog = SMServiceLocator.Resolve<SMDisplayLog>();

			var key = _inputManager.GetSwipe( SMInputSwipe.UpperRight );
			_inputManager._updateEvent.AddLast().Subscribe( _ => {
				var text = $"{key}";
				text.Split( "\n" ).ForEach( line => displayLog.Add( line ) );
			} );
			key._enabledEvent.AddLast().Subscribe( _ => SMLog.Debug( nameof( key._enabledEvent ) ) );

			await UTask.Never( _asyncCanceler );
		} );
	}
}