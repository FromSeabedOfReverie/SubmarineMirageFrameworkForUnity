//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Utility {
	using System;
	using System.IO;
	using System.Text;
	using UnityEngine;
	using GameDevWare.Serialization;
	using KoganeUnityLib;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ シリアル化の便利クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public static class SerializerSMUtility {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● シリアル化
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static byte[] Serialize<T>( T data ) {
			byte[] rawData;
			using ( var stream = new MemoryStream() ) {
				MsgPack.Serialize( data, stream, SerializationOptions.SuppressTypeInformation );
				rawData = stream.ToArray();
			}
			return rawData;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 非シリアル化
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T Deserialize<T>( byte[] rawData ) {
			T data;
			using ( var stream = new MemoryStream( rawData ) ) {
				data = MsgPack.Deserialize<T>( stream, SerializationOptions.SuppressTypeInformation );
			}
			return data;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 複製
		///		配列等の参照型も、深く複製する。
		///		※プロパティ、公開メンバ以外等は、複製されない。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T DeepCopy<T>( T data ) {
			try {
				// とりあえず、外部アセットのシリアライズで複製
				// プロパティ、protected以下、基底クラス継承先メンバ、等々複製されない
				T copyData;
				using ( var stream = new MemoryStream() ) {
					MsgPack.Serialize( data, stream, SerializationOptions.SuppressTypeInformation );
					stream.Position = 0;
					copyData = MsgPack.Deserialize<T>( stream, SerializationOptions.SuppressTypeInformation );
				}
				return copyData;


			// 失敗した場合、Unityのシリアライズで複製
			// 基底クラスや辞書等、複製されない物、多数
			} catch ( Exception e ) {
#if DEVELOP
				Log.Warning( e );
#endif
				var json = JsonUtility.ToJson( data );
				return JsonUtility.FromJson<T>( json );
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● JSON形式にシリアル化
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static string SerializeJSON( object data, bool isPrettyPrint = false ) {
			try {
				// 外部アセットで、JSONに変換
				// 参照型等の変換に、失敗する可能性がある
				var json = Json.SerializeToString( data, SerializationOptions.SuppressTypeInformation );
				if ( isPrettyPrint )	{ json = PrettyPrintFromJSON( json ); }
				return json;


			// 失敗した場合
			} catch ( Exception e ) {
#if DEVELOP
				Log.Warning( e );
#endif
				return string.Empty;

				// UnityでJSONに変換
				// 基底クラスや辞書等、変換出来ない物、多数
//				return JsonUtility.ToJson( data, isPrettyPrint );
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● JSONを整形
		///		参考URL : http://baba-s.hatenablog.com/entry/2018/05/06/120400
		///			該当ソースは、参考元のライセンスが適用されます。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static string PrettyPrintFromJSON( string json ) {
			if ( json.IsNullOrEmpty() )	{ return json; }

			var i = 0;
			var indent = 0;
			var quoteCount = 0;
			var position = -1;
			var sb = new StringBuilder();
			var lastindex = 0;

			while ( true ) {
				if ( i > 0 && json[i] == '"' && json[i - 1] != '\\' )	{ quoteCount++; }

				if ( quoteCount % 2 == 0 ) {
					if ( json[i] == '{' || json[i] == '[' ) {
						indent++;
						position = 1;
					} else if ( json[i] == '}' || json[i] == ']' ) {
						indent--;
						position = 0;
					} else if ( json.Length > i && json[i] == ',' && json[i + 1] == '"' ) {
						position = 1;
					}
					if ( position >= 0 ) {
						sb.AppendLine( json.Substring( lastindex, i + position - lastindex ) );
						sb.Append( new string( ' ', indent * 4 ) );
						lastindex = i + position;
						position = -1;
					}
				}

				i++;

				if ( json.Length <= i ) {
					sb.Append( json.Substring( lastindex ) );
					break;
				}
			}

			return sb.ToString();
		}
	}
}