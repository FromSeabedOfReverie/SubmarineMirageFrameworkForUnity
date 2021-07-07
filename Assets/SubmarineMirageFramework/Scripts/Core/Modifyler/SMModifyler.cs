//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestModifyler
namespace SubmarineMirage.Modifyler {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Base;
	using Extension;
	using Utility;
	using Debug;



	public class SMModifyler : SMRawBase {
		[SMShowLine] public string _name	{ get; private set; }

		public readonly LinkedList<SMModifyData> _data = new LinkedList<SMModifyData>();
		public readonly LinkedList<SMModifyData> _runData = new LinkedList<SMModifyData>();

		[SMShow] public bool _isRunning		{ get; private set; }
		[SMShow] public bool _isHaveData	=> !_data.IsEmpty();
		bool _isInternalDebug				{ get; set; }
		bool _isInternalLock				{ get; set; }
		public Func<bool> _isCanRunEvent	{ private get; set; } = () => true;

		public SMAsyncCanceler _asyncCanceler { get; private set; }



		[SMShow] public bool _isLock {
			get => _isInternalLock;
			set {
				CheckDisposeError( $"{nameof( _isLock )} = {value}" );

#if TestModifyler
				SMLog.Debug( $"{nameof( _isLock )} : start\n{this}" );
#endif
				var isLastInternalLock = _isInternalLock;
				_isInternalLock = value;
				if ( !_isInternalLock && isLastInternalLock ) {
					Run().Forget();
				}
#if TestModifyler
				SMLog.Debug( $"{nameof( _isLock )} : end\n{this}" );
#endif
			}
		}

		[SMShow] public bool _isDebug {
			get => _isInternalDebug;
			set {
				CheckDisposeError( $"{nameof( _isDebug )} = {value}" );

#if TestModifyler
				SMLog.Debug( $"{nameof( _isDebug )} : start\n{this}" );
#endif
				_isInternalDebug = value;
				if ( _isInternalDebug && _asyncCanceler == null ) {
					_asyncCanceler = new SMAsyncCanceler();
				}
#if TestModifyler
				SMLog.Debug( $"{nameof( _isDebug )} : end\n{this}" );
#endif
			}
		}



#region ToString
		public override string AddToString( int indent ) {
			var prefix = StringSMUtility.IndentSpace( indent );

			return string.Join( ",\n",
				"",
				$"{prefix}{nameof( _runData )} : \n" +
					string.Join( ",\n", _runData.Select( d => d.ToLineString( indent + 1 ) ) ),
				$"{prefix}{nameof( _data )} : \n" +
					string.Join( ",\n", _data.Select( d => d.ToLineString( indent + 1 ) ) ),
				$"{prefix}{nameof( _asyncCanceler )} : {( _asyncCanceler?.ToLineString() )}"
			);
		}
#endregion



		public SMModifyler( string name, bool isUseAsync = true ) {
			_name = name;
			if ( isUseAsync )	{ _asyncCanceler = new SMAsyncCanceler(); }

			_disposables.Add( () => {
#if TestModifyler
				SMLog.Debug( $"{nameof( Dispose )} : start\n{this}" );
#endif
				_data.ForEach( d => d.Dispose() );
				_data.Clear();
				// _runData.Clear()は、UniTask並列実行中の変更エラーとなる為、自然終了に任せる
				// 多分、_asyncCanceler解放により、即停止される筈

				_asyncCanceler?.Dispose();

				_isRunning = false;
				_isInternalLock = false;
				_isInternalDebug = false;
				_isCanRunEvent = null;
#if TestModifyler
				SMLog.Debug( $"{nameof( Dispose )} : end\n{this}" );
#endif
			} );
#if TestModifyler
			SMLog.Debug( $"{nameof( SMModifyler )}() : \n{this}" );
#endif
		}



		public async UniTask Register( string name, SMModifyType type,
										Func<UniTask> runEvent, Action cancelEvent = null
		) {
			CheckDisposeError( $"{nameof( Register )}( {name}, {type} )" );

#if TestModifyler
			SMLog.Debug( $"{nameof( Register )} : start\n{this}" );
#endif
			var data = new SMModifyData( name, type, runEvent, cancelEvent );
			switch ( data._type ) {
				case SMModifyType.Interrupt:
					_data.AddFirst( data );
					break;

				case SMModifyType.Priority:
					_data.AddBefore(
						data,
						d => d._type > data._type,
						() => _data.Enqueue( data )
					);
					break;

				case SMModifyType.Single:
					_data.RemoveAll(
						d => d._type == SMModifyType.Single,
						d => d.Dispose()
					);
					_data.Enqueue( data );
					break;

				case SMModifyType.Parallel:
				case SMModifyType.Normal:
					_data.Enqueue( data );
					break;
			}
#if TestModifyler
			SMLog.Debug( $"{nameof( Register )} : end\n{this}" );
#endif

			Run().Forget();

			if ( _asyncCanceler != null ) {
				await UTask.WaitWhile( _asyncCanceler, () => !data._isFinished );
			}
		}



		async UniTask Run() {
			if ( _isRunning )	{ return; }
#if TestModifyler
			SMLog.Debug( $"{nameof( Run )} : start\n{this}" );
#endif

			try {
				_isRunning = true;

				while ( _isHaveData ) {
					if ( _isDispose )			{ break; }
					if ( _isLock )				{ break; }
					if ( !_isCanRunEvent() )	{ break; }

					await WaitDebug();

					var d = _data.Dequeue();
					_runData.AddLast( d );
					if ( d._type == SMModifyType.Parallel ) {
						while ( !_data.IsEmpty() ) {
							d = _data.First.Value;
							if ( d._type != SMModifyType.Parallel )	{ break; }
							_data.RemoveFirst();
							_runData.AddLast( d );
						}
					}

					await WaitDebug();

					await _runData.Select( async rd => {
						try {
							await rd.Run();
							rd.Finish();

						} catch ( OperationCanceledException ) {
							rd.Dispose();
							// Cancelは、Modifyler外に伝搬されない（Run待機が残る為、throwしない）

						} catch ( Exception e ) {
							SMLog.Error( $"{e} : \n{this}" );
							rd.Dispose();
							// Errorは、Modifyler外に伝搬されない（Run待機が残る為、throwしない）
						}
					} );
					_runData.Clear();
				}

			} finally {
				_isRunning = false;
			}
#if TestModifyler
			SMLog.Debug( $"{nameof( Run )} : end\n{this}" );
#endif
		}



		public async UniTask WaitRunning() {
			CheckDisposeError( nameof( WaitRunning ) );

			await UTask.WaitWhile( _asyncCanceler, () => _isRunning || _isHaveData );
		}



		async UniTask WaitDebug() {
			if ( !_isDebug )	{ return; }

#if TestModifyler
			SMLog.Debug( $"{nameof( WaitDebug )} : \n{this}" );
#endif
			await UTask.WaitWhile( _asyncCanceler, () => !Input.GetKeyDown( KeyCode.M ) );
			await UTask.NextFrame( _asyncCanceler );
		}
	}
}