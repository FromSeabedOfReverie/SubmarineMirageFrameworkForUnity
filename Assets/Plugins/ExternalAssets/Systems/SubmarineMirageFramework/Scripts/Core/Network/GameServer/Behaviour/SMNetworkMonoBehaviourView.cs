//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Network {
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using KoganeUnityLib;
	using Base;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ ネットワークのモノ動作のビュークラス
	/// </summary>
	///====================================================================================================
	public abstract class SMNetworkMonoBehaviourView : SMStandardMonoBehaviour, ISMNetworkMonoBehaviourView {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		ISMNetworkMonoBehaviourView _view	{ get; set; }

		[SerializeField] SMNetworkSendType _sendTypeSetter = SMNetworkSendType.None;
		[SerializeField] SMNetworkTransformType _useTransformTypeSetter = SMNetworkTransformType.None;
		[SerializeField] bool _isUseScaleSetter = false;
		[SerializeField] bool _isUseTeleportSetter = false;
		[SerializeField] bool _isUseAnimatorSetter = false;

		public SMNetworkSendType _sendType				=> _sendTypeSetter;
		public SMNetworkTransformType _useTransformType	=> _useTransformTypeSetter;
		public bool _isUseScale		=> _isUseScaleSetter;
		public bool _isUseTeleport	=> _isUseTeleportSetter;
		public bool _isUseAnimator	=> _isUseAnimatorSetter;

		public object _networkView	=> _view != null ? _view._networkView : null;
		public object _owner		=> _view != null ? _view._owner : null;

		public SMNetworkObjectType _objectType	=> _view != null ? _view._objectType : SMNetworkObjectType.Other;
		public int _networkID		=> _view != null ? _view._networkID : -1;
		public int _ownerID			=> _view != null ? _view._ownerID : -1;
		public string _ownerName	=> _view != null ? _view._ownerName : string.Empty;

		public Subject< List<object> > _sendStreamEvent		=> _view != null ? _view._sendStreamEvent : null;
		public Subject< List<object> > _receiveStreamEvent	=> _view != null ? _view._receiveStreamEvent : null;
		public Subject<SMGameServerSendData> _receiveEvent	=> _view != null ? _view._receiveEvent : null;

		///------------------------------------------------------------------------------------------------
		/// ● 生成、削除
		///------------------------------------------------------------------------------------------------
		protected override void Awake() {
#if PHOTON_UNITY_NETWORKING
			_view = GetComponent<SMPhotonMonoBehaviourView>();
			_disposables.AddFirst( () => {
				_view.Dispose();
				_view = null;
			} );
#endif

			base.Awake();
		}

		void Reset() {
			ApplyComponents();
		}

		///------------------------------------------------------------------------------------------------
		/// ● 情報を送受信
		///------------------------------------------------------------------------------------------------
		public void Send<TTarget, TData>( TTarget target, TData data )
			where TTarget : class
			where TData : SMGameServerSendData
		{
			if ( _view == null )	{ return; }

			_view.Send( target, data );
		}

		///------------------------------------------------------------------------------------------------
		/// ● 部品を適用
		///------------------------------------------------------------------------------------------------
		public void ApplyComponents() {
#if PHOTON_UNITY_NETWORKING
			var v = gameObject.GetOrAddComponent<SMPhotonMonoBehaviourView>();
			v.ApplyComponents();
#endif
		}
	}
}