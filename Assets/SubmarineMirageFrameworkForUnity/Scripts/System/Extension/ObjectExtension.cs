//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Extension {
	using System.Linq;
	using System.Reflection;
	using KoganeUnityLib;
	using Utility;
	///====================================================================================================
	/// <summary>
	/// ■ 物の拡張クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public static class ObjectExtension {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>文字列変換の区切文字</summary>
		const string TO_STRING_SEPARATOR = "\n";
		/// <summary>文字列変換の表示書式</summary>
		const string TO_STRING_FORMAT = "{0} :\t{1}";
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 大体の名前を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static string GetAboutName( this object self ) {
			if ( self == null )	{ return "null"; }

			var type = self.GetType();
			if ( !type.IsGenericType ) { return type.Name; }

			var name = type.Name.Split( '`' )[0];
			return name;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 名前を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static string GetName( this object self ) {
			if ( self == null )	{ return "null"; }

			var type = self.GetType();
			if ( !type.IsGenericType ) { return type.Name; }

			var name = GetAboutName( type );
			var parameter = string.Join(
				", ",
				type.GetGenericArguments()
					.Select( c => c.GetName() )
			);
			return $"{name}<{parameter}>";
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文字列に変換（深奥）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static string ToDeepString( this object self ) {
			// とりあえず、外部アセットでJSON変換
			var result = SerializerUtility.SerializeJSON( self, true );
			// 失敗した場合、リフレクションで変換
			if ( result.IsNullOrEmpty() )	{ result = self.ToStringReflection(); }
			return $"{self.GetAboutName()}\n{result}";
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文字列に変換（公開フィールド、プロパティ）
		///		参考URL : http://baba-s.hatenablog.com/entry/2014/02/27/000000
		///			該当ソースは、参考元のライセンスが適用されます。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		static string ToStringReflection( this object self ) {
			return string.Join(
				TO_STRING_SEPARATOR,
				self.ToStringFields(), self.ToStringProperties(), ""
			);
		}
		/// <summary>
		/// ● 文字列に変換（公開フィールド）
		/// </summary>
		static string ToStringFields( this object self ) {
			var fields = self
				.GetType()
				.GetFields( BindingFlags.Instance | BindingFlags.Public )
				.Select( field => {
					try {
						var value = field.GetValue( self );
						return string.Format( TO_STRING_FORMAT, field.Name, value );
					} catch {
						return "";
					}
				} )
				.Where( s => !s.IsNullOrEmpty() );
			return string.Join( TO_STRING_SEPARATOR, fields );
		}
		/// <summary>
		/// ● 文字列に変換（公開プロパティ）
		/// </summary>
		static string ToStringProperties( this object self ) {
			var properties = self
				.GetType()
				.GetProperties( BindingFlags.Instance | BindingFlags.Public )
				.Where( property => property.CanRead )
				.Select( property => {
					try {
						var value = property.GetValue( self, null );
						return string.Format( TO_STRING_FORMAT, property.Name, value );
					} catch {
						return "";
					}
				} )
				.Where( s => !s.IsNullOrEmpty() );
			return string.Join( TO_STRING_SEPARATOR, properties );
		}
	}
}