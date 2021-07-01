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
	using Modifyler;
	using Extension;
	using Utility;
	using Debug;



	public abstract class BaseSMEvent : SMRawBase {
		// Reverse時に再代入される為、readonly未使用
		LinkedList<BaseSMEventData> _events	{ get; set; } = new LinkedList<BaseSMEventData>();
		readonly SMModifyler _modifyler = new SMModifyler( false );

		[SMShow] public bool _isRunning	{ get; private set; }
		bool _isInternalDebug			{ get; set; }

		SMAsyncCanceler _asyncCancelerForRun	{ get; set; }
		SMAsyncCanceler _asyncCancelerForDebug	{ get; set; }



		[SMShow] public bool _isDebug {
			get => _isInternalDebug;
			set {
				_isInternalDebug = value;
				if ( _isInternalDebug && _asyncCancelerForDebug == null ) {
					_asyncCancelerForDebug = new SMAsyncCanceler();
				}
			}
		}



		public BaseSMEvent() {
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



		protected void InsertFirst( string findKey, BaseSMEventData data ) {
			data.Set( this );

			_modifyler.Register(
				nameof( InsertFirst ),
				SMModifyType.Normal,
				async () => {
					_events.AddBefore(
						data,
						d => d._key == findKey,
						() => throw new NotSupportedException( string.Join( "\n",
							$"{nameof( BaseSMEvent )}.{nameof( InsertFirst )} : 未登録 : {findKey}",
							$"{_events}"
						) )
					);
					await UTask.DontWait();
				},
				() => data.Dispose()
			).Forget();
		}

		protected void InsertLast( string findKey, BaseSMEventData data ) {
			data.Set( this );

			_modifyler.Register(
				nameof( InsertLast ),
				SMModifyType.Normal,
				async () => {
					_events.AddAfter(
						data,
						d => d._key == findKey,
						() => throw new NotSupportedException( string.Join( "\n",
							$"{nameof( BaseSMEvent )}.{nameof( InsertLast )} : 未登録 : {findKey}",
							$"{_events}"
						) )
					);
					await UTask.DontWait();
				},
				() => data.Dispose()
			).Forget();
		}



		protected void AddFirst( BaseSMEventData data ) {
			data.Set( this );

			_modifyler.Register(
				nameof( AddFirst ),
				SMModifyType.Normal,
				async () => {
					_events.AddFirst( data );
					await UTask.DontWait();
				},
				() => data.Dispose()
			).Forget();
		}

		protected void AddLast( BaseSMEventData data ) {
			data.Set( this );

			_modifyler.Register(
				nameof( AddLast ),
				SMModifyType.Normal,
				async () => {
					_events.AddLast( data );
					await UTask.DontWait();
				},
				() => data.Dispose()
			).Forget();
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