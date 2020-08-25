//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.MultiEvent {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UniRx;
	using KoganeUnityLib;
	using Extension;


	// TODO : コメント追加、整頓


	public class EventModifyler<T> : IRawDisposableExtension {
		BaseMultiEvent<T> _owner;
		readonly Queue< EventModifyData<T> > _data = new Queue< EventModifyData<T> >();
		bool _isRunning;
		public bool _isDispose => _disposables.IsDisposed;
		readonly CompositeDisposable _disposables = new CompositeDisposable();


		public EventModifyler( BaseMultiEvent<T> owner ) {
			_owner = owner;

			_disposables.Add( Disposable.Create( () => {
				_data
					.Where( data => data._function != null )
					.ForEach( data => _owner.OnRemove( data._function ) );
				_data.Clear();
			} ) );
		}

		~EventModifyler() => Dispose();

		public void Dispose() => _disposables.Dispose();


		public void Register( EventModifyData<T> data ) {
			_owner.CheckDisposeError( data );
			data._owner = _owner;
			_data.Enqueue( data );

			if ( !_isRunning )	{ Run(); }
		}


		void Run() {
			if ( _isRunning )	{ return; }

			_isRunning = true;
			while ( !_data.IsEmpty() ) {
				var d = _data.Dequeue();
				d.Run();
			}
			_isRunning = false;
		}


		public override string ToString() => string.Join( "\n",
			$"{this.GetAboutName()}(",
			$"    {nameof( _isRunning )} : {_isRunning}",
			$"    {nameof( _data )} :",
			string.Join( "\n", _data.Select( d => $"        {d}" ) ),
			")"
		);
	}
}