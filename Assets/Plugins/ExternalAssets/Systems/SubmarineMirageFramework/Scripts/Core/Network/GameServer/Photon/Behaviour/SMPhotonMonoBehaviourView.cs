//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestNetwork
#if PHOTON_UNITY_NETWORKING
namespace SubmarineMirage {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using Photon.Pun;
	using Photon.Realtime;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ フォトンのモノ動作のビュークラス
	/// </summary>
	///====================================================================================================
	public class SMPhotonMonoBehaviourView
		: MonoBehaviourPun, ISMNetworkMonoBehaviourView, ISMLightBase, IPunObservable
	{
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		static readonly Dictionary<SMNetworkSendType, ViewSynchronization> OBJECT_TYPE_TO_PHOTON_TYPE
			= new Dictionary<SMNetworkSendType, ViewSynchronization>
		{
			{ SMNetworkSendType.None,				ViewSynchronization.Off },
			{ SMNetworkSendType.ReliableDelta,		ViewSynchronization.ReliableDeltaCompressed },
			{ SMNetworkSendType.UnreliableChange,	ViewSynchronization.UnreliableOnChange },
			{ SMNetworkSendType.Unreliable,			ViewSynchronization.Unreliable },
		};


		public uint _id { get; private set; }
		public bool _isDispose { get; private set; }

		public SMNetworkSendType _sendType				{ get; private set; }
		public SMNetworkTransformType _useTransformType	{ get; private set; }
		public bool _isUseScale		{ get; private set; }
		public bool _isUseTeleport	{ get; private set; }
		public bool _isUseAnimator	{ get; private set; }

		public object _networkView	=> photonView;
		public object _owner		=> photonView.Owner;

		public SMNetworkObjectType _objectType => (
			photonView.IsRoomView	? SMNetworkObjectType.World :
			photonView.IsMine		? SMNetworkObjectType.Mine
									: SMNetworkObjectType.Other
		);
		public int _networkID		=> photonView.ViewID;
		public int _ownerID			=> photonView.Owner.ActorNumber;
		public string _ownerName	=> photonView.Owner.NickName;

		public Subject< List<object> > _sendStreamEvent		{ get; private set; } = new Subject< List<object> >();
		public Subject< List<object> > _receiveStreamEvent	{ get; private set; } = new Subject< List<object> >();
		public Subject<SMGameServerSendData> _receiveEvent	{ get; private set; }
			= new Subject<SMGameServerSendData>();



#region ToString
		public virtual string AddToString( int indent )
			=> string.Empty;
#endregion

		///------------------------------------------------------------------------------------------------
		/// ● 生成、削除
		///------------------------------------------------------------------------------------------------
		void Awake() {
			_id = SMIDCounter.GetNewID( this );
		}

		void OnDestroy()
			=> Dispose();

		public virtual void Dispose() {
			if ( _isDispose )	{ return; }
			_isDispose = true;

			_sendStreamEvent.Dispose();
			_receiveStreamEvent.Dispose();
			_receiveEvent.Dispose();

			if ( _objectType == SMNetworkObjectType.Mine ) {
				var gameServerModel = SMServiceLocator.Resolve<SMNetworkManager>()?._gameServerModel;
				gameServerModel?.Destroy( gameObject );
			}
		}

		///------------------------------------------------------------------------------------------------
		/// ● 情報を送受信
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 情報送信
		/// </summary>
		public void Send<TTarget, TData>( TTarget target, TData data )
			where TTarget : class
			where TData : SMGameServerSendData
		{
#if TestNetwork
			SMLog.Debug( $"{nameof( Send )} : {data}", SMLogTag.Server );
#endif
			data._sendSeconds = 0;
			data._sender = null;
			data._view = null;
			var rawData = SerializerSMUtility.Serialize( data );
			var typeName = data.GetType().AssemblyQualifiedName;

			switch ( target ) {
				case SMGameServerSendTarget sendTarget: {
					var rpcTarget = ToRPCTarget( sendTarget );
					photonView.RPC( nameof( Receive ), rpcTarget, typeName, rawData );
					break;
				}
				case Player player: {
					photonView.RPC( nameof( Receive ), player, typeName, rawData );
					break;
				}
				case RpcTarget rpcTarget: {
					photonView.RPC( nameof( Receive ), rpcTarget, typeName, rawData );
					break;
				}
				default: {
					throw new InvalidOperationException( string.Join( "\n",
						$"{this.GetName()}.{nameof( Send )}<{typeof( TData ).GetName()}>"
							+ " : キャスト失敗",
						$"無効な型 : {nameof( target )}",
						$"{nameof( SMGameServerSendTarget )}, {nameof( Player )}, {nameof( RpcTarget )}のみ指定可能"
					) );
				}
			}
		}

		/// <summary>
		/// ● 情報受信
		/// </summary>
		[PunRPC] public void Receive( string typeName, byte[] rawData, PhotonMessageInfo info ) {
			var type = Type.GetType( typeName );
			var data = SerializerSMUtility.Deserialize( type, rawData ) as SMGameServerSendData;
			data._sendSeconds = ( float )info.SentServerTime;
			data._sender = info.Sender;
			data._view = info.photonView;
#if TestNetwork
			SMLog.Debug( $"{nameof( Receive )} : {data}", SMLogTag.Server );
#endif
			_receiveEvent.OnNext( data );
		}

		/// <summary>
		/// ● 直列で情報送信（呼戻）
		/// </summary>
		public void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info ) {
			// 送信
			if ( stream.IsWriting ) {
				var os = new List<object>();
				_sendStreamEvent.OnNext( os );
				os.ForEach( o => stream.SendNext( o ) );
			}
			// 受信
			if ( stream.IsReading ) {
				var os = stream.ToArray().ToList();
				_receiveStreamEvent.OnNext( os );
			}
		}

