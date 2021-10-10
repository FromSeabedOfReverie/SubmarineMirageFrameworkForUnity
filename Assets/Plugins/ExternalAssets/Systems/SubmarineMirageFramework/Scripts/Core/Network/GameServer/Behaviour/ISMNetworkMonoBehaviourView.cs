//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System.Collections.Generic;
	using UniRx;
	///====================================================================================================
	/// <summary>
	/// ■ ネットワークのモノ動作のビューインターフェース
	/// </summary>
	///====================================================================================================
	public interface ISMNetworkMonoBehaviourView : IBaseSM {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		SMNetworkSendType _sendType					{ get; }
		SMNetworkTransformType _useTransformType	{ get; }
		bool _isUseScale	{ get; }
		bool _isUseTeleport	{ get; }
		bool _isUseAnimator	{ get; }

		object _networkView	{ get; }
		object _owner		{ get; }

		SMNetworkObjectType _objectType	{ get; }
		int _networkID		{ get; }
		int _ownerID		{ get; }
		string _ownerName	{ get; }

		Subject< List<object> > _sendStreamEvent	{ get; }
		Subject< List<object> > _receiveStreamEvent	{ get; }
		Subject<SMGameServerSendData> _receiveEvent	{ get; }

		///------------------------------------------------------------------------------------------------
		/// ● 情報を送受信
		///------------------------------------------------------------------------------------------------
		void Send<TTarget, TData>( TTarget target, TData data )
			where TTarget : class
			where TData : SMGameServerSendData;

		///------------------------------------------------------------------------------------------------
		/// ● 部品を適用
		///------------------------------------------------------------------------------------------------
		void ApplyComponents();
	}
}