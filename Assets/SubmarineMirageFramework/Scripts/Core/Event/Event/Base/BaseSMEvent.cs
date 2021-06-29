//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Base;
	using SubmarineMirage.Modifyler;
	using Event.Modifyler;
	using Extension;
	using Utility;
	using Debug;



	public abstract class BaseSMEvent : SMRawBase {
		// ReverseSMEventで再代入される為、public setter
		public LinkedList<BaseSMEventData> _events	{ get; set; } = new LinkedList<BaseSMEventData>();
		public SMModifyler _modifyler	{ get; private set; }

		[SMShow] public bool _isRunning	{ get; private set; }
		bool _isInternalDebug			{ get; set; }

		[SMShow] public bool _isDebug {
			get => _isInternalDebug;
			set {
				_isInternalDebug = value;
				if ( _isInternalDebug && _asyncCancelerForDebug == null ) {
					_asyncCancelerForDebug = new SMAsyncCanceler();
				}
			}
		}

		SMAsyncCanceler _asyncCancelerForRun	{ get; set; }
		SMAsyncCanceler _asyncCancelerForDebug	{ get; set; }



		public BaseSMEvent() {
			_modifyler = new SMModifyler( this, typeof( SMEventModifyData ), false );

			_disposables.Add( () => {
				_modifyler.Dispose();
				_events.ForEach( data => data.Dispose() );
				_events.Clear();

				_asyncCancelerForDebug?.Dispose();
				_asyncCancelerForRun?.Cancel();
				_asyncCancelerForRun = null;

				_isRunning = false;
			} );
		}



		protected void Remove( string findKey )
			=> _modifyler.Register( new RemoveSMEvent( findKey ) );

		protected void Reverse()
			=> _modifyler.Register( new ReverseSMEvent() );

		protected void InsertFirst( string findKey, BaseSMEventData data ) {
			data.Set( this );
			_modifyler.Register( new InsertFirstSMEvent( findKey, data ) );
		}

		protected void InsertLast( string findKey, BaseSMEventData data ) {
			data.Set( this );
			_modifyler.Register( new InsertLastSMEvent( findKey, data ) );
		}

		protected void AddFirst( BaseSMEventData data ) {
			data.Set( this );
			_modifyler.Register( new AddFirstSMEvent( data ) );
		}

		protected void AddLast( BaseSMEventData data ) {
			data.Set( this );
			_modifyler.Register( new AddLastSMEvent( data ) );
		}



		protected async UniTask Run( SMAsyncCanceler canceler ) {
			if ( _isDispose ) {
				throw new ObjectDisposedException(
					$"{this.GetAboutName()}.{nameof( Run )}", $"既に解放済 : {this}" );
			}
			if ( _isRunning ) {
				SMLog.Warning( $"{this.GetAboutName()}.{nameof( Run )} : 既に実行中の為、未実行\n{this}" );
				return;
			}

			try {
				_isRunning = true;
				_modifyler._isLock = true;
				_asyncCancelerForRun = canceler;

				for ( var n = _events.First; n != null; n = n.Next ) {
					if ( _isDispose )	{ break; }

					await WaitDebug();
					await n.Value.Run( _asyncCancelerForRun );
				}

			} catch ( OperationCanceledException ) {
				throw;

			} finally {
				_asyncCancelerForRun = null;
				_modifyler._isLock = false;
				_isRunning = false;
			}
		}



		async UniTask WaitDebug() {
			if ( !_isDebug )	{ return; }

			await UTask.WaitWhile( _asyncCancelerForDebug, () => !Input.GetKeyDown( KeyCode.E ) );
			await UTask.NextFrame( _asyncCancelerForDebug );
		}



		public override string ToString( int indent, bool isUseHeadIndent = true ) {
			var mPrefix = StringSMUtility.IndentSpace( indent + 1 );

			return base.ToString( indent, isUseHeadIndent ).InsertFirst(
				")",
				string.Join( ",\n",
					$"{mPrefix}{nameof( _events )} : \n" +
						string.Join( ",\n", _events.Select( d => d.ToLineString( indent + 2 ) ) ),
					$"{mPrefix}{nameof( _modifyler )} : {_modifyler.ToString( indent + 1 )},",
					""
				),
				false
			);
		}
	}
}