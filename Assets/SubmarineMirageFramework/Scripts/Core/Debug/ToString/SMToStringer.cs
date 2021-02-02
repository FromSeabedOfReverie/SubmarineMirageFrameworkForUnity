//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Debug.ToString {
	using System;
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using KoganeUnityLib;
	using Base;
	using Data;
	using Extension;
	using Utility;



	// TODO : コメント追加、整頓



	public class SMToStringer : SMLightBase {
		[SMHide] object _owner	{ get; set; }
		readonly Dictionary<string, SMToStringData> _toStrings = new Dictionary<string, SMToStringData>();
		readonly Dictionary<string, SMToLineStringData> _toLineStrings =
			new Dictionary<string, SMToLineStringData>();


		public SMToStringer( object owner ) {
			_owner = owner;
			_owner.GetType().GetAllNotAttributeMembers<SMHideAttribute>().ForEach( i =>
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


		public string DefaultValue( object value, int indent, bool isUseInternalLineString ) {
			if ( value == null )	{ return "null"; }

			switch ( value ) {
				case IBaseSM baseSM:
					if ( isUseInternalLineString )	{ return baseSM.ToLineString( indent ); }
					else							{ return baseSM.ToString( indent ); }

				case Type type:
					return type.GetAboutName();

				case IEnumerable enumerable:
					var parentI = StringSMUtility.IndentSpace( indent );
					indent++;
					var memberI = StringSMUtility.IndentSpace( indent );
					var membersS = string.Join( ",\n", enumerable.SelectRaw( o =>
						$"{memberI}{DefaultValue( o, indent, isUseInternalLineString )}"
					) );
					return $"{{\n{membersS}\n{parentI}}}";

				default:
					var t = value.GetType();
					if ( t.IsGenericType ) {
						var gt = t.GetGenericTypeDefinition();
						if ( gt == typeof( KeyValuePair<,> ) ) {
							var k = t.GetProperty( "Key" ).GetValue( value );
							var v = t.GetProperty( "Value" ).GetValue( value );
							var sk = DefaultValue( k, indent, isUseInternalLineString );
							var sv = DefaultValue( v, indent, isUseInternalLineString );
							return $"{sk} : {sv}";
						}
					}

					return value.ToString();
			}
		}

		public string DefaultLineValue( object value ) {
			if ( value == null )	{ return "null"; }

			switch ( value ) {
				case IBaseSM baseSM:
					return baseSM.ToLineString();

				case Type type:
					return type.GetAboutName();

				case IEnumerable enumerable:
					return enumerable.CountRaw().ToString();

				default:
					var t = value.GetType();
					if ( t.IsGenericType ) {
						var gt = t.GetGenericTypeDefinition();
						if ( gt == typeof( KeyValuePair<,> ) ) {
							var k = t.GetProperty( "Key" ).GetValue( value );
							var v = t.GetProperty( "Value" ).GetValue( value );
							var sk = DefaultLineValue( k );
							var sv = DefaultLineValue( v );
							return $"{sk}:{sv}";
						}
					}

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