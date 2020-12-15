//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Debug.ToString.Data {
	using System;



	// TODO : コメント追加、整頓



	public class SMToStringData : BaseSMToStringData {
		public string _name	{ get; set; }
		public Func<int, string> _valueEvent	{ get; set; }
		public SMToStringData( string name, Func<int, string> valueEvent ) {
			_name = name;
			_valueEvent = valueEvent;
		}
	}
}