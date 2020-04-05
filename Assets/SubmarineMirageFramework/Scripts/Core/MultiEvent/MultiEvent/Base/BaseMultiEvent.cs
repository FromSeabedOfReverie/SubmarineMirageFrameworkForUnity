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
	using Extension;


	// TODO : コメント追加、整頓


	public abstract class BaseMultiEvent<T> : IDisposableExtension {
		public readonly List< KeyValuePair<string, T> > _events = new List< KeyValuePair<string, T> >();
		protected EventModifyler<T> _eventModifyler;
		protected readonly ReactiveProperty<bool> _isInvoking = new ReactiveProperty<bool>();
		public MultiDisposable _disposables	{ get; private set; }
		protected virtual bool _isDispose => _disposables._isDispose;


		public BaseMultiEvent() {
			_eventModifyler = new EventModifyler<T>( this );
			_isInvoking
				.SkipLatestValueOnSubscribe()
				.Where( is_ => !is_ )
				.Subscribe( is_ => _eventModifyler.Run() );
			SetDisposables();
		}

		protected virtual void SetDisposables() {
			_disposables = new MultiDisposable();
			_disposables.AddLast(
				_isInvoking,
				_eventModifyler
			);
			_disposables.AddLast( () => {
				_events.ForEach( pair => OnRemove( pair.Value ) );
				_events.Clear();
			} );
		}

		public virtual void Dispose() => _disposables.Dispose();

		~BaseMultiEvent() => Dispose();


		protected void RegisterEventModifyler( EventModifyData<T> data ) {
			_eventModifyler.Register( data );
			if ( !_isInvoking.Value )	{ _eventModifyler.Run(); }
		}


		public void InsertFirst( string findKey, string key, T function ) {
			RegisterEventModifyler( new InsertEventModifyData<T>(
				findKey, EventAddType.First, key, function
			) );
		}

		public void InsertFirst( string findKey, T function ) {
			RegisterEventModifyler( new InsertEventModifyData<T>(
				findKey, EventAddType.First, string.Empty, function
			) );
		}

		public void InsertLast( string findKey, string key, T function ) {
			RegisterEventModifyler( new InsertEventModifyData<T>(
				findKey, EventAddType.Last, key, function
			) );
		}

		public void InsertLast( string findKey, T function ) {
			RegisterEventModifyler( new InsertEventModifyData<T>(
				findKey, EventAddType.Last, string.Empty, function
			) );
		}


		public void AddFirst( string key, T function ) {
			RegisterEventModifyler( new AddEventModifyData<T>(
				EventAddType.First, key, function
			) );
		}

		public void AddFirst( T function ) {
			RegisterEventModifyler( new AddEventModifyData<T>(
				EventAddType.First, string.Empty, function
			) );
		}

		public void AddLast( string key, T function ) {
			RegisterEventModifyler( new AddEventModifyData<T>(
				EventAddType.Last, key, function
			) );
		}

		public void AddLast( T function ) {
			RegisterEventModifyler( new AddEventModifyData<T>(
				EventAddType.Last, string.Empty, function
			) );
		}


		public void Reverse() {
			RegisterEventModifyler( new ReverseEventModifyData<T>() );
		}


		public void Remove( string key ) {
			RegisterEventModifyler( new RemoveEventModifyData<T>( key ) );
		}

		public abstract void OnRemove( T function );


		public void CheckDisposeError( EventModifyData<T> _data = null ) {
			if ( !_isDispose )	{ return; }
			if ( _data != null && _data._function != null ) {
				OnRemove( _data._function );
			}
			throw new ObjectDisposedException( "_disposables", "既に解放済" );
		}


		public override string ToString() {
			return (
				$"{this.GetAboutName()} (\n"
				+ $"{_eventModifyler}\n"
				+ string.Join( "\n", _events.Select( pair => $"    {pair.Key} : {pair.Value.GetAboutName()}" ) )
				+ "\n)"
			);
		}
	}
}