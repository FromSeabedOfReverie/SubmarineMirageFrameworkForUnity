//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Base.Modifyler {
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;
	using SubmarineMirage.Base;
	using Task;
	using Extension;
	using Utility;



	// TODO : コメント追加、整頓



	public abstract class BaseSMFSMModifyler<TOwner, TModifyler, TData> : SMStandardBase
		where TOwner : BaseSMFSMModifylerOwner<TOwner, TModifyler, TData>
		where TModifyler : BaseSMFSMModifyler<TOwner, TModifyler, TData>
		where TData : BaseSMFSMModifyData<TOwner, TModifyler, TData>
	{
		protected TOwner _owner	{ get; private set; }
		protected readonly LinkedList<TData> _data = new LinkedList<TData>();
		bool _isRunning	{ get; set; }
		protected abstract SMTaskCanceler _asyncCanceler	{ get; }


		public BaseSMFSMModifyler( TOwner owner ) {
			_owner = owner;

			_disposables.AddLast(
				SMTaskRunner.s_instance._isUpdating
					.Where( b => !b )
					.Subscribe( _ => Run().Forget() )
			);
			_disposables.AddLast( () => {
				_data.ForEach( d => d.Dispose() );
				_data.Clear();
				_isRunning = false;
			} );
		}

		public void Register( TData data ) {
			data.Set( _owner );
			if ( _isDispose ) {
				data.Dispose();
				return;
			}

			switch( data._type ) {
				case SMFSMModifyType.FirstLinker:
				case SMFSMModifyType.Linker:
				case SMFSMModifyType.FirstRunner:
					_data.AddBefore(
						data,
						d => d._type > data._type,
						() => _data.Enqueue( data )
					);
					break;
				case SMFSMModifyType.Runner:
					_data.Enqueue( data );
					break;
			}
			if ( !_isRunning )	{ Run().Forget(); }
		}

		public async UniTask RegisterAndRun( TData data ) {
			Register( data );
			await WaitRunning();
		}


		public async UniTask Run() {
			if ( _isRunning )	{ return; }

			_isRunning = true;
			while ( !_data.IsEmpty() ) {
				if ( _isDispose )	{ break; }
				if ( SMTaskRunner.s_instance._isUpdating.Value )	{ break; }
				var d = _data.Dequeue();
				await d.Run();
			}
			_isRunning = false;
		}

		public UniTask WaitRunning()
			=> UTask.WaitWhile( _asyncCanceler, () => _isRunning || !_data.IsEmpty() );


		public override void SetToString() {
			base.SetToString();
			_toStringer.SetValue( nameof( _owner ), i => _owner.ToLineString() );
			_toStringer.SetValue( nameof( _data ), i => "\n" + string.Join( ",\n",
				_data.Select( d => d.ToLineString( i + 1 ) )
			) );
		}
	}
}