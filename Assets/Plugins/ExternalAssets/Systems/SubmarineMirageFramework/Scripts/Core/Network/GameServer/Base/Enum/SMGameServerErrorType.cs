//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {



	/// <summary>ゲームサーバーの接続失敗の型</summary>
	public enum SMGameServerErrorType {
		/// <summary>ネットワークに未接続</summary>
		NoNetwork,
		/// <summary>禁則単語を使用</summary>
		UseProhibitionWord,
		/// <summary>パスワードが不一致</summary>
		MismatchPassword,
		/// <summary>部屋が満室</summary>
		FullRoom,
		/// <summary>その他</summary>
		Other,
	}
}