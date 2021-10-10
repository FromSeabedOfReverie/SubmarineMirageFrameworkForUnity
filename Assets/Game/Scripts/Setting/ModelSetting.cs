//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace Game {
	using System.Collections.Generic;
	using SubmarineMirage;



	/// <summary>
	/// ■ モデルの設定クラス
	/// </summary>
	public class ModelSetting : SMStandardBase {
		public readonly List<IModel> _registerModels;



		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public ModelSetting() {
			_disposables.AddFirst( () => {
				_registerModels.Clear();
			} );

			_registerModels = new List<IModel> {
				// ここに、Modelを登録
			};
		}
	}
}