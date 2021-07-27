//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using UnityEngine;
	using UniRx;
	using Service;
	using Task;
	using Extension;
	using Setting;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 接続状態の管理クラス
	/// </summary>
	///====================================================================================================
	public class SMNetworkManager : SMTask, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>実行型</summary>
		public override SMTaskRunType _type => SMTaskRunType.Sequential;

		/// <summary>接続中か？</summary>
		public bool _isConnecting	{ get; private set; }
		/// <summary>安定接続か？</summary>
		public readonly ReactiveProperty<bool> _isStableConnection = new ReactiveProperty<bool>();
		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMNetworkManager() {
			_disposables.AddFirst( () => {
				_isConnecting = false;
				_isStableConnection.Dispose();
			} );
		}

		/// <summary>
		/// ● 作成
		/// </summary>
		public override void Create() {
			const float CHECK_STABLE_CONECT_SECOND = 1; // 安定接続判定秒数
			var seconds = 0f;   // 接続秒数
			var timeManager = SMServiceLocator.Resolve<SMTimeManager>();

			// ● 更新
			_updateEvent.AddLast().Subscribe( _ => {
				_isConnecting = Application.internetReachability != NetworkReachability.NotReachable;

				// 接続状態が一定秒有る場合、安定接続と判定
				if ( _isConnecting ) {
					seconds = Mathf.Min( seconds + timeManager._unscaledDeltaTime, CHECK_STABLE_CONECT_SECOND );
				} else {
					seconds = 0;
				}
				_isStableConnection.Value = seconds >= CHECK_STABLE_CONECT_SECOND;
			} );


			// 安定接続に遷移した場合、各管理クラスをリフレッシュ
			_isStableConnection
				.Where( @is => @is )
/*
// TODO : 課金、広告の監視設計を考える
				.Where( _ => GamePurchaseManager.s_isCreated )
				.Where( _ => GamePurchaseManager.s_instance._isInitialized )
				.Where( _ => GameAdvertisementManager.s_isCreated )
				.Where( _ => GameAdvertisementManager.s_instance._isInitialized )
*/
				.Subscribe( _ => {
					SMLog.Debug( $"{this.GetAboutName()} : 安定接続遷移時リフレッシュ", SMLogTag.Server );
/*
					GameAdvertisementManager.s_instance.Refresh();
					GamePurchaseManager.s_instance.Refresh();
*/
				} );
		}
	}
}