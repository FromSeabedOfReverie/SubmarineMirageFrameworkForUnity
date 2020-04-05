//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.Save {
	using UnityEngine;
	using UniRx.Async;
	using Data.File;
	///====================================================================================================
	/// <summary>
	/// ■ 設定情報の管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class SettingDataManager : SaveData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>設定情報の、保存書類名</summary>
		const string FILE_NAME = "SettingData.data";

		/// <summary>設定情報</summary>
		public SettingData _data	{ get; private set; } = new SettingData();
		/// <summary>暗号化書類の、読み書き</summary>
		CryptoLoader _loader	=> FileManager.s_instance._cryptoLoader;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Load() {
			// 展示版の場合、新規情報のまま、未読込
			if ( _data._edition == SettingData.Edition.Exhibition ) {

			// 評価版、製品版の場合
			} else {
				// 読込成功で、バージョンが等しい場合、登録
				var newData = await _loader.Load<SettingData>( FILE_NAME );
				if ( newData != null && newData._version == Application.version ) {
					_data = newData;
				}
			}

			await _data.Load();
			await base.Load();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 保存
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Save() {
			await _data.Save();
			await _loader.Save( FILE_NAME, _data );
			await base.Save();
		}
	}
}