//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestModifyler
namespace SubmarineMirage.Modifyler {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;
	using Base;
	using Extension;
	using Utility;
	using Debug;



	public class SMModifyler : SMRawBase {
		public readonly LinkedList<SMModifyData> _data = new LinkedList<SMModifyData>();
		public readonly LinkedList<SMModifyData> _runData = new LinkedList<SMModifyData>();

		[SMShow] public bool _isRunning		{ get; private set; }
		[SMShow] public bool _isHaveData	=> !_data.IsEmpty();
		bool _isInternalDebug				{ get; set; }
		readonly ReactiveProperty<bool> _isInternalLock = new ReactiveProperty<bool>();
		public Func<bool> _isCanRunEvent	{ private get; set; } = () => true;

		public SMAsyncCanceler _asyncCanceler { get; private set; }



		[SMShow] public bool _isDebug {
			get => _isInternalDebug;
			set {
				CheckDisposeError( nameof( _isDebug ) );

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

		[SMShow] public bool _isLock {
			get => _isInternalLock.Value;
			set {
				CheckDisposeError( nameof( _isLock ) );

#if TestModifyler
				SMLog.Debug( $"{nameof( _isLock )} : start\n{this}" );
#endif
				_isInternalLock.Value = value;
#if TestModifyler
				SMLog.Debug( $"{nameof( _isLock )} : end\n{this}" );
#endif
			}
		}



		public SMModifyler( bool isUseAsync = true ) {
			if ( isUseAsync )	{ _asyncCanceler = new SMAsyncCanceler(); }

			_isInternalLock
				.SkipLatestValueOnSubscribe()
				.Where( @is => !@is )
				.Subscribe( _ => Run().Forget() );

			_disposables.Add( () => {
#if TestModifyler
				SMLog.Debug( $"{nameof( Dispose )} : start\n{this}" );
#endif
				_data.ForEach( d => d.Dispose() );
				_data.Clear();
				// UniTask並列実行中の配列変更は、エラーとなる為、自然終了に任せる
				// 多分、_asyncCanceler解放により、即停止される筈
//				_runData.Clear();

				_asyncCanceler?.Dispose();

				_isRunning = false;
				_isInternalLock.Dispose();
				_isInternalLock.Value = false;
				_isInternalDebug = false;
				_isCanRunEvent = null;
#if TestModifyler
				SMLog.Debug( $"{nameof( Dispose )} : end\n{this}" );
#endif
			} );
#if TestModifyler
			SMLog.Debug( $"Create : \n{this}" );
#endif
		}



		public async UniTask Register( string name, SMModifyType type,
										Func<UniTask> runEvent, Action cancelEvent = null
		) {
			CheckDisposeError( nameof( Register ) );

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

					try {
						await _runData.Select( async rd => {
							try {
								await rd.Run();
								rd.Finish();

							} catch ( OperationCanceledException ) {
								rd.Dispose();
								throw;

							} catch ( Exception ) {
								rd.Dispose();
								throw;
							}
						} );

					} finally {
// TODO : エラーになりそう
						_runData.Clear();
					}
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

			SMLog.Debug( $"{nameof( WaitDebug )} : \n{this}" );
			await UTask.WaitWhile( _asyncCanceler, () => !Input.GetKeyDown( KeyCode.M ) );
			await UTask.NextFrame( _asyncCanceler );
		}

		void CheckDisposeError( string name ) {
			if ( !_isDispose ) { return; }

			throw new ObjectDisposedException(
				$"{this.GetAboutName()}.{name}", $"既に解放済 : \n{this}" );
		}



		public override string ToString( int indent, bool isUseHeadIndent = true ) {
			var mPrefix = StringSMUtility.IndentSpace( indent + 1 );

			return base.ToString( indent, isUseHeadIndent ).InsertFirst(
				")",
				string.Join( ",\n",
					$"{mPrefix}{nameof( _data )} : \n" +
						string.Join( ",\n", _data.Select( d => d.ToLineString( indent + 2 ) ) ),
					$"{mPrefix}{nameof( _runData )} : \n" +
						string.Join( ",\n", _runData.Select( d => d.ToLineString( indent + 2 ) ) ),
					$"{mPrefix}{nameof( _asyncCanceler )} : {( _asyncCanceler?.ToLineString() )}",
					""
				),
				false
			);
		}
	}
}