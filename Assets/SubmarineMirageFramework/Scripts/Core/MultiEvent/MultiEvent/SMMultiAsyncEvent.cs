//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.MultiEvent {
	using System;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMMultiAsyncEvent : BaseSMMultiEvent< Func<SMAsyncCanceler, UniTask> > {
		static readonly string EVENT_KEY = $"{nameof( SMMultiAsyncEvent )}.{nameof( Run )}";
		string _eventKey	{ get; set; }
		[SMHide] SMAsyncCanceler _canceler	{ get; set; }
		[SMHide] Func<bool, bool> _isThrowCancelEvent	{ get; set; }
		[SMShowLine] public bool _isRunning	{ get; private set; }


		public SMMultiAsyncEvent( Func<bool, bool> isThrowCancelEvent = null ) {
			_eventKey = $"{EVENT_KEY}{_id}";
			_isThrowCancelEvent = isThrowCancelEvent;
		}

		public override void Dispose() {
			// _disposables.Add()だと、最初期実行登録ができない
			_canceler?.Cancel();
			_canceler?._cancelEvent?.Remove( _eventKey );
			_canceler = null;
			base.Dispose();
		}

		public override void OnRemove( Func<SMAsyncCanceler, UniTask> function ) {}


		public async UniTask Run( SMAsyncCanceler canceler ) {
			CheckDisposeError();

			if ( _isRunning ) {
				SMLog.Warning( $"既に実行中の為、未実行 : {this}" );
				return;
			}

			var isCancelByCanceler = false;
			try {
				_isRunning = true;
				_canceler = canceler;
				_canceler._cancelEvent.AddLast( _eventKey ).Subscribe( _ => isCancelByCanceler = true );
				var temp = _events.Copy();
				foreach ( var pair in temp ) {
					if ( isCancelByCanceler )	{ break; }
					await pair.Value.Invoke( _canceler );
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


		public override string ToString( int indent, bool isUseHeadIndent = true ) {
			indent++;
			var mPrefix = StringSMUtility.IndentSpace( indent );

			return base.ToString( indent, isUseHeadIndent ).InsertFirst( ")",
				string.Join( "\n",
					$"{mPrefix}{nameof( _eventKey )} : {_eventKey},",
					$"{mPrefix}{nameof( _isRunning )} : {_isRunning},"
				)
				 + "\n"
			);
		}
	}
}