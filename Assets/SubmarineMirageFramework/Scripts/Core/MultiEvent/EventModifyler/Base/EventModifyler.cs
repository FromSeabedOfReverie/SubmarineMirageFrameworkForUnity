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


	public class EventModifyler<T> : IDisposable {
		readonly List< EventModifyData<T> > _eventData = new List< EventModifyData<T> >();
		BaseMultiEvent<T> _owner;
		readonly CompositeDisposable _disposables = new CompositeDisposable();

		public EventModifyler( BaseMultiEvent<T> owner ) {
			_owner = owner;
			_disposables.Add( Disposable.Create( () => {
				_eventData
					.Where( data => data._function != null )
					.ForEach( data => _owner.OnRemove( data._function ) );
				_eventData.Clear();
			} ) );
		}

		public void Dispose() => _disposables.Dispose();

		public void Register( EventModifyData<T> data ) {
			_owner.CheckDisposeError( data );
			data._owner = _owner;
			_eventData.Add( data );
		}

		public void Run() {
			if ( _eventData.IsEmpty() )	{ return; }
			_eventData.ForEach( data => data.Run() );
			_eventData.Clear();
		}

		public override string ToString() =>
			$"{this.GetAboutName()} (\n{string.Join( "\n", _eventData.Select( data => $"    {data}" ) )}\n)";
	}
}