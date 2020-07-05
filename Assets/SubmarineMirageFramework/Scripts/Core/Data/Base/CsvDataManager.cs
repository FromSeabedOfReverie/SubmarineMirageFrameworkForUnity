//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using System.IO;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using File;
	using Debug;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ CSV情報の管理クラス
	///----------------------------------------------------------------------------------------------------
	///		CSVを利用する各種情報クラスは、この管理クラスで纏める。
	/// </summary>
	///====================================================================================================
	public abstract class CSVDataManager<TKey, TValue> : BaseDataManager<TKey, TValue>
		where TValue : CSVData<TKey>, new()
	{
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>読込階層</summary>
		public string _path { get; private set; }
		/// <summary>読込書類名</summary>
		public string _fileName { get; private set; }
		/// <summary>書類の型</summary>
		protected FileLoader.Type _fileType { get; private set; }
		/// <summary>読込開始の行数</summary>
		protected int _loadStartIndex { get; private set; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public CSVDataManager(
			string path, string fileName, FileLoader.Type fileType, int loadStartIndex )
		{
			SetPath( path, fileName, fileType, loadStartIndex );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定（階層）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void SetPath( string path, string fileName, FileLoader.Type fileType, int loadStartIndex ) {
			_path = path;
			_fileName = fileName;
			_fileType = fileType;
			_loadStartIndex = loadStartIndex;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Load() {
			var loader = FileManager.s_instance._csvLoader;
			var path = Path.Combine( _path, _fileName );
			var data = await loader.Load( _fileType, path, false ); // 指定書類を読込

			// データが存在しない場合、エラー表示
			if ( data == null ) {
				Log.Error( $"{this.GetAboutName()} : データ読込失敗", Log.Tag.Data );

			// データが存在する場合
			} else {
				// 全情報を設定
				for ( var i = _loadStartIndex; i < data.Count; i++ ) {
					Set( _fileName, i - _loadStartIndex, data[i] );
				}
			}

			await base.Load();

//			Log.Debug( _allData.ToDeepString() );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected virtual void Set( string fileName, int index, List<string> texts ) {
			var data = new TValue();
			data.Set( fileName, index, texts );
			Register( data._registerKey, data );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 保存
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Save() {
			// TODO : 情報からCSVに保存を実装
			await base.Save();
		}
	}
}