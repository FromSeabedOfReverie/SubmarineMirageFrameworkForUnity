//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using System.Linq;
	using System.Collections.Generic;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Base;
	using Extension;
	using Utility;



	// TODO : コメント追加、整頓



	public abstract class BaseSMTaskModifyler<TOwner, TModifyler, TData, TTarget>
		: SMStandardBase, IBaseSMTaskModifyler
		where TOwner : IBaseSMTaskModifylerOwner<TModifyler>
		where TModifyler : IBaseSMTaskModifyler
		where TData : class, IBaseSMTaskModifyData<TOwner, TModifyler, TTarget>
		where TTarget : class, IBaseSMTaskModifyDataTarget
	{
		protected TOwner _owner	{ get; private set; }
		protected readonly LinkedList<TData> _data = new LinkedList<TData>();
		bool _isRunning	{ get; set; }
		protected abstract SMTaskCanceler _asyncCanceler	{ get; }


		public BaseSMTaskModifyler( TOwner owner ) {
			_owner = owner;

			_disposables.AddLast( () => {
				_data.ForEach( d => d.Dispose() );
				_data.Clear();
			} );
		}

		public void Register( TData data ) {
			data.Set( _owner );

			switch( data._type ) {
				case SMTaskModifyType.FirstLinker:
				case SMTaskModifyType.Linker:
				case SMTaskModifyType.FirstRunner:
					_data.AddBefore(
						data,
						d => d._type > data._type,
						() => _data.Enqueue( data )
					);
					break;
				case SMTaskModifyType.Runner:
					_data.Enqueue( data );
					break;
			}
			if ( !_isRunning )	{ Run().Forget(); }
		}

		public async UniTask RegisterAndRun( TData data ) {
			Register( data );
			await WaitRunning();
		}

		public void Unregister( TTarget remove ) => _data.RemoveAll(
			d => d._target == remove,
			d => d.Dispose()
		);


		async UniTask Run() {
			if ( _isRunning )	{ return; }

			_isRunning = true;
			while ( !_data.IsEmpty() ) {
				var d = _data.Dequeue();
				await d.Run();
			}
			_isRunning = false;
		}

		public UniTask WaitRunning()
			=> UTask.WaitWhile( _asyncCanceler, () => _isRunning );


		public override void SetToString() {
			base.SetToString();
			_toStringer.SetValue( nameof( _owner ), i => _owner.ToLineString() );
			_toStringer.SetValue( nameof( _data ), i => "\n" + string.Join( ",\n",
				_data.Select( d => d.ToLineString( i + 1 ) )
			) );
		}
	}
}