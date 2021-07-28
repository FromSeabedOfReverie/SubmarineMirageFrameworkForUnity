//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using System.Collections.Generic;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 広告の情報クラス
	///		広告情報を保存している。
	/// </summary>
	///====================================================================================================
	public class SMAdvertisementData : SMCSVData<SMAdvertisementType> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>辞書への登録鍵</summary>
		public override SMAdvertisementType _registerKey => _type;

		/// <summary>型</summary>
		public SMAdvertisementType _type { get; private set; }
		/// <summary>Android番号</summary>
		public string _androidID { get; private set; }
		/// <summary>iOS番号</summary>
		public string _iOSID { get; private set; }
		/// <summary>補足説明</summary>
		public string _info { get; private set; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Setup( string fileName, int index, List<string> texts ) {
			_type		= texts[0].ToEnum<SMAdvertisementType>();
			_androidID	= texts[1];
			_iOSID		= texts[2];
			_info		= texts[3];
		}
	}
}