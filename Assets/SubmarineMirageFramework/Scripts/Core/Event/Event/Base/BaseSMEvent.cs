//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestEvent
namespace SubmarineMirage.Event {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Base;
	using Modifyler;
	using Extension;
	using Utility;
	using Debug;



	public abstract class BaseSMEvent : SMRawBase {
		// Reverse時に再代入される為、readonly未使用
		LinkedList<BaseSMEventData> _events	{ get; set; } = new LinkedList<BaseSMEventData>();
		// this.GetAboutName()使用には、コンストラクタでthis参照が必要の為、下記で設定しない
		[SMShow] readonly SMModifyler _modifyler;

		[SMShow] public bool _isRunning	{ get; private set; }
		bool _isInternalDebug			{ get; set; }

		SMAsyncCanceler _asyncCancelerForRun	{ get; set; }
		SMAsyncCanceler _asyncCancelerForDebug	{ get; set; }



		[SMShow] public bool _isDebug {
			get => _isInternalDebug;
			set {
				CheckDisposeError( $"{nameof( _isDebug )} = {value}" );

#if TestEvent
				SMLog.Debug( $"{nameof( _isDebug )} : start\n{this}" );
#endif
				_isInternalDebug = value;
				if ( _isInternalDebug && _asyncCancelerForDebug == null ) {
					_asyncCancelerForDebug = new SMAsyncCanceler();
				}
#if TestEvent
				SMLog.Debug( $"{nameof( _isDebug )} : end\n{this}" );
#endif
			}
		}



#region ToString
		public override string AddToString( int indent ) {
			var prefix = StringSMUtility.IndentSpace( indent );

			return string.Join( ",\n",
				"",
				$"{prefix}{nameof( _events )} : \n" +
					string.Join( ",\n", _events.Select( d => d.ToLineString( indent + 1 ) ) ),
				$"{prefix}{nameof( _asyncCancelerForRun )} : {( _asyncCancelerForRun?.ToLineString() )}",
				$"{prefix}{nameof( _asyncCancelerForDebug )} : {( _asyncCancelerForDebug?.ToLineString() )}"
			);
		}
#endregion



		public BaseSMEvent() {
			_modifyler = new SMModifyler( this.GetAboutName(), false );

			_disposables.Add( () => {
#if TestEvent
				SMLog.Debug( $"{nameof( Dispose )} : start\n{this}" );
#endif
				_modifyler.Dispose();
				_events.ForEach( d => d.Dispose() );
				_events.Clear();

				_asyncCancelerForDebug?.Dispose();
				_asyncCancelerForRun?.Cancel();
				_asyncCancelerForRun = null;

				_isRunning = false;
				_isInternalDebug = false;
#if TestEvent
				SMLog.Debug( $"{nameof( Dispose )} : end\n{this}" );
#endif
			} );
#if TestEvent
			SMLog.Debug( $"{this.GetAboutName()}() : \n{this}" );
#endif
		}



		protected async UniTask Run( SMAsyncCanceler canceler ) {
			CheckDisposeError( nameof( Run ) );
			if ( _isRunning ) {
				SMLog.Warning( $"{this.GetAboutName()}.{nameof( Run )} : 既に実行中の為、未実行\n{this}" );
				return;
			}
#if TestEvent
			SMLog.Debug( $"{nameof( Run )} : start\n{this}" );
#endif

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
				// Run呼び出し先も停止したい為、外に伝搬する
				throw;

			} finally {
				if ( !_isDispose ) {
					_asyncCancelerForRun = null;
					_modifyler._isLock = false;
					_isRunning = false;
				}
			}
#if TestEvent
			SMLog.Debug( $"{nameof( Run )} : end\n{this}" );
#endif
		}



		protected void Remove( string findKey ) => _modifyler.Register(
			nameof( Remove ),
			SMModifyType.Normal,
			async () => {
				_events.RemoveAll(
					d => d._key == findKey,
					d => d.Dispose()
				);
				await UTask.DontWait();
			}
		).Forget();

		protected void Reverse() => _modifyler.Register(
			nameof( Reverse ),
			SMModifyType.Normal,
			async () => {
				_events = _events.Reverse();
				await UTask.DontWait();
			}
		).Forget();



		protected void InsertFirst( string findKey, BaseSMEventData data ) => _modifyler.Register(
			nameof( InsertFirst ),
			SMModifyType.Normal,
			async () => {
				_events.AddBefore(
					data,
					d => d._key == findKey,
					() => NoSupportError( nameof( InsertFirst ), findKey )
				);
				await UTask.DontWait();
			},
			() => data.Dispose()
		).Forget();

		protected void InsertLast( string findKey, BaseSMEventData data ) => _modifyler.Register(
			nameof( InsertLast ),
			SMModifyType.Normal,
			async () => {
				_events.AddAfter(
					data,
					d => d._key == findKey,
					() => NoSupportError( nameof( InsertLast ), findKey )
				);
				await UTask.DontWait();
			},
			() => data.Dispose()
		).Forget();



		protected void AddFirst( BaseSMEventData data ) => _modifyler.Register(
			nameof( AddFirst ),
			SMModifyType.Normal,
			async () => {
				_events.AddFirst( data );
				await UTask.DontWait();
			},
			() => data.Dispose()
		).Forget();

		protected void AddLast( BaseSMEventData data ) => _modifyler.Register(
			nameof( AddLast ),
			SMModifyType.Normal,
			async () => {
				_events.AddLast( data );
				await UTask.DontWait();
			},
			() => data.Dispose()
		).Forget();



		async UniTask WaitDebug() {
			if ( !_isDebug )	{ return; }

#if TestEvent
			SMLog.Debug( $"{nameof( WaitDebug )} : \n{this}" );
#endif
			await UTask.WaitWhile( _asyncCancelerForDebug, () => !Input.GetKeyDown( KeyCode.E ) );
			await UTask.NextFrame( _asyncCancelerForDebug );
		}

		void NoSupportError( string name, string findKey )
			=> throw new NotSupportedException( string.Join( "\n",
				$"未登録 : {findKey}",
				$"{nameof( BaseSMEvent )}.{name}",
				$"{_events}"
			) );
	}
}