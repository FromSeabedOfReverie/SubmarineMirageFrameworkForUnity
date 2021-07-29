//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using System.Collections.Generic;
	using Base;
	using Service;
	///====================================================================================================
	/// <summary>
	/// ■ データの設定クラス
	/// </summary>
	///====================================================================================================
	public interface ISMDataSetting : ISMStandardBase, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		Dictionary< SMDataSettingType, List<IBaseSMDataManager> > _datas	{ get; }

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void RegisterDatas( SMDataSettingType type );
	}
}