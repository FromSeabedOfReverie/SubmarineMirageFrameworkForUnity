//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using System.Linq;
	using System.Collections.Generic;
	using KoganeUnityLib;
	using Base;
	using Service;
	///====================================================================================================
	/// <summary>
	/// ■ データ設定の基盤クラス
	/// </summary>
	///====================================================================================================
	public abstract class BaseSMDataSetting : SMStandardBase, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		public Dictionary< SMDataSettingType, List<IBaseSMDataManager> > _datas { get; protected set; }
			= new Dictionary< SMDataSettingType, List<IBaseSMDataManager> >();
		SMAllDataManager _allDataManager	{ get; set; }

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public BaseSMDataSetting() {
			_disposables.AddLast( () => {
				_datas
					.SelectMany( pair => pair.Value )
					.ForEach( d => d.Dispose() );
				_datas.Clear();

				_allDataManager = null;
			} );
		}

		/// <summary>
		/// ● 設定
		/// </summary>
		public virtual void Setup() {
			_allDataManager = SMServiceLocator.Resolve<SMAllDataManager>();
		}

		///------------------------------------------------------------------------------------------------
		/// ● 登録、解除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録
		/// </summary>
		public void RegisterDatas( SMDataSettingType type ) {
			_datas[type].ForEach( d => _allDataManager.Register( d.GetType(), d ) );
			// Dispose前に、参照を切る
			_datas[type].Clear();
			_datas.Remove( type );
		}
	}
}