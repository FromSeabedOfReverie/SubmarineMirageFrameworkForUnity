//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using System;
	using System.IO;
	using System.Collections.Generic;
	using KoganeUnityLib;
	using Service;
	using File;
	using Utility;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ CSV情報の管理クラス
	///		CSVを利用する各種情報クラスは、この管理クラスで纏める。
	/// </summary>
	///====================================================================================================
	public class SMCSVDataManager<TKey, TValue> : BaseSMDataManager<TKey, TValue>
		where TValue : SMCSVData<TKey>, new()
	{
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>読込階層</summary>
		public readonly string _path;
		/// <summary>読込書類名</summary>
		public readonly string _fileName;
		/// <summary>書類の型</summary>
		protected readonly SMFileLocation _fileType;
		/// <summary>読込開始の行数</summary>
		protected readonly int _loadStartIndex;

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMCSVDataManager( string path, string fileName, SMFileLocation fileType, int loadStartIndex ) {
			_path = path;
			_fileName = fileName;
			_fileType = fileType;
			_loadStartIndex = loadStartIndex;

			_loadEvent.AddFirst( async canceler => {
				var loader = SMServiceLocator.Resolve<SMFileManager>()._csvLoader;
				var path = Path.Combine( _path, _fileName );
				var datas = await loader.Load( _fileType, path, false );    // 指定書類を読込

				// データが存在しない場合、エラー
				if ( datas == null ) {
					throw new InvalidOperationException( string.Join( "\n",
						$"データ読込失敗 : ",
						$"{nameof( SMCSVDataManager<TKey, TValue> )}.()",
						$"{nameof( _path )} : {_path}",
						$"{this}"
					) );
				}

				// 全情報を設定
				datas.ForEach( ( d, i ) => SetData( _fileName, i, d ) );
			} );

			_saveEvent.AddFirst( async canceler => {
// TODO : 情報からCSVに保存を実装
				await UTask.DontWait();
			} );
		}

		/// <summary>
		/// ● データを設定
		/// </summary>
		protected virtual void SetData( string fileName, int index, List<string> texts ) {
			var data = new TValue();
			data.Setup( fileName, index, texts );
			Register( data._registerKey, data );
		}
	}
}