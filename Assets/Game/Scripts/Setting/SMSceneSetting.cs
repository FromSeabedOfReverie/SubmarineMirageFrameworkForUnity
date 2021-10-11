//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace Game {
	using System;
	using System.Collections.Generic;
	using SubmarineMirage;



	/// <summary>
	/// ■ シーンデータの設定クラス
	///		登録すると、SMSceneManagerから遷移できる。
	/// </summary>
	public class SMSceneSetting : BaseSMSceneSetting {

		/// <summary>
		/// ● 設定
		/// </summary>
		public override void Setup() {
			_datas = new Dictionary< Type, IEnumerable<Type> >() {
				{
					// アプリ終了まで、永続読込のシーン
					typeof( ForeverSMScene ),
					new Type[] {
						typeof( ForeverSMScene ),
					}
				}, {
					// アクティブ状態とする、メインシーン
					typeof( MainSMScene ),
					new Type[] {
						// ここに、Unityシーンと対応する、クラスを登録（シーン名 + SMScene）
						typeof( UnknownSMScene ),
						typeof( TitleSMScene ),
						typeof( GameSMScene ),
						typeof( GameOverSMScene ),
						typeof( GameClearSMScene ),
					}
				}, {
					// UI配置専用のシーン
					typeof( UISMScene ),
					new Type[] {
						typeof( UINoneSMScene ),
					}
				}, {
					// デバッグ用のシーン
					typeof( DebugSMScene ),
					new Type[] {
						typeof( DebugSMScene ),
					}
				},
			};
		}
	}
}