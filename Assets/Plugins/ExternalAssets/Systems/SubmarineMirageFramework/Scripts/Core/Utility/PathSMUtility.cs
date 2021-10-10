//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestPathUtility
namespace SubmarineMirage {
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
		/// ● 判定
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● URLか？
		/// </summary>
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

		///------------------------------------------------------------------------------------------------
		/// ● 取得
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 名前を取得
		/// </summary>
		public static string GetName( string path )
			=> Path.GetFileNameWithoutExtension( path );

		/// <summary>
		/// ● 名前と拡張子を取得
		/// </summary>
		public static string GetNameAndExtension( string path )
			=> Path.GetFileName( path );

		/// <summary>
		/// ● 拡張子を取得
		/// </summary>
		public static string GetExtension( string path )
			=> Path.GetExtension( path );

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 階層を作成
		/// </summary>
		public static void Create( string path ) {
			// 階層が存在する場合、未処理
			if ( Directory.Exists( path ) )	{ return; }

			// 階層作成
			Directory.CreateDirectory( path );
#if TestPathUtility
			SMLog.Debug( $"階層作成 : {path}" );
#endif
		}

		/// <summary>
		/// ● 階層を削除
		/// </summary>
		public static void Delete( string path ) {
			// 階層が存在しない場合、未処理
			if ( !Directory.Exists( path ) ) { return; }

			// 階層削除
			Directory.Delete( path, true );
#if TestPathUtility
			SMLog.Debug( $"階層削除 : {path}" );
#endif
		}
	}
}