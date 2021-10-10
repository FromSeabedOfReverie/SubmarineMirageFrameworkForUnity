//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using UnityEngine;
	using UniRx;
	///====================================================================================================
	/// <summary>
	/// ■ ネットワークの管理クラス
	/// </summary>
	///====================================================================================================
	public class SMNetworkManager : SMTask, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>安定接続判定秒数</summary>
		const float CHECK_STABLE_CONECT_SECOND = 1;
		/// <summary>最大プレイヤー数</summary>
		public const int MAX_PLAYERS = SMMainSetting.MAX_PLAYERS;


		/// <summary>実行型</summary>
		[SMShowLine] public override SMTaskRunType _type => SMTaskRunType.Sequential;

		/// <summary>接続中か？</summary>
		[SMShowLine] public bool _isConnect	{ get; private set; }
		/// <summary>安定接続か？</summary>
		public readonly ReactiveProperty<bool> _isStableConnect = new ReactiveProperty<bool>();

		[SMShow] public SMGameServerModel _gameServerModel { get; private set; }

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMNetworkManager() {
#if PHOTON_UNITY_NETWORKING
			var model = new SMPhotonServerModel( this );
			_gameServerModel = model;
			var view = SMPhotonServerView.Create( model );
			_disposables.AddFirst( () => {
				view.Dispose();
			} );
#endif

			_disposables.AddFirst( () => {
				_isConnect = false;
				_isStableConnect.Dispose();
				_gameServerModel?.Dispose();
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
				_isConnect = Application.internetReachability != NetworkReachability.NotReachable;

				// 接続状態が一定秒有る場合、安定接続と判定
				if ( _isConnect ) {
					seconds = Mathf.Min( seconds + timeManager._unscaledDeltaTime, CHECK_STABLE_CONECT_SECOND );
				} else {
					seconds = 0;
				}
				_isStableConnect.Value = seconds >= CHECK_STABLE_CONECT_SECOND;
			} );


			// 安定接続に遷移した場合、各管理クラスをリフレッシュ
			_isStableConnect
				.Where( @is => @is )
				.Subscribe( _ => {
//					SMLog.Debug( $"{this.GetAboutName()} : 安定接続リフレッシュ", SMLogTag.Server );
				} );
		}
	}
}