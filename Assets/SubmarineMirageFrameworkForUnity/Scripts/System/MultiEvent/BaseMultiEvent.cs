//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.MultiEvent {
	using System;
	using System.Linq;
	using System.Diagnostics;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using KoganeUnityLib;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class BaseMultiEvent<T> : IDisposableExtension {
		protected enum AddType {
			First,
			Last,
		}
		protected readonly List< KeyValuePair<string, T> > _events
			= new List< KeyValuePair<string, T> >();
//		readonly List< KeyValuePair<string, T> > _nextEvents = new List< KeyValuePair<string, T> >();
		protected readonly List<string> _removeKeys = new List<string>();
		protected readonly ReactiveProperty<bool> _isInvoking = new ReactiveProperty<bool>();
		public MultiDisposable _disposables	{ get; private set; }
		protected virtual bool _isDispose => _disposables._isDispose;
		protected string _ownerName;


		public BaseMultiEvent() {
			var frame = new StackFrame( 4 );
			_ownerName = frame.GetMethod().ReflectedType.Name;

			_isInvoking
				.SkipLatestValueOnSubscribe()
				.Where( is_ => !is_ )
				.Subscribe( is_ => CheckRemove() );

			SetDisposables();
		}

		protected virtual void SetDisposables() {
			_disposables = new MultiDisposable();
			_disposables.AddLast( _isInvoking );
			_disposables.AddLast( () => {
				foreach ( var pair in _events ) {
					OnRemove( pair.Value );
				}
				_events.Clear();
				_removeKeys.Clear();
				Log.Debug( $"Dispose {this.GetAboutName()} {_ownerName}" );
			} );
		}

		public virtual void Dispose() => _disposables.Dispose();

		~BaseMultiEvent() => Dispose();


		protected void Insert( string findKey, AddType type, string key, T function ) {
			CheckDisposeError( function );
			var pair = new KeyValuePair<string, T>( key, function );
			var i = _events.FindIndex( p => p.Key == findKey );
			if ( i == -1 )	{ NoEventError( findKey ); }
			switch ( type ) {
				case AddType.First:	i -= 0;	break;
				case AddType.Last:	i += 1;	break;
			}
			i = Mathf.Clamp( i, 0, _events.Count );
			_events.Insert( i, pair );
		}

		public void InsertFirst( string findKey, string key, T function ) {
			Insert( findKey, AddType.First, key, function );
		}

		public void InsertFirst( string findKey, T function ) {
			Insert( findKey, AddType.First, string.Empty, function );
		}

		public void InsertLast( string findKey, string key, T function ) {
			Insert( findKey, AddType.Last, key, function );
		}

		public void InsertLast( string findKey, T function ) {
			Insert( findKey, AddType.Last, string.Empty, function );
		}


		protected void Add( AddType type, string key, T function ) {
			CheckDisposeError( function );
			var pair = new KeyValuePair<string, T>( key, function );
			switch ( type ) {
				case AddType.First:	_events.InsertFirst( pair );	break;
				case AddType.Last:	_events.Add( pair );			break;
			}
		}

		public void AddFirst( string key, T function ) {
			Add( AddType.First, key, function );
		}

		public void AddFirst( T function ) {
			Add( AddType.First, string.Empty, function );
		}

		public void AddLast( string key, T function ) {
			Add( AddType.Last, key, function );
		}

		public void AddLast( T function ) {
			Add( AddType.Last, string.Empty, function );
		}


		public void Remove( string key ) {
			CheckDisposeError();
			_removeKeys.Add( key );
			if ( !_isInvoking.Value )	{ CheckRemove(); }
		}

		void CheckRemove() {
			if ( _removeKeys.IsEmpty() )	{ return; }
			_removeKeys.ForEach( key => {
				var i = _events.RemoveAll( pair => {
					var isRemove = pair.Key == key;
					if ( isRemove )	{ OnRemove( pair.Value ); }
					return isRemove;
				} );
				if ( i == 0 )	{ NoEventError( key ); }
				else { Log.Debug( $"remove {key}" ); }
			} );
			_removeKeys.Clear();
		}

		protected abstract void OnRemove( T function );


		protected void CheckDisposeError( T addFunction = default( T ) ) {
			if ( !_isDispose )	{ return; }
			if ( addFunction != null ) {
				OnRemove( addFunction );
			}
			throw new ObjectDisposedException( "_disposables", "既に解放済" );
		}

		void NoEventError( string key ) {
			throw new KeyNotFoundException( $"イベント関数が未登録 : {key}" );
		}


		public override string ToString() {
			return (
				$"{this.GetAboutName()} (\n"
				+ string.Join( "\n", _events.Select( pair => $"    {pair.Key} : {pair.Value.GetAboutName()}" ) )
				+ "\n)"
			);
		}
	}
}