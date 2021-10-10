//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace Game {
	using System;
	using SubmarineMirage.FSM;
	using SubmarineMirage.Scene;



	/// <summary>
	/// ■ シーンデータの設定クラス
	///		登録すると、SMSceneManagerから遷移できる。
	/// </summary>
	public class SMSceneSetting : BaseSMSceneSetting {

		/// <summary>
		/// ● 設定
		/// </summary>
		public override void Setup() {
			_datas = new SMFSMGenerateList {
				{
					// アプリ終了まで、永続読込のシーン
					new Type[] { typeof( ForeverSMScene ), },
					typeof( ForeverSMScene )
				}, {
					// アクティブ状態とする、メインシーン
					// ここに、Unityシーンと対応する、クラスを登録（シーン名 + SMScene）
					new Type[] {
						typeof( UnknownSMScene ),
						typeof( TitleSMScene ),
						typeof( GameSMScene ),
						typeof( GameOverSMScene ),
						typeof( GameClearSMScene ),
					},
					typeof( MainSMScene )
				}, {
					// UI配置専用のシーン
					new Type[] {
						typeof( UINoneSMScene ),
					},
					typeof( UISMScene )
				}, {
					// デバッグ用のシーン
					new Type[] { typeof( DebugSMScene ), },
					typeof( DebugSMScene )
				},
			};
		}
	}
}