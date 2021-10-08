//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#if PHOTON_UNITY_NETWORKING
namespace SubmarineMirage.Network {
	using System;
	using UniRx;
	using FSM;
	using Extension;
	using Utility;
	///====================================================================================================
	/// <summary>
	/// ■ 部屋の、フォトン状態クラス
	/// </summary>
	///====================================================================================================
	public abstract class SMPhotonRoomState : SMPhotonState {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>部屋の型</summary>
		[SMShow] public abstract SMGameServerRoomType _type { get; }
		/// <summary>部屋（内部）</summary>
		SMPhotonRoom _internalRoom { get; set; }
		/// <summary>部屋</summary>
		[SMShow] public SMPhotonRoom _room {
			get => _internalRoom;
			set {
				_internalRoom = value;
				_owner._currentRoomEvent.OnNext( _internalRoom );
			}
		}

		protected override Type _disconnectStateType => typeof( DisconnectSMPhotonRoomState );

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		public SMPhotonRoomState() {
			_enterEvent.AddFirst( _registerEventKey, async canceler => {
				_room = _room;
				await UTask.DontWait();
			} );
			_enterEvent.AddLast( _registerEventKey, async canceler => {
				_owner._playerCountEvent.Value = _owner._playerCount;
				await UTask.DontWait();
			} );

			_exitEvent.AddLast( _registerEventKey, async canceler => {
				_room = null;
				_owner._playerCountEvent.Value = _owner._playerCount;
				await UTask.DontWait();
			} );
		}

		public override void Setup( object owner, SMFSM fsm ) {
			base.Setup( owner, fsm );

			_disposables.AddFirst(
				_owner._playerCountEvent
					.Where( _ => _room != null )
					.Subscribe( i => {
						_room._playerCount = i;
						_room = _room;
					} )
			);
		}



		/// <summary>
		/// ● 部屋失敗（呼戻）
		/// </summary>
		public void OnError( short returnCode, string message ) {
			OnError(
				new GameServerSMException(
					(
						!_owner._networkManager._isConnect	? SMGameServerErrorType.NoNetwork
															: SMGameServerErrorType.Other
					),
					null,
					string.Join( "\n",
						$"サーバー接続失敗 : {this.GetAboutName()}.{nameof( OnError )}",
						$"{nameof( returnCode )} : {returnCode}",
						$"{message}"
					),
					true
				)
			);
		}
	}
}
#endif