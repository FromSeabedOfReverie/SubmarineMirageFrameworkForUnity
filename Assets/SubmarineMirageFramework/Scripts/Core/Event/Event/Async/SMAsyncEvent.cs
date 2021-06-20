//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event {
	using System;
	using System.Linq;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Extension;
	using Utility;
	using Debug;



	public class SMAsyncEvent : BaseSMEvent {
		static readonly string EVENT_KEY = $"{nameof( SMAsyncEvent )}.{nameof( Run )}";
		[SMShow] string _eventKey	{ get; set; }
		SMAsyncCanceler _canceler	{ get; set; }
		Func<bool, bool> _isThrowCancelEvent	{ get; set; }



		public SMAsyncEvent( Func<bool, bool> isThrowCancelEvent = null ) {
			_eventKey = $"{EVENT_KEY}{_id}";
			_isThrowCancelEvent = isThrowCancelEvent;
		}

		public override void Dispose() {
			// _disposables.Add()だと、最初期実行登録ができない
			if ( _canceler != null ) {
				_canceler.Cancel();
				_canceler._cancelEvent.Remove( _eventKey );
				_canceler = null;
			}
			base.Dispose();
		}



		public async UniTask Run( SMAsyncCanceler canceler ) {
			var isCancelByCanceler = false;
			try {
				_isRunning = true;
				_canceler = canceler;
				_canceler._cancelEvent.AddLast( _eventKey ).Subscribe( _ => isCancelByCanceler = true );
				_events
					.Select( e => e as SMAsyncEventData )
					.ForEach( e => e._canceler = _canceler );

				foreach ( var data in _events ) {
					if ( isCancelByCanceler )	{ break; }
					await data.Run();
				}

			} catch ( OperationCanceledException ) {
				if ( _isThrowCancelEvent == null || _isThrowCancelEvent.Invoke( isCancelByCanceler ) ) {
					throw;
				}

			} finally {
				_canceler?._cancelEvent?.Remove( _eventKey );
				_canceler = null;
				_isRunning = false;
			}
		}
	}
}