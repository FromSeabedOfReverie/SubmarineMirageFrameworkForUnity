//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using UnityEngine.Rendering;
	///====================================================================================================
	/// <summary>
	/// ■ スプラッシュ画面の待機クラス
	/// </summary>
	///====================================================================================================
	public class SMSplashScreenWaiter : SMTask {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>実行型</summary>
		public override SMTaskRunType _type => SMTaskRunType.Sequential;
		///------------------------------------------------------------------------------------------------
		/// ● 生成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 作成
		/// </summary>
		public override void Create() {
			// そのまま使うと、スプラッシュ画面が残ったまま、前回入力の値が反映される為、
			// スクリプト実行順序の最後に代入して、タイミングをずらす
			_selfInitializeEvent.AddLast( "", async canceler => {
				await UTask.WaitWhile( canceler, () => !SplashScreen.isFinished );
				await UTask.NextFrame( canceler );
			} );
		}
	}
}