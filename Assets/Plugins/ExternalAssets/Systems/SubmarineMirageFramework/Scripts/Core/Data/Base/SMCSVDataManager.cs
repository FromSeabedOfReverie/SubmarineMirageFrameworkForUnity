//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;
	using System.IO;
	using System.Collections.Generic;
	using KoganeUnityLib;
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
		[SMShow] public readonly string _path;
		/// <summary>読込書類名</summary>
		[SMShow] public readonly string _fileName;
		/// <summary>書類の型</summary>
		[SMShow] protected readonly SMFileLocation _location;
		/// <summary>読込開始の行数</summary>
		protected readonly int _loadStartIndex;

#region ToString
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文章変換を設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void SetToString() {
			base.SetToString();

			_toStringer.AddLine( nameof( TValue ), () => typeof( TValue ).GetAboutName() );
		}
#endregion

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMCSVDataManager( string path, string fileName, SMFileLocation location, int loadStartIndex ) {
			_path = path;
			_fileName = fileName;
			_location = location;
			_loadStartIndex = loadStartIndex;

			_loadEvent.AddFirst( async canceler => {
				var loader = SMServiceLocator.Resolve<SMFileManager>().Get<SMCSVLoader>();
				var allPath = Path.Combine( _path, _fileName );
				var datas = await loader.Load( _location, allPath, false );    // 指定書類を読込

				// データが存在しない場合、エラー
				if ( datas == null ) {
					throw new InvalidOperationException( string.Join( "\n",
						$"データ読込失敗 : ",
						$"{nameof( SMCSVDataManager<TKey, TValue> )}.()",
						$"{nameof( allPath )} : {allPath}",
						$"{this}"
					) );
				}

				// 全情報を設定
				for ( var i = _loadStartIndex; i < datas.Count; i++ ) {
					SetData( _fileName, i - _loadStartIndex, datas[i] );
				}
			} );
/*
			_loadEvent.AddLast( async canceler => {
				SMLog.Debug( this );
				await UTask.DontWait();
			} );
*/

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