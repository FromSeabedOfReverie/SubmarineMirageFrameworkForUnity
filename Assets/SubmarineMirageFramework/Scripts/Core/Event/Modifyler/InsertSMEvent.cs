//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event.Modifyler {
	using System.Collections.Generic;
	using Event.Modifyler.Base;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class InsertSMEvent<T> : SMEventModifyData<T> {
		[SMShowLine] string _findKey	{ get; set; }
		[SMShowLine] SMEventAddType _type	{ get; set; }
		[SMShowLine] string _key	{ get; set; }


		public InsertSMEvent( string findKey, SMEventAddType type, string key, T function ) {
			_findKey = findKey;
			_type = type;
			_key = key;
			_function = function;
		}


		public override void Run() {
			var pair = new KeyValuePair<string, T>( _key, _function );

			switch ( _type ) {
				case SMEventAddType.First:
					_owner._events.AddBefore(
						pair,
						p => p.Key == _findKey,
						() => NoEventError( _findKey )
					);
					return;
				case SMEventAddType.Last:
					_owner._events.AddAfter(
						pair,
						p => p.Key == _findKey,
						() => NoEventError( _findKey )
					);
					return;
			}
		}
	}
}