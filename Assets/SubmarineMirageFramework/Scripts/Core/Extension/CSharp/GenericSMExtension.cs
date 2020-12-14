//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using Utility;
	///====================================================================================================
	/// <summary>
	/// ■ ジェネリックの拡張クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public static class GenericSMExtension {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 複製
		///		配列等の参照型も、深く複製する。
		///		※プロパティ、公開メンバ以外等は、複製されない。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static T DeepCopy<T>( this T self ) {
// MemberwiseCloneだと浅く、配列までコピーされず、各種クラス内にprotectedでしか記述できない
//			return (T)self.MemberwiseClone();

			return SerializerSMUtility.DeepCopy( self );
		}
	}
}