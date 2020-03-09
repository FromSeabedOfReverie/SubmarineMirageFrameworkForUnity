//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.MultiEvent {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using KoganeUnityLib;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class BaseMultiEvent<T> : IDisposable {
		protected enum AddType {
			First,
			Last,
		}
		protected readonly List< KeyValuePair<string, T> > _events
			= new List< KeyValuePair<string, T> >();
		protected readonly List<string> _removeKeys = new List<string>();
		protected readonly Subject<T> _onRemoveEvent = new Subject<T>();
		protected readonly ReactiveProperty<bool> _isInvoking = new ReactiveProperty<bool>();
		bool _isDispose;

		public BaseMultiEvent() {
			_isInvoking
				.SkipLatestValueOnSubscribe()
				.Where( is_ => !is_ )
				.Subscribe( is_ => CheckRemove() );
		}

		protected void Insert( string findKey, AddType type, string key, T function ) {
			var pair = new KeyValuePair<string, T>( key, function );
			var i = _events.FindIndex( p => p.Key == findKey );
			if ( i == -1 ) {
				Log.Error( $"イベント関数が未登録 : {findKey}" );
				throw new ArgumentOutOfRangeException( findKey, "イベント関数が未登録" );
			}
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
			_removeKeys.Add( key );
			if ( !_isInvoking.Value )	{ CheckRemove(); }
		}
		void CheckRemove() {
			if ( _removeKeys.IsEmpty() )	{ return; }
			_removeKeys.ForEach( key => {
				_events.RemoveAll( pair => {
					var isRemove = pair.Key == key;
					if ( isRemove )	{ _onRemoveEvent.OnNext( pair.Value ); }
					return isRemove;
				} );
			} );
			_removeKeys.Clear();
		}

		public override string ToString() {
			return (
				$"{this.GetAboutName()} (\n"
				+ string.Join( "\n", _events.Select( pair => pair.Key ) )
				+ "\n )"
			);
//			return this.ToDeepString();
		}

		public void Dispose() {
			if ( _isDispose )	{ return; }
			_isDispose = true;
			foreach ( var pair in _events ) {
				_onRemoveEvent.OnNext( pair.Value );
			}
			_onRemoveEvent.OnCompleted();
			_onRemoveEvent.Dispose();
			_isInvoking.Dispose();
			_events.Clear();
			Log.Debug( $"Dispose {this.GetAboutName()}" );
		}

		~BaseMultiEvent() {
			Dispose();
		}
	}
}