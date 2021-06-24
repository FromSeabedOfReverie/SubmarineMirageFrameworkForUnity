//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
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
		ISMModifyTarget _target	{ get; set; }
		[SMShow] Type _baseDataType { get; set; }
		public readonly LinkedList<SMModifyData> _data = new LinkedList<SMModifyData>();
		public readonly LinkedList<SMModifyData> _runData = new LinkedList<SMModifyData>();

		[SMShow] public bool _isRunning		{ get; private set; }
		[SMShow] public bool _isHaveData	=> !_data.IsEmpty();
		bool _isInternalDebug				{ get; set; }

		[SMShow] public bool _isDebug {
			get => _isInternalDebug;
			set {
				_isInternalDebug = value;
				if ( _isInternalDebug && _asyncCanceler == null ) {
					_asyncCanceler = new SMAsyncCanceler();
				}
			}
		}

		public readonly ReactiveProperty<bool> _isLock = new ReactiveProperty<bool>();
		public Func<bool> _isCanRunEvent	{ private get; set; } = () => true;

		public SMAsyncCanceler _asyncCanceler { get; private set; }



		public SMModifyler( ISMModifyTarget target, Type baseDataType, bool isUseAsync = true ) {
			_target = target;
			_baseDataType = baseDataType;
			if ( isUseAsync )	{ _asyncCanceler = new SMAsyncCanceler(); }

			_isLock
				.Where( @is => !@is )
				.Subscribe( _ => Run().Forget() );

			_disposables.Add( () => {
				_data.ForEach( d => d.Dispose() );
				_data.Clear();
				_runData.Clear();

				_asyncCanceler?.Dispose();

				_isRunning = false;
				_isLock.Dispose();
				_isCanRunEvent = null;
			} );
		}



		public void Register( SMModifyData data ) {
			if ( _isDispose ) {
				throw new ObjectDisposedException(
					$"{this.GetAboutName()}.{nameof( Register )}", $"既に解放済 : {this}" );
			}
			var type = data.GetType();
			if ( !type.IsInheritance( _baseDataType ) ) {
				throw new InvalidOperationException( string.Join( "\n",
					$"{this.GetAboutName()}.{nameof( Register )} : 違う基底クラスのデータを指定",
					$"{nameof( type )} : {type}",
					$"{nameof( _baseDataType )} : {_baseDataType}"
				) );
			}

			data.Set( _target, this );

			switch ( data._type ) {
				case SMModifyType.Interrupt:
					_data.AddFirst( data );
					break;

				case SMModifyType.First:
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
				case SMModifyType.Last:
					_data.Enqueue( data );
					break;
			}
			Run().Forget();
		}

		public void Unregister( Func<SMModifyData, bool> isFindEvent ) {
			if ( _isDispose ) {
				throw new ObjectDisposedException(
					$"{this.GetAboutName()}.{nameof( Unregister )}", $"既に解放済 : {this}" );
			}

			_data.RemoveAll(
				d => isFindEvent( d ),
				d => d.Dispose()
			);
		}



		public async UniTask Run() {
			if ( _isDispose ) {
				throw new ObjectDisposedException(
					$"{this.GetAboutName()}.{nameof( Run )}", $"既に解放済 : {this}" );
			}
			if ( _isRunning )		{ return; }

			try {
				_isRunning = true;
				while ( _isHaveData ) {
					if ( _isDispose )			{ break; }
					if ( _isLock.Value )		{ break; }
					if ( !_isCanRunEvent() )	{ break; }

					if ( _isDebug ) {
						await UTask.WaitWhile( _asyncCanceler, () => !Input.GetKeyDown( KeyCode.M ) );
						await UTask.NextFrame( _asyncCanceler );
					}

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

					if ( _isDebug ) {
						await UTask.WaitWhile( _asyncCanceler, () => !Input.GetKeyDown( KeyCode.M ) );
						await UTask.NextFrame( _asyncCanceler );
					}

					await _runData.Select( async rd => {
						await rd.Run();
						rd.Finish();
					} );
					_runData.Clear();
				}

			} catch ( OperationCanceledException ) {
				throw;

			} finally {
				_isRunning = false;
			}
		}

		public async UniTask WaitRunning() {
			if ( _isDispose ) {
				throw new ObjectDisposedException(
					$"{this.GetAboutName()}.{nameof( WaitRunning )}", $"既に解放済 : {this}" );
			}

			await UTask.WaitWhile( _asyncCanceler, () => _isRunning || _isHaveData );
		}

		public async UniTask RegisterAndWaitRunning( SMModifyData data ) {
			if ( _isDispose ) {
				throw new ObjectDisposedException(
					$"{this.GetAboutName()}.{nameof( RegisterAndWaitRunning )}", $"既に解放済 : {this}" );
			}

			Register( data );
			await UTask.WaitWhile( _asyncCanceler, () => !data._isFinished );
		}



		public override string ToString( int indent, bool isUseHeadIndent = true ) {
			indent++;
			var mPrefix = StringSMUtility.IndentSpace( indent );

			return base.ToString( indent, isUseHeadIndent ).InsertFirst(
				")",
				string.Join( "\n",
					$"{mPrefix}{nameof( _target )}{_target.ToLineString( indent )}",
					$"{mPrefix}{nameof( _data )}{_data.ToLineString( indent )}",
					$"{mPrefix}{nameof( _runData )}{_runData.ToLineString( indent )}",
					""
				),
				false
			);
		}
/*
		public override string ToString( int indent, bool isUseHeadIndent = true ) {
			var prefix = StringSMUtility.IndentSpace( indent );
			var hPrefix = isUseHeadIndent ? prefix : "";
			indent++;
			var mPrefix = StringSMUtility.IndentSpace( indent );
			indent++;

			return string.Join( "\n",
				$"{hPrefix}{this.GetAboutName()}(",
				$"{mPrefix}{nameof( _isRunning )} : {_isRunning},",
				$"{mPrefix}{nameof( _data )} :",
				string.Join( ",\n", _data.Select( d =>
					d.ToLineString( indent )
				) ),
				$"{prefix})"
			);
		}
*/
	}
}