//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event {
	using System;
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



	public abstract class BaseSMEvent : SMRawBase, ISMModifyTarget {
		// ReverseSMEventで再代入される為、public setter
		public LinkedList<BaseSMEventData> _events	{ get; set; } = new LinkedList<BaseSMEventData>();
		public SMModifyler _modifyler	{ get; private set; }

		[SMShow] public bool _isRunning		{ get; private set; }
		bool _isInternalDebug { get; set; }

		[SMShow] public bool _isDebug {
			get => _isInternalDebug;
			set {
				_isInternalDebug = value;
				if ( _isInternalDebug && _asyncCancelerForDebug == null ) {
					_asyncCancelerForDebug = new SMAsyncCanceler();
				}
			}
		}

		public SMAsyncCanceler _asyncCancelerForDebug { get; private set; }



		public BaseSMEvent() {
			_modifyler = new SMModifyler( this, typeof( SMEventModifyData ), false );

			_disposables.Add( () => {
				_modifyler.Dispose();
				_events.ForEach( data => data.Dispose() );
				_events.Clear();

				_asyncCancelerForDebug?.Dispose();
				_isRunning = false;
			} );
		}



		protected void Remove( string findKey )
			=> _modifyler.Register( new RemoveSMEvent( findKey ) );

		protected void Reverse()
			=> _modifyler.Register( new ReverseSMEvent() );

		protected void InsertFirst( string findKey, BaseSMEventData data )
			=> _modifyler.Register( new InsertFirstSMEvent( findKey, data ) );

		protected void InsertLast( string findKey, BaseSMEventData data )
			=> _modifyler.Register( new InsertLastSMEvent( findKey, data ) );

		protected void AddFirst( BaseSMEventData data )
			=> _modifyler.Register( new AddFirstSMEvent( data ) );

		protected void AddLast( BaseSMEventData data )
			=> _modifyler.Register( new AddLastSMEvent( data ) );



		protected async UniTask Run( SMAsyncCanceler canceler ) {
			CheckDisposeError();
			if ( _isRunning ) {
				SMLog.Warning( $"{this.GetAboutName()}.{nameof( Run )} : 既に実行中の為、未実行\n{this}" );
				return;
			}

// TODO : _isRunning中は、リンク変更出来ないようにする
			_isRunning = true;
			for ( var n = _events.First; n != null; n = n.Next ) {
				if ( _isDispose )	{ break; }

				if ( _isDebug ) {
					await UTask.WaitWhile( _asyncCancelerForDebug, () => !Input.GetKeyDown( KeyCode.E ) );
					await UTask.NextFrame( _asyncCancelerForDebug );
				}

//				try {
					await n.Value.Run( canceler );
//				} catch ( OperationCanceledException ) {
//				}

				if ( _isDebug ) {
					await UTask.WaitWhile( _asyncCancelerForDebug, () => !Input.GetKeyDown( KeyCode.E ) );
					await UTask.NextFrame( _asyncCancelerForDebug );
				}
			}
			_isRunning = false;
		}



		protected void CheckDisposeError( SMEventModifyData data = null ) {
			if ( !_isDispose )	{ return; }
			data?.Dispose();
			throw new ObjectDisposedException( $"{nameof( _disposables )}", "既に解放済" );
		}



		public override string ToString( int indent, bool isUseHeadIndent = true ) {
			indent++;
			var mPrefix = StringSMUtility.IndentSpace( indent );

			return base.ToString( indent, isUseHeadIndent ).InsertFirst(
				")",
				string.Join( "\n",
					$"{mPrefix}{nameof( _events )} : {_events.ToLineString( indent )}",
					$"{mPrefix}{nameof( _modifyler )} : {_modifyler.ToString( indent )},",
					""
				),
				false
			);
		}
	}
}