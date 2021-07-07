//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using System;
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using KoganeUnityLib;
	using Base;
	using Utility;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 物の拡張クラス
	/// </summary>
	///====================================================================================================
	public static class ObjectSMExtension {
		///------------------------------------------------------------------------------------------------
		/// ● 名前を取得
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 大体の名前を取得
		/// </summary>
		public static string GetAboutName( this object self ) {
			if ( self == null )	{ return "null"; }
			return self.GetType().GetAboutName();
		}
		/// <summary>
		/// ● 名前を取得
		/// </summary>
		public static string GetName( this object self ) {
			if ( self == null )	{ return "null"; }
			return self.GetType().GetName();
		}
		///------------------------------------------------------------------------------------------------
		/// ● 文字列に変換
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文字列に変換（詳細表示）
		/// </summary>
		public static string ToDeepString( this object self ) {
			if ( self == null )	{ return "null"; }

			// とりあえず、外部アセットでJSON変換
			var result = SerializerSMUtility.SerializeJSON( self, true );
			// 失敗した場合、リフレクションで変換
			if ( result.IsNullOrEmpty() )	{ result = self.ToStringReflection(); }
			return result;
		}
		/// <summary>
		/// ● 文字列に変換（リフレクション使用）
		/// </summary>
		public static string ToStringReflection( this object self ) {
			if ( self == null )	{ return "null"; }

			return string.Join( "\n",
				$"{self.GetAboutName()}(",
				string.Join( ",\n", self.GetType().GetAllMembers().Select( i =>
					$"    {i.Name} : {i.GetValue( self )}"
				) ),
				")"
			);
		}
		/// <summary>
		/// ● 文字列に変換（Show属性のみ表示）
		/// </summary>
		public static string ToShowString( this object self, int indent = 0,
												bool isUseInternalLineString = false,
												bool isUseLineString = false, bool isUseHeadIndent = true
		) {
			var prefix = StringSMUtility.IndentSpace( indent );
			var hPrefix = isUseHeadIndent ? prefix : "";

			if ( self == null ) { return $"{hPrefix}null"; }

			var isNextUseLineString = isUseInternalLineString || isUseLineString;


			switch ( self ) {
				case ISMStandardBase baseSM:
					if ( isUseLineString )	{ return baseSM.ToLineString( isUseHeadIndent ? indent : 0 ); }
					else					{ return baseSM.ToString( indent, isUseHeadIndent ); }

				case Type type:
					return $"{hPrefix}{type.GetAboutName()}";

				case Enum e:
				case string s:
					return $"{hPrefix}{self}";

				case IEnumerable enumerable:
					indent++;
					var ss = string.Join( ",\n", enumerable.SelectRaw( o =>
						ToShowString( o, indent, isUseInternalLineString, isNextUseLineString, true )
					) );
					if ( !ss.IsNullOrEmpty() ) {
						return string.Join( "\n",
							$"{hPrefix}{{",
							ss,
							$"{prefix}}}"
						);
					}
					return $"{hPrefix}{{}}";

				default:
					var t = self.GetType();

					if ( t.IsPrimitive ) {
						return $"{hPrefix}{self}";
					}

					if ( t.IsGenericType ) {
						var gt = t.GetGenericTypeDefinition();
						if ( gt == typeof( KeyValuePair<,> ) ) {
							var k = t.GetProperty( "Key" ).GetValue( self );
							var v = t.GetProperty( "Value" ).GetValue( self );
							var sk = ToShowString(
								k, indent, isUseInternalLineString, isNextUseLineString, true );
							var sv = ToShowString(
								v, indent, isUseInternalLineString, isNextUseLineString, false );
							return $"{sk} : {sv}";
						}
					}


					indent++;
					var mPrefix = StringSMUtility.IndentSpace( indent );
					var members = self.GetType().GetAllAttributeMembers<SMShowAttribute>();

					var ms = string.Join( ",\n", members.Select( i =>
						$"{mPrefix}{i.Name} : " +
							ToShowString(
								i.GetValue( self ), indent, isUseInternalLineString, isNextUseLineString, false )
					) );
					switch ( self ) {
						case ISMLightBase lb:	ms += lb.AddToString( indent );	break;
						case ISMRawBase rb:		ms += rb.AddToString( indent );	break;
					}
					return string.Join( "\n",
						$"{hPrefix}{self.GetAboutName()}(",
						ms,
						$"{prefix})"
					);
			}
		}
		/// <summary>
		/// ● 文字列に変換（ShowLine属性のみ一列表示）
		/// </summary>
		public static string ToLineString( this object self, int indent = 0 ) {
			var prefix = StringSMUtility.IndentSpace( indent );

			if ( self == null )	{ return $"{prefix}null"; }


			switch ( self ) {
				case ISMStandardBase baseSM:
					return baseSM.ToLineString( indent );

				case Type type:
					return $"{prefix}{type.GetAboutName()}";

				case Enum e:
				case string s:
					return $"{prefix}{self}";

				case IEnumerable enumerable:
					return prefix + string.Join( ",", enumerable.SelectRaw( o =>
						ToLineString( o )
					) );

				default:
					var t = self.GetType();

					if ( t.IsPrimitive ) {
						return $"{prefix}{self}";
					}

					if ( t.IsGenericType ) {
						var gt = t.GetGenericTypeDefinition();
						if ( gt == typeof( KeyValuePair<,> ) ) {
							var k = t.GetProperty( "Key" ).GetValue( self );
							var v = t.GetProperty( "Value" ).GetValue( self );
							return $"{prefix}{ToLineString( k )}:{ToLineString( v )}";
						}
					}


					var members = self.GetType().GetAllAttributeMembers<SMShowLineAttribute>();

					return string.Join( " ",
						$"{prefix}{self.GetAboutName()}(",
						string.Join( " ", members.Select( i =>
							ToLineString( i.GetValue( self ) )
						) ),
						")"
					);
			}
		}
	}
}