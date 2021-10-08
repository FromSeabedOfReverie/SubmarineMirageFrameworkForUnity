//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#if PHOTON_UNITY_NETWORKING
namespace SubmarineMirage.Network {
	using Utility;
	///====================================================================================================
	/// <summary>
	/// ■ マスターサーバーが、未接続の、フォトン状態クラス
	/// </summary>
	///====================================================================================================
	public class DisconnectSMPhotonMasterState : SMPhotonMasterState {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		[SMShow] public override SMGameServerType _type => SMGameServerType.Disconnect;



		public DisconnectSMPhotonMasterState() {
			_enterEvent.Remove( _registerEventKey );
			_asyncUpdateEvent.Remove( _registerEventKey );
			_exitEvent.Remove( _registerEventKey );

			_enterEvent.AddLast( _registerEventKey, async canceler => {
				_status = SMGameServerStatus.Disconnect;
				_error = null;
				await UTask.DontWait();
			} );

			_exitEvent.AddLast( _registerEventKey, async canceler => {
				_status = SMGameServerStatus.Disconnect;
				_error = null;
				await UTask.DontWait();
			} );
		}



		protected override bool Connect()
			=> false;

		protected override bool Disconnect()
			=> false;
	}
}
#endif