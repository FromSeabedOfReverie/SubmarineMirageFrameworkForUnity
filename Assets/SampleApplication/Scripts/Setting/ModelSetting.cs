using System.Collections.Generic;
using Sample;
using SubmarineMirage.Base;
using SubmarineMirage.Debug;



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
			// --------------------------------------------------------------------------------------------
			//  ★ ここに、Modelを登録してください。
			// --------------------------------------------------------------------------------------------
			// こんな感じに
			new SampleModel(),

			new MotionModel(),
			new FieldModel(),
			new ScoreModel(),
			new MainGameManagementModel(),
		};
	}
}