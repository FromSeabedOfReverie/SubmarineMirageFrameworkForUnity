//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestNetwork
#if PHOTON_UNITY_NETWORKING
namespace SubmarineMirage.Network {
	using Photon.Realtime;
	using Photon.Pun;
	using Extension;
	using Setting;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 部屋が、作成の、フォトン状態クラス
	/// </summary>
	///====================================================================================================
	public class CreateRoomSMPhotonRoomState : SMPhotonRoomState {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		[SMShow] public override SMGameServerRoomType _type => SMGameServerRoomType.Create;



		public CreateRoomSMPhotonRoomState() {
		}



		protected override bool Connect() {
			var option = new RoomOptions();
			option.MaxPlayers = ( byte )_room._maxPlayerCount;
			var isSuccess = PhotonNetwork.CreateRoom( _room.ToToken(), option );
#if TestNetwork
			SMLog.Debug( $"{this.GetAboutName()}.{nameof( Connect )} : {isSuccess}", SMLogTag.Server );
#endif
			return isSuccess;
		}

		protected override bool Disconnect() {
			var isSuccess = PhotonNetwork.LeaveRoom();
#if TestNetwork
			SMLog.Debug( $"{this.GetAboutName()}.{nameof( Disconnect )} : {isSuccess}", SMLogTag.Server );
#endif
			return isSuccess;
		}
	}
}
#endif