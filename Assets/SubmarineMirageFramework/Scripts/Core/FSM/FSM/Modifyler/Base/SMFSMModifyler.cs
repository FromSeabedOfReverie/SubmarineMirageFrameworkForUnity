//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler.Base {
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using SubmarineMirage.Base;
	using FSM.Base;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMFSMModifyler : SMStandardBase {
		SMFSMBody _owner	{ get; set; }
		[SMShow] readonly LinkedList<SMFSMModifyData> _data = new LinkedList<SMFSMModifyData>();
		[SMShowLine] bool _isRunning	{ get; set; }


#region ToString
		public override void SetToString() {
			base.SetToString();
			_toStringer.SetValue( nameof( _data ), i => _toStringer.DefaultValue( _data, i, true ) );
		}
#endregion


		public SMFSMModifyler( SMFSMBody owner ) {
			_owner = owner;

			_disposables.AddLast( () => {
				Reset();
				_isRunning = false;
			} );
		}


		public void Reset() {
			_data.ForEach( d => d.Dispose() );
			_data.Clear();
		}


		public bool IsHaveData() => !_data.IsEmpty();


		public void Register( SMFSMModifyData data ) {
			data.Set( _owner );
			if ( _isDispose ) {
				data.Dispose();
				return;
			}

			switch( data._type ) {
				case SMFSMModifyType.FirstRunner:
					_data.AddBefore(
						data,
						d => d._type > data._type,
						() => _data.Enqueue( data )
					);
					break;
				case SMFSMModifyType.SingleRunner:
					_data.RemoveAll(
						d => d._type == SMFSMModifyType.SingleRunner,
						d => d.Dispose()
					);
					_data.Enqueue( data );
					break;
				case SMFSMModifyType.Runner:
					_data.Enqueue( data );
					break;
			}
//			SMLog.Debug( this );
			Run().Forget();
		}

		public async UniTask RegisterAndRun( SMFSMModifyData data ) {
			Register( data );
			await WaitRunning();
		}


		public async UniTask Run() {
			if ( _isRunning )	{ return; }

			_isRunning = true;
//			SMLog.Warning( "Run" );
			while ( IsHaveData() ) {
				if ( _isDispose )				{ break; }
// TODO : この辺りのフラグ、ごちゃごちゃしてるので、修正
				if ( !_owner._isFinalizing ) {
					if ( !_owner._isInitialized )	{ break; }
					if ( !_owner._isActive )		{ break; }
				}
				var d = _data.Dequeue();
				await d.Run();
			}
			_isRunning = false;
		}


		public UniTask WaitRunning()
			=> UTask.WaitWhile( _owner._asyncCancelerOnDispose, () => _isRunning || IsHaveData() );
	}
}