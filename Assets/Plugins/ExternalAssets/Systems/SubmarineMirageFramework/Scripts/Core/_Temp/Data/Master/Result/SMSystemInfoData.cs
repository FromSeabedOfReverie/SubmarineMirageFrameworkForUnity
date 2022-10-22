//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using System.Collections.Generic;
	///====================================================================================================
	/// <summary>
	/// ■ システムの情報クラス
	/// </summary>
	///====================================================================================================
	public class SMSystemInfoData : SMResultData<SMSystemInfo> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>題目</summary>
		[SMShow] public string _title { get; private set; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Setup( string fileName, int index, List<string> texts ) {
			base.Setup( fileName, index, texts );

			_title = texts[2];
		}
	}
}