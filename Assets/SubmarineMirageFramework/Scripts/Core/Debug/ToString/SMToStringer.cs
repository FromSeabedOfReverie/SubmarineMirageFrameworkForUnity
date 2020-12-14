//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Debug.ToString {
	using System;
	using System.Linq;
	using System.Reflection;
	using System.Collections;
	using System.Collections.Generic;
	using KoganeUnityLib;
	using Base;
	using Data;
	using Extension;
	using Utility;



	// TODO : コメント追加、整頓



	public class SMToStringer {
		object _owner	{ get; set; }
		readonly Dictionary<string, SMToStringData> _toStrings = new Dictionary<string, SMToStringData>();
		readonly Dictionary<string, SMToLineStringData> _toLineStrings =
			new Dictionary<string, SMToLineStringData>();


		public SMToStringer( object owner ) {
			_owner = owner;
			_owner.GetType().GetAllNotAttributeMembers<HideAttribute>().ForEach( i =>
				_toStrings[i.Name] = new SMToStringData(
					i.Name,
					indent => DefaultValue( i.GetValue( _owner ), indent )
				)
			);
			_owner.GetType().GetAllAttributeMembers<ShowLineAttribute>().ForEach( i =>
				_toLineStrings[i.Name] = new SMToLineStringData(
					() => DefaultLineValue( i.GetValue( _owner ) )
				)
			);
		}

		public string DefaultValue( object value, int indent ) {
			switch ( value ) {
				case ISMBase smBase:
					return smBase.ToString( indent );
/*
				case KeyValuePair<> pair:
					var v = DefaultValue( indent, pair.Value );
					return $"{pair.Key} : {v}";
				case Array a:
					indent++;
					return string.Join( ",\n", a.Select( d =>
						$"{StringSMUtility.IndentSpace( indent )}{DefaultValue( indent, d )}"
					) );
				case IDictionary<> d:
					indent++;
					return string.Join( ",\n", d.Select( pair =>
						$"{StringSMUtility.IndentSpace( indent )}{DefaultValue( indent, d )}"
					) );
				case IEnumerable<> e:
					indent++;
					return string.Join( ",\n", e.Select( d =>
						$"{StringSMUtility.IndentSpace( indent )}{DefaultValue( indent, d )}"
					) );
*/
				default:
					return value.ToString();
			}
		}

		public string DefaultLineValue( object value ) {
			switch ( value ) {
				case ISMBase smBase:
					return smBase.ToLineString();
/*
				case KeyValuePair<> pair:
					return pair.Key;
				case Array a:
					return a.Length.ToString();
				case IDictionary<> d:
					return d.Count().ToString();
				case IEnumerable<> e:
					return e.Count().ToString();
*/
				default:
					return value.ToString();
			}
		}


		public void SetName( string name, string newName )
			=> _toStrings[name]._name = newName;

		public void SetValue( string name, Func<int, string> valueEvent )
			=> _toStrings[name]._valueEvent = valueEvent;

		public void SetLineValue( string name, Func<string> valueEvent )
			=> _toLineStrings[name]._valueEvent = valueEvent;


		public void Add( string name, Func<int, string> valueEvent )
			=> _toStrings[name] = new SMToStringData( name, valueEvent );

		public void AddLine( string name, Func<string> valueEvent )
			=> _toLineStrings[name] = new SMToLineStringData( valueEvent );


		public void Hide( string name ) => _toStrings.Remove( name );

		public void HideLine( string name ) => _toLineStrings.Remove( name );


		public string Run( int indent = 0 ) {
			var nameI = StringSMUtility.IndentSpace( indent );
			indent++;
			var memberI = StringSMUtility.IndentSpace( indent );
			return string.Join( "\n",
				$"{nameI}{_owner.GetAboutName()}(",
				string.Join( ",\n", _toStrings.Select( pair =>
					$"{memberI}{pair.Value._name} : {pair.Value._valueEvent( indent )}"
				) ),
				$"{nameI})"
			);
		}

		public string RunLine( int indent = 0 ) {
			var nameI = StringSMUtility.IndentSpace( indent );
			return string.Join( " ",
				$"{nameI}{_owner.GetAboutName()}(",
				string.Join( " ", _toLineStrings.Select( pair =>
					$"{pair.Value._valueEvent()}"
				) ),
				")"
			);
		}
	}
}