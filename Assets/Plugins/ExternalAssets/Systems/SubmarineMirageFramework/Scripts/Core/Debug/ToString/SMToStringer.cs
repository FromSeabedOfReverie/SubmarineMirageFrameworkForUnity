//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using KoganeUnityLib;



	public class SMToStringer : SMLightBase {
		object _owner	{ get; set; }
		[SMShow] readonly Dictionary<string, SMToStringData> _toStrings
			= new Dictionary<string, SMToStringData>();
		[SMShow] readonly Dictionary<string, SMToLineStringData> _toLineStrings
			= new Dictionary<string, SMToLineStringData>();


		public SMToStringer( object owner ) {
			_owner = owner;
			_owner.GetType().GetAllAttributeMembers<SMShowAttribute>().ForEach( i =>
				_toStrings[i.Name] = new SMToStringData(
					i.Name,
					indent => DefaultValue( i.GetValue( _owner ), indent, false )
				)
			);
			_owner.GetType().GetAllAttributeMembers<SMShowLineAttribute>().ForEach( i =>
				_toLineStrings[i.Name] = new SMToLineStringData(
					() => DefaultLineValue( i.GetValue( _owner ) )
				)
			);
		}

		public override void Dispose() {
			_owner = null;
			_toStrings.ForEach( pair => pair.Value.Dispose() );
			_toStrings.Clear();
			_toLineStrings.ForEach( pair => pair.Value.Dispose() );
			_toLineStrings.Clear();
		}


		public string DefaultValue( object value, int indent, bool isUseInternalLineString )
			=> value.ToShowString( indent, isUseInternalLineString, false, false );

		public string DefaultLineValue( object value )
			=> value.ToLineString();


		public void SetName( string name, string newName )
			=> _toStrings[name]._name = newName;

		public void SetValue( string name, Func<int, string> valueEvent )
			=> _toStrings[name]._valueEvent = valueEvent;

		public void SetLineValue( string name, Func<string> valueEvent )
			=> _toLineStrings[name]._valueEvent = valueEvent;


		public void Add( string name, Func<int, string> valueEvent ) {
			_toStrings[name] = new SMToStringData( name, valueEvent );
		}

		public void AddLine( string name, Func<string> valueEvent ) {
			_toLineStrings[name] = new SMToLineStringData( valueEvent );
		}


		public void Hide( string name ) => _toStrings.Remove( name );

		public void HideLine( string name ) => _toLineStrings.Remove( name );


		public string Run( int indent = 0, bool isUseHeadIndent = true ) {
			var prefix = StringSMUtility.IndentSpace( indent );
			var hPrefix = isUseHeadIndent ? prefix : "";
			indent++;
			var mPrefix = StringSMUtility.IndentSpace( indent );

			return string.Join( "\n",
				$"{hPrefix}{_owner.GetName()}(",
				string.Join( ",\n", _toStrings.Select( pair =>
					$"{mPrefix}{pair.Value._name} : {pair.Value._valueEvent( indent )}"
				) ),
				$"{prefix})"
			);
		}

		public string RunLine( int indent = 0 ) {
			var prefix = StringSMUtility.IndentSpace( indent );
			return string.Join( " ",
				$"{prefix}{_owner.GetName()}(",
				string.Join( " ", _toLineStrings.Select( pair =>
					$"{pair.Value._valueEvent()}"
				) ),
				")"
			);
		}
	}
}