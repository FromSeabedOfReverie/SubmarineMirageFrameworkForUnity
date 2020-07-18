//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.MultiEvent {
	using System.Collections.Generic;
	using UnityEngine;
	using Extension;


	// TODO : コメント追加、整頓


	public class InsertEventModifyData<T> : EventModifyData<T> {
		string _findKey;
		EventAddType _type;
		string _key;


		public InsertEventModifyData( string findKey, EventAddType type, string key, T function ) {
			_findKey = findKey;
			_type = type;
			_key = key;
			_function = function;
		}


		public override void Run() {
			var pair = new KeyValuePair<string, T>( _key, _function );
			var i = _owner._events.FindIndex( p => p.Key == _findKey );
			if ( i == -1 )	{ NoEventError( _findKey ); }
			switch ( _type ) {
				case EventAddType.First:	i -= 0;	break;
				case EventAddType.Last:		i += 1;	break;
			}
			i = Mathf.Clamp( i, 0, _owner._events.Count );
			_owner._events.Insert( i, pair );
		}


		public override string ToString() => $"{this.GetAboutName()}( {_findKey}, {_type}, {_key} )";
	}
}