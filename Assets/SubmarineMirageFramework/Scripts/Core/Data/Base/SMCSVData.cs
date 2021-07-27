//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using System.Collections.Generic;
	///====================================================================================================
	/// <summary>
	/// ■ CSV情報のクラス
	///		CSVを利用する各種情報クラスは、このクラスを継承する。
	/// </summary>
	///====================================================================================================
	public abstract class SMCSVData<T> : BaseSMData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>辞書への登録鍵</summary>
		public abstract T _registerKey	{ get; }

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMCSVData() {
		}

		/// <summary>
		/// ● 設定
		/// </summary>
		public abstract void Setup( string fileName, int index, List<string> texts );
	}
}