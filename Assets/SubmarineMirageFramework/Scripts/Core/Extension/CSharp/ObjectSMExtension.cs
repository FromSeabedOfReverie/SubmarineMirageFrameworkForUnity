//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using System.Linq;
	using KoganeUnityLib;
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
		/// ● 文字列に変換（Hide属性以外のみ表示）
		/// </summary>
		public static string ToShowString( this object self, int indent = 0 ) {
			if ( self == null )	{ return "null"; }

			var nameI = StringSMUtility.IndentSpace( indent );
			var memberI = StringSMUtility.IndentSpace( indent + 1 );

			return string.Join( "\n",
				$"{nameI}{self.GetAboutName()}(",
				string.Join( ",\n", self.GetType().GetAllNotAttributeMembers<SMHideAttribute>().Select( i =>
					$"{memberI}{i.Name} : {i.GetValue( self )}"
				) ),
				$"{nameI})"
			);
		}
		/// <summary>
		/// ● 文字列に変換（ShowLine属性のみ一列表示）
		/// </summary>
		public static string ToLineString( this object self, int indent = 0 ) {
			if ( self == null )	{ return "null"; }

			var nameI = StringSMUtility.IndentSpace( indent );

			return string.Join( " ",
				$"{nameI}{self.GetAboutName()}(",
				string.Join( " ", self.GetType().GetAllAttributeMembers<SMShowLineAttribute>().Select( i =>
					$"{i.GetValue( self )}"
				) ),
				")"
			);
		}
	}
}