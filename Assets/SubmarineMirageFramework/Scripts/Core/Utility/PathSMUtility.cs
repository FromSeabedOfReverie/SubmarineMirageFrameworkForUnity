//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Utility {
	using System;
	using System.IO;
	///====================================================================================================
	/// <summary>
	/// ■ 階層の便利クラス
	///		Pathはstaticクラスなので、拡張関数を定義出来ない為、代わりに便利クラスを作成した。
	/// </summary>
	///====================================================================================================
	public static class PathSMUtility {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● URLか？
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static bool IsURL( string path ) {
			if ( File.Exists( path ) )			{ return false; }
			if ( path.StartsWith( "file:" ) )	{ return false; }

			try {
				var uri = new Uri( path );
				return true;

			} catch {
				return false;
			}
		}
	}
}