		///------------------------------------------------------------------------------------------------
		/// ● 部品を適用
		///------------------------------------------------------------------------------------------------
		public void ApplyComponents() {
			var topView = GetComponent<SMNetworkMonoBehaviourView>();

			var realUseTransformType = topView._useTransformType;
			if ( realUseTransformType == SMNetworkTransformType.Auto ) {
				realUseTransformType = (
					gameObject.HasComponent<Rigidbody>()	? SMNetworkTransformType.RigidBody :
					gameObject.HasComponent<Rigidbody2D>()	? SMNetworkTransformType.RigidBody
															: SMNetworkTransformType.Transform
				);
			}

			ApplyPhotonView( topView );
			ApplyPhotonTransformView( topView, realUseTransformType );
			ApplyPhotonRigidbodyView( topView, realUseTransformType );
			ApplyPhotonRigidbody2DView( topView, realUseTransformType );
			ApplyPhotonAnimatorView( topView );

			_sendType = topView._sendType;
			_useTransformType = realUseTransformType;
			_isUseScale = topView._isUseScale;
			_isUseTeleport = topView._isUseTeleport;
			_isUseAnimator = topView._isUseAnimator;
		}


		void ApplyPhotonComponent<T>( bool isUse, bool isChange, Action<T> setEvent ) where T : MonoBehaviour {
			if ( !isUse ) {
				DestroyImmediate( GetComponent<T>(), true );
				return;
			}
			var v = gameObject.GetComponent<T>();
			if ( v == null ) {
				v = gameObject.AddComponent<T>();
				setEvent?.Invoke( v );
				return;
			}
			if ( isChange ) {
				setEvent?.Invoke( v );
			}
		}

		void ApplyPhotonView( SMNetworkMonoBehaviourView topView )
			=> ApplyPhotonComponent<PhotonView>(
				true,
				_sendType != topView._sendType,
				v => {
					v.OwnershipTransfer = OwnershipOption.Fixed;
					v.Synchronization = ToViewSynchronization( topView._sendType );
					v.observableSearch = PhotonView.ObservableSearch.AutoFindAll;
				}
			);

		void ApplyPhotonTransformView( SMNetworkMonoBehaviourView topView,
										SMNetworkTransformType realUseTransformType
		) => ApplyPhotonComponent<PhotonTransformView>(
			(
				realUseTransformType == SMNetworkTransformType.Transform ||
				( realUseTransformType != SMNetworkTransformType.None && topView._isUseScale )
			), (
				_useTransformType != realUseTransformType ||
				_isUseScale != topView._isUseScale
			), v => {
				var isUse = realUseTransformType == SMNetworkTransformType.Transform;
				v.m_SynchronizePosition = isUse;
				v.m_SynchronizeRotation = isUse;
				v.m_SynchronizeScale = topView._isUseScale;
				v.m_UseLocal = false;
			}
		);

