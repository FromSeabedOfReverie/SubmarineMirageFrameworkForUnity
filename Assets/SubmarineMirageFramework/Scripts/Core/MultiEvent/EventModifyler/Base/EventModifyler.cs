//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.MultiEvent {
	using System.Linq;
	using System.Collections.Generic;
	using UniRx;
	using KoganeUnityLib;
	using Base;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class EventModifyler<T> : SMRawBase {
		[Hide] BaseMultiEvent<T> _owner;
		readonly Queue< EventModifyData<T> > _data = new Queue< EventModifyData<T> >();
		bool _isRunning;


		public EventModifyler( BaseMultiEvent<T> owner ) {
			_owner = owner;

			_disposables.Add( () => {
				_data
					.Where( data => data._function != null )
					.ForEach( data => _owner.OnRemove( data._function ) );
				_data.Clear();
			} );
		}


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