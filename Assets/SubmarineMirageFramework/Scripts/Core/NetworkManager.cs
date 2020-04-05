//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using UnityEngine;
	using UniRx;
	using Singleton;
	using Extension;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 接続状態の管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class NetworkManager : Singleton<NetworkManager> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>安定接続判定秒数</summary>
		const float CHECK_STABLE_CONECT_SECOND = 1;

		/// <summary>接続中か？</summary>
		public bool _isConnecting => Application.internetReachability != NetworkReachability.NotReachable;

		/// <summary>安定接続か？</summary>
		public readonly ReactiveProperty<bool> _isStableConnection = new ReactiveProperty<bool>();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public NetworkManager() {
			var connectionSecond = 0f;	// 接続秒数

			// ● 更新
			_updateEvent
				.Subscribe(
					_ => {
						// 接続状態が一定秒有る場合、安定接続と判定
						connectionSecond =
							_isConnecting	? connectionSecond + TimeManager.s_instance._unscaledDeltaTime
											: 0;
						connectionSecond = Mathf.Clamp( connectionSecond, 0, CHECK_STABLE_CONECT_SECOND );
						_isStableConnection.Value = connectionSecond >= CHECK_STABLE_CONECT_SECOND;
					}
				);


			// 安定接続に遷移した場合、各管理クラスをリフレッシュ
			_isStableConnection
				.Where( is_ => is_ )
/*
				// TODO : 課金、広告の監視設計を考える
				.Where( _ => GamePurchaseManager.s_isCreated )
				.Where( _ => GamePurchaseManager.s_instance._isInitialized )
				.Where( _ => GameAdvertisementManager.s_isCreated )
				.Where( _ => GameAdvertisementManager.s_instance._isInitialized )
*/
				.Subscribe( _ => {
					Log.Debug( $"{this.GetAboutName()} : 安定接続遷移時リフレッシュ", Log.Tag.Server );
/*
					GameAdvertisementManager.s_instance.Refresh();
					GamePurchaseManager.s_instance.Refresh();
*/
				} );
		}
	}
}