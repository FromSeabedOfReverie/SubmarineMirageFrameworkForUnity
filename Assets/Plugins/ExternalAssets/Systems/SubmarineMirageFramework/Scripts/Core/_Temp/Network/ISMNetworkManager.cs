//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using UniRx;
	///====================================================================================================
	/// <summary>
	/// ■ ネットワークの管理クラスの設計図
	/// </summary>
	///====================================================================================================
	public interface ISMNetworkManager : ISMService, ISMStandardBase {
		///------------------------------------------------------------------------------------------------
		/// ● プロパティ
		///------------------------------------------------------------------------------------------------
		/// <summary>ゲームサーバーの管理者</summary>
//		public ISMGameServerManager GameServerManager { get; }

		/// <summary>接続中か？</summary>
		public bool IsConnect { get; }

		/// <summary>安定接続か？イベント</summary>
		public IReactiveProperty<bool> IsStableConnectEvent { get; }
	}
}