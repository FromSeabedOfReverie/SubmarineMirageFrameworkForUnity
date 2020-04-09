//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Editor.Build {
	using System.IO;
	using System.Linq;
	using UnityEngine;
	using UniRx.Async;
	using KoganeUnityLib;
	using Process;
	using Extension;
	using Data.File;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ ライセンス記述のクラス
	///----------------------------------------------------------------------------------------------------
	///		C#、シェーダー等の、全プログラム書類の行頭に、ライセンスを一括記述する為に使用。
	///		TODO : BOM付き書類の場合、BOMが行中に含まれ、Unityで読込エラーになる
	/// </summary>
	///====================================================================================================
	public class WriteLicense : BaseProcess {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>挿入する、ライセンス文章</summary>
		const string LICENSE_TEXT =
@"//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
";
		/// <summary>プログラム書類の階層</summary>
		const string PATH = "SubmarineMirageFrameworkForUnity/Scripts/System";
		/// <summary>検索から除外する、フォルダ名の一覧</summary>
		readonly string[] EXCLUDE_FOLDERS = new string[] {
			"System\\Audio\\",
			"System\\Build\\",
			"System\\Data\\",
		};
		/// <summary>検索する、拡張子</summary>
		const string EXTENSION = "*.cs";
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public WriteLicense() : base() {
			// ● 読込
			_loadEvent += async () => {
				var topPath = Path.Combine( Application.dataPath, PATH );

				// 書類階層の一覧を取得
				var fileNames = Directory.GetFiles( topPath, EXTENSION, SearchOption.AllDirectories )
					// 除外書類以外を取得
					.Where( path =>
						EXCLUDE_FOLDERS.All( f => path.IndexOf( f ) == -1 )
					)
					.ToList();
//				Log.Debug( fileNames.ToDeepString() );

				// 書類の読込、ライセンス記述、保存
				await UniTask.WhenAll(
					fileNames.Select( async path => {
						var fileText = await FileManager.s_instance._fileLoader.LoadExternal<string>( path );
						// ※ BOM付きだった場合、BOMが行中に含まれてしまう、要修正
						fileText = LICENSE_TEXT + fileText;
//						Log.Debug( fileText );
						await FileManager.s_instance._fileLoader.SaveExternal( path, fileText );
					} )
				);
			};
		}
	}
}