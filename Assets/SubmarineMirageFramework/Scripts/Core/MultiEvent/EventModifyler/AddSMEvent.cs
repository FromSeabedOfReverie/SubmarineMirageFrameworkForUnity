//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.MultiEvent {
	using System.Collections.Generic;
	using KoganeUnityLib;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class AddSMEvent<T> : SMEventModifyData<T> {
		[SMShowLine] SMEventAddType _type	{ get; set; }
		[SMShowLine] string _key	{ get; set; }


		public AddSMEvent( SMEventAddType type, string key, T function ) {
			_type = type;
			_key = key;
			_function = function;
		}


		public override void Run() {
			var pair = new KeyValuePair<string, T>( _key, _function );
			switch ( _type ) {
				case SMEventAddType.First:	_owner._events.InsertFirst( pair );	break;
				case SMEventAddType.Last:	_owner._events.Add( pair );			break;
			}
		}
	}
}