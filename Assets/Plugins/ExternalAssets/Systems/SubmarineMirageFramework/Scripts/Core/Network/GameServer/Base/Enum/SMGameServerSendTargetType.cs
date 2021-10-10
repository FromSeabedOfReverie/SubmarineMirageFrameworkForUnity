//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {



	/// <summary>ゲームサーバーの送信対象型</summary>
	public enum SMGameServerSendTargetType {
		/// <summary>全員に送信（自身はネットワーク送信しない）</summary>
		All,
		/// <summary>全員に送信（自身もネットワーク送信する）</summary>
		AllByNetwork,
		/// <summary>サーバークライアントに送信</summary>
		Server,
		/// <summary>自身以外に送信</summary>
		Other,
	}
}