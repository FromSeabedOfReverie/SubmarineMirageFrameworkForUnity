//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestNetwork
#if PHOTON_UNITY_NETWORKING
namespace SubmarineMirage {
	using Photon.Pun;
	///====================================================================================================
	/// <summary>
	/// ■ マスターサーバーが、オンライン接続の、フォトン状態クラス
	/// </summary>
	///====================================================================================================
	public class OnlineSMPhotonMasterState : SMPhotonMasterState {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		[SMShow] public override SMGameServerType _type => SMGameServerType.Online;



		public OnlineSMPhotonMasterState() {
			_enterEvent.AddLast( _registerEventKey, async canceler => {
				if ( _status == SMGameServerStatus.Connect ) {
					await _owner._roomFSM.ChangeState<LobbySMPhotonRoomState>();
				}
			} );

			_exitEvent.AddFirst( _registerEventKey, async canceler => {
				if ( !( _owner._roomFSM._state is DisconnectSMPhotonRoomState ) ) {
					await _owner._roomFSM.ChangeState<DisconnectSMPhotonRoomState>();
				}
			} );
		}



		protected override bool Connect() {
			PhotonNetwork.OfflineMode = false;
			PhotonNetwork.GameVersion = SMMainSetting.APPLICATION_VERSION;
			var isSuccess = PhotonNetwork.ConnectUsingSettings();
#if TestNetwork
			SMLog.Debug( $"{this.GetName()}.{nameof( Connect )} : {isSuccess}", SMLogTag.Server );
#endif
			return isSuccess;
		}

		protected override bool Disconnect() {
			PhotonNetwork.Disconnect();
			var isSuccess = true;
#if TestNetwork
			SMLog.Debug( $"{this.GetName()}.{nameof( Disconnect )} : {isSuccess}", SMLogTag.Server );
#endif
			return isSuccess;
		}
	}
}
#endif