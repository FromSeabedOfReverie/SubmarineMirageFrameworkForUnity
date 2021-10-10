//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {



	/// <summary>ネットワークの送信型</summary>
	public enum SMNetworkSendType {
		/// <summary>送信しない</summary>
		None,
		/// <summary>確実に、差分を送信</summary>
		ReliableDelta,
		/// <summary>不確実に、変更時に送信</summary>
		UnreliableChange,
		/// <summary>不確実に、常時送信</summary>
		Unreliable,
	}
}