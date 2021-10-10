//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using UniRx;
	///====================================================================================================
	/// <summary>
	/// ■ ゲームサーバーのモデルクラス
	/// </summary>
	///====================================================================================================
	public abstract class SMGameServerModel : SMStandardBase {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>接続の型</summary>
		[SMShow] public abstract SMGameServerType _type	{ get; }

		/// <summary>プレイヤー名</summary>
		[SMShow] public abstract string _playerName	{ get; protected set; }
		/// <summary>プレイヤー数</summary>
		[SMShow] public abstract int _playerCount	{ get; }

		/// <summary>全て接続完了か？</summary>
		[SMShow] public abstract bool _isConnect	{ get; }
		/// <summary>サーバーか？</summary>
		[SMShow] public abstract bool _isServer		{ get; }
		/// <summary>活動中か？</summary>
		[SMShow] public abstract bool _isActive		{ get; set; }
		/// <summary>鍵部屋か？</summary>
		[SMShow] public abstract bool _isLockRoom	{ get; set; }

		/// <summary>プレイヤー数のイベント</summary>
		public readonly ReactiveProperty<int> _playerCountEvent			= new ReactiveProperty<int>();
		/// <summary>現在の部屋のイベント</summary>
		public readonly Subject<SMGameServerRoom> _currentRoomEvent		= new Subject<SMGameServerRoom>();
		/// <summary>部屋一覧のイベント</summary>
		public readonly Subject< List<SMGameServerRoom> > _roomsEvent	= new Subject< List<SMGameServerRoom> >();
		/// <summary>失敗のイベント</summary>
		public readonly Subject<GameServerSMException> _errorEvent		= new Subject<GameServerSMException>();
		/// <summary>サーバーから情報受信のイベント</summary>
		public readonly Subject<SMGameServerSendData> _receiveEvent		= new Subject<SMGameServerSendData>();

		///------------------------------------------------------------------------------------------------
		/// ● 接続
		///------------------------------------------------------------------------------------------------
		/// <summary>● ネットワーク接続</summary>
		public abstract UniTask<bool> Connect( bool isOnline, string playerName );
		/// <summary>● 接続解除</summary>
		public abstract UniTask<bool> Disconnect();

		/// <summary>● 控室に入室</summary>
		public abstract UniTask<bool> EnterLobby();

		/// <summary>● 部屋を作成</summary>
		public abstract UniTask<bool> CreateRoom( string name, string password, int maxPlayerCount );
		/// <summary>● 部屋に入室</summary>
		public abstract UniTask<bool> EnterRoom( SMGameServerRoom room, string inputPassword );

		///------------------------------------------------------------------------------------------------
		/// ● 情報を送受信
		///------------------------------------------------------------------------------------------------
		/// <summary>● サーバーに情報送信</summary>
		public abstract void Send<T>( SMGameServerSendTarget target, T data,
										SMGameServerSendType sendType = SMGameServerSendType.Reliable
		) where T : SMGameServerSendData;

		///------------------------------------------------------------------------------------------------
		/// ● ゲーム物の生成、破棄
		///------------------------------------------------------------------------------------------------
		/// <summary>● ゲーム物を生成</summary>
		public abstract GameObject Instantiate( string name, Vector3 position = default,
												Quaternion rotation = default
		);
		/// <summary>● ゲーム物を破棄</summary>
		public abstract void Destroy( GameObject gameObject );



		/// <summary>● ネットワーク送信情報を破棄</summary>
		public abstract void RemoveSendData( object target );

		/// <summary>● ネットワーク描画を検索</summary>
		public abstract object FindNetworkView( int networkID );
	}
}