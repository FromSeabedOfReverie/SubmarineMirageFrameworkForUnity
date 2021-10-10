//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestNetwork
#if PHOTON_UNITY_NETWORKING
namespace SubmarineMirage {
	using System.Linq;
	using System.Collections.Generic;
	using Photon.Realtime;
	using Photon.Pun;
	///====================================================================================================
	/// <summary>
	/// ■ 部屋が、控室の、フォトン状態クラス
	/// </summary>
	///====================================================================================================
	public class LobbySMPhotonRoomState : SMPhotonRoomState {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		[SMShow] public override SMGameServerRoomType _type => SMGameServerRoomType.Join;

		/// <summary>部屋の一覧</summary>
		[SMShow] readonly Dictionary<string, SMPhotonRoom> _rooms = new Dictionary<string, SMPhotonRoom>();



		public LobbySMPhotonRoomState() {
			_exitEvent.AddLast( _registerEventKey, async canceler => {
				_rooms.Clear();
				SendRooms();
				await UTask.DontWait();
			} );


			_disposables.AddFirst( () => {
				_rooms.Clear();
			} );
		}



		protected override bool Connect() {
			var isSuccess = PhotonNetwork.JoinLobby();
#if TestNetwork
			SMLog.Debug( $"{this.GetAboutName()}.{nameof( Connect )} : {isSuccess}", SMLogTag.Server );
#endif
			return isSuccess;
		}

		protected override bool Disconnect() {
			var isSuccess = PhotonNetwork.LeaveLobby();
#if TestNetwork
			SMLog.Debug( $"{this.GetAboutName()}.{nameof( Disconnect )} : {isSuccess}", SMLogTag.Server );
#endif
			return isSuccess;
		}



		public void OnUpdateRoom( List<RoomInfo> differenceRooms ) {
			differenceRooms.ForEach( i => {
				if ( i.RemovedFromList && i.PlayerCount == 0 ) {
					_rooms.Remove( i.Name );
					return;
				}

				try {
					_rooms[i.Name] = new SMPhotonRoom( i );
				} catch ( GameServerSMException e ) {
					SMLog.Error( e, SMLogTag.Server );
					// 他人の部屋のエラーは、伝達しない
				}
			} );

			SendRooms();
		}

		public void SendRooms() {
			_owner._roomsEvent.OnNext(
				_rooms
					.Select( pair => pair.Value as SMGameServerRoom )
					.ToList()
			);
		}
	}
}
#endif