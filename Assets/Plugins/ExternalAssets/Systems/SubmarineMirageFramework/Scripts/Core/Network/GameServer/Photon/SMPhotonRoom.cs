//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#if PHOTON_UNITY_NETWORKING
namespace SubmarineMirage.Network {
	using Photon.Realtime;
	using Photon.Pun;
	using KoganeUnityLib;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ フォトンの部屋クラス
	/// </summary>
	///====================================================================================================
	public class SMPhotonRoom : SMGameServerRoom {
		const string PASSWORD_SPLIT_TEXT = ":password:";

		[SMShowLine] public override bool _isLock {
			get => PhotonNetwork.CurrentRoom != null ? !PhotonNetwork.CurrentRoom.IsOpen : false;
			set {
				if ( PhotonNetwork.CurrentRoom != null ) {
					PhotonNetwork.CurrentRoom.IsOpen = !value;
				}
			}
		}



		static SMPhotonRoom() {
			s_prohibitionWords.Add( PASSWORD_SPLIT_TEXT );
		}

		public SMPhotonRoom( string name, string password, int maxPlayerCount )
			: base( name, password, maxPlayerCount )
		{}

		public SMPhotonRoom( RoomInfo room ) : base() {
			var ss = ToNameAndPassword( room.Name );
			_name = ss[0];
			_password = ss[1];

			_playerCount = room.PlayerCount;
			_maxPlayerCount = room.MaxPlayers;

			_isActive = !room.RemovedFromList && room.IsOpen && !IsFull();
		}

		public override void Dispose() {
		}



		public bool IsEqual( string token ) {
			var ss = ToNameAndPassword( token );
			var name = ss[0];
			var password = ss[1];
			return _name == name && _password == password;
		}



		public override string ToToken()
			=> $"{_name}{PASSWORD_SPLIT_TEXT}{_password}";

		public static string[] ToNameAndPassword( string token ) {
			var ns = token.Split( PASSWORD_SPLIT_TEXT );
			var name = ns[0];
			var password = ns.Length > 1 ? ns[1] : string.Empty;

			if ( ns.Length > 2 ) {
				throw new GameServerSMException(
					SMGameServerErrorType.UseProhibitionWord,
					null,
					string.Join( "\n",
						$"{nameof( SMPhotonRoom )}.{nameof( ToNameAndPassword )} : 禁則単語を使用",
						$"{nameof( token )} : {token}"
					),
					false
				);
			}

			return new string[] { name, password };
		}
	}
}
#endif