//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Modifyler {
	using System;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Base;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMModifyler : SMStandardBase {
		ISMModifyTarget _target	{ get; set; }
		[SMShow] Type _baseDataType { get; set; }
		[SMShow] readonly LinkedList<SMModifyData> _data = new LinkedList<SMModifyData>();

		[SMShow] public bool _isRunning	{ get; private set; }
		public Func<bool> _isCanRunEvent	{ private get; set; } = () => true;
		public bool _isHaveData => !_data.IsEmpty();

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

			_disposables.AddLast( () => {
				Reset();

				_asyncCanceler.Dispose();
				_isRunning = false;
				_isCanRunEvent = null;
			} );
		}


		public void Reset() {
			_data.ForEach( d => d.Dispose() );
			_data.Clear();
		}

		public void Move( SMModifyler remove ) {
			remove._data.ForEach( d => Register( d ) );
			remove._data.Clear();
			remove.Dispose();
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
				case SMModifyType.FirstLinker:
				case SMModifyType.Linker:
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

				case SMModifyType.Runner:
					_data.Enqueue( data );
					break;
			}
			Run().Forget();
		}

		public void Reregister( SMModifyler newModifyler, Func<SMModifyData, bool> isFindEvent )
			=> _data.RemoveAll(
				d => isFindEvent( d ),
				d => newModifyler.Register( d )
			);

		public void Unregister( Func<SMModifyData, bool> isFindEvent ) => _data.RemoveAll(
			d => isFindEvent( d ),
			d => d.Dispose()
		);


		public async UniTask RegisterAndRun( SMModifyData data ) {
			Register( data );
			await WaitRunning();
		}

		public async UniTask Run() {
			if ( _isRunning )	{ return; }

			_isRunning = true;
			while ( _isHaveData ) {
				if ( _isDispose )			{ break; }
				if ( !_isCanRunEvent() )	{ break; }

				var d = _data.Dequeue();
				await d.Run();
			}
			_isRunning = false;
		}

		public UniTask WaitRunning()
			=> UTask.WaitWhile( _asyncCanceler, () => _isRunning || _isHaveData );
	}
}