		void ApplyPhotonRigidbodyView( SMNetworkMonoBehaviourView topView,
										SMNetworkTransformType realUseTransformType
		) => ApplyPhotonComponent<PhotonRigidbodyView>(
			(
				realUseTransformType == SMNetworkTransformType.RigidBody &&
				gameObject.HasComponent<Rigidbody>()
			), (
				_useTransformType != realUseTransformType ||
				_isUseTeleport != topView._isUseTeleport
			), v => {
				v.m_TeleportEnabled = topView._isUseTeleport;
				v.m_SynchronizeVelocity = true;
				v.m_SynchronizeAngularVelocity = true;
			}
		);

		void ApplyPhotonRigidbody2DView( SMNetworkMonoBehaviourView topView,
											SMNetworkTransformType realUseTransformType
		) => ApplyPhotonComponent<PhotonRigidbody2DView>(
			(
				realUseTransformType == SMNetworkTransformType.RigidBody &&
				gameObject.HasComponent<Rigidbody2D>()
			), (
				_useTransformType != realUseTransformType ||
				_isUseTeleport != topView._isUseTeleport
			), v => {
				v.m_TeleportEnabled = topView._isUseTeleport;
				v.m_SynchronizeVelocity = true;
				v.m_SynchronizeAngularVelocity = true;
			}
		);

		void ApplyPhotonAnimatorView( SMNetworkMonoBehaviourView topView )
			=> ApplyPhotonComponent<PhotonAnimatorView>(
				(
					topView._isUseAnimator &&
					gameObject.HasComponent<Animator>()
				),
				_isUseAnimator != topView._isUseAnimator,
				v => {
#if UNITY_EDITOR
					var a = GetComponent<Animator>().runtimeAnimatorController
						as UnityEditor.Animations.AnimatorController;

					a.layers.Length.Times( i => {
						v.SetLayerSynchronized( i, PhotonAnimatorView.SynchronizeType.Continuous );
					} );

					a.parameters.ForEach( p => {
						switch ( p.type ) {
							case AnimatorControllerParameterType.Float:
								v.SetParameterSynchronized( p.name, PhotonAnimatorView.ParameterType.Float,
									PhotonAnimatorView.SynchronizeType.Continuous );
								break;
							case AnimatorControllerParameterType.Int:
								v.SetParameterSynchronized( p.name, PhotonAnimatorView.ParameterType.Int,
									PhotonAnimatorView.SynchronizeType.Continuous );
								break;
							case AnimatorControllerParameterType.Bool:
								v.SetParameterSynchronized( p.name, PhotonAnimatorView.ParameterType.Bool,
									PhotonAnimatorView.SynchronizeType.Discrete );
								break;
							case AnimatorControllerParameterType.Trigger:
								v.SetParameterSynchronized( p.name, PhotonAnimatorView.ParameterType.Trigger,
									PhotonAnimatorView.SynchronizeType.Discrete );
								break;
						}
					} );
#endif
				}
			);

		///------------------------------------------------------------------------------------------------
		/// ● 変換
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● フォトンのRPCTargetに変換
		/// </summary>
		RpcTarget ToRPCTarget( SMGameServerSendTarget target ) {
			switch ( target._type ) {
				case SMGameServerSendTargetType.All:
					return target._isKeep ? RpcTarget.AllBuffered : RpcTarget.All;

				case SMGameServerSendTargetType.AllByNetwork:
					return target._isKeep ? RpcTarget.AllBufferedViaServer : RpcTarget.AllViaServer;

				case SMGameServerSendTargetType.Server:
					if ( target._isKeep && SMDebugManager.IS_DEVELOP ) {
						SMLog.Warning(
							string.Join( "\n",
								$"{this.GetName()}.{nameof( ToRPCTarget )} : 変換を妥協",
								$"未対応 : {SMGameServerSendTargetType.Server}で、{nameof( target._isKeep )}"
									+ $" : {true}",
								$"矯正 : {nameof( target._isKeep )} : {false}"
							),
							SMLogTag.Server
						);
					}
					return RpcTarget.MasterClient;

				case SMGameServerSendTargetType.Other:
					return target._isKeep ? RpcTarget.OthersBuffered : RpcTarget.Others;

				default:
					throw new InvalidOperationException( string.Join( "\n",
						$"{this.GetName()}.{nameof( ToRPCTarget )} : 変換失敗",
						$"謎の型 : {target._type}"
					) );
			}
		}

		/// <summary>
		/// ● フォトンのViewSynchronizationに変換
		/// </summary>
		ViewSynchronization ToViewSynchronization( SMNetworkSendType type )
			=> OBJECT_TYPE_TO_PHOTON_TYPE[type];

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