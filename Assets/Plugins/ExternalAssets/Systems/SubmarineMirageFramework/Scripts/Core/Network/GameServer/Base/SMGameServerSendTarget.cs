//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	///====================================================================================================
	/// <summary>
	/// ■ ゲームサーバーの送信対象クラス
	/// </summary>
	///====================================================================================================
	public class SMGameServerSendTarget : SMLightBase {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>送信対象の型</summary>
		public SMGameServerSendTargetType _type	{ get; private set; }
		/// <summary>後参加プレイヤー用に、送信情報を確保するか？</summary>
		public bool _isKeep	{ get; private set; }
		///------------------------------------------------------------------------------------------------
		/// ● 生成、破棄
		///------------------------------------------------------------------------------------------------
		public SMGameServerSendTarget( SMGameServerSendTargetType type, bool isKeep = true ) {
			_type = type;
			_isKeep = isKeep;
		}

		public override void Dispose() {
		}
	}
}