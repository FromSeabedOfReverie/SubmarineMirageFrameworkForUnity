//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {



	/// <summary>ゲームサーバーの接続状態</summary>
	public enum SMGameServerStatus {
		/// <summary>接続切断</summary>
		Disconnect,
		/// <summary>接続成功</summary>
		Connect,
		/// <summary>接続失敗</summary>
		Error,
	}
}