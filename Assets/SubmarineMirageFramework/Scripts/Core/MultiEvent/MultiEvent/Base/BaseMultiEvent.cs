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
	using Base;
	using Extension;


	// TODO : コメント追加、整頓


	public abstract class BaseMultiEvent<T> : SMRawBase {
		public readonly List< KeyValuePair<string, T> > _events = new List< KeyValuePair<string, T> >();
		protected EventModifyler<T> _modifyler	{ get; private set; }


		public BaseMultiEvent() {
			_modifyler = new EventModifyler<T>( this );
			_disposables.Add( () => {
				_modifyler.Dispose();
				_events.ForEach( pair => OnRemove( pair.Value ) );
				_events.Clear();
			} );
		}


		protected void Register( EventModifyData<T> data ) => _modifyler.Register( data );


		public void InsertFirst( string findKey, string key, T function )
			=> Register( new InsertEventModifyData<T>( findKey, EventAddType.First, key, function ) );

		public void InsertFirst( string findKey, T function )
			=> Register( new InsertEventModifyData<T>( findKey, EventAddType.First, string.Empty, function ) );

		public void InsertLast( string findKey, string key, T function )
			=> Register( new InsertEventModifyData<T>( findKey, EventAddType.Last, key, function ) );

		public void InsertLast( string findKey, T function )
			=> Register( new InsertEventModifyData<T>( findKey, EventAddType.Last, string.Empty, function ) );


		public void AddFirst( string key, T function )
			=> Register( new AddEventModifyData<T>( EventAddType.First, key, function ) );

		public void AddFirst( T function )
			=> Register( new AddEventModifyData<T>( EventAddType.First, string.Empty, function ) );

		public void AddLast( string key, T function )
			=> Register( new AddEventModifyData<T>( EventAddType.Last, key, function ) );

		public void AddLast( T function )
			=> Register( new AddEventModifyData<T>( EventAddType.Last, string.Empty, function ) );


		public void Reverse() => Register( new ReverseEventModifyData<T>() );


		public void Remove( string key ) => Register( new RemoveEventModifyData<T>( key ) );

		public abstract void OnRemove( T function );


		public void CheckDisposeError( EventModifyData<T> _data = null ) {
			if ( !_isDispose )	{ return; }
			if ( _data != null && _data._function != null ) {
				OnRemove( _data._function );
			}
			throw new ObjectDisposedException( $"{nameof( _disposables )}", "既に解放済" );
		}


		public override string ToString() => string.Join( "\n",
			$"{this.GetAboutName()}(",
			$"    {nameof( _id )} : {_id}",
			$"    {nameof( _isDispose )} : {_isDispose}",
			$"    {nameof( _events )} :",
			string.Join( "\n", _events.Select( pair => $"        {pair.Key} : {pair.Value.GetAboutName()}" ) ),
			$"    {nameof( _modifyler )} : {_modifyler}",
			")"
		);
	}
}