//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process {
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using KoganeUnityLib;
	using Extension;

	public abstract class BaseMultiEvent<T> : IDisposable {
		public enum AddType {
			First,
			Last,
		}
		protected readonly List< KeyValuePair<string, T> > _events
			= new List< KeyValuePair<string, T> >();

		void Insert( string findKey, AddType type, string key, T function ) {
			var pair = new KeyValuePair<string, T>( key, function );
			var i = _events.FindIndex( p => p.Key == findKey );
			if ( i == -1 ) {
				Debug.LogError( $"{findKey} : 処理が無い為、末尾に追加" );
				i = _events.Count;
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

		void Add( AddType type, string key, T function ) {
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

		public override string ToString() {
			return this.ToDeepString();
		}

		public void Dispose() {
			_events.Clear();
		}

		~BaseMultiEvent() {
			Dispose();
			Debug.Log( "Delete BaseMultiEvent" );
		}
	}
}