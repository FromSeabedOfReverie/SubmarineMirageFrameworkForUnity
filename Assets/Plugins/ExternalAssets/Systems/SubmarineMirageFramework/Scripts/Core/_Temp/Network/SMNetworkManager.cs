//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using UnityEngine;
	using UniRx;
	///====================================================================================================
	/// <summary>
	/// ■ ネットワークの管理クラス
	/// </summary>
	///====================================================================================================
	public class SMNetworkManager : SMTask, ISMNetworkManager {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>安定接続判定秒数</summary>
		const float CHECK_STABLE_CONECT_SECOND = 1;


		[SMShowLine] public override SMTaskRunType _type => SMTaskRunType.Sequential;

		readonly ReactiveProperty<bool> _isStableConnect = new ReactiveProperty<bool>();

		///------------------------------------------------------------------------------------------------
		/// ● プロパティ
		///------------------------------------------------------------------------------------------------
		[SMShow] public int MaxPlayers
			=> SMMainSetting.MAX_PLAYERS;

//		[SMShow] public ISMGameServerManager GameServerManager { get; }

		[SMShowLine] public bool IsConnect { get; private set; }

		public IReactiveProperty<bool> IsStableConnectEvent
			=> _isStableConnect;

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMNetworkManager() {
/*
#if PHOTON_UNITY_NETWORKING
			var model = new SMPhotonServerModel( this );
			GameServerManager = model;
			var view = SMPhotonServerView.Create( model );
			_disposables.AddFirst( () => {
				view.Dispose();
			} );
#endif
*/

			_disposables.AddFirst( () => {
				IsConnect = false;
				_isStableConnect.Dispose();
//				GameServerManager?.Dispose();
			} );
		}

		/// <summary>
		/// ● 作成
		/// </summary>
		public override void Create() {
			var seconds = 0f;   // 接続秒数
			var timeManager = SMServiceLocator.Resolve<SMTimeManager>();

			// ● 更新
			_updateEvent.AddLast().Subscribe( _ => {
				IsConnect = Application.internetReachability != NetworkReachability.NotReachable;

				// 接続状態が一定秒有る場合、安定接続と判定
				if ( IsConnect ) {
					seconds = Mathf.Min( seconds + timeManager._unscaledDeltaTime, CHECK_STABLE_CONECT_SECOND );
				} else {
					seconds = 0;
				}
				_isStableConnect.Value = seconds >= CHECK_STABLE_CONECT_SECOND;
			} );
		}
	}
}