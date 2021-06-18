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
	using KoganeUnityLib;
	using Base;
	using Extension;
	using Utility;
	using Debug;



	public class SMModifyler : SMStandardBase {
		ISMModifyTarget _target	{ get; set; }
		[SMShow] Type _baseDataType { get; set; }
		[SMShow] public readonly LinkedList<SMModifyData> _data = new LinkedList<SMModifyData>();
		[SMShow] public readonly LinkedList<SMModifyData> _runData = new LinkedList<SMModifyData>();

		[SMShow] public bool _isRunning		{ get; private set; }
		[SMShow] public bool _isHaveData	=> !_data.IsEmpty();
		[SMShow] public bool _isDebug		{ get; set; }
		public Func<bool> _isCanRunEvent	{ private get; set; } = () => true;

		public readonly SMAsyncCanceler _asyncCanceler = new SMAsyncCanceler();



#region ToString
		public override void SetToString() {
			base.SetToString();
			_toStringer.Add( nameof( _target ), i => _toStringer.DefaultValue( _target, i, true ) );
			_toStringer.SetValue( nameof( _data ), i => _toStringer.DefaultValue( _data, i, true ) );
		}
#endregion



		public SMModifyler( ISMModifyTarget target, Type baseDataType ) {
			_target = target;
			_baseDataType = baseDataType;

			_disposables.AddFirst( () => {
				Reset();
				_runData.Clear();

				_asyncCanceler.Dispose();
				_isRunning = false;
				_isCanRunEvent = null;
			} );
		}

		public void Reset() {
			_data.ForEach( d => d.Dispose() );
			_data.Clear();
		}



		public void Register( SMModifyData data ) {
			var type = data.GetType();
			if ( !type.IsInheritance( _baseDataType ) ) {
				throw new InvalidOperationException(
					$"基盤状態が違う、データを指定 : {type}, {_baseDataType}" );
			}

			data.Set( _target, this );
			if ( _isDispose ) {
				data.Dispose();
				return;
			}

			switch( data._type ) {
				case SMModifyType.FirstRunner:
					_data.AddBefore(
						data,
						d => d._type > data._type,
						() => _data.Enqueue( data )
					);
					break;

				case SMModifyType.SingleRunner:
					_data.RemoveAll(
						d => d._type == SMModifyType.SingleRunner,
						d => d.Dispose()
					);
					_data.Enqueue( data );
					break;

				case SMModifyType.ParallellRunner:
				case SMModifyType.Runner:
					_data.Enqueue( data );
					break;
			}
			Run().Forget();
		}

		public void Unregister( Func<SMModifyData, bool> isFindEvent ) => _data.RemoveAll(
			d => isFindEvent( d ),
			d => d.Dispose()
		);



		public async UniTask Run() {
			if ( _isRunning )	{ return; }

			_isRunning = true;
			while ( _isHaveData ) {
				if ( _isDispose )			{ break; }
				if ( !_isCanRunEvent() )	{ break; }

				if ( _isDebug ) {
					await UTask.WaitWhile( _asyncCanceler, () => !Input.GetKeyDown( KeyCode.M ) );
					await UTask.NextFrame( _asyncCanceler );
				}

				var d = _data.Dequeue();
				_runData.AddLast( d );

				if ( d._type == SMModifyType.ParallellRunner ) {
					while ( !_data.IsEmpty() ) {
						d = _data.First.Value;
						if ( d._type != SMModifyType.ParallellRunner )	{ break; }
						_data.RemoveFirst();
						_runData.AddLast( d );
					}
				}

				if ( _isDebug ) {
					await UTask.WaitWhile( _asyncCanceler, () => !Input.GetKeyDown( KeyCode.M ) );
					await UTask.NextFrame( _asyncCanceler );
				}

				await _runData.Select( async rd => {
//					try {
						await rd.Run();
						rd.Finish();
//					} catch ( OperationCanceledException ) {
//					}
				} );
				_runData.Clear();
			}
			_isRunning = false;
		}

		public UniTask WaitRunning()
			=> UTask.WaitWhile( _asyncCanceler, () => _isRunning || _isHaveData );

		public async UniTask RegisterAndWaitRunning( SMModifyData data ) {
			Register( data );
			await UTask.WaitWhile( _asyncCanceler, () => !data._isFinished );
		}
	}
}