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
	/// ■ マスターサーバーが、オフライン接続の、フォトン状態クラス
	/// </summary>
	///====================================================================================================
	public class OfflineSMPhotonMasterState : SMPhotonMasterState {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		[SMShow] public override SMGameServerType _type => SMGameServerType.Offline;



		public OfflineSMPhotonMasterState() {
			_exitEvent.AddFirst( _registerEventKey, async canceler => {
				if ( !( _owner._roomState is DisconnectSMPhotonRoomState ) ) {
					await _owner._roomFSM.ChangeState<DisconnectSMPhotonRoomState>();
				}
			} );
			_exitEvent.AddLast( _registerEventKey, async canceler => {
				PhotonNetwork.OfflineMode = false;
				await UTask.DontWait();
			} );
		}



		protected override bool Connect() {
			PhotonNetwork.OfflineMode = true;
			var isSuccess = true;
#if TestNetwork
			SMLog.Debug( $"{this.GetAboutName()}.{nameof( Connect )} : {isSuccess}", SMLogTag.Server );
#endif
			return isSuccess;
		}

		protected override bool Disconnect() {
			PhotonNetwork.Disconnect();
			var isSuccess = true;
#if TestNetwork
			SMLog.Debug( $"{this.GetAboutName()}.{nameof( Disconnect )} : {isSuccess}", SMLogTag.Server );
#endif
			return isSuccess;
		}
	}
}
#endif