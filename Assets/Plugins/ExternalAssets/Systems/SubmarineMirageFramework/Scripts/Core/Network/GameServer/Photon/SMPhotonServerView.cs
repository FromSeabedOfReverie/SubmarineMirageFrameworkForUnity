//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestNetwork
#if PHOTON_UNITY_NETWORKING
namespace SubmarineMirage {
	using System.Collections.Generic;
	using UnityEngine;
	using Photon.Realtime;
	using Photon.Pun;
	using ExitGames.Client.Photon;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ フォトンサーバーのビュークラス
	/// </summary>
	///====================================================================================================
	public class SMPhotonServerView : MonoBehaviourPunCallbacks, IOnEventCallback, ISMLightBase {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		public uint _id { get; private set; }
		SMPhotonServerModel _model { get; set; }



#region ToString
		public virtual string AddToString( int indent )
			=> string.Empty;
#endregion

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		public static SMPhotonServerView Create( SMPhotonServerModel model ) {
			var go = new GameObject( nameof( SMPhotonServerView ) );
			go.DontDestroyOnLoad();
			var view = go.AddComponent<SMPhotonServerView>();
			view.Setup( model );
#if TestNetwork
			SMLog.Debug( $"作成 : {nameof( SMPhotonServerView )}", SMLogTag.Server );
#endif
			return view;
		}

		public void Setup( SMPhotonServerModel model ) {
			_id = SMIDCounter.GetNewID( this );
			_model = model;
		}

		void OnDestroy()
			=> Dispose();

		public virtual void Dispose() {
			gameObject.Destroy();
		}

		///------------------------------------------------------------------------------------------------
		/// ● 呼戻
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● マスターサーバーに接続成功（呼戻）
		/// </summary>
		public override void OnConnectedToMaster() {
			base.OnConnectedToMaster();
			_model._masterFSM._state.OnConnect();
		}

		/// <summary>
		/// ● マスターサーバーの接続切断（呼戻）
		/// </summary>
		public override void OnDisconnected( DisconnectCause cause ) {
			base.OnDisconnected( cause );
			_model._masterFSM._state.OnDisconnect( cause );
		}



		/// <summary>
		/// ● 控室に参加成功（呼戻）
		/// </summary>
		public override void OnJoinedLobby() {
			base.OnJoinedLobby();
			_model._roomFSM._state.OnConnect();
		}

		/// <summary>
		/// ● 控室から退室成功（呼戻）
		/// </summary>
		public override void OnLeftLobby() {
			base.OnLeftLobby();
			_model._roomFSM._state.OnDisconnect();
		}

		/// <summary>
		/// ● 部屋一覧を更新（呼戻）
		/// </summary>
		public override void OnRoomListUpdate( List<RoomInfo> roomList ) {
			base.OnRoomListUpdate( roomList );
			( _model._roomFSM._state as LobbySMPhotonRoomState ).OnUpdateRoom( roomList );
		}



		/// <summary>
		/// ● 部屋の作成成功（呼戻）
		/// </summary>
		public override void OnCreatedRoom() {
			base.OnCreatedRoom();
			_model._roomFSM._state.OnConnect();
		}

		/// <summary>
		/// ● 部屋の作成失敗（呼戻）
		/// </summary>
		public override void OnCreateRoomFailed( short returnCode, string message ) {
			base.OnCreateRoomFailed( returnCode, message );
			_model._roomFSM._state.OnError( returnCode, message );
		}



		/// <summary>
		/// ● 部屋に参加成功（呼戻）
		/// </summary>
		public override void OnJoinedRoom() {
			base.OnJoinedRoom();
			_model._roomFSM._state.OnConnect();
		}

		/// <summary>
		/// ● 部屋に参加失敗（呼戻）
		/// </summary>
		public override void OnJoinRoomFailed( short returnCode, string message ) {
			base.OnJoinRoomFailed( returnCode, message );
			_model._roomFSM._state.OnError( returnCode, message );
		}



		/// <summary>
		/// ● 部屋から退室成功（呼戻）
		/// </summary>
		public override void OnLeftRoom() {
			base.OnLeftRoom();
			_model._roomFSM._state.OnDisconnect();
		}



		/// <summary>
		/// ● プレイヤー入室（呼戻）
		/// </summary>
		public override void OnPlayerEnteredRoom( Player newPlayer ) {
			base.OnPlayerEnteredRoom( newPlayer );
			_model._playerCountEvent.Value = _model._playerCount;
		}

		/// <summary>
		/// ● プレイヤー退室（呼戻）
		/// </summary>
		public override void OnPlayerLeftRoom( Player otherPlayer ) {
			base.OnPlayerLeftRoom( otherPlayer );
			_model._playerCountEvent.Value = _model._playerCount;
		}



		/// <summary>
		/// ● サーバーから情報受信
		/// </summary>
		public void OnEvent( EventData photonEvent ) {
			_model.Receive( photonEvent );
		}

		///------------------------------------------------------------------------------------------------
		/// ● 文字列に変換
		///------------------------------------------------------------------------------------------------
		public override string ToString() => ToString( 0 );

		public virtual string ToString( int indent, bool isUseHeadIndent = true )
			=> this.ToShowString( indent, false, false, isUseHeadIndent );

		public virtual string ToLineString( int indent = 0 ) => ObjectSMExtension.ToLineString( this, indent );
	}
}
#endif